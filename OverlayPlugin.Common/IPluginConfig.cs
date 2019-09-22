using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace RainbowMage.OverlayPlugin
{
    public interface IPluginConfig
    {
        OverlayConfigList<IOverlayConfig> Overlays { get; set; }
        bool FollowLatestLog { get; set; }
        bool HideOverlaysWhenNotActive { get; set; }
        Version Version { get; set; }
        bool IsFirstLaunch { get; set; }
        Dictionary<string, JObject> EventSourceConfigs { get; set; }
    }
}
