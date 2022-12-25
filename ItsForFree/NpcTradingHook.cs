using BepInEx.Logging;
using HarmonyLib;
using PotionCraft.Npc.MonoBehaviourScripts;

namespace ItsForFree
{
    internal class NpcTradingHook
    {
        //private static ManualLogSource Log => ItsForFreePlugin.Log;

        public static bool Intercept = false;

        [HarmonyPrefix, HarmonyPatch(typeof(NpcTrading), "GetReward")]
        public static void GetReward_Prefix(ref int gold, ref int popularity, int karma, float experience)
        {
            if (Intercept)
            {
                //Log.LogDebug($"GetReward: {gold} gold set to 0");
                gold = 0;
                popularity *= 2;
                Intercept = false;
            }
        }
    }
}
