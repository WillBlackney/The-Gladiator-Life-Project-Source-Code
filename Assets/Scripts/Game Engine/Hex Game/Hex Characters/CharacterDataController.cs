using System.Collections.Generic;
using UnityEngine;
using WeAreGladiators.Items;
using WeAreGladiators.Abilities;
using WeAreGladiators.Perks;
using WeAreGladiators.Utilities;
using WeAreGladiators.Persistency;
using WeAreGladiators.Player;
using System.Linq;
using WeAreGladiators.Audio;
using WeAreGladiators.Boons;
using System.Globalization;

namespace WeAreGladiators.Characters
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
        [SerializeField] private EnemyTemplateSO[] allEnemyTemplateSOs; 

        [Header("Recruit Generation")]
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
        [SerializeField] private int mightLower = 5;
        [SerializeField] private int mightUpper = 10;
        [Space(10)]
        [SerializeField] private int constitutionLower = 55;
        [SerializeField] private int constitutionUpper = 65;
        [Space(10)]
        [SerializeField] private int accuracyLower = 55;
        [SerializeField] private int accuracyUpper = 65;
        [Space(10)]
        [SerializeField] private int dodgeLower= 5;
        [SerializeField] private int dodgeUpper = 10;
        [Space(10)]
        [SerializeField] private int fitnessLower = 95;
        [SerializeField] private int fitnessUpper = 105;
        [Space(10)]
        [SerializeField] private int resolveLower = 5;
        [SerializeField] private int resolveUpper = 10;
        [Space(10)]
        [SerializeField] private int witsLower = 5;
        [SerializeField] private int witsUpper = 10;

        public int MightLower => mightLower;
        public int MightUpper => mightUpper;
        public int AccuracyLower => accuracyLower;
        public int AccuracyUpper => accuracyUpper;
        public int DodgeLower => dodgeLower;
        public int DodgeUpper => dodgeUpper;
        public int FitnessLower => fitnessLower;
        public int FitnessUpper => fitnessUpper;
        public int ConstitutionLower => constitutionLower;
        public int ConstitutionUpper => constitutionUpper;
        public int ResolveLower => resolveLower;
        public int ResolveUpper => resolveUpper;
        public int WitsLower => witsLower;
        public int WitsUpper => witsUpper;


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
        public EnemyTemplateSO FindEnemyTemplateByName(string name)
        {
            EnemyTemplateSO ret = null;
            foreach(EnemyTemplateSO e in allEnemyTemplateSOs)
            {
                if(e.myName == name)
                {
                    ret = e;
                    break;
                }
            }
            return ret;
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
            newCharacter.mySubName = template.mySubName;
            newCharacter.race = template.race;
            newCharacter.audioProfile = GetAudioProfileForRace(newCharacter.race);
            newCharacter.modelSize = template.modelSize;
            SetStartingLevelAndXpValues(newCharacter);
            if (template.background == CharacterBackground.None) newCharacter.background = GetBackgroundData(CharacterBackground.Unknown);
            else newCharacter.background = GetBackgroundData(template.background);

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
        public HexCharacterData GenerateCharacterDataFromEnemyTemplate(EnemyTemplateSO template)
        {
            Debug.Log("CharacterDataController.GenerateEnemyDataFromEnemyTemplate() called...");
            HexCharacterData newCharacter = new HexCharacterData();

            // Set up background + story data
            newCharacter.myName = template.myName;
            newCharacter.race = template.race;
            newCharacter.audioProfile = template.audioProfile;
            newCharacter.modelSize = template.modelSize;
            newCharacter.xpReward = template.xpReward;
            newCharacter.baseArmour = template.baseArmour;
            newCharacter.ignoreStress = template.ignoreStress;

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
            newCharacter.behaviour = template.behaviour;

            // Item Data
            newCharacter.itemSet = new ItemSet();

            // Fixed item loadout
            if(!template.randomizeItemSet)
                ItemController.Instance.CopySerializedItemManagerIntoStandardItemManager(template.itemSet, newCharacter.itemSet);

            // Random item loadout
            else
            {
                GenerateCharacterArmour(newCharacter, template.possibleArmourLoadouts);
                GenerateCharacterWeapons(newCharacter, template.recruitWeaponLoadouts);
            }

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
            newCharacter.mySubName = original.mySubName;
            newCharacter.race = original.race;
            newCharacter.audioProfile = original.audioProfile;
            
            newCharacter.background = original.background;
            newCharacter.modelSize = original.modelSize;
            newCharacter.xpReward = original.xpReward;
            newCharacter.baseArmour = original.baseArmour;
            newCharacter.ignoreStress = original.ignoreStress;

            // Set Xp + Level
            newCharacter.currentLevel = original.currentLevel;
            newCharacter.currentMaxXP = original.currentMaxXP;
            newCharacter.currentXP = original.currentXP;
            newCharacter.dailyWage = original.dailyWage;
            newCharacter.perkPoints = original.perkPoints;
            newCharacter.PerkTree = new PerkTreeData(original);

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
            if (data.currentStress > 20)
                data.currentStress = 20;
            // Zealots can never reach shattered stress state
            if (DoesCharacterHaveBackground(data.background, CharacterBackground.Witch) &&
                data.currentStress > 19) data.currentStress = 19;
            else if (data.currentStress < 0)
                data.currentStress = 0;
        }
        public void SetCharacterHealth(HexCharacterData data, int newValue)
        {
            Debug.Log("CharacterDataController.SetCharacterHealth() called for '" +
                data.myName + "', new health value = " + newValue.ToString());

            data.currentHealth = newValue;

            // prevent current health exceeding max health
            int maxHealth = StatCalculator.GetTotalMaxHealth(data);
            if (data.currentHealth > maxHealth)
                data.currentHealth = maxHealth;
            else if (data.currentHealth < 0) data.currentHealth = 0;
        }
        public void SetCharacterMaxHealth(HexCharacterData data, int newValue)
        {
            Debug.Log("CharacterDataController.SetCharacterMaxHealth() called for '" +
                data.myName + "', new max health value = " + newValue.ToString());

            data.attributeSheet.maxHealth = newValue;
            int totalMaxHealth = StatCalculator.GetTotalMaxHealth(data);

            // prevent current health exceeding max health
            OnConstitutionOrMaxHealthChanged(data, totalMaxHealth);
        }
        public void OnConstitutionOrMaxHealthChanged(HexCharacterData character, int newTotalMaxHealth)
        {
            // prevent current health exceeding max health
            if (character.currentHealth > newTotalMaxHealth)
                character.currentHealth = newTotalMaxHealth;

        }

        #endregion

        // Build Character Roster
        #region
        public void BuildCharacterRoster(List<HexCharacterData> characters)
        {
            AllPlayerCharacters.Clear();
            foreach (HexCharacterData c in characters)
                AddCharacterToRoster(c);
        }
        public void AddCharacterToRoster(HexCharacterData character)
        {
            if(AllPlayerCharacters.Contains(character) == false) AllPlayerCharacters.Add(character);
        }
        public void RemoveCharacterFromRoster(HexCharacterData character, bool death = true)
        {
            if (AllPlayerCharacters.Contains(character)) AllPlayerCharacters.Remove(character);
            if (death) 
            { 
                // to do: add character info to orbituary, apply character death to score, etc
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
        public int GetTotalRosterDailyWage()
        {
            int ret = 0;
            foreach (HexCharacterData character in AllPlayerCharacters) ret += character.dailyWage;
            return ret;
        }
        public void HandlePassiveStressAndHealthRecoveryOnNewDayStart()
        {
            foreach(HexCharacterData c in AllPlayerCharacters)
            {
                //SetCharacterStress(c, c.currentStress - 1);
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
            /*
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
        public int GetMaxXpCapForLevel(int level)
        {
            // each level requires 50 more XP than the previous.
            if (level == 1) return 50;
            else return 50 + (75 * (level - 1));
        }
        private void SetCharacterLevel(HexCharacterData data, int newLevelValue)
        {
            data.currentLevel = newLevelValue;
        }
        public int HandleGainXP(HexCharacterData data, int xpGained, bool applyXpMods = true)
        {
            int ret = 0;
            float xpGainMod = 1f;
            if(applyXpMods) xpGainMod = StatCalculator.GetCharacterXpGainRate(data);        

            xpGained = (int)(xpGained * xpGainMod);
            ret = xpGained;

            // check spill over + level up first
            int spillOver = (data.currentXP + xpGained) - data.currentMaxXP;

            // Level up occured with spill over XP
            if (spillOver > 0)
            {
                // Gain level
                HandleLevelUp(data);

                // Restart the xp gain procces with the spill over amount
                HandleGainXP(data, spillOver, false);

            }

            // Level up with no spill over
            else if (spillOver == 0)
            {
                // Gain level
                HandleLevelUp(data);
            }

            // Gain xp without leveling up
            else
            {
                data.currentXP += xpGained;
            }

            return ret;
        }
        public void HandleLevelUp(HexCharacterData data)
        {
            if (data.currentLevel == 6) return;

            // Gain level
            SetCharacterLevel(data, data.currentLevel + 1);

            // Do attribute level up roll result logic
            data.attributeRolls.Add(AttributeRollResult.GenerateRoll(data));

            // Gain perk level up
            if (PerkController.Instance.GetAllLevelUpPerksOnCharacter(data).Count < 5)
                data.perkPoints++;

            // Gain talent level up
            if ((data.currentLevel == 3 || data.currentLevel == 5) && data.talentPairings.Count < 3)
                data.talentPoints += 1;

            // Reset current xp
            data.currentXP = 0;

            // Increase max xp on level up
            data.currentMaxXP = GetMaxXpCapForLevel(data.currentLevel);
        }
        #endregion

        // Recruit Generation + Character Deck Logic
        #region
        private List<HexCharacterData> GenerateCharacterDeck(int deckSize = 20)
        {
            List<HexCharacterData> newCharacterDeck = new List<HexCharacterData>();
            BackgroundData[] backgrounds = AllCharacterBackgrounds.ShuffledCopy();

            // TO DO IN FUTURE: if anything in the game should increase/decrease the chances of certain
            // backgrounds appearing, that logic should go here

            int currentIndex = 0;
            int totalLoops = 0;

            while(newCharacterDeck.Count < deckSize && totalLoops < 1000)
            {
                BackgroundData bg = backgrounds[currentIndex];
                if (bg.recruitable)
                {
                    // Roll for character generation, then generate on success
                    if (RandomGenerator.NumberBetween(1, 100) <= bg.spawnChance)
                        newCharacterDeck.Add(GenerateRecruitCharacter(bg));
                }

                if (currentIndex >= backgrounds.Count() - 1)
                    currentIndex = -1;

                currentIndex++;
                totalLoops++;
            }

            Debug.Log("GenerateCharacterDeck() complete, total background iterations: " + totalLoops.ToString());

            return newCharacterDeck;
        }
        public void AutoGenerateAndCacheNewCharacterDeck()
        {
            CharacterDeck.Clear();
            CharacterDeck = GenerateCharacterDeck();
            CharacterDeck.Shuffle();
        }
        public HexCharacterData GenerateRecruitCharacter(BackgroundData bgData, bool allowLevelBoosts = true)
        {
            Debug.Log("CharacterDataController.GenerateRecruitCharacter() called, generating from background: " + bgData.backgroundType.ToString());

            // Setup
            HexCharacterData newCharacter = new HexCharacterData();
            newCharacter.background = bgData;
            RecruitLoadoutData loadoutData = bgData.loadoutBuckets[RandomGenerator.NumberBetween(0, bgData.loadoutBuckets.Count - 1)];
            newCharacter.modelSize = CharacterModelSize.Normal;

            // Set up background, race and story data
            newCharacter.race = GetRandomRace(bgData.validRaces);
            newCharacter.myName = GetRandomCharacterName(newCharacter.race);
            newCharacter.dailyWage = RandomGenerator.NumberBetween(newCharacter.background.dailyWageMin, newCharacter.background.dailyWageMax);
                                               
            // Setup stats + stars
            newCharacter.attributeSheet = new AttributeSheet();
            GenerateRecruitCharacterCoreAttributeRolls(newCharacter.attributeSheet, newCharacter.background);
            GenerateCharacterStarRolls(newCharacter.attributeSheet, 3);

            // Set up perks + quirks
            newCharacter.passiveManager = new PerkManagerModel(newCharacter);
            var quirks = GetAndApplyRandomQuirksToCharacter(newCharacter);
            newCharacter.PerkTree = new PerkTreeData(newCharacter);

            // Generate Subname
            newCharacter.mySubName = GetRandomCharacterSubName(quirks, bgData, newCharacter.race);

            // Set up health
            SetCharacterMaxHealth(newCharacter, 0);
            SetCharacterHealth(newCharacter, StatCalculator.GetTotalMaxHealth(newCharacter));                       

            // Set up starting XP + level
            SetStartingLevelAndXpValues(newCharacter);
            if (allowLevelBoosts)
            {
                int startingLevelBoosts = RandomGenerator.NumberBetween(bgData.lowerLevelLimit, bgData.upperLevelLimit) - newCharacter.currentLevel;
                for (int i = 0; i < startingLevelBoosts; i++)
                    HandleLevelUp(newCharacter);
            }
           

            // Randomize character appearance 
            newCharacter.modelParts = new List<string>();
            CharacterModelTemplateSO randomRaceModel = GetRandomModelTemplate(newCharacter.race);
            newCharacter.modelParts.AddRange(randomRaceModel.bodyParts);

            // Determine starting talent
            TalentSchool startingTalent = loadoutData.possibleStartingTalents[RandomGenerator.NumberBetween(0, loadoutData.possibleStartingTalents.Length - 1)];
            newCharacter.talentPairings.Add(new TalentPairing(startingTalent, 1));

            // Determine weapons + head/body load out
            newCharacter.itemSet = new ItemSet();
            GenerateCharacterWeapons(newCharacter, loadoutData.possibleWeaponLoadouts);
            GenerateCharacterArmour(newCharacter, loadoutData.possibleArmourLoadouts);

            // Determine and learn abilities
            newCharacter.abilityBook = new AbilityBook();
            List<AbilityData> abilities = GenerateRecruitAbilities(newCharacter, loadoutData, RandomGenerator.NumberBetween(bgData.minStartingAbilities, bgData.maxStartingAbilities));
            foreach(AbilityData a in abilities)            
                newCharacter.abilityBook.HandleLearnNewAbility(a);
            
            // Learn weapon abilities
            newCharacter.abilityBook.HandleLearnAbilitiesFromItemSet(newCharacter.itemSet);

            // TO DO: generate a cool sub name (based off race, talent, quirk or background)
            // TO DO: generate character background story

            return newCharacter;
        }       
        private void GenerateRecruitCharacterCoreAttributeRolls(AttributeSheet sheet, BackgroundData background)
        {
            sheet.might.value = RandomGenerator.NumberBetween(background.mightLower + mightLower, background.mightUpper + mightUpper);
            sheet.constitution.value = RandomGenerator.NumberBetween(background.constitutionLower + constitutionLower, background.constitutionUpper + constitutionUpper);
            sheet.accuracy.value = RandomGenerator.NumberBetween(background.accuracyLower + accuracyLower, background.accuracyUpper + accuracyUpper);
            sheet.dodge.value = RandomGenerator.NumberBetween(background.dodgeLower + dodgeLower, background.dodgeUpper + dodgeUpper);
            sheet.wits.value = RandomGenerator.NumberBetween(background.witsLower + witsLower, background.witsUpper + witsUpper);
            sheet.fitness.value = RandomGenerator.NumberBetween(background.fatigueLower + fitnessLower, background.fatigueUpper + fitnessUpper);
            sheet.resolve.value = RandomGenerator.NumberBetween(background.resolveLower + resolveLower, background.resolveUpper + resolveUpper);
        }
        private void GenerateCharacterWeapons(HexCharacterData character, RecruitWeaponLoadout[] loadout)
        {
            if (loadout.Length == 0) return;
            RecruitWeaponLoadout weaponBasket = loadout[RandomGenerator.NumberBetween(0, loadout.Length - 1)];
            ItemDataSO chosenMH = null;
            ItemDataSO chosenOH = null;

            // Choose random item data  
            if (weaponBasket.mainHandProspects.Length > 0)
                chosenMH = weaponBasket.mainHandProspects[RandomGenerator.NumberBetween(0, weaponBasket.mainHandProspects.Length - 1)];

            if (weaponBasket.offhandProspects.Length > 0)
                chosenOH = weaponBasket.offhandProspects[RandomGenerator.NumberBetween(0, weaponBasket.offhandProspects.Length - 1)];

            // Create and assign items
            if (chosenMH != null)
                character.itemSet.mainHandItem = ItemController.Instance.GenerateNewItemWithRandomEffects(chosenMH);

            if (chosenOH != null && (chosenMH == null || (chosenMH != null && chosenMH.handRequirement == HandRequirement.OneHanded)))
                character.itemSet.offHandItem = ItemController.Instance.GenerateNewItemWithRandomEffects(chosenOH);
        }
        private void GenerateCharacterArmour(HexCharacterData character, RecruitArmourLoadout[] loadout)
        {
            if (loadout.Length == 0) return;
            RecruitArmourLoadout armourBasket = loadout[RandomGenerator.NumberBetween(0, loadout.Length - 1)];
            ItemDataSO chosenBody = null;
            ItemDataSO chosenHead = null;

            // Choose random item data
            if (armourBasket.bodyProspects.Length > 0)
                chosenBody = armourBasket.bodyProspects[RandomGenerator.NumberBetween(0, armourBasket.bodyProspects.Length - 1)];

            if (armourBasket.headProspects.Length > 0)            
                chosenHead = armourBasket.headProspects[RandomGenerator.NumberBetween(0, armourBasket.headProspects.Length - 1)];
                        
            // Create and assign items
            if (chosenBody != null)
                character.itemSet.bodyArmour = ItemController.Instance.GenerateNewItemWithRandomEffects(chosenBody);

            if (chosenHead != null)
                character.itemSet.headArmour = ItemController.Instance.GenerateNewItemWithRandomEffects(chosenHead);
        }
        private List<AbilityData> GenerateRecruitAbilities(HexCharacterData character,  RecruitLoadoutData loadout, int totalAbilities = 1)
        {
            List<AbilityData> ret = new List<AbilityData>();
            List<AbilityDataSO> prospects = new List<AbilityDataSO>();
            List<TalentSchool> characterTalents = new List<TalentSchool>();
            foreach (TalentPairing tp in character.talentPairings)
                characterTalents.Add(tp.talentSchool);

            foreach(AbilityDataSO a in loadout.possibleAbilities)
            {
                if(a.talentRequirementData == null ||
                  (a.talentRequirementData != null &&
                   characterTalents.Contains(a.talentRequirementData.talentSchool)))
                {
                    if(AbilityController.Instance.DoesCharacterMeetAbilityWeaponRequirement(character.itemSet, a.weaponRequirement) ||
                       (character.itemSet.mainHandItem == null && character.itemSet.offHandItem == null))                    
                        prospects.Add(a);  
                }
            }

            // Get X random abilities, convert to data file, add them to returned list
            prospects.Shuffle();
            for (int i = 0; i < totalAbilities && i < prospects.Count; i++)
                ret.Add(AbilityController.Instance.BuildAbilityDataFromScriptableObjectData(prospects[i]));


            return ret;
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
                CoreAttribute.Wits,
                CoreAttribute.Fitness
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
                else if (attributes[i] == CoreAttribute.Fitness) sheet.fitness.stars = starsGained;
            }
        }
        private string GetRandomCharacterName(CharacterRace race)
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
        private string GetRandomCharacterSubName(List<PerkIconData> quirks, BackgroundData bgData, CharacterRace race)
        {
            string ret = "";
            List<string> possibleSubNames = new List<string>();

            for (int i = 0; i < quirks.Count; i++) possibleSubNames.AddRange(quirks[i].possibleSubNames);
            possibleSubNames.AddRange(bgData.possibleSubNames);
            possibleSubNames.AddRange(GetRaceData(race).possibleSubNames);
            if(possibleSubNames.Count > 0)
            {
                possibleSubNames.Shuffle();
                ret = possibleSubNames[0];
            }

            return ret;

        }
        public CharacterModelTemplateSO GetRandomModelTemplate(CharacterRace race)
        {
            List<CharacterModelTemplateSO> validTemplates = new List<CharacterModelTemplateSO>();

            foreach (CharacterModelTemplateSO mt in allModelTemplateSOs)
            {
                if (mt.race == race)
                    validTemplates.Add(mt);
            }

            return validTemplates[RandomGenerator.NumberBetween(0, validTemplates.Count - 1)];

        }
        public CharacterRace GetRandomRace(List<CharacterRace> validRaces)
        {
            return validRaces[RandomGenerator.NumberBetween(0, validRaces.Count - 1)];
        }      
        private List<PerkIconData> GetAndApplyRandomQuirksToCharacter(HexCharacterData character)
        {
            List<PerkIconData> ret = new List<PerkIconData>();
            for (int i = 0; i < 2; i++)
            {
                int roll = RandomGenerator.NumberBetween(1, 2);
                if(i == 0 || (i != 0 && roll == 1))
                {
                    int typeRoll = RandomGenerator.NumberBetween(1, 100);

                    // Good perk
                    if (typeRoll < 45) ret.Add(GetAndApplyRandomQuirkToCharacter(character, PerkController.Instance.PositiveQuirks));

                    // Bad perk
                    else if (typeRoll < 90) ret.Add(GetAndApplyRandomQuirkToCharacter(character, PerkController.Instance.NegativeQuirks));

                    // Neutral perk
                    else ret.Add(GetAndApplyRandomQuirkToCharacter(character, PerkController.Instance.NeutralQuirks));
                }          
            }
            return ret;
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
                    // Check character does not have a perk/race that is incompatible with the prospect perk
                    bool pass = true;
                    foreach (Perk p in possiblePerk.perksThatBlockThis)
                    {
                        if (PerkController.Instance.DoesCharacterHavePerk(character.passiveManager, p))
                        {
                            pass = false;
                            break;
                        }
                    }

                    // Check race
                    if (possiblePerk.racesThatBlockThis.Contains(character.race)) pass = false;

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
        public int GetCharacterInitialHiringCost(HexCharacterData character)
        {
            int cost = character.background.baseRecruitCost;
            int levelMod = (character.currentLevel - 1) * 75;
            int itemsCost = ItemController.Instance.GetCharacterItemsGoldValue(character.itemSet);
            cost += levelMod + itemsCost;

            // Check boons
            if(character.background.backgroundType == CharacterBackground.Gladiator &&
                BoonController.Instance.DoesPlayerHaveBoon(BoonTag.UnemployedGladiators))
            {
                cost = (int) (cost * 0.5f);
            }

            else if (character.background.backgroundType == CharacterBackground.Inquisitor &&
               BoonController.Instance.DoesPlayerHaveBoon(BoonTag.UnemployedInquisitors))
            {
                cost = (int)(cost * 0.5f);
            }

            else if (character.background.backgroundType == CharacterBackground.Witch &&
              BoonController.Instance.DoesPlayerHaveBoon(BoonTag.WitchAccession))
            {
                cost = (int)(cost * 0.5f);
            }

            // round to the nearest 10 gold
            cost = Mathf.RoundToInt(cost / 10) * 10;
            return cost;
        }
        public AudioProfileType GetAudioProfileForRace(CharacterRace race)
        {
            if (race == CharacterRace.Human)
                return AudioProfileType.Human_1;

            else if (race == CharacterRace.Elf)
                return AudioProfileType.Elf_1;

            else if (race == CharacterRace.Undead ||
                race == CharacterRace.Demon ||
                race == CharacterRace.Ent)
                return AudioProfileType.Undead_1;

            else if (race == CharacterRace.Satyr)
                return AudioProfileType.Satyr_1;

            else if (race == CharacterRace.Orc)
                return AudioProfileType.Orc_1;

            else if (race == CharacterRace.Goblin)
                return AudioProfileType.Goblin_1;

            else if (race == CharacterRace.Gnoll)
                return AudioProfileType.Gnoll_1;

            else
                return AudioProfileType.None;
        }
        #endregion

    }
}