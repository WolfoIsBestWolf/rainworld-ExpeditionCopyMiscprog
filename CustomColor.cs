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

namespace ExpeditionCopyMiscProg
{
    public class CustomColor : OptionInterface
    {

        public static void ExpdCustomColor_Singal(On.Menu.CharacterSelectPage.orig_Singal orig, Menu.CharacterSelectPage self, Menu.MenuObject sender, string message)
        {
            orig(self, sender, message);
            if (ModManager.MMF && self.menu.manager.rainWorld.progression.miscProgressionData.colorsEnabled.ContainsKey(ExpeditionData.slugcatPlayer.value) && self.menu.manager.rainWorld.progression.miscProgressionData.colorsEnabled[ExpeditionData.slugcatPlayer.value])
            {
                List<Color> list = new List<Color>();
                for (int i = 0; i < self.menu.manager.rainWorld.progression.miscProgressionData.colorChoices[ExpeditionData.slugcatPlayer.value].Count; i++)
                {
                    Vector3 vector = new Vector3(1f, 1f, 1f);
                    if (self.menu.manager.rainWorld.progression.miscProgressionData.colorChoices[ExpeditionData.slugcatPlayer.value][i].Contains(","))
                    {
                        string[] array = self.menu.manager.rainWorld.progression.miscProgressionData.colorChoices[ExpeditionData.slugcatPlayer.value][i].Split(new char[]
                        {
                            ','
                        });
                        vector = new Vector3(float.Parse(array[0], NumberStyles.Any, CultureInfo.InvariantCulture), float.Parse(array[1], NumberStyles.Any, CultureInfo.InvariantCulture), float.Parse(array[2], NumberStyles.Any, CultureInfo.InvariantCulture));
                    }
                    list.Add(Custom.HSL2RGB(vector[0], vector[1], vector[2]));
                }
                PlayerGraphics.customColors = list;
            }
            else
            {
                PlayerGraphics.customColors = null;
            }
        }


        public static void ExpdCustomColor_StartButton_OnPressDone(On.Menu.ChallengeSelectPage.orig_StartButton_OnPressDone orig, Menu.ChallengeSelectPage self, Menu.Remix.MixedUI.UIfocusable trigger)
        {
            orig(self, trigger);
            if (ModManager.MMF && self.menu.manager.rainWorld.progression.miscProgressionData.colorsEnabled.ContainsKey(ExpeditionData.slugcatPlayer.value) && self.menu.manager.rainWorld.progression.miscProgressionData.colorsEnabled[ExpeditionData.slugcatPlayer.value])
            {
                List<Color> list = new List<Color>();
                for (int i = 0; i < self.menu.manager.rainWorld.progression.miscProgressionData.colorChoices[ExpeditionData.slugcatPlayer.value].Count; i++)
                {
                    Vector3 vector = new Vector3(1f, 1f, 1f);
                    if (self.menu.manager.rainWorld.progression.miscProgressionData.colorChoices[ExpeditionData.slugcatPlayer.value][i].Contains(","))
                    {
                        string[] array = self.menu.manager.rainWorld.progression.miscProgressionData.colorChoices[ExpeditionData.slugcatPlayer.value][i].Split(new char[]
                        {
                            ','
                        });
                        vector = new Vector3(float.Parse(array[0], NumberStyles.Any, CultureInfo.InvariantCulture), float.Parse(array[1], NumberStyles.Any, CultureInfo.InvariantCulture), float.Parse(array[2], NumberStyles.Any, CultureInfo.InvariantCulture));
                    }
                    list.Add(Custom.HSL2RGB(vector[0], vector[1], vector[2]));
                }
                PlayerGraphics.customColors = list;
            }
            else
            {
                PlayerGraphics.customColors = null;
            }
        }

    }
}
