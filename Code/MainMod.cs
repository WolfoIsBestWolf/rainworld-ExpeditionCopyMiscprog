using BepInEx;
using UnityEngine;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System;
using System.Linq;
using System.Collections.Generic;
using MoreSlugcats;
using Expedition;
using RWCustom;
using System.Globalization;
using Unity.Mathematics;

namespace ExpeditionCopyMiscProg
{
    [BepInPlugin("wolfo.expeditioncopymiscprog", "ExpeditionCopyMiscProg", "1.2.2")]
    public class ExpeditionCopyMiscProg : BaseUnityPlugin
    {
        public static bool initialized = false;
        public void OnEnable()
        {
            On.RainWorld.OnModsInit += AddConfigHook;
        }

        public static bool hasStoredData = false;
        public static List<MultiplayerUnlocks.SandboxUnlockID> transferTokenSandbox = new List<MultiplayerUnlocks.SandboxUnlockID>();
        public static List<MultiplayerUnlocks.LevelUnlockID> transferTokenArena = new List<MultiplayerUnlocks.LevelUnlockID>();
        public static List<MultiplayerUnlocks.SafariUnlockID> transferTokenSafari = new List<MultiplayerUnlocks.SafariUnlockID>();
        public static List<MultiplayerUnlocks.SlugcatUnlockID> transferTokenSlug = new List<MultiplayerUnlocks.SlugcatUnlockID>();
        public static List<ChatlogData.ChatlogID> transferTokenChatlog = new List<ChatlogData.ChatlogID>();

        public void AddConfigHook(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);
            if (initialized)
            {
                return;
            }
            initialized = true;
            UnityEngine.Debug.Log("ExpeditionCopyMiscProg Loaded");
            MachineConnector.SetRegisteredOI("expeditioncopymiscprog", WConfig.instance);

            //Since Official system added just imitating that
            IL.Room.Loaded += EnableTokensInExpedition;

            On.PlayerProgression.Destroy += CopyingData;
            On.PlayerProgression.MiscProgressionData.FromString += SavingData;

            On.Menu.OptionsMenu.SetCurrentlySelectedOfSeries += OptionsSaveSlotButtonProtect; //Idk if still needed
            On.Region.RegionColor += Region_RegionColor;

           
            IL.Menu.SleepAndDeathScreen.GetDataFromGame += TokenTrackerInExpedition;
            IL.MoreSlugcats.CollectiblesTracker.ctor += TokenTrackerAllRegions;

            //Maybe read in broadcast progress from the save file,
            //And then just choose next one based on that.
            //On.MoreSlugcats.ChatlogData.getLinearBroadcast += ChatlogData_getLinearBroadcast;
          
        }
        private Color Region_RegionColor(On.Region.orig_RegionColor orig, string regionName)
        {
            if (WConfig.cfgTokenTrackerColorful.Value)
            {
                regionName = regionName.ToUpperInvariant();
            }
            return orig(regionName);
        }

        private string[] ChatlogData_getLinearBroadcast(On.MoreSlugcats.ChatlogData.orig_getLinearBroadcast orig, int id, bool postPebbles)
        {
            if (Custom.rainWorld.ExpeditionMode)
            {
              
            }

            return orig(id,postPebbles);
        }
 

        public void CopyingData(On.PlayerProgression.orig_Destroy orig, PlayerProgression self, int previousSaveSlot)
        {
            //Debug.Log("ExpeditionCopyMiscProg: PlayerProgression_Destroy : PreviousSaveSlot: " + previousSaveSlot + " CurrentSaveSlot: " + self.rainWorld.options.saveSlot);
            //Less than 0 is Expd and current not Expd, then other going from Main to Expd
            if (previousSaveSlot < 0 && self.rainWorld.options.saveSlot >= 0 || previousSaveSlot >= 0 && self.rainWorld.options.saveSlot < 0)
            {
                Debug.Log("ExpeditionCopyMiscProg: Copying Data from "+previousSaveSlot);
                transferTokenSandbox = self.miscProgressionData.sandboxTokens;
                transferTokenArena = self.miscProgressionData.levelTokens;
                transferTokenSafari = self.miscProgressionData.safariTokens;
                transferTokenSlug = self.miscProgressionData.classTokens;
                transferTokenChatlog = self.miscProgressionData.discoveredBroadcasts;
                hasStoredData = true;
            }
            else
            {
                Debug.Log("ExpeditionCopyMiscProg: Discarding copied Data");
                transferTokenSandbox.Clear();
                transferTokenArena.Clear();
                transferTokenSafari.Clear();
                transferTokenSlug.Clear();
                transferTokenChatlog.Clear();
                hasStoredData = false;
            }
            
            orig(self, previousSaveSlot);
        }

        public void SavingData(On.PlayerProgression.MiscProgressionData.orig_FromString orig, PlayerProgression.MiscProgressionData self, string s)
        {
            orig(self, s);
            //Debug.Log("ExpeditionCopyMiscProg: PlayerProgression.MiscProgressionData_FromString : CurrentSaveSlot: " + self.owner.rainWorld.options.saveSlot);
            //Debug.Log("ExpeditionCopyMiscProg: Pre Tokens Amount B:" + self.sandboxTokens.Count + " G:" + self.levelTokens.Count + " R:" + self.safariTokens.Count + " G:" + self.classTokens.Count);
            if (self.owner.rainWorld.options != null && hasStoredData)
            {
                self.owner.SyncLoadModState(); //Why is this needed still
                Debug.Log("ExpeditionCopyMiscProg: Saving Data to " + self.owner.rainWorld.options.saveSlot);

                if (self.sandboxTokens.Count < transferTokenSandbox.Count)
                {
                    self.sandboxTokens = new List<MultiplayerUnlocks.SandboxUnlockID>(transferTokenSandbox);
                }
                if (self.levelTokens.Count < transferTokenArena.Count)
                {
                    self.levelTokens = new List<MultiplayerUnlocks.LevelUnlockID>(transferTokenArena);
                }
                if (self.safariTokens.Count < transferTokenSafari.Count)
                {
                    self.safariTokens = new List<MultiplayerUnlocks.SafariUnlockID>(transferTokenSafari);
                }
                if (self.classTokens.Count < transferTokenSlug.Count)
                {
                    self.classTokens = new List<MultiplayerUnlocks.SlugcatUnlockID>(transferTokenSlug);
                }
                if (self.discoveredBroadcasts.Count < transferTokenChatlog.Count)
                {
                    self.discoveredBroadcasts = new List<ChatlogData.ChatlogID>(transferTokenChatlog);
                }
                self.owner.SaveProgression(false, true);
                //Debug.Log("ExpeditionCopyMiscProg: Post Tokens Amount B:" + self.sandboxTokens.Count + " G:" + self.levelTokens.Count + " R:" + self.safariTokens.Count + " G:" + self.classTokens.Count);
            }
           
        }


        public void OptionsSaveSlotButtonProtect(On.Menu.OptionsMenu.orig_SetCurrentlySelectedOfSeries orig, Menu.OptionsMenu self, string series, int to)
        {
            //Debug.Log("SetCurrentlySelectedOfSeries "+series);
            if (series == "SaveSlot")
            {
                transferTokenSandbox.Clear();
                transferTokenArena.Clear();
                transferTokenSafari.Clear();
                transferTokenSlug.Clear();
                transferTokenChatlog.Clear();
                hasStoredData = false;
            }
            orig(self, series, to);
        }

 
        private static void TokenTrackerAllRegions(ILContext il)
        {
            ILCursor c = new(il);
            c.TryGotoNext(MoveType.Before,
             x => x.MatchLdstr("ctNone"));

            if (c.TryGotoPrev(MoveType.After,
             x => x.MatchCallOrCallvirt("System.Collections.Generic.List`1<System.String>", "Contains")))
            {
                c.EmitDelegate<Func<bool, bool>>((karma) =>
                {
                    if (Custom.rainWorld.ExpeditionMode)
                    {
                        if (WConfig.cfgTokenTrackerAll.Value)
                        {
                            return true;
                        }
                    }   
                    return karma;
                });
                //Debug.Log("ExpeditionCopyMiscProg: TokenTrackerAllRegions");
            }
            else
            {
                Debug.Log("ExpeditionCopyMiscProg:TokenTrackerAllRegions Failed");
            }

 
        }

        public static void TokenTrackerInExpedition(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.After,
                x => x.MatchCallOrCallvirt("Menu.SleepAndDeathScreen", "get_IsStarveScreen"),
                x => x.MatchBrfalse(out _),
                x => x.MatchLdsfld("ModManager", "Expedition")))
            {
                c.EmitDelegate<Func<bool, bool>>((self) =>
                {
                    return false;
                });
                //Debug.Log("ExpeditionCopyMiscProg: Token Tracker Hook Success");
            }
            else
            {
                Debug.Log("ExpeditionCopyMiscProg: Token Tracker Hook Failed");
            }
        }

        public static void EnableTokensInExpedition(ILContext il)
        {
            ILCursor c = new(il);
            bool bool1 = c.TryGotoNext(MoveType.After,
                x => x.MatchLdsfld("PlacedObject/Type", "BlueToken"));
            bool bool2 = c.TryGotoPrev(MoveType.After,
                x => x.MatchCallvirt("RainWorld", "get_ExpeditionMode"));
            if (bool1 && bool2)
            {
                c.EmitDelegate<Func<bool, bool>>((self) =>
                {
                    return false;
                });
            }
            else
            {
                Debug.Log("ExpeditionCopyMiscProg: Failed Token IL Hook");
            }
        }

        

        


    }
}
