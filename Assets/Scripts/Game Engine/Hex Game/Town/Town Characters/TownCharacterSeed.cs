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
        [SerializeField] private TownCharacterOutfitSet[] possibleOutfits;
        [SerializeField] private CharacterModelTemplateSO[] possibleTemplates;

        public List<string> GetRandomAppearance()
        {
            List<string> ret = new List<string>();
            ret.AddRange(possibleTemplates.GetRandomElement().bodyParts);
            ret.AddRange(possibleOutfits.GetRandomElement().Clothing);
            return ret;
        }

    }

    [System.Serializable]
    public class TownCharacterOutfitSet
    {
        [SerializeField] private string[] clothing;

        public string[] Clothing => clothing;
    }
}