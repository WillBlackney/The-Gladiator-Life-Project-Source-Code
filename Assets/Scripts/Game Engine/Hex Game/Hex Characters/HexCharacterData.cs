using System.Collections.Generic;
using Sirenix.Serialization;
using UnityEngine;
using WeAreGladiators.Abilities;
using WeAreGladiators.AI;
using WeAreGladiators.Audio;
using WeAreGladiators.Items;
using WeAreGladiators.Perks;
using WeAreGladiators.UCM;

namespace WeAreGladiators.Characters
{
    public class HexCharacterData
    {

        [Header("Item Properties")]
        public AbilityBook abilityBook;
        public List<AttributeRollResult> attributeRolls = new List<AttributeRollResult>();

        [Header("Attributes")]
        public AttributeSheet attributeSheet;
        public AudioProfileType audioProfile;
        public BackgroundData background;
        public int baseArmour;

        [Header("AI Logic")]
        public AIBehaviour behaviour;

        [Header("Health Properties")]
        public int currentHealth;
        public int currentLevel;
        public int currentMaxXP;

        [Header("Stress Properties")]
        public int currentStress;
        public TownActivity currentTownActivity = TownActivity.None;

        [Header("XP Attributes")]
        public int currentXP;
        public int dailyWage;

        [Header("Misc Properties")]
        public Vector2 formationPosition = Vector2.zero;
        public bool ignoreStress = false;

        [Header("Item Properties")]
        public ItemSet itemSet = new ItemSet();

        [Header("Model Properties")]
        public List<string> modelParts;

        public string modelPrefabString;
        public CharacterModelSize modelSize;
        [Header("Story Properties")]
        public string myName;
        public string mySubName;

        [Header("Passive Properties")]
        public PerkManagerModel passiveManager;
        public int perkPoints = 0;
        [OdinSerialize]
        private PerkTreeData perkTree;
        public CharacterRace race;
        public List<TalentPairing> talentPairings = new List<TalentPairing>();

        public int talentPoints = 0;
        public int xpReward;
        public PerkTreeData PerkTree
        {
            get
            {
                if (perkTree == null && PerkController.Instance != null)
                {
                    perkTree = new PerkTreeData(this);
                }
                return perkTree;
            }
            set => perkTree = value;
        }

        public AudioProfileType AudioProfile
        {
            get
            {
                if (audioProfile == AudioProfileType.None &&
                    CharacterDataController.Instance != null)
                {
                    audioProfile = CharacterDataController.Instance.GetAudioProfileForRace(race);
                }
                return audioProfile;
            }
        }

        public CharacterModel ModelPrefab
        {
            get
            {
                CharacterModel ret = null;
                foreach (CharacterModel model in CharacterDataController.Instance.AllCharacterModels)
                {
                    if (model.name == modelPrefabString)
                    {
                        ret = model;
                        break;
                    }
                }
                return ret;
            }
        }
    }
}
