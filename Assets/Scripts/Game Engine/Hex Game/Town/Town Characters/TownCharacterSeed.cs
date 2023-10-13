using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeAreGladiators.Characters;
using WeAreGladiators.UCM;

namespace WeAreGladiators.TownFeatures
{
    [CreateAssetMenu]
    public class TownCharacterSeed : ScriptableObject
    {
        [SerializeField] private List<string> clothingPieces = new List<string>();
        [SerializeField] private CharacterModelTemplateSO[] possibleTemplates;

        public List<string> GetRandomAppearance()
        {
            List<string> ret = new List<string>();
            ret.AddRange(possibleTemplates.GetRandomElement().bodyParts);
            ret.AddRange(clothingPieces);
            return ret;
        }

    }
}