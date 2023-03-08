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
        public int fitnessRoll;

        public static AttributeRollResult GenerateRoll(HexCharacterData character)
        {
            // to do in future: consider character attribute stars
            int lower = 3;
            int upper = 5;

            AttributeRollResult roll = new AttributeRollResult();
            roll.mightRoll = RandomGenerator.NumberBetween(lower, upper) + character.attributeSheet.might.stars;
            roll.accuracyRoll = RandomGenerator.NumberBetween(lower, upper) + character.attributeSheet.accuracy.stars;
            roll.dodgeRoll = RandomGenerator.NumberBetween(lower, upper) + character.attributeSheet.dodge.stars;
            roll.resolveRoll = RandomGenerator.NumberBetween(lower, upper) + character.attributeSheet.resolve.stars;
            roll.constitutionRoll = RandomGenerator.NumberBetween(lower, upper) + character.attributeSheet.constitution.stars;
            roll.witsRoll = RandomGenerator.NumberBetween(lower, upper) + character.attributeSheet.wits.stars;
            roll.fitnessRoll = RandomGenerator.NumberBetween(lower, upper) + character.attributeSheet.fitness.stars;

            return roll;

        }
    }
    [System.Serializable]
    public class PerkTreeData
    {
        [OdinSerialize]
        private bool hasGeneratedTree = false;
        [OdinSerialize]
        private List<PerkTreePerk> perkChoices = new List<PerkTreePerk>();
        public List<PerkTreePerk> PerkChoices
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
            // TO DO: Filter out already known perks

            choices.Shuffle();
            perkChoices = new List<PerkTreePerk>();
            int tier = 1;
            for (int i = 0; i < 15 && i < choices.Count; i++)
            {
                perkChoices.Add(new PerkTreePerk(new ActivePerk(choices[i].perkTag, 1, choices[i]), tier));
                if ((i + 1) % 3 == 0) tier += 1;
            }
                
        }

        public PerkTreeData()
        {
            GenerateTree();
        }
        public int nextAvailableTier = 1;

       
    }

    public class PerkTreePerk
    {
        public ActivePerk perk;
        public int tier = 1;

        public PerkTreePerk(ActivePerk perk, int tier)
        {
            this.perk = perk;
            this.tier = tier;
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