using BepInEx;
using UnityEngine;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using MoreSlugcats;
using Menu;
using System.Globalization;
using System.Collections.Generic;
using RWCustom;
using Expedition;
 
namespace ExpeditionCopyMiscProg
{
    public class ColorMenu : CheckBox.IOwnCheckBox, Slider.ISliderOwner
    {
        public static ColorMenu instance;

        public static void Start()
        {

            //On.Menu.ExpeditionMenu.SetCurrentlySelectedOfSeries += ColorMenu.ExpeditionMenu_SetCurrentlySelectedOfSeries;
            On.Menu.CharacterSelectPage.UpdateSelectedSlugcat += CharacterSelectPage_UpdateSelectedSlugcat;
            On.Menu.ExpeditionMenu.Singal += ExpeditionMenu_Singal;
            On.Menu.ExpeditionMenu.SliderSetValue += ExpeditionMenu_SliderSetValue;
            On.Menu.ExpeditionMenu.ValueOfSlider += ExpeditionMenu_ValueOfSlider;
            On.Menu.CharacterSelectPage.RemoveSprites += CharacterSelectPage_RemoveSprites;
            On.Menu.CharacterSelectPage.LoadGame += CharacterSelectPage_LoadGame;
        }

        private static void CharacterSelectPage_LoadGame(On.Menu.CharacterSelectPage.orig_LoadGame orig, CharacterSelectPage self)
        {
            orig(self);
            if (!WConfig.cfgCustomColorMenu.Value)
            {
                return;
            }
            if (ModManager.MMF && Custom.rainWorld.progression.miscProgressionData.colorsEnabled.ContainsKey(instance.selectedSlugName.value) && Custom.rainWorld.progression.miscProgressionData.colorsEnabled[instance.selectedSlugName.value])
            {
                List<Color> list = new List<Color>();
                for (int i = 0; i < Custom.rainWorld.progression.miscProgressionData.colorChoices[instance.selectedSlugName.value].Count; i++)
                {
                    Vector3 vector = new Vector3(1f, 1f, 1f);
                    if (Custom.rainWorld.progression.miscProgressionData.colorChoices[instance.selectedSlugName.value][i].Contains(","))
                    {
                        string[] array = Custom.rainWorld.progression.miscProgressionData.colorChoices[instance.selectedSlugName.value][i].Split(new char[]
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

        private static void CharacterSelectPage_RemoveSprites(On.Menu.CharacterSelectPage.orig_RemoveSprites orig, CharacterSelectPage self)
        {
            orig(self);
            if (instance != null)
            {
                if (instance.colorInterface != null)
                {
                    instance.colorInterface.RemoveSprites();
                }
                instance.RemoveColorInterface();
            }
           
        }

        private static float ExpeditionMenu_ValueOfSlider(On.Menu.ExpeditionMenu.orig_ValueOfSlider orig, ExpeditionMenu self, Slider slider)
        {
           if (instance != null)
            {
                if (slider.ID == MMFEnums.SliderID.Hue || slider.ID == MMFEnums.SliderID.Saturation || slider.ID == MMFEnums.SliderID.Lightness)
                {
                    return instance.ValueOfSlider(slider);
                }
            }
            return orig(self,slider);
        }

        private static void ExpeditionMenu_SliderSetValue(On.Menu.ExpeditionMenu.orig_SliderSetValue orig, ExpeditionMenu self, Slider slider, float f)
        {
            if (instance != null)
            {
                if (slider.ID == MMFEnums.SliderID.Hue || slider.ID == MMFEnums.SliderID.Saturation || slider.ID == MMFEnums.SliderID.Lightness)
                {
                    instance.SliderSetValue(slider, f);
                    return;
                }
            }
            orig(self,slider,f);
        }

        private static void ExpeditionMenu_Singal(On.Menu.ExpeditionMenu.orig_Singal orig, ExpeditionMenu self, MenuObject sender, string message)
        {
            orig(self, sender, message);
            if (!WConfig.cfgCustomColorMenu.Value)
            {
                return;
            }
            if (message != null)
            {
                if (message == "DEFAULTCOL")
                {
                    SlugcatStats.Name name = ExpeditionData.slugcatPlayer;
                    int index = instance.activeColorChooser;
                    self.manager.rainWorld.progression.miscProgressionData.colorChoices[name.value][index] = instance.colorInterface.defaultColors[instance.activeColorChooser];
                    float f = instance.ValueOfSlider(instance.hueSlider);
                    float f2 = instance.ValueOfSlider(instance.satSlider);
                    float f3 = instance.ValueOfSlider(instance.litSlider);
                    instance.SliderSetValue(instance.hueSlider, f);
                    instance.SliderSetValue(instance.satSlider, f2);
                    instance.SliderSetValue(instance.litSlider, f3);
                    self.PlaySound(SoundID.MENU_Remove_Level);
                }
                else if (message.StartsWith("MMFCUSTOMCOLOR"))
                {
                    self.PlaySound(SoundID.MENU_Button_Standard_Button_Pressed);
                    int num = int.Parse(message.Substring("MMFCUSTOMCOLOR".Length), NumberStyles.Any, CultureInfo.InvariantCulture);
                    if (num == instance.activeColorChooser)
                    {
                        instance.RemoveColorInterface();
                        self.PlaySound(SoundID.MENU_Remove_Level);
                        return;
                    }
                    instance.activeColorChooser = num;
                    instance.AddColorInterface();
                    self.PlaySound(SoundID.MENU_Button_Standard_Button_Pressed);
                }

            }
        }

        public static void CharacterSelectPage_UpdateSelectedSlugcat(On.Menu.CharacterSelectPage.orig_UpdateSelectedSlugcat orig, CharacterSelectPage self, int num)
        {
           orig(self, num);
            if (!WConfig.cfgCustomColorMenu.Value)
            {
                return;
            }
            if (ModManager.MMF && !ModManager.JollyCoop)
            {
                if (instance == null)
                {
                    instance = new ColorMenu();
                }
                instance.expdMenu = self.menu;
                UnityEngine.Debug.Log(instance.expdMenu);
                instance.selectedSlug = num;
                instance.selectedSlugName = ExpeditionGame.playableCharacters[num];

                instance.colorsCheckbox = new CheckBox(self.menu, self.menu.pages[1], instance, new Vector2(60f + 200f + 0, 60f), 85f, self.menu.Translate("Custom colors"), "COLORS", false);
                MenuLabel label2 = instance.colorsCheckbox.label;
                label2.pos.x = label2.pos.x + (85f - instance.colorsCheckbox.label.label.textRect.width - 5f);
                instance.colorsCheckbox.selectable = true;
                self.menu.pages[1].subObjects.Add(instance.colorsCheckbox);


                instance.RemoveColorButtons();
                if (instance.colorsCheckbox != null)
                {
                    if (instance.colorsCheckbox.buttonBehav.greyedOut)
                    {
                        instance.colorsCheckbox.buttonBehav.greyedOut = false;
                    }
                    instance.SetChecked(instance.colorsCheckbox, Custom.rainWorld.progression.miscProgressionData.colorsEnabled.ContainsKey(ExpeditionGame.playableCharacters[num].value) && Custom.rainWorld.progression.miscProgressionData.colorsEnabled[ExpeditionGame.playableCharacters[num].value]);
                    instance.colorsCheckbox.buttonBehav.greyedOut = false;
                }

            }
        } 

        public HorizontalSlider hueSlider;
        public HorizontalSlider satSlider;
        public HorizontalSlider litSlider;

        public int activeColorChooser;
        public int selectedSlug;
        public SlugcatStats.Name selectedSlugName;
        public bool colorChecked;

        public SimpleButton defaultColorButton;
        public CheckBox colorsCheckbox;
        public SlugcatSelectMenu.CustomColorInterface colorInterface = null;
        public Menu.Menu expdMenu;
 

        public SlugcatSelectMenu.CustomColorInterface GetColorInterfaceForSlugcat(SlugcatStats.Name slugcatID, Vector2 pos, Menu.Menu menu)
        {
            List<string> names = PlayerGraphics.ColoredBodyPartList(slugcatID);
            List<string> list = PlayerGraphics.DefaultBodyPartColorHex(slugcatID);
            for (int i = 0; i < list.Count; i++)
            {
                Vector3 vector = Custom.RGB2HSL(Custom.hexToColor(list[i]));
                list[i] = string.Concat(new string[]
                {
                    vector[0].ToString(),
                    ",",
                    vector[1].ToString(),
                    ",",
                    vector[2].ToString()
                });
            }
            return new SlugcatSelectMenu.CustomColorInterface(menu, menu.pages[1], pos, slugcatID, names, list);
        }

        public float ValueOfSlider(Slider slider)
        {
            SlugcatStats.Name name = this.selectedSlugName;
            int index = this.activeColorChooser;
            Vector3 vector = new Vector3(1f, 1f, 1f);
            if (Custom.rainWorld.progression.miscProgressionData.colorChoices[name.value][index].Contains(","))
            {
                string[] array = Custom.rainWorld.progression.miscProgressionData.colorChoices[name.value][index].Split(new char[]
                {
                    ','
                });
                vector = new Vector3(float.Parse(array[0], NumberStyles.Any, CultureInfo.InvariantCulture), float.Parse(array[1], NumberStyles.Any, CultureInfo.InvariantCulture), float.Parse(array[2], NumberStyles.Any, CultureInfo.InvariantCulture));
            }
            if (slider.ID == MMFEnums.SliderID.Hue)
            {
                return vector[0];
            }
            if (slider.ID == MMFEnums.SliderID.Saturation)
            {
                return vector[1];
            }
            if (slider.ID == MMFEnums.SliderID.Lightness)
            {
                return vector[2];
            }
            return 0f;
        }

        public void SliderSetValue(Slider slider, float f)
        {
            SlugcatStats.Name name = this.selectedSlugName;
            int num = this.activeColorChooser;
            Vector3 vector = new Vector3(1f, 1f, 1f);
            if (Custom.rainWorld.progression.miscProgressionData.colorChoices[name.value][num].Contains(","))
            {
                string[] array = Custom.rainWorld.progression.miscProgressionData.colorChoices[name.value][num].Split(new char[]
                {
                    ','
                });
                vector = new Vector3(float.Parse(array[0], NumberStyles.Any, CultureInfo.InvariantCulture), float.Parse(array[1], NumberStyles.Any, CultureInfo.InvariantCulture), float.Parse(array[2], NumberStyles.Any, CultureInfo.InvariantCulture));
            }
            if (slider.ID == MMFEnums.SliderID.Hue)
            {
                vector[0] = Mathf.Clamp(f, 0f, 0.99f);
                Custom.rainWorld.progression.miscProgressionData.colorChoices[name.value][num] = string.Concat(new string[]
                {
                    vector[0].ToString(),
                    ",",
                    vector[1].ToString(),
                    ",",
                    vector[2].ToString()
                });
            }
            else if (slider.ID == MMFEnums.SliderID.Saturation)
            {
                vector[1] = Mathf.Clamp(f, 0f, 1f);
                Custom.colorToHex(Custom.HSL2RGB(vector[0], vector[1], vector[2]));
                Custom.rainWorld.progression.miscProgressionData.colorChoices[name.value][num] = string.Concat(new string[]
                {
                    vector[0].ToString(),
                    ",",
                    vector[1].ToString(),
                    ",",
                    vector[2].ToString()
                });
            }
            else if (slider.ID == MMFEnums.SliderID.Lightness)
            {
                vector[2] = Mathf.Clamp(f, 0.01f, 1f);
                Custom.colorToHex(Custom.HSL2RGB(vector[0], vector[1], vector[2]));
                Custom.rainWorld.progression.miscProgressionData.colorChoices[name.value][num] = string.Concat(new string[]
                {
                    vector[0].ToString(),
                    ",",
                    vector[1].ToString(),
                    ",",
                    vector[2].ToString()
                });
            }
            if (this.colorInterface != null)
            {
                this.colorInterface.bodyColors[num].color = Custom.HSL2RGB(vector[0], vector[1], vector[2]);
            }
            this.expdMenu.selectedObject = slider;
        }

        public void AddColorInterface()
        {
            Vector2 vector = new Vector2(1000f - (1366f - Custom.rainWorld.options.ScreenSize.x) / 2f, Custom.rainWorld.options.ScreenSize.y - 100f);
            if (ModManager.JollyCoop)
            {
                ref Vector2 ptr = ref vector;
                ptr[1] = ptr[1] - 40f;
            }
            if (this.colorInterface != null)
            {
                vector[1] = vector[1] - (float)this.colorInterface.bodyColors.Length * 40f;
            }
            if (this.hueSlider == null)
            {
                this.hueSlider = new HorizontalSlider(expdMenu, expdMenu.pages[1], expdMenu.Translate("HUE"), vector, new Vector2(200f, 30f), MMFEnums.SliderID.Hue, false);
                expdMenu.pages[1].subObjects.Add(this.hueSlider);
            }
            if (this.satSlider == null)
            {
                this.satSlider = new HorizontalSlider(expdMenu, expdMenu.pages[1], expdMenu.Translate("SAT"), vector + new Vector2(0f, -40f), new Vector2(200f, 30f), MMFEnums.SliderID.Saturation, false);
                expdMenu.pages[1].subObjects.Add(this.satSlider);
            }
            if (this.litSlider == null)
            {
                this.litSlider = new HorizontalSlider(expdMenu, expdMenu.pages[1], expdMenu.Translate("LIT"), vector + new Vector2(0f, -80f), new Vector2(200f, 30f), MMFEnums.SliderID.Lightness, false);
                expdMenu.pages[1].subObjects.Add(this.litSlider);
            }
            float x = 110f;
            if (expdMenu.CurrLang == InGameTranslator.LanguageID.Japanese || expdMenu.CurrLang == InGameTranslator.LanguageID.French)
            {
                x = 140f;
            }
            else if (expdMenu.CurrLang == InGameTranslator.LanguageID.Italian || expdMenu.CurrLang == InGameTranslator.LanguageID.Spanish)
            {
                x = 180f;
            }
            if (this.defaultColorButton == null)
            {
                this.defaultColorButton = new SimpleButton(expdMenu, expdMenu.pages[1], expdMenu.Translate("Restore Default"), "DEFAULTCOL", vector + new Vector2(0f, -120f), new Vector2(x, 30f));
                expdMenu.pages[1].subObjects.Add(this.defaultColorButton);
            }
            MutualVerticalButtonBind(this.hueSlider, this.colorInterface.bodyButtons[this.colorInterface.bodyButtons.Length - 1]);
            MutualVerticalButtonBind(this.satSlider, this.hueSlider);
            MutualVerticalButtonBind(this.litSlider, this.satSlider);
            MutualVerticalButtonBind(this.defaultColorButton, this.litSlider);
            //MutualVerticalButtonBind(this.nextButton, this.defaultColorButton);
            // this.nextButton.nextSelectable[3] = this.colorInterface.bodyButtons[0];
        }

        public void MutualVerticalButtonBind(MenuObject bottom, MenuObject top)
        {
            bottom.nextSelectable[1] = top;
            top.nextSelectable[3] = bottom;
        }
        /*
        public static void ExpeditionMenu_SetCurrentlySelectedOfSeries(On.Menu.ExpeditionMenu.orig_SetCurrentlySelectedOfSeries orig, ExpeditionMenu self, string series, int to)
        {
            orig(self,series, to);
            
            if (ModManager.MMF)
            {
                if (instance == null)
                {
                    instance = new ColorMenu();
                    instance.expdMenu = self;
                }
                instance.selectedSlug = to;
                instance.selectedSlugName = ExpeditionGame.playableCharacters[to];
          
                instance.colorsCheckbox = new CheckBox(self, self.pages[1], instance, new Vector2(60f + 200f + 0, 60f), 85f, self.Translate("Custom colors"), "COLORS", false);
                MenuLabel label2 = instance.colorsCheckbox.label;
                label2.pos.x = label2.pos.x + (85f - instance.colorsCheckbox.label.label.textRect.width - 5f);
                instance.colorsCheckbox.selectable = true;
                self.pages[1].subObjects.Add(instance.colorsCheckbox);
            }
        }
        */
         

        public void AddColorButtons()
        {
            if (this.colorInterface == null)
            {
                Vector2 pos = new Vector2(1000f - (1366f - Custom.rainWorld.options.ScreenSize.x) / 2f, Custom.rainWorld.options.ScreenSize.y - 100f);
                colorInterface = GetColorInterfaceForSlugcat(this.selectedSlugName, pos, expdMenu);
                expdMenu.pages[1].subObjects.Add(colorInterface);
 
                /*Vector2 pos = new Vector2(1000f - (1366f - this.manager.rainWorld.options.ScreenSize.x) / 2f, this.manager.rainWorld.options.ScreenSize.y - 100f);
                this.colorInterface = this.GetColorInterfaceForSlugcat(this.slugcatColorOrder[this.slugcatPageIndex], pos);
                expdMenu.pages[1].subObjects.Add(this.colorInterface);*/
            }
        }
        public void RemoveColorButtons()
        {
            if (this.colorInterface != null)
            {
                this.colorInterface.RemoveSprites();
                expdMenu.pages[0].RemoveSubObject(this.colorInterface);
                this.colorInterface = null;
            }
            this.RemoveColorInterface();
        }
        public void RemoveColorInterface()
        {
            if (this.hueSlider != null)
            {
                expdMenu.pages[1].RemoveSubObject(this.hueSlider);
                this.hueSlider.RemoveSprites();
                this.hueSlider = null;
            }
            if (this.satSlider != null)
            {
                expdMenu.pages[1].RemoveSubObject(this.satSlider);
                this.satSlider.RemoveSprites();
                this.satSlider = null;
            }
            if (this.litSlider != null)
            {
                expdMenu.pages[1].RemoveSubObject(this.litSlider);
                this.litSlider.RemoveSprites();
                this.litSlider = null;
            }
            if (this.defaultColorButton != null)
            {
                expdMenu.pages[1].RemoveSubObject(this.defaultColorButton);
                this.defaultColorButton.RemoveSprites();
                this.defaultColorButton = null;
            }
            this.activeColorChooser = -1;
            if (this.colorInterface != null)
            {
               // base.MutualVerticalButtonBind(this.nextButton, this.colorInterface.bodyButtons[this.colorInterface.bodyButtons.Length - 1]);
               // this.nextButton.nextSelectable[3] = this.colorInterface.bodyButtons[0];
                return;
            }
            //this.nextButton.nextSelectable[1] = null;
            //this.nextButton.nextSelectable[3] = null;
        }

        public bool GetChecked(CheckBox box)
        {
            if (box.IDString == "COLORS")
            {
                return this.colorChecked;
            }
            return this.colorChecked;
        }

        public void SetChecked(CheckBox box, bool c)
        {
            this.colorChecked = c;
            if (this.colorChecked)
            {
                this.AddColorButtons();
                Custom.rainWorld.progression.miscProgressionData.colorsEnabled[this.selectedSlugName.value] = true;
                return;
            }
            this.RemoveColorButtons();
            Custom.rainWorld.progression.miscProgressionData.colorsEnabled[this.selectedSlugName.value] = false;
        }
    }
}