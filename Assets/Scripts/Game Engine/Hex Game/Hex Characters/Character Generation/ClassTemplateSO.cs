using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexGameEngine.Items;
using HexGameEngine.Abilities;

namespace HexGameEngine.Characters
{
    [CreateAssetMenu(fileName = "New HexClassTemplateSO", menuName = "Hex Class Template")]
    public class ClassTemplateSO : ScriptableObject
    {
        [Header("Core Data")]
        public string templateName;

        [Header("Templates")]
        public List<CharacterRace> possibleRaces = new List<CharacterRace>();
        public int startingAbilityCount = 3;
        public List<AbilityDataSO> possibleAbilities = new List<AbilityDataSO>();
        public List<OutfitTemplateSO> possibleOutfits = new List<OutfitTemplateSO>();
        public List<SerializedItemSet> possibleWeapons = new List<SerializedItemSet>();

        [Header("Talent Data")]
        public List<TalentPairing> talentPairings = new List<TalentPairing>();

    }
}