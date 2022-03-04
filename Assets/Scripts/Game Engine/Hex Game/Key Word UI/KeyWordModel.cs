using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HexGameEngine.UI
{
    [Serializable]
    public class KeyWordModel
    {
        // Properties
        #region
        [Header("Properties")]
        public KeyWordType kewWordType;

        [ShowIf("ShowPassiveType")]
        public Perk passiveType;
        #endregion

        // Odin Show if's
        #region       
        public bool ShowPassiveType()
        {
            return kewWordType == KeyWordType.Perk;
        }
        #endregion
    }

}