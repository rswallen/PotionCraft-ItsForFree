using BepInEx;
using HarmonyLib;

namespace ItsForFree
{
    [BepInPlugin("com.github.rswallen.potioncraft.itsforfree", "It's For Free", "1.0.2")]
    public class ItsForFreePlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            Harmony.CreateAndPatchAll(typeof(DialogueBoxHook));
            Harmony.CreateAndPatchAll(typeof(NpcTradingHook));
        }
    }
}