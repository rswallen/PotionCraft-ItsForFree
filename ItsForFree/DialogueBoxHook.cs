﻿using BepInEx.Logging;
using HarmonyLib;
using PotionCraft.DialogueSystem.Dialogue;
using PotionCraft.DialogueSystem.Dialogue.Data;
using PotionCraft.LocalizationSystem;
using PotionCraft.ManagersSystem;
using PotionCraft.ManagersSystem.TMP;
using PotionCraft.ObjectBased.UIElements.Dialogue;
using PotionCraft.Settings;
using PotionCraft.Utils.Dissolve.DissolvableRenderers;
using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace ItsForFree
{
    internal static class DialogueBoxHook
    {
        //private static ManualLogSource Log => ItsForFreePlugin.Log;

        private static DialogueButton giveAway = null;

        [HarmonyPostfix, HarmonyPatch(typeof(DialogueBox), "SpawnPotionRequestInterface")]
        public static void SpawnPotionRequestInterface_Postfix(PotionRequestNodeData potionRequestNodeData, DialogueData dialogueData)
        {
            //Log.LogDebug("SpawnPotionRequestInterface - Adding extra button to dialogueBox for potion request");
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
            Key leftTextKey = new Key("#dialogue_closeness_give_potion", new List<string>
            {
                ""
            }, false);
            string commonAtlasName = Settings<TMPManagerSettings>.Asset.CommonAtlasName;
            Key rightTextKey = new Key("#haggle_with_clients_popularity_tooltip", new List<string>
            {
                "0<voffset=-0.15em><size=130%><sprite=\"" + commonAtlasName + "\" name=\"Gold Icon Dialogue Locked\"></size>, 2x<voffset=-0.15em><size=130%><sprite=\"" + commonAtlasName + "\" name=\"ReputeIcon Haggle\"></size>"
            }, false);
            Action actionOnRelease = giveAway.actionOnRelease;
            if (actionOnRelease == null)
            {
                actionOnRelease = delegate ()
                {
                    box.ChooseAnswer(5);
                };
            }
            giveAway.SetContent(leftTextKey, rightTextKey, box.givePotionIcon, actionOnRelease, numButtons, null, null, 1f);
            giveAway.SetAlpha(0f);
            giveAway.Locked = true;
            zero.y += giveAway.thisCollider.bounds.size.y + box.spaceAnswers;
            box.dialogueText.minTextBoxY = zero.y - box.spaceAnswers + box.spaceTextFirstAnswer;
            box.dialogueButtons[numButtons] = giveAway;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(DialogueBox), "OnPotionRequestEnd")]
        public static void OnPotionRequestEnd_Prefix()
        {
            //Log.LogDebug("OnPotionRequestEnd - Cleaning up");
            giveAway = null;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(DialogueBox), "ChooseAnswer")]
        public static void ChooseAnswer_Prefix(ref int index)
        {
            if (Managers.Dialogue.State == DialogueState.PotionRequest)
            {
                if (index == 5)
                {
                    //Log.LogDebug("ChooseAnswer - Activating intercept and setting index to 0 (sell)");
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
    }
}