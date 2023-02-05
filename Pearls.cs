using BepInEx;
using UnityEngine;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using MoreSlugcats;


namespace WolfoTestMod
{
    public class Pearls : BaseUnityPlugin
    {
        public static void OnEnable()
        {
            On.PlayerProgression.MiscProgressionData.SetPearlDeciphered += MoonPresent_PearlDeciper;
            On.PlayerProgression.MiscProgressionData.SetPebblesPearlDeciphered += FivePebbles_PearlDecipher;
            On.PlayerProgression.MiscProgressionData.SetDMPearlDeciphered += MoonPast_PearlDecipher;
        }

        //public static List<DataPearl.AbstractDataPearl.DataPearlType> storedPearlsMoon = new List<DataPearl.AbstractDataPearl.DataPearlType>();
        //public static List<DataPearl.AbstractDataPearl.DataPearlType> storedPearlsMoonPast = new List<DataPearl.AbstractDataPearl.DataPearlType>();
        //public static List<DataPearl.AbstractDataPearl.DataPearlType> storedPearlsPebbles = new List<DataPearl.AbstractDataPearl.DataPearlType>();
        public static List<DataPearl.AbstractDataPearl.DataPearlType> storedPearls = new List<DataPearl.AbstractDataPearl.DataPearlType>();


        private static bool FivePebbles_PearlDecipher(On.PlayerProgression.MiscProgressionData.orig_SetPebblesPearlDeciphered orig, PlayerProgression.MiscProgressionData self, DataPearl.AbstractDataPearl.DataPearlType pearlType)
        {
            if (self.owner.rainWorld.ExpeditionMode)
            {
                storedPearls.Add(pearlType);
            }
            return orig(self, pearlType);
        }

        private static bool MoonPast_PearlDecipher(On.PlayerProgression.MiscProgressionData.orig_SetDMPearlDeciphered orig, PlayerProgression.MiscProgressionData self, DataPearl.AbstractDataPearl.DataPearlType pearlType)
        {
            if (self.owner.rainWorld.ExpeditionMode)
            {
                storedPearls.Add(pearlType);
            }
            return orig(self, pearlType);
        }

        private static bool MoonPresent_PearlDeciper(On.PlayerProgression.MiscProgressionData.orig_SetPearlDeciphered orig, PlayerProgression.MiscProgressionData self, DataPearl.AbstractDataPearl.DataPearlType pearlType)
        {
            if (self.owner.rainWorld.ExpeditionMode)
            {
                storedPearls.Add(pearlType);
            }
            return orig(self, pearlType);
        }

        public static void SavePearlToMainSaveFile(PlayerProgression progression)
        {
            Debug.Log("ExpeditionSharedProgress : Last Active Slugcat " + RainWorld.lastActiveSaveSlot);
            Debug.Log("ExpeditionSharedProgress : Saving " + storedPearls.Count + " Pearls");
            foreach (DataPearl.AbstractDataPearl.DataPearlType pearlType in storedPearls)
            {
                Debug.Log("ExpeditionSharedProgress : Saving stored Pearl " + pearlType);
                if (RainWorld.lastActiveSaveSlot == MoreSlugcatsEnums.SlugcatStatsName.Artificer)
                {
                    progression.miscProgressionData.SetPebblesPearlDeciphered(pearlType);
                }
                else if (RainWorld.lastActiveSaveSlot == MoreSlugcatsEnums.SlugcatStatsName.Spear)
                {
                    progression.miscProgressionData.SetDMPearlDeciphered(pearlType);
                }
                else
                {
                    progression.miscProgressionData.SetPearlDeciphered(pearlType);
                }
            }
            storedPearls.Clear();
        }
    }
}
