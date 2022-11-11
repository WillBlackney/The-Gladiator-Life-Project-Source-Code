using HexGameEngine.Abilities;
using HexGameEngine.AI;
using HexGameEngine.Items;
using HexGameEngine.Perks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.Characters
{
    public class HexCharacterData
    {
        [Header("Story Properties")]
        public string myName;
        public string myClassName;
        public CharacterRace race;
        public CharacterModelSize modelSize;
        public BackgroundData background;
        public int xpReward;
        public int baseArmour;

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
        public AITurnRoutine aiTurnRoutine;

        [Header("Item Properties")]
        public ItemSet itemSet = new ItemSet();

        [Header("Item Properties")]
        public AbilityBook abilityBook;
        public List<TalentPairing> talentPairings = new List<TalentPairing>();

        [Header("Misc Properties")]
        public Vector2 formationPosition = Vector2.zero;
        public int dailyWage;

        public List<AttributeRollResult> attributeRolls = new List<AttributeRollResult>();
        public PerkTreeData perkTree = new PerkTreeData();
        public int perkPoints = 0;
        public List<TalentRollResult> talentRolls = new List<TalentRollResult>();
    }
}