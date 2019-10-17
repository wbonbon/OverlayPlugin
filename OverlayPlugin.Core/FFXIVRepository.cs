using System;
using System.Linq;
using Advanced_Combat_Tracker;
using FFXIV_ACT_Plugin.Common;

namespace RainbowMage.OverlayPlugin
{
    /* Taken from FFIXV_ACT_Plugin.Logfile. Copy&pasted to avoid issues if the FFXIV plugin ever changes this enum. */
    public enum LogMessageType
    {
        LogLine,
        ChangeZone,
        ChangePrimaryPlayer,
        AddCombatant,
        RemoveCombatant,
        AddBuff,
        RemoveBuff,
        FlyingText,
        OutgoingAbility,
        IncomingAbility = 10,
        PartyList,
        PlayerStats,
        CombatantHP,
        ParsedPartyMember,
        NetworkStartsCasting = 20,
        NetworkAbility,
        NetworkAOEAbility,
        NetworkCancelAbility,
        NetworkDoT,
        NetworkDeath,
        NetworkBuff,
        NetworkTargetIcon,
        NetworkTargetMarker = 29,
        NetworkBuffRemove,
        NetworkGauge,
        NetworkWorld,
        Network6D,
        NetworkNameToggle,
        NetworkTether,
        NetworkLimitBreak,
        NetworkEffectResult,
        NetworkStatusList,
        NetworkUpdateHp,
        Settings = 249,
        Process,
        Debug,
        PacketDump,
        Version,
        Error,
        Timer
    }

    class FFXIVRepository
    {
        private static IDataRepository repository;
        private static IDataSubscription subscription;

        private static IDataRepository GetRepository()
        {
            if (repository != null)
                return repository;

            var FFXIV = ActGlobals.oFormActMain.ActPlugins.FirstOrDefault(x => x.lblPluginTitle.Text == "FFXIV_ACT_Plugin.dll");
            if (FFXIV != null && FFXIV.pluginObj != null)
            {
                try
                {
                    repository = (IDataRepository)FFXIV.pluginObj.GetType().GetProperty("DataRepository").GetValue(FFXIV.pluginObj);
                }
                catch (Exception ex)
                {
                    Registry.Resolve<ILogger>().Log(LogLevel.Error, $"Failed to retrieve the FFXIV DataRepository: {ex}");
                }
            }

            return repository;
        }

        private static IDataSubscription GetSubscription()
        {
            if (subscription != null)
                return subscription;

            var FFXIV = ActGlobals.oFormActMain.ActPlugins.FirstOrDefault(x => x.lblPluginTitle.Text == "FFXIV_ACT_Plugin.dll");
            if (FFXIV != null && FFXIV.pluginObj != null)
            {
                try
                {
                    subscription = (IDataSubscription)FFXIV.pluginObj.GetType().GetProperty("DataSubscription").GetValue(FFXIV.pluginObj);
                }
                catch (Exception ex)
                {
                    Registry.Resolve<ILogger>().Log(LogLevel.Error, $"Failed to retrieve the FFXIV DataSubscription: {ex}");
                }
            }

            return subscription;
        }

        public static uint GetPlayerID()
        {
            var repo = GetRepository();
            if (repo == null) return 0;

            return repo.GetCurrentPlayerID();
        }

        public static string GetPlayerName()
        {
            var repo = GetRepository();
            if (repo == null) return null;

            var playerId = repo.GetCurrentPlayerID();

            var playerInfo = repo.GetCombatantList().FirstOrDefault(x => x.ID == playerId);
            if (playerInfo == null) return null;

            return playerInfo.Name;
        }

        // LogLineDelegate(uint EventType, uint Seconds, string logline);
        public static void RegisterLogLineHandler(Action<uint, uint, string> handler)
        {
            var sub = GetSubscription();
            sub.LogLine += new LogLineDelegate(handler);
        }

        public static void RegisterNetworkParser(Action<string, long, byte[]> handler)
        {
            var sub = GetSubscription();
            sub.NetworkReceived += new NetworkReceivedDelegate(handler);
        }
    }
}
