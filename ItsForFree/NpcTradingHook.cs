using HarmonyLib;
using PotionCraft.Npc.MonoBehaviourScripts;

namespace ItsForFree
{
    internal class NpcTradingHook
    {
        public static bool Intercept = false;

        [HarmonyPrefix, HarmonyPatch(typeof(NpcTrading), "GetReward")]
        public static void GetReward_Prefix(ref int gold, ref int popularity, int karma, float experience)
        {
            if (Intercept)
            {
                gold = 0;
                popularity *= 2;
                Intercept = false;
            }
        }
    }
}