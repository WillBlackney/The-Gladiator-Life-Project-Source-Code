using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.UI
{
    [CreateAssetMenu]
    public class KeyWordDataBox : ScriptableObject
    {
        // Properties
        #region

        [Header("Properties")]
        public KeyWordType kewWordType;

        [ShowIf("ShowPassiveType")]
        public Perk passiveType;

        [ShowIf("ShowDescription")]
        public List<CustomString> keyWordDescription;

        [Header("Sprite Properties")]
        public bool useSprite;

        [ShowIf("ShowSprite")]
        [PreviewField(75)]
        public Sprite sprite;

        #endregion

        // Odin Show if's
        #region

        public bool ShowSprite()
        {
            return useSprite;
        }
        public bool ShowPassiveType()
        {
            return kewWordType == KeyWordType.Perk;
        }
        public bool ShowDescription()
        {
            if (kewWordType == KeyWordType.Perk)
            {
                return false;
            }
            return true;
        }

        #endregion
    }
}
