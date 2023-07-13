using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

namespace WeAreGladiators.Utilities
{
    [Serializable]
    public class CustomString
    {
        public TextColor color;
        public bool getPhraseFromAbilityValue = false;

        [ShowIf("ShowAbilityEffectType")]
        public AbilityEffectType abilityEffectType;

        [ShowIf("ShowPhrase")]
        [TextArea]
        public string phrase;


        public bool ShowAbilityEffectType()
        {
            if (getPhraseFromAbilityValue == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool ShowPhrase()
        {
            if (getPhraseFromAbilityValue == false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }


}