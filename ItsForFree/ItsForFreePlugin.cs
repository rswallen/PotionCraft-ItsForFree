using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace ItsForFree
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class ItsForFreePlugin : BaseUnityPlugin
    {
        //internal static ManualLogSource Log;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            //Log = Logger;

            Harmony.CreateAndPatchAll(typeof(DialogueBoxHook));
            Harmony.CreateAndPatchAll(typeof(NpcTradingHook));
        }
    }
}
