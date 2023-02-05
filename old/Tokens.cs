using BepInEx;
using UnityEngine;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using MoreSlugcats;

namespace WolfoTestMod
{
    public class Tokens : BaseUnityPlugin
    {
        public static void OnEnable()
        {
            On.PlayerProgression.MiscProgressionData.SetTokenCollected_SandboxUnlockID += BlueToken_SetTokenCollected_SandboxUnlockID;
            On.PlayerProgression.MiscProgressionData.SetTokenCollected_LevelUnlockID += YellowToken_SetTokenCollected_LevelUnlockID;
            On.PlayerProgression.MiscProgressionData.SetTokenCollected_SafariUnlockID += RedToken_SetTokenCollected_SafariUnlockID;
            On.PlayerProgression.MiscProgressionData.SetTokenCollected_SlugcatUnlockID += GreenToken_SetTokenCollected_SlugcatUnlockID;
            On.PlayerProgression.MiscProgressionData.SetBroadcastListened += GrayToken_SetBroadcastListened;
        }


        public static List<MultiplayerUnlocks.SandboxUnlockID> storedBlueTokens = new List<MultiplayerUnlocks.SandboxUnlockID>();
        public static List<MultiplayerUnlocks.LevelUnlockID> storedYellowTokens = new List<MultiplayerUnlocks.LevelUnlockID>();
        public static List<MultiplayerUnlocks.SafariUnlockID> storedRedTokens = new List<MultiplayerUnlocks.SafariUnlockID>();
        public static List<MultiplayerUnlocks.SlugcatUnlockID> storedGreenTokens = new List<MultiplayerUnlocks.SlugcatUnlockID>();
        public static List<ChatlogData.ChatlogID> storedGrayTokens = new List<ChatlogData.ChatlogID>();


        private static bool BlueToken_SetTokenCollected_SandboxUnlockID(On.PlayerProgression.MiscProgressionData.orig_SetTokenCollected_SandboxUnlockID orig, PlayerProgression.MiscProgressionData self, MultiplayerUnlocks.SandboxUnlockID sandboxToken)
        {
            if (self.owner.rainWorld.ExpeditionMode)
            {
                storedBlueTokens.Add(sandboxToken);
            }
            return orig(self, sandboxToken);
        }

        private static bool YellowToken_SetTokenCollected_LevelUnlockID(On.PlayerProgression.MiscProgressionData.orig_SetTokenCollected_LevelUnlockID orig, PlayerProgression.MiscProgressionData self, MultiplayerUnlocks.LevelUnlockID levelToken)
        {
            if (self.owner.rainWorld.ExpeditionMode)
            {
                storedYellowTokens.Add(levelToken);
            }
            return orig(self, levelToken);
        }

        private static bool RedToken_SetTokenCollected_SafariUnlockID(On.PlayerProgression.MiscProgressionData.orig_SetTokenCollected_SafariUnlockID orig, PlayerProgression.MiscProgressionData self, MultiplayerUnlocks.SafariUnlockID safariToken)
        {
            if (self.owner.rainWorld.ExpeditionMode)
            {
                storedRedTokens.Add(safariToken);
            }
            return orig(self, safariToken);
        }

        private static bool GreenToken_SetTokenCollected_SlugcatUnlockID(On.PlayerProgression.MiscProgressionData.orig_SetTokenCollected_SlugcatUnlockID orig, PlayerProgression.MiscProgressionData self, MultiplayerUnlocks.SlugcatUnlockID classToken)
        {
            if (self.owner.rainWorld.ExpeditionMode)
            {
                storedGreenTokens.Add(classToken);
            }
            return orig(self, classToken);
        }

        private static bool GrayToken_SetBroadcastListened(On.PlayerProgression.MiscProgressionData.orig_SetBroadcastListened orig, PlayerProgression.MiscProgressionData self, ChatlogData.ChatlogID chat)
        {
            if (self.owner.rainWorld.ExpeditionMode)
            {
                storedGrayTokens.Add(chat);
            }
            return orig(self, chat);
        }

        public static void SaveTokensToMainSaveFile(PlayerProgression progression)
        {
            Debug.Log("ExpeditionSharedProgress : Token Saving");
            //Debug.Log("ExpeditionSharedProgress : Saving " + storedPearls.Count + " Pearls");
            if (storedBlueTokens.Count > 0)
            {
                foreach (MultiplayerUnlocks.SandboxUnlockID token in storedBlueTokens)
                {
                    progression.miscProgressionData.SetTokenCollected(token);
                }
                storedBlueTokens.Clear();
            }
            if (storedYellowTokens.Count > 0)
            {
                foreach (MultiplayerUnlocks.LevelUnlockID token in storedYellowTokens)
                {
                    progression.miscProgressionData.SetTokenCollected(token);
                }
                storedYellowTokens.Clear();
            }
            if (storedRedTokens.Count > 0)
            {
                foreach (MultiplayerUnlocks.SafariUnlockID token in storedRedTokens)
                {
                    progression.miscProgressionData.SetTokenCollected(token);
                }
                storedRedTokens.Clear();
            }
            if (storedGreenTokens.Count > 0)
            {
                foreach (MultiplayerUnlocks.SlugcatUnlockID token in storedGreenTokens)
                {
                    progression.miscProgressionData.SetTokenCollected(token);
                }
                storedGreenTokens.Clear();
            }
            if (storedGrayTokens.Count > 0)
            {
                foreach (ChatlogData.ChatlogID chat in storedGrayTokens)
                {
                    progression.miscProgressionData.SetBroadcastListened(chat);
                }
                storedGrayTokens.Clear();
            }
        }

    }
}
