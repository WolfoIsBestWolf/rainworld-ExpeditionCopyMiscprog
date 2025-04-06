using BepInEx;
using UnityEngine;
using System;
using MoreSlugcats;
using Expedition;
using RWCustom;
using Menu.Remix.MixedUI;
using JetBrains.Annotations;
using Menu.Remix;

namespace ExpeditionCopyMiscProg
{
    public class WConfig : OptionInterface
    {
	
		public static WConfig instance = new WConfig();

		/*public static Configurable<bool> cfgPauseWarning = ExpeditionCopyMiscProgConfig.instance.config.Bind<bool>("cfgPauseWarning", false, new ConfigurableInfo("Show the exit warning about losing Karma earlier in Main campaigns to avoid accidentally exiting.", null, "", new object[]
			{
				"Early exit warning on Main"
			}));
		public static Configurable<bool> cfgMusicPlayMore = ExpeditionCopyMiscProgConfig.instance.config.Bind<bool>("cfgMusicPlayMore", false, new ConfigurableInfo("Removes the cycle rest from Music triggers so that they can play every cycle instead of every 5 cycles.", null, "", new object[]
			{
				"Music plays more often"
			}));*/
		public static Configurable<bool> cfgTokenTrackerAll = instance.config.Bind<bool>("cfgTokenTrackerAll", true, 
			new ConfigurableInfo("The token tracker, on the sleep screen, during expeditions, will show all regions.", null, "", new object[]
			{
				"Token Tracker shows all regions"
			}));
        public static Configurable<bool> cfgTokenTrackerColorful = instance.config.Bind<bool>("cfgTokenTrackerColorful", true,
            new ConfigurableInfo("The token tracker will have region colored dots. This is a bug fix for vanilla.", null, "", new object[]
            {
                "Token Tracker colored"
            }));
        public static Configurable<bool> cfgCustomColorMenu = instance.config.Bind<bool>("cfgCustomColorMenu", true,
            new ConfigurableInfo("Adds the custom slugcat color menu from Remix to the Expedition character select screen. Only if Jolly Co-op is disabled (that has it's own).", null, "", new object[]
            {
                "Custom Color menu for Expedition"
            }));
 

        public override void Initialize()
		{
			base.Initialize();
			this.Tabs = new OpTab[]
			{
				new OpTab(this, "Options")
			};
			this.AddCheckbox();
		}


        private void PopulateWithConfigs(int tabIndex, ConfigurableBase[][] lists, [CanBeNull] string[] names, [CanBeNull] Color[] colors, int splitAfter)
        {
            new OpLabel(new Vector2(100f, 560f), new Vector2(400f, 30f), this.Tabs[tabIndex].name, FLabelAlignment.Center, true, null);
            OpTab opTab = this.Tabs[tabIndex];
            float num = 40f;
            float num2 = 20f;
            float num3 = tabIndex == 1 ? 540f : 500f;
            UIconfig uiconfig = null;
            for (int i = 0; i < lists.Length; i++)
            {
                if (names != null)
                {
                    var label = new OpLabel(new Vector2(num2, num3 - num + 10f), new Vector2(260f, 30f), "~ " + names[i] + " ~", FLabelAlignment.Center, true, null);
                    if (colors != null)
                    {
                        label.color = colors[i];
                    }
                    opTab.AddItems(new UIelement[]
                    {
                        label
                    });
                }
                FTextParams ftextParams = new FTextParams();
                if (InGameTranslator.LanguageID.UsesLargeFont(Custom.rainWorld.inGameTranslator.currentLanguage))
                {
                    ftextParams.lineHeightOffset = -12f;
                }
                else
                {
                    ftextParams.lineHeightOffset = -5f;
                }
                for (int j = 0; j < lists[i].Length; j++)
                {
                    switch (ValueConverter.GetTypeCategory(lists[i][j].settingType))
                    {
                        case ValueConverter.TypeCategory.Boolean:
                            {
                                num += 30f;
                                Configurable<bool> configurable = lists[i][j] as Configurable<bool>;
                                OpCheckBox opCheckBox = new OpCheckBox(configurable, new Vector2(num2, num3 - num))
                                {
                                    description = OptionInterface.Translate(configurable.info.description),
                                    sign = i
                                };
                                UIfocusable.MutualVerticalFocusableBind(opCheckBox, uiconfig ?? opCheckBox);
                                OpLabel opLabel = new OpLabel(new Vector2(num2 + 40f, num3 - num), new Vector2(240f, 30f), Custom.ReplaceLineDelimeters(OptionInterface.Translate(configurable.info.Tags[0] as string)), FLabelAlignment.Left, false, ftextParams)
                                {
                                    bumpBehav = opCheckBox.bumpBehav,
                                    description = opCheckBox.description
                                };
                                if (colors != null)
                                {
                                    opCheckBox.colorEdge = colors[i];
                                    opLabel.color = colors[i];
                                }
                                opTab.AddItems(new UIelement[]
                                {
                            opCheckBox,
                                opLabel
                                });
                                uiconfig = opCheckBox;
                                break;
                            }
                        case ValueConverter.TypeCategory.Integrals:
                            {
                                num += 36f;
                                Configurable<int> configurable2 = lists[i][j] as Configurable<int>;
                                OpUpdown opUpdown = new OpUpdown(configurable2, new Vector2(num2, num3 - num), 70f)
                                {
                                    description = OptionInterface.Translate(configurable2.info.description),
                                    sign = i
                                };
                                UIfocusable.MutualVerticalFocusableBind(opUpdown, uiconfig ?? opUpdown);
                                OpLabel opLabel2 = new OpLabel(new Vector2(num2 + 80f, num3 - num), new Vector2(120f, 36f), Custom.ReplaceLineDelimeters(OptionInterface.Translate(configurable2.info.Tags[0] as string)), FLabelAlignment.Left, false, ftextParams)
                                {
                                    bumpBehav = opUpdown.bumpBehav,
                                    description = opUpdown.description
                                };
                                if (colors != null)
                                {
                                    opUpdown.colorEdge = colors[i];
                                    opLabel2.color = colors[i];
                                }
                                opTab.AddItems(new UIelement[]
                                {
                            opUpdown,
                            opLabel2
                                });
                                uiconfig = opUpdown;
                                break;
                            }
                        case ValueConverter.TypeCategory.Floats:
                            {
                                Configurable<float> configurable3 = lists[i][j] as Configurable<float>;
                                byte decimalPoints = 1;
                              
                                num += 36f;
                                OpUpdown opUpdown2 = new OpUpdown(configurable3, new Vector2(num2, num3 - num), 70f, decimalPoints)
                                {
                                    description = OptionInterface.Translate(configurable3.info.description),
                                    sign = i
                                };
                                UIfocusable.MutualVerticalFocusableBind(opUpdown2, uiconfig ?? opUpdown2);
                                OpLabel opLabel3 = new OpLabel(new Vector2(num2 + 80f, num3 - num), new Vector2(120f, 36f), Custom.ReplaceLineDelimeters(OptionInterface.Translate(configurable3.info.Tags[0] as string)), FLabelAlignment.Left, false, ftextParams)
                                {
                                    bumpBehav = opUpdown2.bumpBehav,
                                    description = opUpdown2.description
                                };
                                if (colors != null)
                                {
                                    opUpdown2.colorEdge = colors[i];
                                    opLabel3.color = colors[i];
                                }
                                opTab.AddItems(new UIelement[]
                                {
                            opUpdown2,
                            opLabel3
                                });
                                uiconfig = opUpdown2;
                                break;
                            }
                    }
                }
                if (names != null)
                {
                    num3 -= 70f;
                }
                if (i == splitAfter)
                {
                    num2 += 300f;
                    num3 = tabIndex == 1 ? 540f : 500f;
                    num = 40f;
                    uiconfig = null;
                }
            }
            for (int k = 0; k < lists.Length; k++)
            {
                if (k == 0 || k == 1)
                {
                    lists[k][0].BoundUIconfig.SetNextFocusable(UIfocusable.NextDirection.Up, lists[k][0].BoundUIconfig);
                }
                if (k == 0 || k == lists.Length - 1)
                {
                    lists[k][lists[k].Length - 1].BoundUIconfig.SetNextFocusable(UIfocusable.NextDirection.Down, FocusMenuPointer.GetPointer(FocusMenuPointer.MenuUI.SaveButton));
                }
            }
            int num4 = 0;
            for (int l = 1; l < lists.Length; l++)
            {
                for (int m = 0; m < lists[l].Length; m++)
                {
                    if (lists[l][m].BoundUIconfig != null)
                    {
                        lists[l][m].BoundUIconfig.SetNextFocusable(UIfocusable.NextDirection.Right, lists[l][m].BoundUIconfig);
                        if (num4 < lists[0].Length)
                        {
                            if (lists[0][num4].BoundUIconfig == null)
                            {
                                num4++;
                            }
                            else
                            {
                                UIfocusable.MutualHorizontalFocusableBind(lists[0][num4].BoundUIconfig, lists[l][m].BoundUIconfig);
                                lists[0][num4].BoundUIconfig.SetNextFocusable(UIfocusable.NextDirection.Left, FocusMenuPointer.GetPointer(FocusMenuPointer.MenuUI.CurrentTabButton));
                                num4++;
                            }
                        }
                        else
                        {
                            lists[l][m].BoundUIconfig.SetNextFocusable(UIfocusable.NextDirection.Left, lists[0][lists[0].Length - 1].BoundUIconfig);
                        }
                    }
                }
            }
        }


        private void AddCheckbox()
		{
            ConfigurableBase[][] array = new ConfigurableBase[1][];
            array[0] = new ConfigurableBase[]
            {
                cfgTokenTrackerAll,
                cfgTokenTrackerColorful,
                cfgCustomColorMenu,
            };
            string[] names = new string[]
             {
                "Config",
             };
            instance.PopulateWithConfigs(0, array, names, null, 0);

        }



    }
}
