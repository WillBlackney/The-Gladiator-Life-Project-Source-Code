using WeAreGladiators.Abilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeAreGladiators.Characters;

namespace WeAreGladiators.RewardSystems
{
    public class SingleRewardContainer
    {
        public RewardType rewardType;
        public Perk perkOffered;
        public AbilityData abilityOffered;
        public TalentSchool talentOffered;

        // Constructors
        public SingleRewardContainer(Perk p)
        {
            perkOffered = p;
            rewardType = RewardType.Perk;
        }
        public SingleRewardContainer(AbilityData a)
        {
            abilityOffered = a;
            rewardType = RewardType.Ability;
        }
        public SingleRewardContainer(TalentSchool ts)
        {
            talentOffered = ts;
            rewardType = RewardType.Talent;
        }
    }

    public class CharacterRewardContainerSet
    {
        public HexCharacterData myCharacter;
        public List<SingleRewardContainer> rewardChoices = new List<SingleRewardContainer>();
    }
    public class RewardContainerSet
    {
        public List<CharacterRewardContainerSet> allCharacterRewards = new List<CharacterRewardContainerSet>();
    }
    public enum RewardType
    {
        None = 0,
        Ability = 1,
        Perk = 2,
        Talent = 3,
    }

}