using BepInEx;
using UnityEngine;
using System;
using MoreSlugcats;
using Expedition;
using RWCustom;
using Menu.Remix.MixedUI;

namespace ExpeditionCopyMiscProg
{
    public class ExpeditionCopyMiscProgConfig : OptionInterface
    {
	

		public static ExpeditionCopyMiscProgConfig instance = new ExpeditionCopyMiscProgConfig();

		public static Configurable<bool> cfgPauseWarning = ExpeditionCopyMiscProgConfig.instance.config.Bind<bool>("cfgPauseWarning", false, new ConfigurableInfo("Show the exit warning about losing Karma earlier in Main campaigns to avoid accidentally exiting.", null, "", new object[]
			{
				"Early exit warning on Main"
			}));
		public static Configurable<bool> cfgMusicPlayMore = ExpeditionCopyMiscProgConfig.instance.config.Bind<bool>("cfgMusicPlayMore", false, new ConfigurableInfo("Removes the cycle rest from Music triggers so that they can play every cycle instead of every 5 cycles.", null, "", new object[]
			{
				"Music plays more often"
			}));
		public static Configurable<bool> cfgCustomColor = ExpeditionCopyMiscProgConfig.instance.config.Bind<bool>("cfgCustomColor", true, new ConfigurableInfo("Expedition will use the Custom Colors defined in Main.", null, "", new object[]
			{
				"Custom Color shared"
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





		private void AddCheckbox()
		{
			float lableY = 2.5f;
			float labelX = 35f;
			OpCheckBox OptCustomColor = new OpCheckBox(cfgCustomColor, new Vector2(20f, 400f))
			{
				description = cfgCustomColor.info.description
			};
			OpLabel LabelCustomColor = new OpLabel(20f+ labelX, 400f + 2.5f, cfgCustomColor.info.Tags[0] as string, false)
			{
				description = cfgCustomColor.info.description
			};
			OpCheckBox OptPauseWarning = new OpCheckBox(cfgPauseWarning, new Vector2(20f, 400f-40*1))
			{
				description = cfgPauseWarning.info.description
			};
			OpLabel LabelPauseWarning = new OpLabel(20f + labelX, 400f + 2.5f - 40 * 1, cfgPauseWarning.info.Tags[0] as string, false)
			{
				description = cfgPauseWarning.info.description
			};
			OpCheckBox OptMusicPlayMore = new OpCheckBox(cfgMusicPlayMore, new Vector2(20f, 400f - 40 * 2))
			{
				description = cfgMusicPlayMore.info.description
			};
			OpLabel LabelMusicPlayMore = new OpLabel(20f + labelX, 400f + 2.5f - 40 * 2, cfgMusicPlayMore.info.Tags[0] as string, false)
			{
				description = cfgMusicPlayMore.info.description
			};
			this.Tabs[0].AddItems(new UIelement[]
			{
				OptCustomColor,
				LabelCustomColor,
				OptPauseWarning,
				LabelPauseWarning,
				OptMusicPlayMore,
				LabelMusicPlayMore
			});
		}



	}
}
