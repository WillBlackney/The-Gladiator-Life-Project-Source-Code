﻿using HexGameEngine.Abilities;
using HexGameEngine.AI;
using HexGameEngine.Audio;
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
        public AudioProfileType audioProfile;
        public BackgroundData background;

        [Header("Active Properties")]
        public int currentActionPoints;
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
        public ItemSet itemSet;

        [Header("View Components Properties ")]
        public HexCharacterView hexCharacterView;

        [Header("Model Component References")]
        public PerkManagerModel pManager;

        [Header("Data References")]
        public HexCharacterData characterData;

        [Header("Misc")]
        public bool hasMadeTurn;
        public bool hasMadeDelayedTurn;
        public bool hasRequestedTurnDelay;    
        public bool hasDelayedPreviousTurn;
        public bool wasSummonedThisTurn = false;
        public List<VisualEvent> eventStacks = new List<VisualEvent>();

        [Header("Turn Related + Temp Properties")]
        public int tilesMovedThisTurn;
        public int skillAbilitiesUsedThisTurn;
        public int spellAbilitiesUsedThisTurn;
        public int weaponAbilitiesUsedThisTurn;
        public int rangedAttackAbilitiesUsedThisTurn;
        public int meleeAttackAbilitiesUsedThisTurn;
        public int charactersKilledThisTurn;
        public int abilitiesUsedThisCombat = 0;
        public int healthLostThisTurn = 0;
        public bool hasTriggeredSecondWind = false;
        public bool hasTriggeredBringItOn = false;

        [Header("Combat Statistics Properties")]
        public int totalKills;
        public int damageDealtThisCombat;
        public int healthLostThisCombat;
        public int armourLostThisCombat;
        public int stressGainedThisCombat;
        public List<Perk> injuriesGainedThisCombat = new List<Perk>();
        public List<Perk> permanentInjuriesGainedThisCombat = new List<Perk>();
        public int startingArmour;

        // Abilities
        public AbilityBook abilityBook;

        public List<TalentPairing> talentPairings = new List<TalentPairing>();

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

        // Stack Event Logic
        #region
        public VisualEvent GetLastStackEventParent()
        {
            VisualEvent ret = null;
            if (eventStacks.Count == 1) return eventStacks[0];

            for (int i = eventStacks.Count - 1; i > -1; i--)
            {
                if (eventStacks[i].isClosed == false)
                {
                    ret = eventStacks[i];
                    break;
                }
            }
            return ret;
        }
        public void HandlePopStackEvent(VisualEvent stackEvent)
        {
            Debug.Log("HexCharacterModel.HandlePopStackEvent() called...");
            if(eventStacks.Contains(stackEvent) == false)
            {
                Debug.Log("HandlePopStackEvent() attempting to pop event that is not in the character's stack, cancelling...");
                return;
            }
            eventStacks.Remove(stackEvent);
        }
        #endregion

    }

    public enum Facing
    {
        Right = 0,
        Left = 1,
    }
}