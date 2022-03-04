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
        public List<AbilityDataSO> possibleAbilities = new List<AbilityDataSO>();
        public List<OutfitTemplateSO> possibleOutfits = new List<OutfitTemplateSO>();
        public List<SerializedItemSet> possibleWeapons = new List<SerializedItemSet>();

        [Header("Core Attributes")]
        public int strengthMod;
        public int intelligenceMod;
        public int accuracyMod;
        public int dodgeMod;
        public int resolveMod;
        public int constitutionMod;
        public int witsMod;

        [Header("Talent Data")]
        public List<TalentPairing> talentPairings = new List<TalentPairing>();

    }
}