using HexGameEngine.Perks;
using HexGameEngine.Utilities;
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

            return roll;

        }
    }
    public class PerkTreeData
    {
        private List<PerkIconData> perkChoices;
        public List<PerkIconData> PerkChoices
        {
            get
            {
                if (perkChoices == null || perkChoices.Count == 0)
                    GeneratePerkChoices();
                return perkChoices;
            }
        }
        private void GeneratePerkChoices()
        {
            List<PerkIconData> choices = PerkController.Instance.GetAllLevelUpPerks();
            choices.Shuffle();
            perkChoices = new List<PerkIconData>();
            for (int i = 0; i < 10; i++)
                perkChoices.Add(choices[i]);
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