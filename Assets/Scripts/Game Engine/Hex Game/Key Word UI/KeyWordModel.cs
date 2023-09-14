using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace WeAreGladiators.UI
{
    [Serializable]
    public class KeyWordModel
    {

        // Odin Show if's
        #region

        public bool ShowPassiveType()
        {
            return kewWordType == KeyWordType.Perk;
        }

        #endregion
        // Properties
        #region

        [Header("Properties")]
        public KeyWordType kewWordType;

        [ShowIf("ShowPassiveType")]
        public Perk passiveType;

        #endregion
    }

}
