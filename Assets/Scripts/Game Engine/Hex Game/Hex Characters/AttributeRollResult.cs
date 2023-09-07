using WeAreGladiators.Perks;
using WeAreGladiators.Utilities;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

namespace WeAreGladiators.Characters
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
        [OdinSerialize]
        public int nextAvailableTier = 1;

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
            List<PerkIconData> prospects = new List<PerkIconData>();
            prospects.AddRange(PerkController.Instance.GetAllPerkTreePerks());
            prospects.Shuffle();

            List<PerkTreePerk> t1 = new List<PerkTreePerk>();
            List<PerkTreePerk> t2 = new List<PerkTreePerk>();
            List<PerkTreePerk> t3 = new List<PerkTreePerk>();
            List<PerkTreePerk> t4 = new List<PerkTreePerk>();
            List<PerkTreePerk> t5 = new List<PerkTreePerk>();

            for (int i = 0; i < prospects.Count; i++)
            {
                PerkIconData perk = prospects[i];

                if((perk.perkTreeTier == 1 && t1.Count < 3) ||
                    (perk.perkTreeTier == 2 && t2.Count < 3) ||
                    (perk.perkTreeTier == 3 && t3.Count < 3) ||
                    (perk.perkTreeTier == 4 && t4.Count < 3) ||
                    (perk.perkTreeTier == 5 && t5.Count < 3))
                {
                    PerkTreePerk newPerk = new PerkTreePerk(new ActivePerk(perk.perkTag, 1, perk), perk.perkTreeTier);

                    if (perk.perkTreeTier == 1) t1.Add(newPerk);
                    else if (perk.perkTreeTier == 2) t2.Add(newPerk);
                    else if (perk.perkTreeTier == 3) t3.Add(newPerk);
                    else if (perk.perkTreeTier == 4) t4.Add(newPerk);
                    else if (perk.perkTreeTier == 5) t5.Add(newPerk);
                }
            }

            perkChoices.AddRange(t1);
            perkChoices.AddRange(t2);
            perkChoices.AddRange(t3);
            perkChoices.AddRange(t4);
            perkChoices.AddRange(t5);
        }

        public PerkTreeData()
        {
            GenerateTree();
        }
        public PerkTreeData(HexCharacterData character)
        {
            GenerateTree();
            HandleAdjustTreeIfStartingPerkChoiceAlreadyMade(character);
        }
        public PerkTreeData(PerkTreeData original)
        {
            hasGeneratedTree = true;
            foreach (PerkTreePerk pt in original.PerkChoices)
            {
                perkChoices.Add(new PerkTreePerk(pt.perk, pt.tier));
            }
            nextAvailableTier = original.nextAvailableTier;
        }


        public void HandleAdjustTreeIfStartingPerkChoiceAlreadyMade(HexCharacterData myCharacter)
        {
            List<Perk> tierOnePerks = new List<Perk>();
            foreach(PerkTreePerk pt in perkChoices)
            {
                if (pt.perk.Data.perkTreeTier == 1) tierOnePerks.Add(pt.perk.perkTag);
            }
            foreach(ActivePerk p in myCharacter.passiveManager.perks)
            {
                if(p.Data.isOnPerkTree && p.Data.perkTreeTier == 1 &&
                    tierOnePerks.Contains(p.perkTag) == false)
                {
                    perkChoices[0] = new PerkTreePerk(p, 1);
                    nextAvailableTier += 1;
                    break;
                }
            }
        }

       
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