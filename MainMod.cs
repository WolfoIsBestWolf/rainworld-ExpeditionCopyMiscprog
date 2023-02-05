using BepInEx;
using UnityEngine;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System;
using System.Linq;
using System.Collections.Generic;
using MoreSlugcats;

namespace WolfoTestMod
{
    [BepInPlugin("wolfo.wolfotestmod", "WolfoTestMod", "1.0.0")]
    public class WolfoTestMod : BaseUnityPlugin
    {
        public void OnEnable()
        {
            Debug.Log("Wolfo Mod Loaded");
            //On.Menu.SlugcatSelectMenu.SetSlugcatColorOrder += SetSlugcatColorOrder_SetSlugcatColorOrder;
            //On.SlugcatStats.HiddenOrUnplayableSlugcat += SlugcatStats_HiddenOrUnplayableSlugcat; ;
            //On.Menu.MenuScene.BuildMSLandscapeScene += MenuScene_BuildMSLandscapeScene;

            //Pearls.OnEnable();
            //Tokens.OnEnable();
            //Merged Tokens from Main onto Expd

            On.PlayerProgression.LoadProgression += PlayerProgression_LoadProgression;
            On.PlayerProgression.SaveProgression += PlayerProgression_SaveProgression;

            On.Music.MusicPlayer.GameRequestsSong += AlwaysPlayMusic;

            On.RainWorldGame.ctor += RainWorldGame_ctor;
        }

        private void PlayerProgression_SaveProgression(On.PlayerProgression.orig_SaveProgression orig, PlayerProgression self, bool saveMaps, bool saveMiscProg)
        {
            Debug.Log("Saving Progress Sandbox Tokens Amount " + self.miscProgressionData.sandboxTokens.Count);
            orig(self, saveMaps, saveMiscProg);
            CopyDataFromSave(self);
            Debug.Log("ExpeditionSharedProgress Save Progress SaveSlot " + self.rainWorld.options.saveSlot);
        }

        private static int wasExpedition = 0;

        //private static List<string> tempSheltersDiscovered;


        private void PlayerProgression_LoadProgression(On.PlayerProgression.orig_LoadProgression orig, PlayerProgression self)
        {

            orig(self); 
            Debug.Log("ExpeditionSharedProgress Load Progress SaveSlot " + self.rainWorld.options.saveSlot);
            Debug.Log("Sandbox Tokens Amount " + self.miscProgressionData.sandboxTokens.Count);
            if (!self.HasSaveData)
            {
                return;
            }
            else if (!hasStoredData)
            {
                CopyDataFromSave(self);
            }
            else if (!self.rainWorld.ExpeditionMode && wasExpedition == 1 && hasStoredData)
            {
                CopyDataToSave(self);
                Debug.Log("ExpeditionSharedProgress : Saving to Main File");
                wasExpedition = 0;  //This is to Main file
            }
            else if (self.rainWorld.ExpeditionMode && wasExpedition == 0 && hasStoredData)
            {
                CopyDataToSave(self);
                Debug.Log("ExpeditionSharedProgress : Saving to Expedition File");
                wasExpedition = 1; //This is to Expd file
            }
            //CopyDataFromSave(self);
            Debug.Log("Sandbox Tokens Amount " + self.miscProgressionData.sandboxTokens.Count);
        }


        private static bool hasStoredData = false;

        //private static Dictionary<string, Texture2D>? mapDiscoveryTexturesInTrans;
        //private static Dictionary<string, long>? mapLastUpdatedTimeInTrans;
        //private static List<string>? tempSheltersDiscoveredInTrans;

        private static List<MultiplayerUnlocks.SandboxUnlockID>? sandboxTokensInTrans;
        private static List<MultiplayerUnlocks.LevelUnlockID>? levelTokensInTrans;
        private static List<MultiplayerUnlocks.SafariUnlockID>? safariTokensInTrans;
        private static List<MultiplayerUnlocks.SlugcatUnlockID>? classTokensInTrans;

        private static List<DataPearl.AbstractDataPearl.DataPearlType>? decipheredPearlsInTrans;
        private static List<DataPearl.AbstractDataPearl.DataPearlType>? decipheredDMPearlsInTrans;
        private static List<DataPearl.AbstractDataPearl.DataPearlType>? decipheredFuturePearlsInTrans;
        private static List<DataPearl.AbstractDataPearl.DataPearlType>? decipheredPebblesPearlsInTrans;
        private static List<ChatlogData.ChatlogID>? discoveredBroadcastsInTrans;


        private static void CopyDataFromSave(PlayerProgression self)
        {
            sandboxTokensInTrans = new List<MultiplayerUnlocks.SandboxUnlockID>(self.miscProgressionData.sandboxTokens);
            levelTokensInTrans = new List<MultiplayerUnlocks.LevelUnlockID>(self.miscProgressionData.levelTokens);
            safariTokensInTrans = new List<MultiplayerUnlocks.SafariUnlockID>(self.miscProgressionData.safariTokens);
            classTokensInTrans = new List<MultiplayerUnlocks.SlugcatUnlockID>(self.miscProgressionData.classTokens);
            
            decipheredPearlsInTrans = new List<DataPearl.AbstractDataPearl.DataPearlType>(self.miscProgressionData.decipheredPearls);
            decipheredDMPearlsInTrans = new List<DataPearl.AbstractDataPearl.DataPearlType>(self.miscProgressionData.decipheredDMPearls);
            decipheredFuturePearlsInTrans = new List<DataPearl.AbstractDataPearl.DataPearlType>(self.miscProgressionData.decipheredFuturePearls);
            decipheredPebblesPearlsInTrans = new List<DataPearl.AbstractDataPearl.DataPearlType>(self.miscProgressionData.decipheredPebblesPearls);
            discoveredBroadcastsInTrans = new List<ChatlogData.ChatlogID>(self.miscProgressionData.discoveredBroadcasts);

            hasStoredData = true;
            Debug.Log("ExpeditionSharedProgress : Copying Data from SaveSlot " + +self.rainWorld.options.saveSlot);
        }

        private static void CopyDataToSave(PlayerProgression self)
        {

            self.miscProgressionData.sandboxTokens = new List<MultiplayerUnlocks.SandboxUnlockID>(sandboxTokensInTrans);
            self.miscProgressionData.levelTokens = new List<MultiplayerUnlocks.LevelUnlockID>(levelTokensInTrans);
            self.miscProgressionData.safariTokens = new List<MultiplayerUnlocks.SafariUnlockID>(safariTokensInTrans);
            self.miscProgressionData.classTokens = new List<MultiplayerUnlocks.SlugcatUnlockID>(classTokensInTrans);

            self.miscProgressionData.decipheredPearls = new List<DataPearl.AbstractDataPearl.DataPearlType>(decipheredPearlsInTrans);
            self.miscProgressionData.decipheredDMPearls = new List<DataPearl.AbstractDataPearl.DataPearlType>(decipheredDMPearlsInTrans);
            self.miscProgressionData.decipheredFuturePearls = new List<DataPearl.AbstractDataPearl.DataPearlType>(decipheredFuturePearlsInTrans);
            self.miscProgressionData.decipheredPebblesPearls = new List<DataPearl.AbstractDataPearl.DataPearlType>(decipheredPebblesPearlsInTrans);
            self.miscProgressionData.discoveredBroadcasts = new List<ChatlogData.ChatlogID>(discoveredBroadcastsInTrans);

            self.SaveProgression(true,true);

            sandboxTokensInTrans.Clear();
            levelTokensInTrans.Clear();
            safariTokensInTrans.Clear();
            classTokensInTrans.Clear();

            decipheredPearlsInTrans.Clear();
            decipheredDMPearlsInTrans.Clear();
            decipheredFuturePearlsInTrans.Clear();
            decipheredPebblesPearlsInTrans.Clear();
            discoveredBroadcastsInTrans.Clear();

            hasStoredData = false;
            Debug.Log("ExpeditionSharedProgress : Saving Stored Data to SaveSlot " + self.rainWorld.options.saveSlot);
        }

        public static int one = 1;

        private static void RainWorldGame_ctor(On.RainWorldGame.orig_ctor orig, RainWorldGame game, ProcessManager manager)
        {
            if (one == 1)
            {
                one++;
                IL.SlugcatStats.ctor += SlugcatStats_ctor;
                IL.Room.Loaded += EnableTokensInExpedition;
                IL.Menu.SleepAndDeathScreen.GetDataFromGame += TokenTrackerInExpedition;
            }

            //IL.Menu.PauseMenu.ctor += PauseMenu_ctor;

            Debug.Log("Wolfo mod loaded");


            orig(game, manager);
        }

        private static void TokenTrackerInExpedition(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt("Menu.SleepAndDeathScreen", "get_IsStarveScreen"),
                x => x.MatchBrfalse(out _),
                x => x.MatchLdsfld("ModManager", "Expedition")))
            {
                c.Index += 3;
                c.Next.OpCode = OpCodes.Brtrue_S;
                Debug.Log("ExpeditionSharedProgress : Token Tracker Hook Success");
            }
            else
            {
                Debug.Log("ExpeditionSharedProgress : Token Tracker Hook Failed");
            }
        }

        public static void EnableTokensInExpedition(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdfld("PlacedObject", "active"),
                x => x.MatchBrfalse(out _),
                x => x.MatchLdsfld("ModManager", "Expedition")))
            {
                c.Index += 3;
                c.Next.OpCode = OpCodes.Brtrue_S;
                Debug.Log("ExpeditionSharedProgress : Token IL Hook Succeeded");
            }
            else
            {
                Debug.Log("ExpeditionSharedProgress : Failed Token IL Hook");
            }
        }

        private static void PauseMenu_ctor(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before,
                //x => x.MatchStfld("Hud.TextPromt", "pausedWarningText"),
                //x => x.MatchBr(out _),
                //x => x.MatchLdarg(2),
                x => x.MatchLdfld("RainWorldGame", "clock"),
                x => x.MatchLdcI4(1200)))
            {
                c.Index += 5;
                c.Next.Operand = 200;
                Debug.Log("Pause IL Hook Success");
            }
            else
            {
                Debug.Log("Pause IL Hook Failed");
            }
        }

        private static void AlwaysPlayMusic(On.Music.MusicPlayer.orig_GameRequestsSong orig, Music.MusicPlayer self, MusicEvent musicEvent)
        {
            self.hasPlayedASongThisCycle = false;
            musicEvent.cyclesRest = 0;
            orig(self, musicEvent);
        }

        private static void SlugcatStats_ctor(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(1.75f),
                x => x.MatchStfld("SlugcatStats", "runspeedFac"),
                x => x.MatchLdarg(0),
                x => x.MatchLdcR4(1.8f)))
            {
                c.Next.Operand = 25f;
                Debug.Log("Wolfo Rivulete insane speed");
            }
            else
            {
                Debug.LogError("Wolfo IL Failed to apply Safer Spaces Cooldown hook");
            }
        }

        /*
        private static void SetSlugcatColorOrder_SetSlugcatColorOrder(On.Menu.SlugcatSelectMenu.orig_SetSlugcatColorOrder orig, SlugcatSelectMenu self)
        {
            orig(self);
            if (ModManager.MSC)
            {
                self.slugcatColorOrder.Add(MoreSlugcatsEnums.SlugcatStatsName.Sofanthiel);
            }
        }
        private bool SlugcatStats_HiddenOrUnplayableSlugcat(On.SlugcatStats.orig_HiddenOrUnplayableSlugcat orig, SlugcatStats.Name i)
        {
            if ((ModManager.MSC && i == MoreSlugcatsEnums.SlugcatStatsName.Sofanthiel))
            {
                return false;
            }
            return orig(i);
        }
        */
    }
}
