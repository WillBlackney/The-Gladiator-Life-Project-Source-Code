using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace WeAreGladiators.Utilities
{
    [Serializable]
    public class CustomString
    {
        public TextColor color;
        public bool getPhraseFromAbilityValue;

        [ShowIf("ShowAbilityEffectType")]
        public AbilityEffectType abilityEffectType;

        [ShowIf("ShowPhrase")]
        [TextArea]
        public string phrase;

        public bool ShowAbilityEffectType()
        {
            if (getPhraseFromAbilityValue)
            {
                return true;
            }
            return false;
        }
        public bool ShowPhrase()
        {
            if (getPhraseFromAbilityValue == false)
            {
                return true;
            }
            return false;
        }
    }

}
