using BepInEx;
using UnityEngine;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System;
using System.Linq;
using System.Collections.Generic;
using MoreSlugcats;
using Menu.Remix.MixedUI;

namespace ExpeditionCopyMiscProg
{
    public class ExpeditionCopyMiscProgConfig : OptionInterface
    {
	

		public static ExpeditionCopyMiscProgConfig instance = new ExpeditionCopyMiscProgConfig();

		public static Configurable<bool> earlierPauseWarning = ExpeditionCopyMiscProgConfig.instance.config.Bind<bool>("earlierPauseWarning", false, new ConfigurableInfo("Show the exit warning about losing Karma earlier in Main campaigns to avoid accidentally exiting.", null, "", new object[]
			{
				"Early exit warning on Main"
			}));
		public static Configurable<bool> musicPlayMore = ExpeditionCopyMiscProgConfig.instance.config.Bind<bool>("musicPlayMore", false, new ConfigurableInfo("Removes the cycle rest from Music triggers so that they can play every cycle instead of every 5 cycles.", null, "", new object[]
{
				"Music plays more often"
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
			OpCheckBox opCheckBox = new OpCheckBox(earlierPauseWarning, new Vector2(10f, 500f))
			{
				description = earlierPauseWarning.info.description
			};
			OpLabel opLabel = new OpLabel(50f, 505f, earlierPauseWarning.info.Tags[0] as string, false)
			{
				description = earlierPauseWarning.info.description
			};
			OpCheckBox opCheckBox2 = new OpCheckBox(musicPlayMore, new Vector2(10f, 470f))
			{
				description = musicPlayMore.info.description
			};
			OpLabel opLabel2 = new OpLabel(50f, 475f, musicPlayMore.info.Tags[0] as string, false)
			{
				description = musicPlayMore.info.description
			};
			this.Tabs[0].AddItems(new UIelement[]
			{
				opCheckBox,
				opLabel,
				opCheckBox2,
				opLabel2
			});
		}



	}
}
