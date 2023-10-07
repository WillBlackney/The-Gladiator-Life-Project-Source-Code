using System.Collections.Generic;
using UnityEngine;
using WeAreGladiators.Abilities;
using WeAreGladiators.AI;
using WeAreGladiators.Audio;
using WeAreGladiators.HexTiles;
using WeAreGladiators.Items;
using WeAreGladiators.Perks;
using WeAreGladiators.VisualEvents;

namespace WeAreGladiators.Characters
{
    public class HexCharacterModel
    {
        public int abilitiesUsedThisCombat = 0;

        // Abilities
        public AbilityBook abilityBook;
        public ActivationPhase activationPhase = ActivationPhase.NotActivated;
        public Allegiance allegiance;
        public int armourLostThisCombat;

        [Header("Attributes")]
        public AttributeSheet attributeSheet;
        public AudioProfileType audioProfile;
        public BackgroundData background;

        [Header("AI Logic")]
        public AIBehaviour behaviour;

        [Header("Data References")]
        public HexCharacterData characterData;
        public int charactersKilledThisTurn;
        public Controller controller;

        [Header("Active Properties")]
        public int currentActionPoints;
        public int currentArmour;
        public Facing currentFacing;
        public int currentFatigue;
        public int currentHealth;
        public int currentInitiativeRoll;
        public MoraleState currentMoraleState;

        [Header("Position Properties")]
        public LevelNode currentTile;
        public int damageDealtThisCombat;
        public List<VisualEvent> eventStacks = new List<VisualEvent>();
        public bool hasDelayedPreviousTurn;
        public bool hasMadeDelayedTurn;

        [Header("Misc")]
        public bool hasMadeTurn;
        public bool hasRequestedTurnDelay;
        public bool hasTriggeredBringItOn = false;
        public bool hasTriggeredSecondWind = false;
        public int healthLostThisCombat;
        public int healthLostThisTurn = 0;

        [Header("View Components Properties ")]
        public HexCharacterView hexCharacterView;
        public List<Perk> injuriesGainedThisCombat = new List<Perk>();

        [Header("Item Data References")]
        public ItemSet itemSet;
        public LivingState livingState;
        public int meleeAttackAbilitiesUsedThisTurn;
        [Header("General Properties")]
        public string myName;
        public List<Perk> permanentInjuriesGainedThisCombat = new List<Perk>();

        [Header("Model Component References")]
        public PerkManagerModel pManager;
        public CharacterRace race;
        public int rangedAttackAbilitiesUsedThisTurn;
        public int skillAbilitiesUsedThisTurn;
        public int spellAbilitiesUsedThisTurn;
        public int startingArmour;
        public int moraleStatesLoweredThisCombat;
        public bool guaranteedHeartAttacks = false;

        public List<TalentPairing> talentPairings = new List<TalentPairing>();

        [Header("Turn Related + Temp Properties")]
        public int tilesMovedThisTurn;

        [Header("Combat Statistics Properties")]
        public int totalKills;
        public bool wasSummonedThisTurn = false;
        public int weaponAbilitiesUsedThisTurn;

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
            if (eventStacks.Count == 1)
            {
                return eventStacks[0];
            }

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
            if (eventStacks.Contains(stackEvent) == false)
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
        Left = 1
    }
}
