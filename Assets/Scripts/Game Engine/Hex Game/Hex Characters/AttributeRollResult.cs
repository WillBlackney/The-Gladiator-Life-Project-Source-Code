using HexGameEngine.Perks;
using HexGameEngine.Utilities;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.Characters
{
    public class AttributeRollResult
    {
        public int mightRoll;
        public int intelligenceRoll;
        public int constitutionRoll;
        public int resolveRoll;
        public int dodgeRoll;
        public int accuracyRoll;
        public int witsRoll;
        public int fatigueRoll;

        public static AttributeRollResult GenerateRoll(HexCharacterData character)
        {
            // to do in future: consider character attribute stars
            int lower = 2;
            int upper = 4;

            AttributeRollResult roll = new AttributeRollResult();
            roll.mightRoll = RandomGenerator.NumberBetween(lower, upper) + character.attributeSheet.might.stars;
            roll.accuracyRoll = RandomGenerator.NumberBetween(lower, upper) + character.attributeSheet.accuracy.stars;
            roll.dodgeRoll = RandomGenerator.NumberBetween(lower, upper) + character.attributeSheet.dodge.stars;
            roll.resolveRoll = RandomGenerator.NumberBetween(lower, upper) + character.attributeSheet.resolve.stars;
            roll.constitutionRoll = RandomGenerator.NumberBetween(lower, upper) + character.attributeSheet.constitution.stars;
            roll.witsRoll = RandomGenerator.NumberBetween(lower, upper) + character.attributeSheet.wits.stars;
            roll.fatigueRoll = RandomGenerator.NumberBetween(lower, upper) + character.attributeSheet.fatigue.stars;

            return roll;

        }
    }
    [System.Serializable]
    public class PerkTreeData
    {
        [OdinSerialize]
        private bool hasGeneratedTree = false;
        [OdinSerialize]
        private List<ActivePerk> perkChoices = new List<ActivePerk>();
        public List<ActivePerk> PerkChoices
        {
            get
            {
                if (!hasGeneratedTree) GenerateTree();               
                return perkChoices;
            }
        }
        private void GenerateTree()
        {
            if (PerkController.Instance == null || hasGeneratedTree) return;
            hasGeneratedTree = true;
            List<PerkIconData> choices = PerkController.Instance.GetAllLevelUpPerks();
            choices.Shuffle();
            perkChoices = new List<ActivePerk>();
            for (int i = 0; i < 10 && i < choices.Count; i++)
                perkChoices.Add(new ActivePerk(choices[i].perkTag, 1, choices[i]));
        }

        public PerkTreeData()
        {
            GenerateTree();
        }
    }
    
    public class TalentRollResult
    {
        public List<TalentPairing> talentChoices = new List<TalentPairing>();
        public static TalentRollResult GenerateRoll(HexCharacterData character)
        {
            TalentRollResult roll = new TalentRollResult();

            List<TalentPairing> choices = CharacterDataController.Instance.GetValidLevelUpTalents(character);
            choices.Shuffle();
            for (int i = 0; i < 3; i++)
                roll.talentChoices.Add(choices[i]);

            return roll;
        }
    }
}