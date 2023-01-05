using BepInEx.Configuration;
using HarmonyLib;
using PotionCraft.DialogueSystem.Dialogue;
using PotionCraft.DialogueSystem.Dialogue.Data;
using PotionCraft.LocalizationSystem;
using PotionCraft.ManagersSystem;
using PotionCraft.ManagersSystem.TMP;
using PotionCraft.ObjectBased.UIElements.Dialogue;
using PotionCraft.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ItsForFree
{
    internal static class DialogueBoxHook
    {
        private static DialogueButton giveAway = null;

        [HarmonyPostfix, HarmonyPatch(typeof(DialogueBox), "SpawnPotionRequestInterface")]
        public static void SpawnPotionRequestInterface_Postfix(PotionRequestNodeData potionRequestNodeData, DialogueData dialogueData)
        {
            var box = Managers.Dialogue.dialogueBox;

            // make the button array bigger
            int numButtons = box.dialogueButtons.Length;
            var buttons = box.dialogueButtons;
            box.dialogueButtons = new DialogueButton[numButtons + 1];

            Vector2 zero = Vector2.zero;
            for (int i = 0; i < numButtons; i++)
            {
                box.dialogueButtons[i] = buttons[i];
                zero.y += box.dialogueButtons[i].thisCollider.bounds.size.y + box.spaceAnswers;
            }

            giveAway = DialogueBox.dialogueButtonSpawner.Spawn(box.dialogueButtonObjects.Length - box.dialogueButtons.Length + numButtons, zero);
            Key leftTextKey = new("#dialogue_closeness_give_potion", new List<string>
            {
                ""
            }, false);
            
            Action actionOnRelease = giveAway.actionOnRelease;
            actionOnRelease ??= delegate ()
            {
                box.ChooseAnswer(5);
            };

            giveAway.SetContent(leftTextKey, GetRightKey(), box.givePotionIcon, actionOnRelease, numButtons, null, null, 1f);
            giveAway.SetAlpha(0f);
            giveAway.Locked = true;
            zero.y += giveAway.thisCollider.bounds.size.y + box.spaceAnswers;
            box.dialogueText.minTextBoxY = zero.y - box.spaceAnswers + box.spaceTextFirstAnswer;
            box.dialogueButtons[numButtons] = giveAway;

            // proper fix for end dialogue button being locked
            box.dialogueButtons[numButtons - 1].Locked = false;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(DialogueBox), "OnPotionRequestEnd")]
        public static void OnPotionRequestEnd_Prefix()
        {
            giveAway.Locked = false;
            giveAway = null;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(DialogueBox), "ChooseAnswer")]
        public static void ChooseAnswer_Prefix(ref int index)
        {
            if (Managers.Dialogue.State == DialogueState.PotionRequest)
            {
                if (index == 5)
                {
                    index = 0;
                    NpcTradingHook.Intercept = true;
                }
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(DialogueBox), "ChooseAnswer")]
        public static void ChooseAnswer_Postfix(ref int index)
        {
            // just in case
            NpcTradingHook.Intercept = false;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(DialogueBox), "UpdateTradeButtons")]
        public static void UpdateTradeButtons_Postfix()
        {
            if (Managers.Dialogue.State == DialogueState.PotionRequest)
            {
                if ((Managers.Dialogue.dialogueBox.tradeButton != null) && (giveAway != null))
                {
                    giveAway.Locked = Managers.Dialogue.dialogueBox.tradeButton.Locked;
                }
            }
        }

        public static void ConfigUpdated(object sender, SettingChangedEventArgs args)
        {
            if (giveAway != null)
            {
                giveAway.rightText.SetText(GetRightKey());
            }
        }

        public static Key GetRightKey()
        {
            string commonAtlasName = Settings<TMPManagerSettings>.Asset.CommonAtlasName;

            StringBuilder builder = new("0<voffset=-0.15em><size=130%><sprite=\"");
            builder.Append(commonAtlasName);
            builder.Append("\" name=\"Gold Icon Dialogue Locked\"></size></voffset>");

            bool mult = Settings.Multiplier != 1f;
            bool add = Settings.Additive != 0;

            if (mult)
            {
                builder.Append(", ");
                builder.Append(Settings.Multiplier);
                builder.Append("x<voffset=-0.15em><size=130%><sprite=\"");
                builder.Append(commonAtlasName);
                builder.Append("\" name=\"ReputeIcon Haggle\"></size></voffset>");
                if (add)
                {
                    builder.Append("+");
                    builder.Append(Settings.Additive);
                }
            }
            else if (add)
            {
                builder.Append(", +");
                builder.Append(Settings.Additive);
                builder.Append("<voffset=-0.15em><size=130%><sprite=\"");
                builder.Append(commonAtlasName);
                builder.Append("\" name=\"ReputeIcon Haggle\"></size></voffset>");
            }

            return new Key("#haggle_with_clients_popularity_tooltip", new List<string> { builder.ToString() }, false);
        }
    }
}
