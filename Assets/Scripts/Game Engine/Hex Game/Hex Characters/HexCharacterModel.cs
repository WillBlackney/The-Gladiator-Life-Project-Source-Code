using HexGameEngine.Abilities;
using HexGameEngine.AI;
using HexGameEngine.HexTiles;
using HexGameEngine.Items;
using HexGameEngine.Perks;
using HexGameEngine.VisualEvents;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TbsFramework.Pathfinding.Algorithms;
using UnityEngine;

namespace HexGameEngine.Characters
{
    public class HexCharacterModel
    {
        [Header("General Properties")]
        public string myName;
        public Allegiance allegiance;
        public Controller controller;
        public LivingState livingState;
        public ActivationPhase activationPhase = ActivationPhase.NotActivated;
        public CharacterRace race;
        public BackgroundData background;

        [Header("Active Properties")]
        public int currentEnergy;
        public int currentFatigue;
        public int currentHealth;
        public Facing currentFacing;
        public int currentInitiativeRoll;
        public int currentStress;
        public int currentArmour;

        [Header("AI Logic")]
        public AITurnRoutine aiTurnRoutine;

        [Header("Attributes")]
        public AttributeSheet attributeSheet;

        [Header("Position Properties")]
        public LevelNode currentTile;

        [Header("Item Data References")]
        [HideInInspector] public ItemSet itemSet;

        [Header("View Components Properties ")]
        public HexCharacterView hexCharacterView;

        [Header("Model Component References")]
        [HideInInspector] public PerkManagerModel pManager;

        [Header("Data References")]
        [HideInInspector] public HexCharacterData characterData;

        [Header("Misc")]
        public bool hasMadeTurn;
        public bool hasMadeDelayedTurn;
        public bool hasRequestedTurnDelay;    
        public bool hasDelayedPreviousTurn;
        public bool wasSummonedThisTurn = false;
        [HideInInspector] public List<VisualEvent> eventStacks = new List<VisualEvent>();

        [Header("Turn Related + Temp Properties")]
        public int tilesMovedThisTurn;
        public int skillAbilitiesUsedThisTurn;
        public int weaponAbilitiesUsedThisTurn;
        public int rangedAttackAbilitiesUsedThisTurn;
        public int meleeAttackAbilitiesUsedThisTurn;
        public int charactersKilledThisTurn;

        [Header("Combat Statistics Properties")]
        public int totalKills;
        public int healthLostThisCombat;
        public int stressGainedThisCombat;
        public List<Perk> injuriesGainedThisCombat = new List<Perk>();
        public List<Perk> permanentInjuriesGainedThisCombat = new List<Perk>();

        // Abilities
        [HideInInspector] public AbilityBook abilityBook;

        [HideInInspector] public List<TalentPairing> talentPairings = new List<TalentPairing>();

        // Stack Event Logic
        #region
        public VisualEvent GetLastStackEventParent()
        {
            return eventStacks.Count > 0 ? eventStacks[eventStacks.Count - 1] : null;
        }
        #endregion

    }

    public enum Facing
    {
        Right = 0,
        Left = 1,
    }
}