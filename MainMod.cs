using BepInEx;
using UnityEngine;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System;
using System.Linq;
using System.Collections.Generic;
using MoreSlugcats;

namespace ExpeditionCopyMiscProg
{
    [BepInPlugin("wolfo.expeditioncopymiscprog", "ExpeditionCopyMiscProg", "1.0.1")]
    public class ExpeditionCopyMiscProg : BaseUnityPlugin
    {
        public void OnEnable()
        {
            On.RainWorld.OnModsInit += AddConfigHook;
            //Debug Messages here apparently dont play
            //Maybe reuse and clear lists idk
            On.PlayerProgression.LoadProgression += PlayerProgression_LoadProgression;
            On.PlayerProgression.SaveProgression += PlayerProgression_SaveProgression;
            On.Menu.OptionsMenu.SetCurrentlySelectedOfSeries += OptionsSaveSlotButtonProtect;

            On.RainWorldGame.ctor += AddILHooks;

        }

        //static readonly string version = "1.0.1";


        private void AddConfigHook(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);
            Debug.Log("Wolfo Mod Loaded");
            MachineConnector.SetRegisteredOI("expeditioncopymiscprog", ExpeditionCopyMiscProgConfig.instance);
        }


        private void OptionsSaveSlotButtonProtect(On.Menu.OptionsMenu.orig_SetCurrentlySelectedOfSeries orig, Menu.OptionsMenu self, string series, int to)
        {
            //Debug.Log("SetCurrentlySelectedOfSeries "+series);
            if (series == "SaveSlot")
            {
                hasStoredData = false;
                wasExpedition = 0;
            }
            orig(self, series, to);
        }

        private static int wasExpedition = 0;

        private void PlayerProgression_LoadProgression(On.PlayerProgression.orig_LoadProgression orig, PlayerProgression self)
        {
            orig(self); 
            if (self.HasSaveData)
            {
                Debug.Log("ExpeditionCopyMiscProg: Loading SaveSlot " + self.rainWorld.options.saveSlot);
                if (!hasStoredData)
                {
                    CopyDataFromSave(self);
                }
                else if (!self.rainWorld.ExpeditionMode && wasExpedition == 1 && hasStoredData)
                {
                    Debug.Log("ExpeditionCopyMiscProg: Saving to Main File");
                    CopyDataToSave(self);
                    wasExpedition = 0;  //This is to Main file
                }
                else if (self.rainWorld.ExpeditionMode && wasExpedition == 0 && hasStoredData)
                {
                    Debug.Log("ExpeditionCopyMiscProg: Saving to Expedition File");
                    CopyDataToSave(self);
                    wasExpedition = 1; //This is to Expd file
                }
            }
        }

        private bool PlayerProgression_SaveProgression(On.PlayerProgression.orig_SaveProgression orig, PlayerProgression self, bool saveMaps, bool saveMiscProg)
        {
            Debug.Log("ExpeditionCopyMiscProg: Saving SaveSlot " + self.rainWorld.options.saveSlot);
            bool temp = orig(self, saveMaps, saveMiscProg);
            CopyDataFromSave(self);
            return temp;
        }


        private static bool hasStoredData = false;

        //private static Dictionary<string, Texture2D>? mapDiscoveryTexturesInTrans; //Not how Map Progress is Saved 

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
            Debug.Log("ExpeditionCopyMiscProg: Copying Data ");
        }


        private static void CopyDataToSave(PlayerProgression self)
        {
            #pragma warning disable CS8602 // Dereference of a possibly null reference.
            //Debug.Log("Pre  Tokens Amount B:" + self.miscProgressionData.sandboxTokens.Count+" G:"+self.miscProgressionData.levelTokens.Count);
            if (self.miscProgressionData.sandboxTokens.Count < sandboxTokensInTrans.Count)
            {
                self.miscProgressionData.sandboxTokens = new List<MultiplayerUnlocks.SandboxUnlockID>(sandboxTokensInTrans);
            }
            if (self.miscProgressionData.levelTokens.Count < levelTokensInTrans.Count)
            {
                self.miscProgressionData.levelTokens = new List<MultiplayerUnlocks.LevelUnlockID>(levelTokensInTrans);
            }
            if (self.miscProgressionData.safariTokens.Count < safariTokensInTrans.Count)
            {
                self.miscProgressionData.safariTokens = new List<MultiplayerUnlocks.SafariUnlockID>(safariTokensInTrans);
            }
            if (self.miscProgressionData.classTokens.Count < classTokensInTrans.Count)
            {
                self.miscProgressionData.classTokens = new List<MultiplayerUnlocks.SlugcatUnlockID>(classTokensInTrans);
            }
            if (self.miscProgressionData.discoveredBroadcasts.Count < discoveredBroadcastsInTrans.Count)
            {
                self.miscProgressionData.discoveredBroadcasts = new List<ChatlogData.ChatlogID>(discoveredBroadcastsInTrans);
            }
            //Debug.Log("Post Tokens Amount B:" + self.miscProgressionData.sandboxTokens.Count+" G:"+self.miscProgressionData.levelTokens.Count);
            
            self.miscProgressionData.decipheredFuturePearls = new List<DataPearl.AbstractDataPearl.DataPearlType>(decipheredFuturePearlsInTrans);
            //Pearl retrofitting
            decipheredPearlsInTrans.AddRange(self.miscProgressionData.decipheredPearls);
            self.miscProgressionData.decipheredPearls = decipheredPearlsInTrans.Distinct().ToList();

            decipheredDMPearlsInTrans.AddRange(self.miscProgressionData.decipheredDMPearls);
            self.miscProgressionData.decipheredDMPearls = decipheredDMPearlsInTrans.Distinct().ToList();

            decipheredPebblesPearlsInTrans.AddRange(self.miscProgressionData.decipheredPebblesPearls);
            self.miscProgressionData.decipheredPebblesPearls = decipheredPebblesPearlsInTrans.Distinct().ToList();

            hasStoredData = false;
            Debug.Log("ExpeditionCopyMiscProg: Storing Data");
            self.SyncLoadModState(); //Randomly started being needed???
            self.SaveProgression(false,true);
        }

        private static void AddILHooks(On.RainWorldGame.orig_ctor orig, RainWorldGame game, ProcessManager manager)
        {
            Debug.Log("IL Hooks being added");
            IL.Room.Loaded += EnableTokensInExpedition;
            IL.Menu.SleepAndDeathScreen.GetDataFromGame += TokenTrackerInExpedition;
            if (ExpeditionCopyMiscProgConfig.earlierPauseWarning.Value)
            {
                IL.Menu.PauseMenu.ctor += PauseMenu_ctor;
            }
            if (ExpeditionCopyMiscProgConfig.musicPlayMore.Value)
            {
                On.Music.MusicPlayer.GameRequestsSong += AlwaysPlayMusic;
            }
            orig(game, manager);
            On.RainWorldGame.ctor -= AddILHooks; //Seems fine to only ever run this once
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
                Debug.Log("ExpeditionCopyMiscProg : Token Tracker Hook Success");
            }
            else
            {
                Debug.Log("ExpeditionCopyMiscProg : Token Tracker Hook Failed");
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
                Debug.Log("ExpeditionCopyMiscProg : Token IL Hook Succeeded");
            }
            else
            {
                Debug.Log("ExpeditionCopyMiscProg : Failed Token IL Hook");
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
                c.Index += 1;
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


    }
}
