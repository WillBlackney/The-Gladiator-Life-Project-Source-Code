﻿using System.Collections.Generic;
using UnityEngine;
using HexGameEngine.Items;
using HexGameEngine.Abilities;
using HexGameEngine.Perks;
using HexGameEngine.Utilities;
using HexGameEngine.Persistency;
using HexGameEngine.Player;

namespace HexGameEngine.Characters
{
    public class CharacterDataController : Singleton<CharacterDataController>
    {
        // Properties + Components
        #region

        // Inspector
        [Header("Data Buckets")]
        [SerializeField] private HexCharacterTemplateSO[] allCustomCharacterTemplatesSOs;
        [SerializeField] private TalentDataSO[] allTalentData;
        [SerializeField] private RaceDataSO[] allRacialData;
        [SerializeField] private BackgroundDataSO[] allCharacterBackgroundSOs;

        [Header("Character Generation")]
        [SerializeField] private List<ClassTemplateSO> allClassTemplateSOs;
        [SerializeField] private List<ClassTemplateSO> allDraftTemplateSOs;
        [SerializeField] private List<CharacterModelTemplateSO> allModelTemplateSOs;  

        [Header("Character Name Buckets")]
        [SerializeField] string[] humanNames;
        [SerializeField] string[] elfNames;
        [SerializeField] string[] orcNames;
        [SerializeField] string[] satyrNames;
        [SerializeField] string[] goblinNames;
        [SerializeField] string[] entNames;
        [SerializeField] string[] demonNames;
        [SerializeField] string[] gnollNames;
        [SerializeField] string[] undeadNames;
        [Space(20)]

        [Header("Recruit Base Attribute Ranges")]
        [SerializeField] private int mightLower;
        [SerializeField] private int mightUpper;
        [Space(10)]
        [SerializeField] private int constitutionLower;
        [SerializeField] private int constitutionUpper;
        [Space(10)]
        [SerializeField] private int accuracyLower;
        [SerializeField] private int accuracyUpper;
        [Space(10)]
        [SerializeField] private int dodgeLower;
        [SerializeField] private int dodgeUpper;
        [Space(10)]
        [SerializeField] private int fatigueLower;
        [SerializeField] private int fatigueUpper;
        [Space(10)]
        [SerializeField] private int resolveLower;
        [SerializeField] private int resolveUpper;
        [Space(10)]
        [SerializeField] private int witsLower;
        [SerializeField] private int witsUpper;
        [Space(10)]
        // Non-Inspector 
        private HexCharacterData[] allCustomCharacterTemplates;
        private BackgroundData[] allCharacterBackgrounds;
        private List<HexCharacterData> allPlayerCharacters = new List<HexCharacterData>();
        private List<HexCharacterData> characterDeck = new List<HexCharacterData>();
        private List<CharacterRace> validCharacterRaces = new List<CharacterRace>
        { 
            CharacterRace.Elf, 
            CharacterRace.Gnoll, 
            CharacterRace.Goblin,
            CharacterRace.Human,
            CharacterRace.Orc, 
            CharacterRace.Satyr, 
            CharacterRace.Undead
        };
       

        #endregion

        // Getters + Accessors
        #region
        public List<CharacterRace> PlayableRaces
        {
            get { return validCharacterRaces; }
        }
        public TalentDataSO[] AllTalentData
        {
            get { return allTalentData; }
        }
        public BackgroundData[] AllCharacterBackgrounds
        {
            get { return allCharacterBackgrounds; }
            private set { allCharacterBackgrounds = value; }
        }
       
        public HexCharacterData[] AllCustomCharacterTemplates
        {
            get { return allCustomCharacterTemplates; }
            private set { allCustomCharacterTemplates = value; }
        }
        public List<HexCharacterData> AllPlayerCharacters
        {
            get { return allPlayerCharacters; }
            private set { allPlayerCharacters = value; }
        }
        public List<HexCharacterData> CharacterDeck
        {
            get { return characterDeck; }
            private set { characterDeck = value; }
        }

        #endregion

        // Racial Data
        #region
        public RaceDataSO GetRaceData(CharacterRace race)
        {
            RaceDataSO ret = null;

            foreach(RaceDataSO r in allRacialData)
            {
                if(r.racialTag == race)
                {
                    ret = r;
                    break;
                }
            }

            if (ret == null) Debug.LogWarning("GetRaceData() could not racial data for " + race.ToString() + ", returning null...");

            return ret;
        }
        #endregion

        // Initialization + Setup
        #region
        protected override void Awake()
        {
            base.Awake();
            BuildBackgroundLibrary();
            BuildCustomTemplateLibrary();           
        }
        private void BuildBackgroundLibrary()
        {
            Debug.Log("CharacterDataController.BuildBackgroundLibrary() called...");

            List<BackgroundData> tempList = new List<BackgroundData>();

            foreach (BackgroundDataSO dataSO in allCharacterBackgroundSOs)
            {
                tempList.Add(new BackgroundData(dataSO));
            }

            AllCharacterBackgrounds = tempList.ToArray();
        }
      
        private void BuildCustomTemplateLibrary()
        {
            Debug.Log("CharacterDataController.BuildCustomTemplateLibrary() called...");

            List<HexCharacterData> tempList = new List<HexCharacterData>();

            foreach (HexCharacterTemplateSO dataSO in allCustomCharacterTemplatesSOs)
            {
                tempList.Add(ConvertCharacterTemplateToCharacterData(dataSO));
            }

            AllCustomCharacterTemplates = tempList.ToArray();
        }
        #endregion

        // Data Conversion + Cloning
        #region
        public HexCharacterData ConvertCharacterTemplateToCharacterData(HexCharacterTemplateSO template)
        {
            Debug.Log("CharacterDataController.ConvertCharacterTemplateToCharacterData() called...");
            HexCharacterData newCharacter = new HexCharacterData();

            // Set up background + story data
            newCharacter.myName = template.myName;
            newCharacter.myClassName = template.myClassName;
            newCharacter.race = template.race;
            newCharacter.modelSize = template.modelSize;
            SetStartingLevelAndXpValues(newCharacter);
            newCharacter.background = GetBackgroundData(CharacterBackground.Unknown);

            // Setup stats
            newCharacter.attributeSheet = new AttributeSheet();
            template.attributeSheet.CopyValuesIntoOther(newCharacter.attributeSheet);

            // Passive Data
            newCharacter.passiveManager = new PerkManagerModel(newCharacter);
            PerkController.Instance.BuildPassiveManagerFromSerializedPassiveManager(newCharacter.passiveManager, template.serializedPassiveManager);

            // Set up health
            SetCharacterHealth(newCharacter, StatCalculator.GetTotalMaxHealth(newCharacter));

            // UCM Data
            newCharacter.modelParts = new List<string>();
            newCharacter.modelParts.AddRange(template.modelParts);

            // Item Data
            newCharacter.itemSet = new ItemSet();
            ItemController.Instance.CopySerializedItemManagerIntoStandardItemManager(template.itemSet, newCharacter.itemSet);

            // Ability Data
            newCharacter.abilityBook = new AbilityBook(template.abilityBook);

            // Learn weapon abilities
            newCharacter.abilityBook.HandleLearnAbilitiesFromItemSet(newCharacter.itemSet);

            // Talent Data
            foreach (TalentPairing tp in template.talentPairings)
            {
                newCharacter.talentPairings.Add(tp);
            }

            return newCharacter;
        }
        public HexCharacterData GenerateEnemyDataFromEnemyTemplate(EnemyTemplateSO template)
        {
            Debug.Log("CharacterDataController.GenerateEnemyDataFromEnemyTemplate() called...");
            HexCharacterData newCharacter = new HexCharacterData();

            // Set up background + story data
            newCharacter.myName = template.myName;
            newCharacter.race = template.race;
            newCharacter.modelSize = template.modelSize;
            newCharacter.xpReward = template.xpReward;
            newCharacter.baseArmour = template.baseArmour;

            // Setup stats
            newCharacter.attributeSheet = new AttributeSheet();
            template.attributeSheet.CopyValuesIntoOther(newCharacter.attributeSheet);

            // Passive Data
            newCharacter.passiveManager = new PerkManagerModel(newCharacter);
            PerkController.Instance.BuildPassiveManagerFromSerializedPassiveManager(newCharacter.passiveManager, template.serializedPassiveManager);

            // Set up health
            if (template.randomizeHealth)
                SetCharacterMaxHealth(newCharacter, RandomGenerator.NumberBetween(template.lowerHealthLimit, template.upperHealthLimit));
            SetCharacterHealth(newCharacter, StatCalculator.GetTotalMaxHealth(newCharacter));

            // UCM Data
            newCharacter.modelParts = new List<string>();
            newCharacter.modelParts.AddRange(template.modelParts);

            // Ai Routine
            newCharacter.aiTurnRoutine = template.aiTurnRoutine;
            if (newCharacter.aiTurnRoutine == null)
                Debug.LogWarning("Routine is null...");

            // Item Data
            newCharacter.itemSet = new ItemSet();
            ItemController.Instance.CopySerializedItemManagerIntoStandardItemManager(template.itemSet, newCharacter.itemSet);

            // Ability Data
            newCharacter.abilityBook = new AbilityBook(template.abilityBook);

            // Learn weapon abilities
            newCharacter.abilityBook.HandleLearnAbilitiesFromItemSet(newCharacter.itemSet, false);

            return newCharacter;
        }
        public HexCharacterData CloneCharacterData(HexCharacterData original)
        {
            Debug.Log("CharacterDataController.CloneCharacterData() called...");

            HexCharacterData newCharacter = new HexCharacterData();

            // Set up background + story data
            newCharacter.myName = original.myName;
            newCharacter.myClassName = original.myClassName;
            newCharacter.race = original.race;
            newCharacter.background = original.background;
            newCharacter.modelSize = original.modelSize;
            newCharacter.xpReward = original.xpReward;
            newCharacter.baseArmour = original.baseArmour;

            // Set Xp + Level
            newCharacter.currentLevel = original.currentLevel;
            newCharacter.currentMaxXP = original.currentMaxXP;
            newCharacter.currentXP = original.currentXP;
            newCharacter.dailyWage = original.dailyWage;
            newCharacter.perkPoints = original.perkPoints;
            newCharacter.perkTree = original.perkTree;
            newCharacter.recruitCost = original.recruitCost;

            // Set stress
            newCharacter.currentStress = original.currentStress;

            // Setup stats
            newCharacter.attributeSheet = new AttributeSheet();
            original.attributeSheet.CopyValuesIntoOther(newCharacter.attributeSheet);

            // Passive Data
            newCharacter.passiveManager = new PerkManagerModel(newCharacter);
            PerkController.Instance.BuildPassiveManagerFromOtherPassiveManager(original.passiveManager, newCharacter.passiveManager);

            // Set up health
            SetCharacterHealth(newCharacter, StatCalculator.GetTotalMaxHealth(newCharacter));

            // UCM Data
            newCharacter.modelParts = new List<string>();
            newCharacter.modelParts.AddRange(original.modelParts);

            // Item Data
            newCharacter.itemSet = new ItemSet();
            ItemController.Instance.CopyItemManagerDataIntoOtherItemManager(original.itemSet, newCharacter.itemSet);

            // Ability Data
            newCharacter.abilityBook = new AbilityBook(original.abilityBook);
           // foreach (AbilityData a in original.abilityBook.knownAbilities)
           //     newCharacter.abilityBook.HandleLearnNewAbility(a);

            // Attribute rolls        
            newCharacter.attributeRolls = new List<AttributeRollResult>();
            foreach (AttributeRollResult arr in original.attributeRolls)            
                newCharacter.attributeRolls.Add(arr);            

            // Talent Data
            foreach (TalentPairing tp in original.talentPairings)            
                newCharacter.talentPairings.Add(new TalentPairing(tp.talentSchool, tp.level));            

            return newCharacter;

        }
        public BackgroundData GetBackgroundData(CharacterBackground bg)
        {
            BackgroundData ret = null;

            foreach(BackgroundData data in allCharacterBackgrounds)
            {
                if(data.backgroundType == bg)
                {
                    ret = data;
                    break;
                }
            }

            return ret;
        }
        #endregion

        // Modify Attributes + Stats + Health
        #region
        public void SetCharacterStress(HexCharacterData data, int newValue)
        {
            Debug.Log("CharacterDataController.SetCharacterStress() called for '" +
                data.myName + "', new stress value = " + newValue.ToString());

            data.currentStress = newValue;

            // prevent stress exceeding limits
            if (data.currentStress > 100)
                data.currentStress = 100;
            // Zealots can never reach shattered stress state
            else if (DoesCharacterHaveBackground(data.background, CharacterBackground.Zealot) &&
                data.currentStress > 99) data.currentStress = 99;
            else if (data.currentStress < 0)
                data.currentStress = 0;
        }
        public void SetCharacterHealth(HexCharacterData data, int newValue)
        {
            Debug.Log("CharacterDataController.SetCharacterHealth() called for '" +
                data.myName + "', new health value = " + newValue.ToString());

            data.currentHealth = newValue;

            // prevent current health exceeding max health
            if (data.currentHealth > StatCalculator.GetTotalMaxHealth(data))
                data.currentHealth = StatCalculator.GetTotalMaxHealth(data);
            else if (data.currentHealth < 0) data.currentHealth = 0;
        }
        public void SetCharacterMaxHealth(HexCharacterData data, int newValue)
        {
            Debug.Log("CharacterDataController.SetCharacterMaxHealth() called for '" +
                data.myName + "', new max health value = " + newValue.ToString());

            data.attributeSheet.maxHealth = newValue;

            // prevent current health exceeding max health
            if (data.currentHealth > StatCalculator.GetTotalMaxHealth(data))
                data.currentHealth = StatCalculator.GetTotalMaxHealth(data);
        }
        
        #endregion

        // Build Character Roster
        #region
        public void BuildCharacterRoster(List<HexCharacterData> characters)
        {
            allPlayerCharacters.Clear();
            foreach (HexCharacterData c in characters)
                AddCharacterToRoster(c);
        }
        public void AddCharacterToRoster(HexCharacterData character)
        {
            AllPlayerCharacters.Add(character);
            
        }
        public void ClearCharacterRoster()
        {
            AllPlayerCharacters.Clear();
        }
        public void ClearCharacterDeck()
        {
            CharacterDeck.Clear();
        }
        #endregion

        // Town + New day logic
        #region
        public void HandlePayDailyWagesOnNewDayStart()
        {
            foreach(HexCharacterData character in AllPlayerCharacters)
            {
                if(PlayerDataController.Instance.CurrentGold > character.dailyWage)
                {
                    PlayerDataController.Instance.ModifyPlayerGold(-character.dailyWage);
                }
                // if not enough money to pay wage, chance that character will eave the roster, or gain stress?
                else
                {

                }
            }
        }
        public void HandlePassiveStressAndHealthRecoveryOnNewDayStart()
        {
            foreach(HexCharacterData c in AllPlayerCharacters)
            {
                SetCharacterStress(c, c.currentStress - 5);
                SetCharacterHealth(c, c.currentHealth + (int)(StatCalculator.GetTotalMaxHealth(c) * 0.1f));
            }
        }
        #endregion

        // Talent Logic
        #region
        public void HandleLearnNewTalent(HexCharacterData character, TalentSchool talentSchool)
        {
            Debug.Log("CharacterDataController.HandleLearnNewTalent() called, character " + character.myName +
                " gaining talent: " + talentSchool.ToString());

            if(DoesCharacterHaveTalent(character.talentPairings, talentSchool, 1))
            {
                foreach(TalentPairing tp in character.talentPairings)
                {
                    if(tp.talentSchool == talentSchool)
                    {
                        tp.level++;
                    }
                }
            }
            else
            {
                character.talentPairings.Add(new TalentPairing(talentSchool, 1));
            }

        }
        public bool DoesCharacterHaveTalent(List<TalentPairing> talents, TalentSchool ts, int level)
        {
            bool bRet = false;

            foreach (TalentPairing t in talents)
            {
                if(t.talentSchool == ts && t.level >= level)
                {
                    bRet = true;
                    break;
                }
            }

            return bRet;
        }
        public bool DoesCharacterHaveBackground(BackgroundData character, CharacterBackground background)
        {
            if (character != null && character.backgroundType == background) return true;
            else return false;
        }
        public int GetCharacterTalentLevel(List<TalentPairing> allTalents, TalentSchool ts)
        {
            int ret = 0;
            foreach(TalentPairing t in allTalents)
            {
                if(t.talentSchool == ts)
                {
                    ret = t.level;
                    break;
                }                  
            }
            return ret;
        }
        public Sprite GetTalentSprite(TalentSchool talent)
        {
            Sprite sRet = null;
            foreach(TalentDataSO t in AllTalentData)
            {
                if(t.talentSchool == talent)
                {
                    sRet = t.talentSprite;
                    break;
                }
            }

            return sRet;
        }
        public TalentDataSO GetTalentDataFromTalentEnum(TalentSchool talent)
        {
            TalentDataSO dataRet = null;

            foreach(TalentDataSO t in AllTalentData)
            {
                if(t.talentSchool == talent)
                {
                    dataRet = t;
                    break;
                }
            }

            return dataRet;
        }
        public List<TalentPairing> GetValidLevelUpTalents(HexCharacterData character)
        {
            List<TalentPairing> ret = new List<TalentPairing>();
            List<TalentSchool> talentSchools = new List<TalentSchool> { TalentSchool.Divinity, TalentSchool.Guardian, TalentSchool.Manipulation,
            TalentSchool.Naturalism, TalentSchool.Pyromania, TalentSchool.Ranger, TalentSchool.Scoundrel, TalentSchool.Shadowcraft, TalentSchool.Warfare, TalentSchool.Metamorph };

            foreach(TalentSchool ts in talentSchools)
            {
                if (!DoesCharacterHaveTalent(character.talentPairings, ts, 1))
                    ret.Add(new TalentPairing(ts, 1));
               // else if (!DoesCharacterHaveTalent(character.talentPairings, ts, 2))
               //     ret.Add(new TalentPairing(ts, 2));
            }

            List<TalentPairing> invalidPairings = new List<TalentPairing>();

            // filter out invalid perks
            foreach (TalentPairing p in ret)
            {
                // check if talent was previously offered in another roll
                foreach (TalentRollResult roll in character.talentRolls)
                {
                    foreach (TalentPairing p2 in roll.talentChoices)
                    {
                        if (p2.talentSchool == p.talentSchool &&
                            p2.level == p.level)
                        {
                            invalidPairings.Add(p);
                        }
                    }
                }
            }

            foreach(TalentPairing p in invalidPairings)
            {
                ret.Remove(p);
            }

            /*
            foreach (TalentPairing p in invalidPairings)
            {
                for(int i = 0; i < ret.Count; i++)
                {
                    if(ret[i].talentSchool == p.talentSchool &&
                        ret[i].level == p.level)
                    {
                        ret.RemoveAt(i);
                        break;
                    }
                }
            }
            */

            return ret;
        }
        #endregion

        // Save + Load Logic
        #region
        public void BuildMyDataFromSaveFile(SaveGameData saveFile)
        {
            // Build character roster
            AllPlayerCharacters.Clear();
            foreach (HexCharacterData characterData in saveFile.characterRoster)            
                AddCharacterToRoster(characterData);            

            // Build character deck
            CharacterDeck.Clear();
            foreach (HexCharacterData cd in saveFile.characterDeck)
                CharacterDeck.Add(cd);
        }
        public void SaveMyDataToSaveFile(SaveGameData saveFile)
        {
            foreach (HexCharacterData character in AllPlayerCharacters)
                saveFile.characterRoster.Add(character);

            foreach (HexCharacterData character in CharacterDeck)
                saveFile.characterDeck.Add(character);

        }
        #endregion

        // XP + Leveling Logic
        #region
        private void SetStartingLevelAndXpValues(HexCharacterData character)
        {
            character.currentXP = 0;
            character.currentMaxXP = 50;
            SetCharacterLevel(character, 1);
        }
        private int GetMaxXpCapForLevel(int level)
        {
            // each level requires 50 more XP than the previous.
            if (level == 1) return 50;
            else return 50 + (75 * (level - 1));
        }
        private void SetCharacterLevel(HexCharacterData data, int newLevelValue)
        {
            data.currentLevel = newLevelValue;
        }
        public void HandleGainXP(HexCharacterData data, int xpGained)
        {
            float xpGainMod = 1f;
            if(xpGained > 0)
            {
                if (PerkController.Instance.DoesCharacterHavePerk(data.passiveManager, Perk.FastLearner))                
                    xpGainMod += 0.25f;
                
                if (PerkController.Instance.DoesCharacterHavePerk(data.passiveManager, Perk.DimWitted))                
                    xpGainMod -= 0.25f;

                if (PerkController.Instance.DoesCharacterHavePerk(data.passiveManager, Perk.PermanentlyConcussed))
                    xpGainMod -= 0.25f;

                if (DoesCharacterHaveBackground(data.background, CharacterBackground.Doctor))
                    xpGainMod += 0.15f;
            }         

            xpGained = (int)(xpGained * xpGainMod);

            // check spill over + level up first
            int spillOver = (data.currentXP + xpGained) - data.currentMaxXP;

            // Level up occured with spill over XP
            if (spillOver > 0)
            {
                // Gain level
                SetCharacterLevel(data, data.currentLevel + 1);

                // Do attribute level up roll result logic
                data.attributeRolls.Add(AttributeRollResult.GenerateRoll(data));

                // Gain perk level up
                if(PerkController.Instance.GetAllLevelUpPerksOnCharacter(data).Count < 5)
                    data.perkPoints++;

                // Gain talent level up
                if ((data.currentLevel == 3 || data.currentLevel == 5) && data.talentPairings.Count < 3)
                    data.talentRolls.Add(TalentRollResult.GenerateRoll(data));

                // Reset current xp
                data.currentXP = 0;

                // Increase max xp on level up
                data.currentMaxXP = GetMaxXpCapForLevel(data.currentLevel);

                // Restart the xp gain procces with the spill over amount
                HandleGainXP(data, spillOver);


            }

            // Level up with no spill over
            else if (spillOver == 0)
            {
                // Gain level
                SetCharacterLevel(data, data.currentLevel + 1);

                // Do attribute level up roll result logic
                data.attributeRolls.Add(AttributeRollResult.GenerateRoll(data));

                // Gain perk level up
                if (PerkController.Instance.GetAllLevelUpPerksOnCharacter(data).Count < 5)
                    data.perkPoints++;

                // Gain talent level up
                if ((data.currentLevel == 3 || data.currentLevel == 5) && data.talentPairings.Count < 3)
                    data.talentRolls.Add(TalentRollResult.GenerateRoll(data));

                // Reset current xp
                data.currentXP = 0;

                // Increase max xp on level up
                data.currentMaxXP = GetMaxXpCapForLevel(data.currentLevel);

            }

            // Gain xp without leveling up
            else
            {
                data.currentXP += xpGained;
            }
        }
        #endregion
              
        // Character Generation + Character Deck Logic
        #region
        private HexCharacterData GenerateRecruitCharacter(ClassTemplateSO ct, CharacterRace race, int tier = 1)
        {
            Debug.Log("CharacterDataController.GenerateRecruitCharacter() called...");

            HexCharacterData newCharacter = new HexCharacterData();

            // Set up background + story data
            newCharacter.myName = GetRandomCharacterName(race);
            newCharacter.myClassName = ct.templateName;
            newCharacter.race = race;
            newCharacter.modelSize = CharacterModelSize.Normal;
            SetStartingLevelAndXpValues(newCharacter);
            newCharacter.background = GenerateRandomBackgroundForCharacter(race);

            // Set up perks
            newCharacter.passiveManager = new PerkManagerModel(newCharacter);
            GetAndApplyRandomQuirksToCharacter(newCharacter, tier);

            // Setup stats + stars
            newCharacter.attributeSheet = new AttributeSheet();
            GenerateRecruitCharacterAttributeRolls(newCharacter.attributeSheet, newCharacter.background);
            GenerateCharacterStarRolls(newCharacter.attributeSheet, 3);

            // Randomize cost + daily wage
            newCharacter.dailyWage = RandomGenerator.NumberBetween(newCharacter.background.dailyWageMin, newCharacter.background.dailyWageMax);

            // to do: replace this with dynamic function: GetCharacterRecruitCost, which considers base cost, gear, town modifiers, difficulty mods, etc
            newCharacter.recruitCost = newCharacter.background.baseRecruitCost;

            // Set up health
            SetCharacterMaxHealth(newCharacter, 0);
            SetCharacterHealth(newCharacter, StatCalculator.GetTotalMaxHealth(newCharacter));

            // Randomize model appearance + outfit
            newCharacter.modelParts = new List<string>();
            CharacterModelTemplateSO randomRaceModel = GetRandomModelTemplate(race);
            newCharacter.modelParts.AddRange(randomRaceModel.bodyParts);

            // Randomize items
            newCharacter.itemSet = new ItemSet();
            SerializedItemSet randomItemSet = ct.possibleWeapons[RandomGenerator.NumberBetween(0, ct.possibleWeapons.Count - 1)];
            ItemController.Instance.CopySerializedItemManagerIntoStandardItemManager(randomItemSet, newCharacter.itemSet);

            // Set up abilities
            newCharacter.abilityBook = new AbilityBook();
            List<AbilityDataSO> selectedAbilities = GenerateRecruitCharacterAbilitiesFromProspects(ct.possibleAbilities, ct.startingAbilityCount);
            foreach(AbilityDataSO a in selectedAbilities)
            {
                newCharacter.abilityBook.HandleLearnNewAbility(AbilityController.Instance.BuildAbilityDataFromScriptableObjectData(a));
            }

            // Learn weapon abilities
            newCharacter.abilityBook.HandleLearnAbilitiesFromItemSet(newCharacter.itemSet);

            // Talent Data
            foreach (TalentPairing tp in ct.talentPairings)
            {
                newCharacter.talentPairings.Add(new TalentPairing
                {
                    talentSchool = tp.talentSchool,
                    level = tp.level
                });
            }

            return newCharacter;
        }
        private BackgroundData GenerateRandomBackgroundForCharacter(CharacterRace race)
        {
            List<BackgroundData> validBackgrounds = new List<BackgroundData>();
            foreach(BackgroundData b in allCharacterBackgrounds)
            {
                if (b.validRaces.Contains(race))
                    validBackgrounds.Add(b);
            }
            return validBackgrounds[RandomGenerator.NumberBetween(0, validBackgrounds.Count - 1)];
        }
        private void GenerateRecruitCharacterAttributeRolls(AttributeSheet sheet, BackgroundData background)
        {
            sheet.might.value += RandomGenerator.NumberBetween(background.mightLower + mightLower, background.mightUpper + mightUpper);
            sheet.constitution.value += RandomGenerator.NumberBetween(background.constitutionLower + constitutionLower, background.constitutionUpper + constitutionUpper);
            sheet.accuracy.value += RandomGenerator.NumberBetween(background.accuracyLower + accuracyLower, background.accuracyUpper + accuracyUpper);
            sheet.dodge.value += RandomGenerator.NumberBetween(background.dodgeLower + dodgeLower, background.dodgeUpper + dodgeUpper);
            sheet.wits.value += RandomGenerator.NumberBetween(background.witsLower + witsLower, background.witsUpper + witsUpper);
            sheet.fatigue.value += RandomGenerator.NumberBetween(background.fatigueLower + fatigueLower, background.fatigueUpper + fatigueUpper);
            sheet.resolve.value += RandomGenerator.NumberBetween(background.resolveLower + resolveLower, background.resolveUpper + resolveUpper);
        }
        private List<AbilityDataSO> GenerateRecruitCharacterAbilitiesFromProspects(List<AbilityDataSO> prospects, int amount = 3)
        {
            // Determine and randomize valid abilities to learn
            List<AbilityDataSO> abilitiesRet = new List<AbilityDataSO>();
            List<AbilityDataSO> filteredPossibleAbilities = new List<AbilityDataSO>();
            filteredPossibleAbilities.AddRange(prospects);
            filteredPossibleAbilities.Shuffle();

            // Pop the first ability, remember its talent school to prevent trippling up on the same talent school later
            bool multipleTalents = false;
            abilitiesRet.Add(filteredPossibleAbilities[0]);
            TalentSchool firstTalent = abilitiesRet[0].talentRequirementData.talentSchool;
            filteredPossibleAbilities.RemoveAt(0);      
            
            // Check for more than 1 talent school in possible prospects
            foreach(AbilityDataSO a in filteredPossibleAbilities)
            {
                if(a.talentRequirementData.talentSchool != firstTalent)
                {
                    multipleTalents = true;
                    break;
                }
            }

            // Should try and get abilities from two different talents?
            if (multipleTalents)
            {
                for (int i = 0; i < amount - 1; i++)
                {
                    if (i == 0)
                    {
                        for (int j = 0; j < filteredPossibleAbilities.Count; j++)
                        {
                            if (filteredPossibleAbilities[j].talentRequirementData.talentSchool != firstTalent)
                            {
                                abilitiesRet.Add(filteredPossibleAbilities[j]);
                                filteredPossibleAbilities.Remove(filteredPossibleAbilities[j]);
                                break;
                            }
                        }

                    }
                    else
                    {
                        AbilityDataSO a = filteredPossibleAbilities[0];
                        filteredPossibleAbilities.Remove(a);
                        abilitiesRet.Add(a);
                    }
                }
            }

            // Character only has points in 1 talent tree, just grab all the abilities from that tree
            else
            {
                for(int i = 0; i < amount - 1; i++)
                {
                    abilitiesRet.Add(filteredPossibleAbilities[i]);
                }
            }
            return abilitiesRet;
        }
        private void GenerateRecruitCharacterStatRolls(AttributeSheet sheet, int totalMods, int tier)
        {
            List<CoreAttribute> attributes = new List<CoreAttribute> 
            { 
                CoreAttribute.Accuracy, 
                CoreAttribute.Constitution, 
                CoreAttribute.Dodge, 
                CoreAttribute.Resolve, 
                CoreAttribute.Might, 
                CoreAttribute.Wits 
            };

            attributes.Shuffle();

            int baseStatBoost = 0;
            if (tier == 2) baseStatBoost = 3;
            else if (tier == 3) baseStatBoost = 5;

            for (int i = 0; i < totalMods; i++)
            {
                // Randomize how much a stat is boosted or lowered
                int statMod = RandomGenerator.NumberBetween(2, 5);
                int positiveBoostChance = 50;
                if (tier == 2) positiveBoostChance = 40;
                else if (tier == 3) positiveBoostChance = 25;

                // 50% chance to get a negative stat boost
                bool lowerStat = RandomGenerator.NumberBetween(1, 100) <= positiveBoostChance; 
                if (lowerStat) statMod = -statMod;

                if (attributes[i] == CoreAttribute.Accuracy) sheet.accuracy.value += statMod + baseStatBoost;
                else if (attributes[i] == CoreAttribute.Constitution) sheet.constitution.value += statMod + baseStatBoost;
                else if (attributes[i] == CoreAttribute.Dodge) sheet.dodge.value += statMod + baseStatBoost;
                else if (attributes[i] == CoreAttribute.Resolve) sheet.resolve.value += statMod + baseStatBoost;
                else if (attributes[i] == CoreAttribute.Might) sheet.might.value += statMod + baseStatBoost;
                else if (attributes[i] == CoreAttribute.Wits) sheet.wits.value += statMod + baseStatBoost;
            }


        }
        private void GenerateCharacterStarRolls(AttributeSheet sheet, int statsStarred = 3, int minStarsGainedPerStat = 1, int maxStarsGainedPerStat = 2)
        {
            List<CoreAttribute> attributes = new List<CoreAttribute>
            {
                CoreAttribute.Accuracy,
                CoreAttribute.Constitution,
                CoreAttribute.Dodge,
                CoreAttribute.Resolve,
                CoreAttribute.Might,
                CoreAttribute.Wits
            };

            attributes.Shuffle();

            for(int i = 0; i < statsStarred; i++)
            {
                int starsGained = RandomGenerator.NumberBetween(minStarsGainedPerStat, maxStarsGainedPerStat);

                if (attributes[i] == CoreAttribute.Accuracy) sheet.accuracy.stars = starsGained;
                else if (attributes[i] == CoreAttribute.Constitution) sheet.constitution.stars = starsGained;
                else if (attributes[i] == CoreAttribute.Dodge) sheet.dodge.stars = starsGained;
                else if (attributes[i] == CoreAttribute.Resolve) sheet.resolve.stars = starsGained;
                else if (attributes[i] == CoreAttribute.Might) sheet.might.stars = starsGained;
                else if (attributes[i] == CoreAttribute.Wits) sheet.wits.stars = starsGained;
            }
        }
        private int GenerateCharacterDailyWage(int tier = 1)
        {
            int lower = 5;
            int upper = 8;
            if(tier == 2)
            {
                lower = 10;
                upper = 15;
            }
            else if(tier == 3)
            {
                lower = 17;
                upper = 22;
            }

            return RandomGenerator.NumberBetween(lower, upper);
        }
        private int GenerateCharacterRecruitCost(int tier = 1)
        {
            int lower = 30;
            int upper = 50;
            if (tier == 2)
            {
                lower = 75;
                upper = 100;
            }
            else if (tier == 3)
            {
                lower = 125;
                upper = 150;
            }

            return RandomGenerator.NumberBetween(lower, upper);
        }
        public string GetRandomCharacterName(CharacterRace race)
        {
            string nameReturned = "";
            if (race == CharacterRace.Demon)
                nameReturned = demonNames[RandomGenerator.NumberBetween(0, demonNames.Length - 1)];

            if (race == CharacterRace.Elf)
                nameReturned = elfNames[RandomGenerator.NumberBetween(0, elfNames.Length - 1)];

            if (race == CharacterRace.Ent)
                nameReturned = entNames[RandomGenerator.NumberBetween(0, entNames.Length - 1)];

            if (race == CharacterRace.Gnoll)
                nameReturned = gnollNames[RandomGenerator.NumberBetween(0, gnollNames.Length - 1)];

            if (race == CharacterRace.Goblin)
                nameReturned = goblinNames[RandomGenerator.NumberBetween(0, goblinNames.Length - 1)];

            if (race == CharacterRace.Human)
                nameReturned = humanNames[RandomGenerator.NumberBetween(0, humanNames.Length - 1)];

            if (race == CharacterRace.Orc)
                nameReturned = orcNames[RandomGenerator.NumberBetween(0, orcNames.Length - 1)];

            if (race == CharacterRace.Satyr)
                nameReturned = satyrNames[RandomGenerator.NumberBetween(0, satyrNames.Length - 1)];

            if (race == CharacterRace.Undead)
                nameReturned = undeadNames[RandomGenerator.NumberBetween(0, undeadNames.Length - 1)];


            return nameReturned;
        }
        private CharacterModelTemplateSO GetRandomModelTemplate(CharacterRace race)
        {
            List<CharacterModelTemplateSO> validTemplates = new List<CharacterModelTemplateSO>();

            foreach (CharacterModelTemplateSO mt in allModelTemplateSOs)
            {
                if (mt.race == race)
                    validTemplates.Add(mt);
            }

            return validTemplates[RandomGenerator.NumberBetween(0, validTemplates.Count - 1)];

        }
        private CharacterRace GetRandomRace(List<CharacterRace> validRaces)
        {
            return validRaces[RandomGenerator.NumberBetween(0, validRaces.Count - 1)];
        }
        private List<HexCharacterData> GenerateCharacterDeck()
        {
            Debug.Log("CharacterDataController.GenerateCharacterDeck() called...");
            List<HexCharacterData> newCharacterDeck = new List<HexCharacterData>();

            foreach (ClassTemplateSO ct in allClassTemplateSOs)
            {
                //int roll = RandomGenerator.NumberBetween(1, 100);
                //int tier = 1;
                //if (roll >= 91) tier = 3;
                //else if (roll >= 71) tier = 2;
                newCharacterDeck.Add(GenerateRecruitCharacter(ct, GetRandomRace(ct.possibleRaces)));
            }

            return newCharacterDeck;
        }
        public void AutoGenerateAndCacheNewCharacterDeck()
        {
            Debug.Log("AutoGenerateAndCacheNewCharacterDeck() called, generating new character deck");
            CharacterDeck.Clear();
            CharacterDeck = GenerateCharacterDeck();
            CharacterDeck.Shuffle();
        }
        private void GetAndApplyRandomQuirksToCharacter(HexCharacterData character, int tier = 1)
        {
            for(int i = 0; i < 2; i++)
            {
                int goodRange = 40;
                if (tier == 2) goodRange = 50;
                else if (tier == 3) goodRange = 60;
                int roll = RandomGenerator.NumberBetween(1, 2);
                if(roll == 1)
                {
                    int typeRoll = RandomGenerator.NumberBetween(1, 100);
                    // Good perk
                    if (typeRoll < goodRange) GetAndApplyRandomQuirkToCharacter(character, PerkController.Instance.PositiveQuirks);

                    // Bad perk
                    else if (typeRoll < 80) GetAndApplyRandomQuirkToCharacter(character, PerkController.Instance.NegativeQuirks);

                    // Neutral perk
                    else GetAndApplyRandomQuirkToCharacter(character, PerkController.Instance.NeutralQuirks);
                }          
            }
        }
        private PerkIconData GetAndApplyRandomQuirkToCharacter(HexCharacterData character, PerkIconData[] possibleQuirks)
        {
            int loops = 0;
            PerkIconData perk = null;
            while (perk == null && loops < 1000)
            {
                // Choose a random perk
                PerkIconData possiblePerk = possibleQuirks[RandomGenerator.NumberBetween(0, possibleQuirks.Length - 1)];

                // Check character does not already have the perk
                if (!PerkController.Instance.DoesCharacterHavePerk(character.passiveManager, possiblePerk.perkTag))
                {
                    // Check character does not have a perk that is incompatible with the prospect perk
                    bool pass = true;
                    foreach (Perk p in possiblePerk.perksThatBlockThis)
                    {
                        if (PerkController.Instance.DoesCharacterHavePerk(character.passiveManager, p))
                        {
                            pass = false;
                            break;
                        }
                    }

                    if (pass)
                    {
                        PerkController.Instance.ModifyPerkOnCharacterData(character.passiveManager, possiblePerk.perkTag, 1);
                        perk = possiblePerk;
                        break;
                    }
                }
            }

            return perk;
        }
      
        #endregion

    }
}