using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Advanced_Combat_Tracker;
using System.Diagnostics;
using System.Windows.Forms;

namespace RainbowMage.OverlayPlugin.Overlays
{
    partial class MiniParseEventSource : EventSourceBase
    {
        private string prevEncounterId { get; set; }
        private DateTime prevEndDateTime { get; set; }
        private bool prevEncounterActive { get; set; }
        
        // Event Source
        
        public MiniParseEventSourceConfig Config { get; set; }

        public MiniParseEventSource(MiniParseEventSourceConfig config, ILogger logger) : base(logger)
        {
            this.Name = "MiniParse";
            this.Config = Config;

            RegisterEventTypes(new List<string> { "CombatData", "LogLine" });

            ActGlobals.oFormActMain.BeforeLogLineRead += LogLineReader;
        }

        public override Control CreateConfigControl()
        {
            return new MiniParseEventSourceConfigPanel(this);
        }

        public override void LoadConfig(IPluginConfig config)
        {
        }

        protected override void Update()
        {
            if (CheckIsActReady())
            {
                /* // 最終更新時刻に変化がないなら更新を行わない
                if (this.prevEncounterId == ActGlobals.oFormActMain.ActiveZone.ActiveEncounter.EncId &&
                    this.prevEndDateTime == ActGlobals.oFormActMain.ActiveZone.ActiveEncounter.EndTime &&
                    this.prevEncounterActive == ActGlobals.oFormActMain.ActiveZone.ActiveEncounter.Active)
                {
                    return;
                }

                this.prevEncounterId = ActGlobals.oFormActMain.ActiveZone.ActiveEncounter.EncId;
                this.prevEndDateTime = ActGlobals.oFormActMain.ActiveZone.ActiveEncounter.EndTime;
                this.prevEncounterActive = ActGlobals.oFormActMain.ActiveZone.ActiveEncounter.Active;
                */
                DispatchEvent(this.CreateJsonData());
            }
        }

        internal JObject CreateJsonData()
        {
            if (!CheckIsActReady())
            {
                return new JObject();
            }

#if DEBUG
            //var stopwatch = new Stopwatch();
            //stopwatch.Start();
#endif

            var allies = ActGlobals.oFormActMain.ActiveZone.ActiveEncounter.GetAllies();
            Dictionary<string, string> encounter = null;
            List<KeyValuePair<CombatantData, Dictionary<string, string>>> combatant = null;

            var encounterTask = Task.Run(() =>
            {
                encounter = GetEncounterDictionary(allies);
            });
            var combatantTask = Task.Run(() =>
            {
                combatant = GetCombatantList(allies);
            });
            Task.WaitAll(encounterTask, combatantTask);

            if (encounter == null || combatant == null) return new JObject();

            JObject obj = new JObject();

            obj["type"] = "CombatData";
            obj["Encounter"] = JObject.FromObject(encounter);
            obj["Combatant"] = new JObject();

            foreach (var pair in combatant)
            {
                JObject value = new JObject();
                foreach (var pair2 in pair.Value)
                {
                    value.Add(pair2.Key, Util.ReplaceNaNString(pair2.Value, "---"));
                }

                obj["Combatant"][pair.Key.Name] = value;
            }

            obj["isActive"] = ActGlobals.oFormActMain.ActiveZone.ActiveEncounter.Active ? "true" : "false";

#if DEBUG
            //stopwatch.Stop();
            //Log(LogLevel.Trace, "CreateUpdateScript: {0} msec", stopwatch.Elapsed.TotalMilliseconds);
#endif
            return obj;
        }

        private List<KeyValuePair<CombatantData, Dictionary<string, string>>> GetCombatantList(List<CombatantData> allies)
        {
#if DEBUG
            //var stopwatch = new Stopwatch();
            //stopwatch.Start();
#endif

            var combatantList = new List<KeyValuePair<CombatantData, Dictionary<string, string>>>();
            Parallel.ForEach(allies, (ally) =>
            //foreach (var ally in allies)
            {
                var valueDict = new Dictionary<string, string>();
                foreach (var exportValuePair in CombatantData.ExportVariables)
                {
                    try
                    {
                        // NAME タグには {NAME:8} のようにコロンで区切られたエクストラ情報が必要で、
                        // プラグインの仕組み的に対応することができないので除外する
                        if (exportValuePair.Key == "NAME")
                        {
                            continue;
                        }

                        // ACT_FFXIV_Plugin が提供する LastXXDPS は、
                        // ally.Items[CombatantData.DamageTypeDataOutgoingDamage].Items に All キーが存在しない場合に、
                        // プラグイン内で例外が発生してしまい、パフォーマンスが悪化するので代わりに空の文字列を挿入する
                        if (exportValuePair.Key == "Last10DPS" ||
                            exportValuePair.Key == "Last30DPS" ||
                            exportValuePair.Key == "Last60DPS")
                        {
                            if (!ally.Items[CombatantData.DamageTypeDataOutgoingDamage].Items.ContainsKey("All"))
                            {
                                valueDict.Add(exportValuePair.Key, "");
                                continue;
                            }
                        }

                        var value = exportValuePair.Value.GetExportString(ally, "");
                        valueDict.Add(exportValuePair.Key, value);
                    }
                    catch (Exception e)
                    {
                        Log(LogLevel.Debug, "GetCombatantList: {0}: {1}: {2}", ally.Name, exportValuePair.Key, e);
                        continue;
                    }
                }

                lock (combatantList)
                {
                    combatantList.Add(new KeyValuePair<CombatantData, Dictionary<string, string>>(ally, valueDict));
                }
            }
            );

#if DEBUG
            //stopwatch.Stop();
            //Log(LogLevel.Trace, "GetCombatantList: {0} msec", stopwatch.Elapsed.TotalMilliseconds);
#endif

            return combatantList;
        }

        private Dictionary<string, string> GetEncounterDictionary(List<CombatantData> allies)
        {
#if DEBUG
            //var stopwatch = new Stopwatch();
            //stopwatch.Start();
#endif

            var encounterDict = new Dictionary<string, string>();
            foreach (var exportValuePair in EncounterData.ExportVariables)
            {
                try
                {
                    // ACT_FFXIV_Plugin が提供する LastXXDPS は、
                    // ally.Items[CombatantData.DamageTypeDataOutgoingDamage].Items に All キーが存在しない場合に、
                    // プラグイン内で例外が発生してしまい、パフォーマンスが悪化するので代わりに空の文字列を挿入する
                    if (exportValuePair.Key == "Last10DPS" ||
                        exportValuePair.Key == "Last30DPS" ||
                        exportValuePair.Key == "Last60DPS")
                    {
                        if (!allies.All((ally) => ally.Items[CombatantData.DamageTypeDataOutgoingDamage].Items.ContainsKey("All")))
                        {
                            encounterDict.Add(exportValuePair.Key, "");
                            continue;
                        }
                    }

                    var value = exportValuePair.Value.GetExportString(
                        ActGlobals.oFormActMain.ActiveZone.ActiveEncounter,
                        allies,
                        "");
                    //lock (encounterDict)
                    //{
                    encounterDict.Add(exportValuePair.Key, value);
                    //}
                }
                catch (Exception e)
                {
                    Log(LogLevel.Debug, "GetEncounterDictionary: {0}: {1}", exportValuePair.Key, e);
                }
            }

#if DEBUG
            //stopwatch.Stop();
            //Log(LogLevel.Trace, "GetEncounterDictionary: {0} msec", stopwatch.Elapsed.TotalMilliseconds);
#endif

            return encounterDict;
        }

        private static bool CheckIsActReady()
        {
            if (ActGlobals.oFormActMain != null &&
                ActGlobals.oFormActMain.ActiveZone != null &&
                ActGlobals.oFormActMain.ActiveZone.ActiveEncounter != null &&
                EncounterData.ExportVariables != null &&
                CombatantData.ExportVariables != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
