using System.Collections.Generic;
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
        [SerializeField] private HexCharacterTemplateSO[] allCharacterTemplatesSOs;
        [SerializeField] private TalentDataSO[] allTalentData;
        [SerializeField] private RaceDataSO[] allRacialData;

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

        // Non-Inspector
        private HexCharacterData[] allCharacterTemplates;
        private List<HexCharacterData> allPlayerCharacters = new List<HexCharacterData>();
        private List<HexCharacterData> characterDeck = new List<HexCharacterData>();
        private List<CharacterRace> validCharacterRaces = new List<CharacterRace>
        { CharacterRace.Elf, CharacterRace.Gnoll, CharacterRace.Goblin, CharacterRace.Human,
        CharacterRace.Orc, CharacterRace.Satyr, CharacterRace.Undead};
        public static readonly Vector2[] FormationPositions =
        {
            new Vector2(3,1), new Vector2(2,1), new Vector2(4,1), new Vector2(1,1), new Vector2(5,1),
            new Vector2(3,2), new Vector2(2,2), new Vector2(4,2), new Vector2(1,2), new Vector2(5,2),

        };     

        #endregion

        // Getters + Accessors
        #region
        public TalentDataSO[] AllTalentData
        {
            get { return allTalentData; }
        }
        public HexCharacterData[] AllCharacterTemplates
        {
            get { return allCharacterTemplates; }
            private set { allCharacterTemplates = value; }
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

            return ret;
        }
        #endregion

        // Initialization + Setup
        #region
        protected override void Awake()
        {
            base.Awake();
            BuildTemplateLibrary();
        }
        private void BuildTemplateLibrary()
        {
            Debug.Log("CharacterDataController.BuildTemplateLibrary() called...");

            List<HexCharacterData> tempList = new List<HexCharacterData>();

            foreach (HexCharacterTemplateSO dataSO in allCharacterTemplatesSOs)
            {
                tempList.Add(ConvertCharacterTemplateToCharacterData(dataSO));
            }

            AllCharacterTemplates = tempList.ToArray();
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
            newCharacter.abilityBook = AbilityController.Instance.ConvertSerializedAbilityBookToUnserialized(template.abilityBook);

            // Talent Data
            foreach(TalentPairing tp in template.talentPairings)
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
            newCharacter.abilityBook = AbilityController.Instance.ConvertSerializedAbilityBookToUnserialized(template.abilityBook);

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
            newCharacter.modelSize = original.modelSize;
            newCharacter.xpReward = original.xpReward;

            // Set Xp + Level
            newCharacter.currentLevel = original.currentLevel;
            newCharacter.currentMaxXP = original.currentMaxXP;
            newCharacter.currentXP = original.currentXP;
            newCharacter.dailyWage = original.dailyWage;
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
            newCharacter.abilityBook = new AbilityBook();
            newCharacter.abilityBook.allKnownAbilities.AddRange(original.abilityBook.allKnownAbilities);

            // Attribute rolls        
            newCharacter.attributeRolls = new List<AttributeRollResult>();
            foreach (AttributeRollResult arr in original.attributeRolls)            
                newCharacter.attributeRolls.Add(arr);            

            // Talent Data
            foreach (TalentPairing tp in original.talentPairings)            
                newCharacter.talentPairings.Add(new TalentPairing(tp.talentSchool, tp.level));            

            return newCharacter;

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
            if(character.formationPosition == Vector2.zero)
            {
                PlaceCharacterOnNextAvailableSlot(character);
            }
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
            TalentSchool.Naturalism, TalentSchool.Pyromania, TalentSchool.Ranger, TalentSchool.Scoundrel, TalentSchool.Shadowcraft, TalentSchool.Warfare };

            foreach(TalentSchool ts in talentSchools)
            {
                if (!DoesCharacterHaveTalent(character.talentPairings, ts, 1))
                    ret.Add(new TalentPairing(ts, 1));
                else if (!DoesCharacterHaveTalent(character.talentPairings, ts, 2))
                    ret.Add(new TalentPairing(ts, 2));
            }

            List<TalentPairing> invalidPairings = new List<TalentPairing>();

            // filter out invalid perks
            foreach (TalentPairing p in ret)
            {
                // check if perk was previously offered in another roll
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
            character.currentMaxXP = 100;
            SetCharacterLevel(character, 1);
        }
        private int GetMaxXpCapForLevel(int level)
        {
            // each level requires 50 more XP than the previous.
            return 100 + (50 * (level - 1));
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
                    xpGainMod -= 0.15f;                
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
                data.perkRolls.Add(PerkRollResult.GenerateRoll(data));

                // Gain talent level up
                if (data.currentLevel == 3 || data.currentLevel == 5)
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
                data.perkRolls.Add(PerkRollResult.GenerateRoll(data));

                // Gain talent level up
                if (data.currentLevel == 3 || data.currentLevel == 5)
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

        // Formation Logic
        #region
        public bool IsFormationPositionAvailable(Vector2 position)
        {
            bool bRet = true;

            foreach (HexCharacterData character in AllPlayerCharacters)
            {
                if (position == character.formationPosition)
                {
                    bRet = false;
                    break;
                }
            }

            return bRet;
        }
        private void PlaceCharacterOnNextAvailableSlot(HexCharacterData character)
        {
            Vector2 position = Vector2.zero;
            foreach(Vector2 spot in FormationPositions)
            {
                if (IsFormationPositionAvailable(spot))
                {
                    position = spot;
                    break;
                }
            }

            if(position == Vector2.zero)
            {
                Debug.LogWarning("PlaceCharacterOnNextAvailableSlot() could not find an available slot...");
            }

            character.formationPosition = position;
        }
        #endregion

        // Character Generation + Character Deck Logic
        #region
        public List<HexCharacterData> GenerateCompanionCharacters(HexCharacterData startingCharacter)
        {
            List<HexCharacterData> charactersRet = new List<HexCharacterData>();
            List<ClassTemplateSO> validTemplates = new List<ClassTemplateSO>();
            List<TalentSchool> bannedTalents = new List<TalentSchool>();

            // determine banned talents
            foreach (TalentPairing tp in startingCharacter.talentPairings)
                bannedTalents.Add(tp.talentSchool);

            // get valid characters
            foreach (ClassTemplateSO t in allClassTemplateSOs)
            {
                bool passedTalentCheck = true;
                foreach (TalentPairing tp in t.talentPairings)
                {
                    if (bannedTalents.Contains(tp.talentSchool))
                    {
                        passedTalentCheck = false;
                        break;
                    }
                }

                if (passedTalentCheck) validTemplates.Add(t);
            }

            Debug.Log("Valid companion templates = " + validTemplates.Count.ToString());
            validTemplates.Shuffle();
            for (int i = 0; i < validTemplates.Count; i++)
            {
                charactersRet.Add(GenerateRecruitCharacter(validTemplates[i], GetRandomRace(validTemplates[i].possibleRaces)));
            }

            return charactersRet;
        }
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

            // Set up perks
            newCharacter.passiveManager = new PerkManagerModel(newCharacter);
            PerkIconData p = PerkController.Instance.GetRacialPerk(race);
            PerkController.Instance.ModifyPerkOnCharacterData(newCharacter.passiveManager, p.perkTag, 1);
            ApplyBackgroundPerksToCharacter(newCharacter, tier);

            // Setup stats + stars
            newCharacter.attributeSheet = new AttributeSheet();
            GenerateRecruitCharacterStatRolls(newCharacter.attributeSheet, RandomGenerator.NumberBetween(2,4), tier);
            int maxStars = 2;
            int minStars = 1;
            if (tier > 1) maxStars = 3;
            if (tier == 3) minStars = 2;
            GenerateCharacterStarRolls(newCharacter.attributeSheet, 3, minStars, maxStars);

            // Randomize cost + daily wage
            newCharacter.dailyWage = GenerateCharacterDailyWage(tier);
            newCharacter.recruitCost = GenerateCharacterRecruitCost(tier);

            // Set up health
            SetCharacterMaxHealth(newCharacter, 100);
            SetCharacterHealth(newCharacter, StatCalculator.GetTotalMaxHealth(newCharacter));

            // Randomize model appearance + outfit
            newCharacter.modelParts = new List<string>();
            OutfitTemplateSO randomOutfit = null;
            if (ct.possibleOutfits.Count == 1) randomOutfit = ct.possibleOutfits[0];
            else randomOutfit = ct.possibleOutfits[RandomGenerator.NumberBetween(0, ct.possibleOutfits.Count - 1)];
            CharacterModelTemplateSO randomRaceModel = GetRandomModelTemplate(race);
            newCharacter.modelParts.AddRange(randomOutfit.outfitParts);
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
                AbilityController.Instance.HandleCharacterDataLearnNewAbility
                    (newCharacter, newCharacter.abilityBook, 
                    AbilityController.Instance.BuildAbilityDataFromScriptableObjectData(a));
            }

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
                CoreAttribute.Constituition, 
                CoreAttribute.Dodge, 
                CoreAttribute.Intelligence, 
                CoreAttribute.Resolve, 
                CoreAttribute.Strength, 
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
                else if (attributes[i] == CoreAttribute.Constituition) sheet.constitution.value += statMod + baseStatBoost;
                else if (attributes[i] == CoreAttribute.Dodge) sheet.dodge.value += statMod + baseStatBoost;
                else if (attributes[i] == CoreAttribute.Intelligence) sheet.intelligence.value += statMod + baseStatBoost;
                else if (attributes[i] == CoreAttribute.Resolve) sheet.resolve.value += statMod + baseStatBoost;
                else if (attributes[i] == CoreAttribute.Strength) sheet.strength.value += statMod + baseStatBoost;
                else if (attributes[i] == CoreAttribute.Wits) sheet.wits.value += statMod + baseStatBoost;
            }


        }
        private void GenerateCharacterStarRolls(AttributeSheet sheet, int statsStarred = 3, int minStarsGainedPerStat = 1, int maxStarsGainedPerStat = 2)
        {
            List<CoreAttribute> attributes = new List<CoreAttribute>
            {
                CoreAttribute.Accuracy,
                CoreAttribute.Constituition,
                CoreAttribute.Dodge,
                CoreAttribute.Intelligence,
                CoreAttribute.Resolve,
                CoreAttribute.Strength,
                CoreAttribute.Wits
            };

            attributes.Shuffle();

            for(int i = 0; i < statsStarred; i++)
            {
                int starsGained = RandomGenerator.NumberBetween(minStarsGainedPerStat, maxStarsGainedPerStat);

                if (attributes[i] == CoreAttribute.Accuracy) sheet.accuracy.stars += starsGained;
                else if (attributes[i] == CoreAttribute.Constituition) sheet.constitution.stars += starsGained;
                else if (attributes[i] == CoreAttribute.Dodge) sheet.dodge.stars += starsGained;
                else if (attributes[i] == CoreAttribute.Intelligence) sheet.intelligence.stars += starsGained;
                else if (attributes[i] == CoreAttribute.Resolve) sheet.resolve.stars += starsGained;
                else if (attributes[i] == CoreAttribute.Strength) sheet.strength.stars += starsGained;
                else if (attributes[i] == CoreAttribute.Wits) sheet.wits.stars += starsGained;
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
                int roll = RandomGenerator.NumberBetween(1, 100);
                int tier = 1;
                if (roll >= 91) tier = 3;
                else if (roll >= 71) tier = 2;
                newCharacterDeck.Add(GenerateRecruitCharacter(ct, GetRandomRace(ct.possibleRaces),tier));
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
        private void ApplyBackgroundPerksToCharacter(HexCharacterData character, int tier = 1)
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
                    if (typeRoll < goodRange) GetValidBackroundPerkForCharacter(character, PerkController.Instance.PositiveBackgroundPerks);

                    // Bad perk
                    else if (typeRoll < 80) GetValidBackroundPerkForCharacter(character, PerkController.Instance.NegativeBackgroundPerks);

                    // Neutral perk
                    else GetValidBackroundPerkForCharacter(character, PerkController.Instance.NeutralBackgroundPerks);
                }          
            }
        }
        private PerkIconData GetValidBackroundPerkForCharacter(HexCharacterData character, PerkIconData[] possiblePerks)
        {
            int loops = 0;
            PerkIconData perk = null;
            while (perk == null && loops < 1000)
            {
                // Choose a random perk
                PerkIconData possiblePerk = possiblePerks[RandomGenerator.NumberBetween(0, possiblePerks.Length - 1)];

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