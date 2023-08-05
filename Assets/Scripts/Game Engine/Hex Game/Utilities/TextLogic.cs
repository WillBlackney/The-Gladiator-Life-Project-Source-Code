using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace WeAreGladiators.Utilities
{
    public static class TextLogic
    {
        // Properties + Color Codes  </color>   
        #region
        [Header("RGBA Colour Codes")]
        public static string white = "<color=#FFFFFF>";
        public static string brownBodyText = "<color=#DDC6AB>";
        public static string orangeHeaderText = "<color=#BC8252>";
        public static string yellow = "<color=#FFF91C>";
        public static string blueNumber = "<color=#92E0FF>";
        public static string neutralYellow = "<color=#F8FF00>";
        public static string redText = "<color=#FF6262>";
        public static string lightGreen = "<color=#57FF34>";
        public static string lightRed = "<color=#FF4747>";
        public static string rareTextBlue = "<color=#90FFD0>";

        public static string physical = "<color=#FF9500>";
        public static string magic = "<color=#C975FF>";
        public static string fire = "<color=#FF6637>";
        public static string frost = "<color=#3687FF>";
        public static string shadow = "<color=#CF01BC>";
        public static string air = "<color=#36EDFF>";
        public static string poison = "<color=#00EC4A>";

        #endregion

        // Build stuff
        #region
        public static string ConvertCustomStringListToString(List<CustomString> csList)
        {
            string stringReturned = "";
            foreach (CustomString cs in csList)
            {
                stringReturned += ReturnColoredText(cs.phrase, GetColorCodeFromEnum(cs.color));
            }

            return stringReturned;
        }
        public static string ConvertCustomStringToString(CustomString cs)
        {
            return ReturnColoredText(cs.phrase, GetColorCodeFromEnum(cs.color));
        }
        #endregion

        // Get strings, colours and text
        #region
        public static string ReturnColoredText(string text, string color)
        {
            // Just give it a string and a color reference,
            // and this function takes care of everything
            if (color == "") return text;
            return (color + text + "</color>");
        }
        private static string GetColorCodeFromEnum(TextColor color)
        {
            string colorCodeReturned = "";

            // standard colors
            if (color == TextColor.KeyWordYellow)
            {
                colorCodeReturned = neutralYellow;
            }
            else if (color == TextColor.BlueNumber)
            {
                colorCodeReturned = blueNumber;
            }
            else if (color == TextColor.BrownBodyText)
            {
                colorCodeReturned = brownBodyText;
            }
            else if (color == TextColor.OrangeHeaderText)
            {
                colorCodeReturned = orangeHeaderText;
            }
            else if (color == TextColor.White)
            {
                colorCodeReturned = white;
            }

            // Damage type colors
            else if (color == TextColor.PhysicalBrown)
            {
                colorCodeReturned = physical;
            }
            else if (color == TextColor.MagicPurple)
            {
                colorCodeReturned = magic;
            }
            else if (color == TextColor.FireRed)
            {
                colorCodeReturned = fire;
            }
            else if (color == TextColor.PoisonGreen)
            {
                colorCodeReturned = poison;
            }
            else if (color == TextColor.AirBlue)
            {
                colorCodeReturned = air;
            }
            else if (color == TextColor.ShadowPurple)
            {
                colorCodeReturned = shadow;
            }
            else if (color == TextColor.FireRed)
            {
                colorCodeReturned = fire;
            }

            // Other misc colours
            else if (color == TextColor.LightGreen)
            {
                colorCodeReturned = lightGreen;
            }
            else if (color == TextColor.LightRed)
            {
                colorCodeReturned = lightRed;
            }
            else if (color == TextColor.RareTextBlue)
            {
                colorCodeReturned = rareTextBlue;
            }

            return colorCodeReturned;
        }
        #endregion

        // General string functions
        #region
        public static string SplitByCapitals(string originalString)
        {
            // Function seperates a string by capitals, 
            // puts a space at the end of each string, then
            // recombines them into one string

            string stringReturned = originalString;

            StringBuilder builder = new StringBuilder();
            foreach (char c in stringReturned)
            {
                if (Char.IsUpper(c) && builder.Length > 0) builder.Append(' ');
                builder.Append(c);
            }

            stringReturned = builder.ToString();

            return stringReturned;
        }
        #endregion

        
    }
}