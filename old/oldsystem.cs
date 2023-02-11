


On.PlayerProgression.LoadProgression += PlayerProgression_LoadProgression;
On.PlayerProgression.SaveProgression += PlayerProgression_SaveProgression;

//private static Dictionary<string, Texture2D>? mapDiscoveryTexturesInTrans; //Not how Map Progress is Saved 

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


private static void CopyDataFromSave(PlayerProgression self)
{
    sandboxTokensInTrans = new List<MultiplayerUnlocks.SandboxUnlockID>(self.miscProgressionData.sandboxTokens);
    levelTokensInTrans = new List<MultiplayerUnlocks.LevelUnlockID>(self.miscProgressionData.levelTokens);
    safariTokensInTrans = new List<MultiplayerUnlocks.SafariUnlockID>(self.miscProgressionData.safariTokens);
    classTokensInTrans = new List<MultiplayerUnlocks.SlugcatUnlockID>(self.miscProgressionData.classTokens);

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


    hasStoredData = false;
    Debug.Log("ExpeditionCopyMiscProg: Storing Data");
    self.SyncLoadModState(); //Randomly started being needed???
    self.SaveProgression(false, true);
}
