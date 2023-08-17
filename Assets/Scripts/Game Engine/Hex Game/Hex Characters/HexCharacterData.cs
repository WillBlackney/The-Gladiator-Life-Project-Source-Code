using WeAreGladiators.Abilities;
using WeAreGladiators.AI;
using WeAreGladiators.Audio;
using WeAreGladiators.Items;
using WeAreGladiators.Perks;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeAreGladiators.Characters
{
    public class HexCharacterData
    {
        [Header("Story Properties")]
        public string myName;
        public string mySubName;
        public CharacterRace race;
        public AudioProfileType audioProfile;
        public CharacterModelSize modelSize;
        public BackgroundData background;
        public int xpReward;
        public int baseArmour;
        public bool ignoreStress = false;

        [Header("Passive Properties")]
        public PerkManagerModel passiveManager;

        [Header("Attributes")]
        public AttributeSheet attributeSheet;

        [Header("Health Properties")]
        public int currentHealth;

        [Header("Stress Properties")]
        public int currentStress;

        [Header("XP Attributes")]
        public int currentXP;
        public int currentMaxXP;
        public int currentLevel;

        [Header("Model Properties")]
        public List<string> modelParts;

        [Header("AI Logic")]
        public AIBehaviour behaviour;

        [Header("Item Properties")]
        public ItemSet itemSet = new ItemSet();

        [Header("Item Properties")]
        public AbilityBook abilityBook;
        public List<TalentPairing> talentPairings = new List<TalentPairing>();

        [Header("Misc Properties")]
        public Vector2 formationPosition = Vector2.zero;
        public int dailyWage;
        public TownActivity currentTownActivity = TownActivity.None;
        public List<AttributeRollResult> attributeRolls = new List<AttributeRollResult>();
        [OdinSerialize]
        private PerkTreeData perkTree;
        public int perkPoints = 0;
        //public List<TalentRollResult> talentRolls = new List<TalentRollResult>();
        public int talentPoints = 0;
        public PerkTreeData PerkTree
        {
            get
            {
                if (perkTree == null && PerkController.Instance != null) perkTree = new PerkTreeData(this);
                return perkTree;
            }
            set { perkTree = value; }
        }

        public AudioProfileType AudioProfile
        {
            get
            {
                if(audioProfile == AudioProfileType.None &&
                    CharacterDataController.Instance != null)
                {
                    audioProfile = CharacterDataController.Instance.GetAudioProfileForRace(race);
                }
                return audioProfile;
            }
        }



    }
}