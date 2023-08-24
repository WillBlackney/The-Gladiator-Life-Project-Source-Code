using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using WeAreGladiators.Characters;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.GameIntroEvent
{
    [System.Serializable]
    public class GameIntroChoiceData 
    {
        public string buttonText;
        public PageTag[] possiblePagesLoaded;
        public HexCharacterTemplateSO characterGained;

    }
}