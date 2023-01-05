using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace ItsForFree
{
    [BepInPlugin("com.github.rswallen.potioncraft.itsforfree", "It's For Free", "1.0.3")]
    public class ItsForFreePlugin : BaseUnityPlugin
    {
        internal static new ConfigFile Config;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            Harmony.CreateAndPatchAll(typeof(DialogueBoxHook));
            Harmony.CreateAndPatchAll(typeof(NpcTradingHook));

            Config = base.Config;
            Settings.Initialize();
        }
    }
}
