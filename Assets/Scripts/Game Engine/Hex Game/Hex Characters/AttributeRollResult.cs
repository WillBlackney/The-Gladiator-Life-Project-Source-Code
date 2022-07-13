using HexGameEngine.Perks;
using HexGameEngine.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.Characters
{
    public class AttributeRollResult
    {
        public int strengthRoll;
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
            roll.strengthRoll = RandomGenerator.NumberBetween(lower, upper) + character.attributeSheet.strength.stars;
            roll.intelligenceRoll = RandomGenerator.NumberBetween(lower, upper) + character.attributeSheet.intelligence.stars;
            roll.accuracyRoll = RandomGenerator.NumberBetween(lower, upper) + character.attributeSheet.accuracy.stars;
            roll.dodgeRoll = RandomGenerator.NumberBetween(lower, upper) + character.attributeSheet.dodge.stars;
            roll.resolveRoll = RandomGenerator.NumberBetween(lower, upper) + character.attributeSheet.resolve.stars;
            roll.constitutionRoll = RandomGenerator.NumberBetween(lower, upper) + character.attributeSheet.constitution.stars;
            roll.witsRoll = RandomGenerator.NumberBetween(lower, upper) + character.attributeSheet.wits.stars;

            return roll;

        }
    }
    public class LevelUpPerkSet
    {
        public List<PerkIconData> perkChoices = new List<PerkIconData>();
        public LevelUpPerkSet()
        {
            List<PerkIconData> choices = PerkController.Instance.GetAllLevelUpPerks();
            choices.Shuffle();
            for (int i = 0; i < 10; i++)
                perkChoices.Add(choices[i]);
        }
        public LevelUpPerkSet(LevelUpPerkSet clonedFrom)
        {
            perkChoices.AddRange(clonedFrom.perkChoices);
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