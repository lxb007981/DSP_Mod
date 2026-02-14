using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace StationChargeBoost
{
    [BepInPlugin(__GUID__, __NAME__, __VERSION__)]
    public class StationChargeBoost : BaseUnityPlugin
    {
        public const string __NAME__ = "StationChargeBoost";
        public const string __GUID__ = "lxb007981.dsp." + __NAME__;
        public const string __VERSION__ = "0.0.1";

        Harmony harmony;
        public new static readonly BepInEx.Logging.ManualLogSource Logger =
                BepInEx.Logging.Logger.CreateLogSource(__NAME__);

        public static class Configs
        {
            public static int ConfigMaxChargePower = 1000; // in megawatts
        }

        public void Awake()
        {
            // Plugin startup logic
            harmony = new Harmony(__GUID__);
            Configs.ConfigMaxChargePower = Config.Bind("General", "MaxChargePower", 1000, 
                                               "The maximum charge power (mw) for stellar stations.").Value;
            Logger.LogInfo($"Plugin {__GUID__} is loaded!");
            harmony.PatchAll(typeof(StationChargeBoost));
        }

        public void OnDestroy()
        {
            harmony.UnpatchSelf();
        }

        [HarmonyPrefix, HarmonyPatch(typeof(UIStationWindow), "OnMaxChargePowerSliderValueChange")]
        public static void OnMaxChargePowerSliderValueChange_Prefix(ref UIStationWindow __instance, ref float value)
        {
            if (__instance.transport.stationPool[__instance.stationId].isStellar)
            {
                value = 10 + (value - 10) * (Configs.ConfigMaxChargePower - 30) / 270;
            }
        }

        [HarmonyPrefix, HarmonyPatch(typeof(UIControlPanelStationInspector), "OnMaxChargePowerSliderValueChange")]
        public static void OnMaxChargePowerSliderValueChange_Prefix(ref UIControlPanelStationInspector __instance,
                                                                    ref float value)
        {
            if (__instance.transport.stationPool[__instance.stationId].isStellar)
            {
                value = 10 + (value - 10) * (Configs.ConfigMaxChargePower - 30) / 270;
            }
        }        
    }
}