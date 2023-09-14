﻿using UnityEngine;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.Libraries
{
    public class ColorLibrary : Singleton<ColorLibrary>
    {
        // Talent Colors
        #region

        [Header("Talent Colors")]
        public Color neutralColor;
        public Color warfareColor;
        public Color guardianColor;
        public Color pyromaniaColor;
        public Color manipulationColor;
        public Color rangerColor;
        public Color shadowcraftColor;
        public Color divinityColor;
        public Color naturalismColor;
        public Color corruptionColor;
        public Color scoundrelColor;
        public Color metamorphColor;

        [Header("Misc Card Colors")]
        public Color racialColor;
        public Color afflictionColor;

        #endregion

        // Rarity Colors
        #region

        [Header("Rarity Colors")]
        public Color commonColor;
        public Color rareColor;
        public Color epicColor;
        public Color legendaryColor;

        #endregion

        // Overlay Colors
        #region

        [Header("Screen Overlay Colors")]
        public Color whiteOverlayColour;
        public Color fireOverlayColour;
        public Color purpleOverlayColour;
        public Color poisonGreenOverlayColour;

        [Header("Font Colors")]
        public Color cardUpgradeFontColor;

        #endregion

        // Logic
        #region

        public Color GetTalentColor(TalentSchool talent)
        {
            Color colorReturned = neutralColor;

            if (talent == TalentSchool.Warfare)
            {
                colorReturned = warfareColor;
            }
            else if (talent == TalentSchool.Guardian)
            {
                colorReturned = guardianColor;
            }
            else if (talent == TalentSchool.Scoundrel)
            {
                colorReturned = scoundrelColor;
            }
            else if (talent == TalentSchool.Ranger)
            {
                colorReturned = rangerColor;
            }
            else if (talent == TalentSchool.Pyromania)
            {
                colorReturned = pyromaniaColor;
            }
            else if (talent == TalentSchool.Naturalism)
            {
                colorReturned = naturalismColor;
            }
            else if (talent == TalentSchool.Shadowcraft)
            {
                colorReturned = shadowcraftColor;
            }

            else if (talent == TalentSchool.Divinity)
            {
                colorReturned = divinityColor;
            }
            else if (talent == TalentSchool.Manipulation)
            {
                colorReturned = manipulationColor;
            }
            else if (talent == TalentSchool.Metamorph)
            {
                colorReturned = metamorphColor;
            }

            colorReturned = new Color(colorReturned.r, colorReturned.g, colorReturned.b, colorReturned.a);

            return colorReturned;
        }
        public Color GetRarityColor(Rarity rarity)
        {
            Color colorReturned = commonColor;

            if (rarity == Rarity.Common)
            {
                colorReturned = commonColor;
            }
            else if (rarity == Rarity.Rare)
            {
                colorReturned = rareColor;
            }
            else if (rarity == Rarity.Epic)
            {
                colorReturned = epicColor;
            }
            else if (rarity == Rarity.Legendary)
            {
                colorReturned = legendaryColor;
            }

            return colorReturned;
        }
        public Color GetOverlayColor(ScreenOverlayColor overlay)
        {
            Color colorReturned = whiteOverlayColour;

            if (overlay == ScreenOverlayColor.Fire)
            {
                colorReturned = fireOverlayColour;
            }
            else if (overlay == ScreenOverlayColor.White)
            {
                colorReturned = whiteOverlayColour;
            }
            else if (overlay == ScreenOverlayColor.Purple)
            {
                colorReturned = purpleOverlayColour;
            }
            else if (overlay == ScreenOverlayColor.PoisonGreen)
            {
                colorReturned = poisonGreenOverlayColour;
            }

            return colorReturned;
        }

        #endregion
    }
}
