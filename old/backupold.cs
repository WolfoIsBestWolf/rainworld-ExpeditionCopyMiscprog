/*
    [BepInPlugin("wolfo.wolfotestmod", "WolfoTestMod", "1.0")]
    public class WolfoTestMod : BaseUnityPlugin
    {



        public static void SavePearlToMainSaveFile(ProcessManager manager, DataPearl.AbstractDataPearl.DataPearlType pearlType)
        {
            if (!manager.rainWorld.ExpeditionMode)
            {
                return;
            }
            Debug.Log(RainWorld.lastActiveSaveSlot.value);
            Debug.Log("SetPearlToMain"+pearlType);
            Debug.LogWarning("OriginalSaveSlot" + manager.rainWorld.options.saveSlot);
            manager.rainWorld.options.saveSlot = -(manager.rainWorld.options.saveSlot + 1);
            manager.rainWorld.progression.Destroy();
            manager.rainWorld.progression = new PlayerProgression(manager.rainWorld, true);
           
            Debug.Log("GotPastSaveSlotChange " + manager.rainWorld.options.saveSlot);

           Debug.Log(RainWorld.lastActiveSaveSlot.value);

           if (RainWorld.lastActiveSaveSlot == MoreSlugcatsEnums.SlugcatStatsName.Artificer)
           {
               manager.rainWorld.progression.miscProgressionData.SetPebblesPearlDeciphered(pearlType);
           }
           else if (RainWorld.lastActiveSaveSlot == MoreSlugcatsEnums.SlugcatStatsName.Spear)
           {
               manager.rainWorld.progression.miscProgressionData.SetDMPearlDeciphered(pearlType);
           }
           else
           {
               manager.rainWorld.progression.miscProgressionData.SetPearlDeciphered(pearlType);
           }
           Debug.Log("SavedPearl "+pearlType);
           manager.rainWorld.options.saveSlot = -(manager.rainWorld.options.saveSlot + 1);
           manager.rainWorld.progression.Destroy();
           manager.rainWorld.progression = new PlayerProgression(manager.rainWorld, true);
           Debug.Log("SaveSlotChangedBackToExpd" + manager.rainWorld.options.saveSlot);


*/


/*
private void MenuScene_BuildMSLandscapeScene(On.Menu.MenuScene.orig_BuildMSLandscapeScene orig, MenuScene self)
{
    self.sceneFolder = "Scenes" + System.IO.Path.DirectorySeparatorChar.ToString() + "Landscape - MS";
    if (self.flatMode)
    {
        self.AddIllustration(new MenuIllustration(self.menu, self, self.sceneFolder, "Landscape - MS - Flat -old", new Vector2(683f, 384f), false, true));
    }
    else
    {
        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "MS_Landscape - 8 -old", new Vector2(85f, 91f), 8f, MenuDepthIllustration.MenuShader.Normal));
        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "MS_Landscape - 7 -old", new Vector2(85f, 91f), 8f, MenuDepthIllustration.MenuShader.LightEdges));
        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "MS_Landscape - 6 -old", new Vector2(85f, 91f), 8f, MenuDepthIllustration.MenuShader.Normal));
        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "MS_Landscape - 5 -old", new Vector2(85f, 91f), 6f, MenuDepthIllustration.MenuShader.LightEdges));
        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "MS_Landscape - 4 -old", new Vector2(85f, 91f), 3f, MenuDepthIllustration.MenuShader.Normal));
        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "MS_Landscape - 3 -old", new Vector2(85f, 91f), 2f, MenuDepthIllustration.MenuShader.Normal));
        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "MS_Landscape - 2 -old", new Vector2(143f, 81f), 1.1f, MenuDepthIllustration.MenuShader.Normal));
        self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "MS_Landscape - 1 -old", new Vector2(96f, 39f), 0.5f, MenuDepthIllustration.MenuShader.Normal));
    }
    if (self.menu.ID == ProcessManager.ProcessID.FastTravelScreen || self.menu.ID == ProcessManager.ProcessID.RegionsOverviewScreen)
    {
        self.AddIllustration(new MenuIllustration(self.menu, self, string.Empty, "Title_MS_Shadow", new Vector2(0.01f, 0.01f), true, false));
        self.AddIllustration(new MenuIllustration(self.menu, self, string.Empty, "Title_MS", new Vector2(0.01f, 0.01f), true, false));
        self.flatIllustrations[self.flatIllustrations.Count - 1].sprite.shader = self.menu.manager.rainWorld.Shaders["MenuText"];
    }
}
*/
/*
 *         public static void EnableSexsInExpedition(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdstr("Preventing Token spawn in Expedition mode")))
            {
                c.Next.Operand = "Sex";
                Debug.Log("Wolfo IL Sexcess");
            }
            else
            {
                Debug.LogError("Wolfo IL Failed to apply Safer Spaces Cooldown hook");
            }
        }
*/
