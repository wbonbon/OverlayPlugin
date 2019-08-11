using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Advanced_Combat_Tracker;
using FFXIV_ACT_Plugin;
using FFXIV_ACT_Plugin.Common;

namespace RainbowMage.OverlayPlugin
{
    class FFXIVRepository
    {
        private static IDataRepository repository;

        private static IDataRepository GetRepository()
        {
            if (repository != null)
                return repository;

            var FFXIV = ActGlobals.oFormActMain.ActPlugins.FirstOrDefault(x => x.lblPluginTitle.Text == "FFXIV_ACT_Plugin.dll");
            if (FFXIV == null || FFXIV.pluginObj == null)
            {
                return null;
            } else {
                repository = ((FFXIV_ACT_Plugin.FFXIV_ACT_Plugin) FFXIV.pluginObj).DataRepository;
                return repository;
            }
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
    }
}
