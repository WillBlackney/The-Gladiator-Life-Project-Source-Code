using System;
using System.Collections.Generic;
using UnityEngine;

namespace WeAreGladiators.Characters
{
    [CreateAssetMenu(fileName = "New CharacterModelTemplateSO", menuName = "CharacterModelTemplateSO")]
    public class CharacterModelTemplateSO : ScriptableObject
    {
        [Header("Properties")]
        public CharacterRace race;
        public List<string> bodyParts = new List<string>();
    }
    [Serializable]
    public class CharacterModelTemplateBasket
    {
        [Header("Properties")]
        public CharacterRace race;
        public CharacterModelTemplateSO[] templates;
    }
}
