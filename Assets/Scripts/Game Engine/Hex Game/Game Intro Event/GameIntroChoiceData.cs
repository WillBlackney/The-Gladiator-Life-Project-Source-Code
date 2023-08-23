using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.GameIntroEvent
{
    [System.Serializable]
    public class GameIntroChoiceData 
    {
        public List<CustomString> buttonText;
        public PageTag pageLoaded;

    }
}