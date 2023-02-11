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
    [BepInPlugin("wolfo.expeditioncopymiscprog", "ExpeditionCopyMiscProg", "1.0.3")]
    public class ExpeditionCopyMiscProg : BaseUnityPlugin
    {
        public void OnEnable()
        {
            On.RainWorld.OnModsInit += AddConfigHook;
            //Since Official system added just imitating that
            On.PlayerProgression.Destroy += CopyingData;
            On.PlayerProgression.MiscProgressionData.FromString += SavingData;

            On.Menu.OptionsMenu.SetCurrentlySelectedOfSeries += OptionsSaveSlotButtonProtect; //Idk if still needed

            On.RainWorldGame.ctor += AddILHooks;

        }

        private void AddConfigHook(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);
            Debug.Log("ExpeditionCopyMiscProg Loaded");
            MachineConnector.SetRegisteredOI("expeditioncopymiscprog", ExpeditionCopyMiscProgConfig.instance);
        }

        private static bool hasStoredData = false;

        private static List<MultiplayerUnlocks.SandboxUnlockID> sandboxTokensInTrans = new List<MultiplayerUnlocks.SandboxUnlockID>();
        private static List<MultiplayerUnlocks.LevelUnlockID> levelTokensInTrans = new List<MultiplayerUnlocks.LevelUnlockID>();
        private static List<MultiplayerUnlocks.SafariUnlockID> safariTokensInTrans = new List<MultiplayerUnlocks.SafariUnlockID>();
        private static List<MultiplayerUnlocks.SlugcatUnlockID> classTokensInTrans = new List<MultiplayerUnlocks.SlugcatUnlockID>();

        private void CopyingData(On.PlayerProgression.orig_Destroy orig, PlayerProgression self, int previousSaveSlot)
        {
            //Debug.Log("ExpeditionCopyMiscProg: PlayerProgression_Destroy : PreviousSaveSlot: " + previousSaveSlot + " CurrentSaveSlot: " + self.rainWorld.options.saveSlot);
            if (previousSaveSlot < 0 && self.rainWorld.options.saveSlot >= 0 || previousSaveSlot >= 0 && self.rainWorld.options.saveSlot < 0)
            {
                Debug.Log("ExpeditionCopyMiscProg: Copying Data from "+previousSaveSlot);
                sandboxTokensInTrans = self.miscProgressionData.sandboxTokens;
                levelTokensInTrans = self.miscProgressionData.levelTokens;
                safariTokensInTrans = self.miscProgressionData.safariTokens;
                classTokensInTrans = self.miscProgressionData.classTokens;
                hasStoredData = true;
            }
            else
            {
                Debug.Log("ExpeditionCopyMiscProg: Discarding copied Data");
                sandboxTokensInTrans.Clear();
                levelTokensInTrans.Clear();
                safariTokensInTrans.Clear();
                classTokensInTrans.Clear();
                hasStoredData = false;
            }
            
            orig(self, previousSaveSlot);
        }

        private void SavingData(On.PlayerProgression.MiscProgressionData.orig_FromString orig, PlayerProgression.MiscProgressionData self, string s)
        {
            orig(self, s);
            //Debug.Log("ExpeditionCopyMiscProg: PlayerProgression.MiscProgressionData_FromString : CurrentSaveSlot: " + self.owner.rainWorld.options.saveSlot);
            //Debug.Log("ExpeditionCopyMiscProg: Pre Tokens Amount B:" + self.sandboxTokens.Count + " G:" + self.levelTokens.Count + " R:" + self.safariTokens.Count + " G:" + self.classTokens.Count);
            if (self.owner.rainWorld.options != null && hasStoredData)
            {
                self.owner.SyncLoadModState(); //Why is this needed still
                Debug.Log("ExpeditionCopyMiscProg: Saving Data to "+self.owner.rainWorld.options.saveSlot);
                if (self.sandboxTokens.Count < sandboxTokensInTrans.Count)
                {
                    self.sandboxTokens = new List<MultiplayerUnlocks.SandboxUnlockID>(sandboxTokensInTrans);
                }
                if (self.levelTokens.Count < levelTokensInTrans.Count)
                {
                    self.levelTokens = new List<MultiplayerUnlocks.LevelUnlockID>(levelTokensInTrans);
                }
                if (self.safariTokens.Count < safariTokensInTrans.Count)
                {
                    self.safariTokens = new List<MultiplayerUnlocks.SafariUnlockID>(safariTokensInTrans);
                }
                if (self.classTokens.Count < classTokensInTrans.Count)
                {
                    self.classTokens = new List<MultiplayerUnlocks.SlugcatUnlockID>(classTokensInTrans);
                }
                self.owner.SaveProgression(false, true);
                //Debug.Log("ExpeditionCopyMiscProg: Post Tokens Amount B:" + self.sandboxTokens.Count + " G:" + self.levelTokens.Count + " R:" + self.safariTokens.Count + " G:" + self.classTokens.Count);
            }
           
        }


        private void OptionsSaveSlotButtonProtect(On.Menu.OptionsMenu.orig_SetCurrentlySelectedOfSeries orig, Menu.OptionsMenu self, string series, int to)
        {
            //Debug.Log("SetCurrentlySelectedOfSeries "+series);
            if (series == "SaveSlot")
            {
                sandboxTokensInTrans.Clear();
                levelTokensInTrans.Clear();
                safariTokensInTrans.Clear();
                classTokensInTrans.Clear();
                hasStoredData = false;
            }
            orig(self, series, to);
        }

        private static void AddILHooks(On.RainWorldGame.orig_ctor orig, RainWorldGame game, ProcessManager manager)
        {
            Debug.Log("ExpeditionCopyMiscProg: IL Hooks being added");
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
                Debug.Log("ExpeditionCopyMiscProg: Token Tracker Hook Success");
            }
            else
            {
                Debug.Log("ExpeditionCopyMiscProg: Token Tracker Hook Failed");
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
                Debug.Log("ExpeditionCopyMiscProg: Token IL Hook Succeeded");
            }
            else
            {
                Debug.Log("ExpeditionCopyMiscProg: Failed Token IL Hook");
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
                Debug.Log("ExpeditionCopyMiscProg: Pause IL Hook Success");
            }
            else
            {
                Debug.Log("ExpeditionCopyMiscProg: Pause IL Hook Failed");
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
