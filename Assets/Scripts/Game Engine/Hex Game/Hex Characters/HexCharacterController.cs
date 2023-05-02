using DG.Tweening;
using HexGameEngine.Abilities;
using HexGameEngine.Combat;
using HexGameEngine.HexTiles;
using HexGameEngine.Perks;
using HexGameEngine.TurnLogic;
using HexGameEngine.UCM;
using Spriter2UnityDX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexGameEngine.Utilities;
using HexGameEngine.VisualEvents;
using HexGameEngine.Audio;
using CardGameEngine.UCM;
using HexGameEngine.AI;
using System.Linq;
using System;
using HexGameEngine.Pathfinding;
using HexGameEngine.UI;
using HexGameEngine.Items;
using HexGameEngine.Libraries;
using System.Threading.Tasks;
using Sirenix.Utilities;

namespace HexGameEngine.Characters
{
    public class HexCharacterController : Singleton<HexCharacterController>
    {
        // Properties
        #region
        [Header("Active Characters")]
        private List<HexCharacterModel> allCharacters = new List<HexCharacterModel>();
        private List<HexCharacterModel> allDefenders = new List<HexCharacterModel>();
        private List<HexCharacterModel> graveyard = new List<HexCharacterModel>();
        private List<HexCharacterModel> allEnemies = new List<HexCharacterModel>();
        private List<HexCharacterModel> allSummonedDefenders = new List<HexCharacterModel>();
        #endregion

        // Getters + Accessors
        #region
        public List<HexCharacterModel> AllCharacters
        {
            get
            {
                return allCharacters;
            }
            private set
            {
                allCharacters = value;
            }
        }
        public List<HexCharacterModel> AllDefenders
        {
            get
            {
                return allDefenders;
            }
            private set
            {
                allDefenders = value;
            }
        }
        public List<HexCharacterModel> AllSummonedDefenders
        {
            get
            {
                return allSummonedDefenders;
            }
            private set
            {
                allSummonedDefenders = value;
            }
        }
        public List<HexCharacterModel> AllEnemies
        {
            get
            {
                return allEnemies;
            }
            private set
            {
                allEnemies = value;
            }
        }
        public List<HexCharacterModel> Graveyard
        {
            get
            {
                return graveyard;
            }
            private set
            {
                graveyard = value;
            }
        }
        #endregion

        // Create Characters Logic
        #region
        public void CreateAllPlayerCombatCharacters(List<HexCharacterData> characters)
        {
            foreach(HexCharacterData c in characters)
            {
                if(c.currentHealth > 0)
                {
                    LevelNode spawnPos = LevelController.Instance.GetRandomSpawnableLevelNode(LevelController.Instance.GetPlayerSpawnZone());
                    CreatePlayerHexCharacter(c, spawnPos);
                }
            }
        }
        public void CreateAllPlayerCombatCharacters(List<CharacterWithSpawnData> characters)
        {
            foreach (CharacterWithSpawnData cw in characters)
            {
                if (cw.characterData.currentHealth <= 0 == false)                
                    CreatePlayerHexCharacter(cw.characterData, LevelController.Instance.GetHexAtGridPosition(cw.spawnPosition));
            }
        }
        public void CreatePlayerHexCharacter(HexCharacterData data, LevelNode startPosition)
        {
            // Create GO + View
            HexCharacterView vm = CreateCharacterEntityView();            

            // Create data object
            HexCharacterModel model = new HexCharacterModel();

            // Connect model to view and data
            model.hexCharacterView = vm;
            vm.character = model;
            model.characterData = data;

            // Set up positioning in world
            LevelController.Instance.PlaceCharacterOnHex(model, startPosition, true);

            // Face enemies
            LevelController.Instance.SetCharacterFacing(model, Facing.Right);

            // Set type + allegiance
            model.controller = Controller.Player;
            model.allegiance = Allegiance.Player;

            // Set up view
            SetCharacterViewStartingState(model);

            // Copy data from character data into new model
            SetupCharacterFromCharacterData(model, data);

            // Add to persistency
            AddDefenderToPersistency(model);
        }
        public HexCharacterModel CreateEnemyHexCharacter(HexCharacterData data, LevelNode startPosition)
        {
            // Create GO + View
            HexCharacterView vm = CreateCharacterEntityView();

            // Create data object
            HexCharacterModel model = new HexCharacterModel();

            // Connect model to view and data
            model.hexCharacterView = vm;
            vm.character = model;
            model.characterData = data;

            // Set up positioning in world
            LevelController.Instance.PlaceCharacterOnHex(model, startPosition, true);

            // Face enemies
            LevelController.Instance.SetCharacterFacing(model, Facing.Left);

            // Set type + allegiance
            model.controller = Controller.AI;
            model.allegiance = Allegiance.Enemy;

            // set enemy view model size
            SetCharacterModelSize(vm, data.modelSize);

            // Set up view
            SetCharacterViewStartingState(model);

            // Copy data from enemy data into new model
            SetupCharacterFromEnemyData(model, data);

            // Add to persistency
            AddEnemyToPersistency(model);

            return model;
        }
        public HexCharacterModel CreateEnemyHexCharacterForPlayer(HexCharacterData data, LevelNode startPosition)
        {
            // Create GO + View
            HexCharacterView vm = CreateCharacterEntityView();

            // Create data object
            HexCharacterModel model = new HexCharacterModel();

            // Connect model to view and data
            model.hexCharacterView = vm;
            vm.character = model;
            model.characterData = data;

            // Set up positioning in world
            LevelController.Instance.PlaceCharacterOnHex(model, startPosition, true);

            // Face enemies
            LevelController.Instance.SetCharacterFacing(model, Facing.Right);

            // Set type + allegiance
            model.controller = Controller.AI;
            model.allegiance = Allegiance.Player;

            // Set up view
            SetCharacterViewStartingState(model);

            // Copy data from enemy data into new model
            SetupCharacterFromEnemyData(model, data);

            // Add to persistency
            AddDefenderToPersistency(model);

            return model;
        }
        public HexCharacterModel CreateSummonedHexCharacter(EnemyTemplateSO data, LevelNode startPosition, Allegiance allegiance)
        {
            // Create GO + View
            HexCharacterView vm = CreateCharacterEntityView();

            // Create data object
            HexCharacterModel model = new HexCharacterModel();
            HexCharacterData characterData = CharacterDataController.Instance.GenerateEnemyDataFromEnemyTemplate(data);

            // Connect model to view
            model.hexCharacterView = vm;
            vm.character = model;
            model.characterData = characterData;

            // Set up positioning in world
            LevelController.Instance.PlaceCharacterOnHex(model, startPosition, true);

            // Face character
            Facing facing = Facing.Right;
            if (allegiance == Allegiance.Enemy) facing = Facing.Left;
            LevelController.Instance.SetCharacterFacing(model, facing);

            // Set type + allegiance
            model.controller = Controller.AI;
            model.allegiance = allegiance;

            // Set up view
            SetCharacterViewStartingState(model);

            // Copy data from character data into new model
            SetupCharacterFromEnemyData(model, characterData);

            // Add to persistency
            if (allegiance == Allegiance.Player) AddSummonedDefenderToPersistency(model);
            else AddEnemyToPersistency(model);

            // Prevent character taking its turn this turn cycle
            model.wasSummonedThisTurn = true;

            return model;
        }
        private HexCharacterView CreateCharacterEntityView()
        {
            return Instantiate(PrefabHolder.Instance.CharacterEntityModel, transform.position, Quaternion.identity).GetComponent<HexCharacterView>();
        }
        private void SetCharacterModelSize(HexCharacterView view, CharacterModelSize size)
        {
            if (size == CharacterModelSize.Small)
            {
                // Resize model
                view.ucmSizingParent.transform.localScale = new Vector3(0.9f, 0.9f, 1f);

                // Re-position model
                view.ucmSizingParent.transform.localPosition = new Vector3(0f, -0.025f, 0f);
            }
            else if (size == CharacterModelSize.Normal)
            {
                // Resize model
                view.ucmSizingParent.transform.localScale = new Vector3(1, 1, 1f);

                // Re-position model
                view.ucmSizingParent.transform.localPosition = new Vector3(0f, 0f, 0f);
            }
            else if (size == CharacterModelSize.Large)
            {
                // Resize model
                view.ucmSizingParent.transform.localScale = new Vector3(1.2f, 1.2f, 1f);

                // Re-position model
                view.ucmSizingParent.transform.localPosition = new Vector3(0.05f, 0.05f, 0f);
            }
            else if (size == CharacterModelSize.Massive)
            {
                // Resize model
                view.ucmSizingParent.transform.localScale = new Vector3(1.4f, 1.4f, 1f);

                // Re-position model
                view.ucmSizingParent.transform.localPosition = new Vector3(0.1f, 0.1f, 0f);
            }
        }
        private void SetCharacterViewStartingState(HexCharacterModel character)
        {
            // TO DO: Uncomment when we want stres bars back
            /*
            HexCharacterView view = character.hexCharacterView;
            if (!character.characterData.ignoreStress) view.stressBarWorld.transform.parent.gameObject.SetActive(true);
            else view.stressBarWorld.transform.parent.gameObject.SetActive(false);
            */
        }
        private void SetupCharacterFromCharacterData(HexCharacterModel character, HexCharacterData data)
        {
            // Set general info
            character.characterData = data;
            character.myName = data.myName;
            character.race = data.race;
            character.background = data.background;
            //character.audioProfile = data.audioProfile;

            // Setup stats
            character.attributeSheet = new AttributeSheet();
            data.attributeSheet.CopyValuesIntoOther(character.attributeSheet);      

            // Set up passive traits
            PerkController.Instance.BuildPlayerCharacterEntityPassivesFromCharacterData(character, data);

            // Set up health, armour, energy + stress with views
            ModifyBaseMaxActionPoints(character, 0); // modify by 0 just to update views
            ModifyMaxHealth(character, 0); // modify by 0 just to update views
            ModifyHealth(character, data.currentHealth, false);
            ModifyStress(character, data.currentStress, false, false, false);
            int armour = ItemController.Instance.GetTotalArmourBonusFromItemSet(data.itemSet);
            character.startingArmour = armour;
            ModifyArmour(character, armour);

            // Misc UI setup
            //character.hexCharacterView.characterNameTextUI.text = character.myName;

            // Set up items
            ItemController.Instance.RunItemSetupOnHexCharacterFromItemSet(character, data.itemSet);

            // Build UCMs
            CharacterModeller.BuildModelFromStringReferences(character.hexCharacterView.ucm, data.modelParts);
            CharacterModeller.ApplyItemSetToCharacterModelView(character.itemSet, character.hexCharacterView.ucm);
            SetCharacterModelSize(character.hexCharacterView, data.modelSize);

            // Build activation window
            TurnController.Instance.CreateActivationWindow(character);           

            // Setup abilities
            AbilityController.Instance.BuildHexCharacterAbilityBookFromData(character, data.abilityBook);

            // Talents
            character.talentPairings = data.talentPairings;
        }
        private void SetupCharacterFromEnemyData(HexCharacterModel character, HexCharacterData data)
        {
            // Set general info
            character.myName = data.myName;
            character.race = data.race;
            character.characterData = data;
            //character.audioProfile = data.audioProfile;

            // Setup stats
            character.attributeSheet = new AttributeSheet();
            data.attributeSheet.CopyValuesIntoOther(character.attributeSheet);

            // Set up passive traits
            PerkController.Instance.BuildPlayerCharacterEntityPassivesFromCharacterData(character, data);

            // Set up health, armour and energy views
            ModifyBaseMaxActionPoints(character, 0);
            ModifyMaxHealth(character, 0);
            ModifyHealth(character, StatCalculator.GetTotalMaxHealth(character));
            ModifyStress(character, data.currentStress, false, false, false);

            // AI Logic
            character.aiTurnRoutine = data.aiTurnRoutine;
            if (character.aiTurnRoutine == null)
                Debug.LogWarning("Routine is null...");

            // Build UCM
            CharacterModeller.BuildModelFromStringReferences(character.hexCharacterView.ucm, data.modelParts);            

            SetCharacterModelSize(character.hexCharacterView, data.modelSize);

            // Build activation window
            TurnController.Instance.CreateActivationWindow(character);

            // Set up items + armour and stats from items
            ItemController.Instance.RunItemSetupOnHexCharacterFromItemSet(character, data.itemSet);
            int armour = data.baseArmour + ItemController.Instance.GetTotalArmourBonusFromItemSet(data.itemSet);
            character.startingArmour = armour;
            ModifyArmour(character, armour);

            // to do: change this => not every enemy will want to have its look reflect its gear => sometimes we want to override this
            CharacterModeller.ApplyItemSetToCharacterModelView(character.itemSet, character.hexCharacterView.ucm);

            // Setup abilities
            AbilityController.Instance.BuildHexCharacterAbilityBookFromData(character, data.abilityBook);
        }
        public void SpawnEnemyEncounter(EnemyEncounterData encounterData)
        {
            List<LevelNode> backupSpawnLocations = LevelController.Instance.GetEnemySpawnZone();
          
            // Create all enemies in wave
            foreach (CharacterWithSpawnData dataSet in encounterData.enemiesInEncounter)
            {
                LevelNode spawnNode = LevelController.Instance.GetHexAtGridPosition(dataSet.spawnPosition);
                if (Pathfinder.IsHexSpawnable(spawnNode) == false) 
                    spawnNode = LevelController.Instance.GetRandomSpawnableLevelNode(backupSpawnLocations, false);
                CreateEnemyHexCharacter(dataSet.characterData, spawnNode);
            }

        }      
        public void SpawnEnemyEncounterAsPlayerTeam(EnemyEncounterData encounterData)
        {
            List<LevelNode> backupSpawnLocations = LevelController.Instance.GetPlayerSpawnZone();

            // Create all enemies in wave
            foreach (CharacterWithSpawnData dataSet in encounterData.enemiesInEncounter)
            {
                LevelNode spawnNode = LevelController.Instance.GetHexAtGridPosition(dataSet.spawnPosition);
                if (Pathfinder.IsHexSpawnable(spawnNode) == false)
                    spawnNode = LevelController.Instance.GetRandomSpawnableLevelNode(backupSpawnLocations, false);
                CreateEnemyHexCharacterForPlayer(dataSet.characterData, spawnNode);
            }
        }
        

        #endregion

        // Modify Health
        #region
        public void ModifyHealth(HexCharacterModel character, int healthGainedOrLost, bool relayToCharacterData = true)
        {
            Debug.Log("CharacterEntityController.ModifyHealth() called for " + character.myName);

            int finalHealthValue = character.currentHealth;
            finalHealthValue += healthGainedOrLost;

            // prevent health increasing over maximum
            if (finalHealthValue > StatCalculator.GetTotalMaxHealth(character))            
                finalHealthValue = StatCalculator.GetTotalMaxHealth(character);            

            // prevent health going less then 0
            if (finalHealthValue < 0)            
                finalHealthValue = 0;            

            // Set health after calculation
            character.currentHealth = finalHealthValue;

            if (healthGainedOrLost < 0) 
            {
                character.healthLostThisTurn += healthGainedOrLost;
                character.healthLostThisCombat += healthGainedOrLost;
            }

            // Relay changes to character data
            if (character.characterData != null && relayToCharacterData)            
                CharacterDataController.Instance.SetCharacterHealth(character.characterData, character.currentHealth);            

            Debug.Log(character.myName + " health value = " + character.currentHealth.ToString());
            VisualEventManager.Instance.CreateVisualEvent(() => UpdateHealthGUIElements(character, finalHealthValue, StatCalculator.GetTotalMaxHealth(character)), QueuePosition.Back, 0, 0, character.GetLastStackEventParent());
        
            // Check 'Second Wind'
            if(StatCalculator.GetCurrentHealthAsPercentageOfMaxHealth(character) < 50 &&
                character.currentHealth > 0 &&
                PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.SecondWind) &&
                character.hasTriggeredSecondWind == false)
            {
                Vector3 pos = character.hexCharacterView.WorldPosition;
                character.hasTriggeredSecondWind = true;
                VisualEventManager.Instance.CreateVisualEvent(() =>
                {
                    VisualEffectManager.Instance.CreateStatusEffect(pos, "Second Wind!");
                    VisualEffectManager.Instance.CreateGeneralBuffEffect(pos);
                }, QueuePosition.Back, 0.5f, 0.5f, character.GetLastStackEventParent());
                
                // Gain 2 guard
                PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Guard, 2, true, 0.5f);

                // Recover all fatigue
                ModifyCurrentFatigue(character, -character.currentFatigue);
            }
        }
        public void ModifyMaxHealth(HexCharacterModel character, int maxHealthGainedOrLost)
        {
            Debug.Log("CharacterEntityController.ModifyMaxHealth() called for " + character.myName);

            character.attributeSheet.maxHealth += maxHealthGainedOrLost;

            // relay changes to character data
            if (character.characterData != null)
            {
                CharacterDataController.Instance.SetCharacterMaxHealth(character.characterData, character.attributeSheet.maxHealth);
            }

            int currentHealth = character.currentHealth;
            VisualEventManager.Instance.CreateVisualEvent(() => UpdateHealthGUIElements
                (character, currentHealth, StatCalculator.GetTotalMaxHealth(character)), QueuePosition.Back, 0, 0);

            // Update health if it now excedes max health
            if (character.currentHealth > StatCalculator.GetTotalMaxHealth(character))
            {
                ModifyHealth(character, StatCalculator.GetTotalMaxHealth(character) - character.currentHealth);
            }
        }
        private void UpdateHealthGUIElements(HexCharacterModel character, int health, int maxHealth)
        {
            Debug.Log("CharacterEntityController.UpdateHealthGUIElements() called, health = " + health.ToString() + ", maxHealth = " + maxHealth.ToString());

            if (character.hexCharacterView == null) return;

            // Convert health int values to floats
            float currentHealthFloat = health;
            float currentMaxHealthFloat = maxHealth;
            float healthBarFloat = currentHealthFloat / currentMaxHealthFloat;

            // Modify WORLD space ui
            character.hexCharacterView.healthBarWorldUnder.DOKill();
            character.hexCharacterView.healthBarWorldUnder.DOValue(healthBarFloat, 0.75f).SetEase(Ease.InQuart);
            character.hexCharacterView.healthBarWorld.value = healthBarFloat;
            character.hexCharacterView.healthTextWorld.text = health.ToString();
            //character.hexCharacterView.maxHealthTextWorld.text = maxHealth.ToString();

            // Modify Screen UI elements
            if (TurnController.Instance.EntityActivated == character && character.controller == Controller.Player)
                CombatUIController.Instance.UpdateHealthComponents(health, maxHealth);

        }
        #endregion

        // Modify Armour
        #region
        public void ModifyArmour(HexCharacterModel character, int armourGainedOrLost)
        {
            Debug.Log("CharacterEntityController.ModifyArmour() called for " + character.myName);

            int finalBlockValue = character.currentArmour;
            int maxArmour = character.startingArmour;
            finalBlockValue += armourGainedOrLost;

            // Prevent armour going less then 0
            if (finalBlockValue < 0)
            {
                finalBlockValue = 0;
            }

            // Set health after calculation
            character.currentArmour = finalBlockValue;

            Debug.Log(character.myName + " armour value = " + character.currentArmour.ToString());

            VisualEventManager.Instance.CreateVisualEvent(() => UpdateArmourGUIElements(character, finalBlockValue, maxArmour), QueuePosition.Back, 0, 0, character.GetLastStackEventParent());
        }
        private void UpdateArmourGUIElements(HexCharacterModel character, int armour, int maxArmour)
        {
            Debug.Log("CharacterEntityController.UpdateArmourGUIElements() called, armour = " + armour.ToString() + ", maxArmour = " + maxArmour.ToString());

            if (character.hexCharacterView == null) return;

            // Convert health int values to floats
            float currentArmourFloat = armour;
            float currentMaxArmourFloat = maxArmour;
            float armourBarFloat = currentArmourFloat / currentMaxArmourFloat;

            // Modify WORLD space ui
            character.hexCharacterView.armourBarWorldUnder.DOKill();
            character.hexCharacterView.armourBarWorldUnder.DOValue(armourBarFloat, 0.75f).SetEase(Ease.InQuart);
            character.hexCharacterView.armourBarWorld.value = armourBarFloat;
            character.hexCharacterView.armourTextWorld.text = armour.ToString();

            // Modify Screen UI elements
            if (TurnController.Instance.EntityActivated == character && character.controller == Controller.Player)
                CombatUIController.Instance.UpdateArmourComponents((int)currentArmourFloat, (int) currentMaxArmourFloat);

        }
        #endregion

        // Modify Stress
        #region
        public void ModifyStress(HexCharacterModel character, int stressGainedOrLost, bool relayToCharacterData = true, bool showVFX = false, bool allowRecursiveChecks = true)
        {
            Debug.Log("CharacterEntityController.ModifyStress() called for " + character.myName);

            HexCharacterView view = character.hexCharacterView;           

            // Check + Update views first
            if (character.characterData.ignoreStress)
            {
                if (view != null)
                {
                    view.stressStateIconWorld.gameObject.SetActive(false);
                    //view.stressBarVisualParent.SetActive(false);
                    //view.perkIconsPanel.SetPosition(false);
                }
                return;
            }
            else if (!character.characterData.ignoreStress && view != null)
            {
                view.stressStateIconWorld.gameObject.SetActive(true);
                //view.stressBarVisualParent.SetActive(true);
                //view.perkIconsPanel.SetPosition(true);
            }

            if (character.currentStress >= 100 && stressGainedOrLost > 0) return;
            // Zealots can never reach shattered stress state
            if (CharacterDataController.Instance.DoesCharacterHaveBackground(character.background, CharacterBackground.Zealot) &&
                character.currentStress >= 99 && stressGainedOrLost > 0) return;

            // Check courage token
            if (stressGainedOrLost > 0 &&
                PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Courage))
            {
                PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Courage, -1);
                return;
            }

            int originalStress = character.currentStress;
            int finalStress = character.currentStress;
            StressState previousStressState = CombatController.Instance.GetStressStateFromStressAmount(character.currentStress);

            // Unshakeable perk
            if (stressGainedOrLost > 1 && PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Unshakeable))
                stressGainedOrLost = stressGainedOrLost / 2;

           finalStress += stressGainedOrLost;            

            // prevent stress increasing over maximum
            if (finalStress > 100)            
                finalStress = 100;            

            // prevent stress going less then 0
            if (finalStress < 0)            
                finalStress = 0;

            // Determine new stress state
            StressState newStressState = CombatController.Instance.GetStressStateFromStressAmount(finalStress);
            
            // Set stress after calculation
            character.stressGainedThisCombat += finalStress - originalStress;
            character.currentStress = finalStress;

            // relay changes to character data
            if (character.characterData != null && relayToCharacterData)            
                CharacterDataController.Instance.SetCharacterStress(character.characterData, finalStress);            

            // Stress VFX
            /* Un comment to reenable stress gained VFX
            if (stressGainedOrLost > 0 && showVFX)
            {
                if (character.GetLastStackEventParent() != null &&
                    !character.GetLastStackEventParent().isClosed)
                {
                    VisualEventManager.Instance.CreateVisualEvent(() =>
                        VisualEffectManager.Instance.CreateStressGainedEffect(view.WorldPosition, stressGainedOrLost), QueuePosition.Back, 0, 0f, character.GetLastStackEventParent());
                }
                else
                {
                    VisualEventManager.Instance.CreateVisualEvent(() =>
                    VisualEffectManager.Instance.CreateStressGainedEffect(view.WorldPosition, stressGainedOrLost), QueuePosition.Back, 0, 0);
                }

            }*/

            // Update stress related GUI
            VisualEventManager.Instance.CreateVisualEvent(() => UpdateStressGUIElements(character, finalStress), QueuePosition.Back, 0, 0);

            // Check if there was a change of stress state, up or down
            if(previousStressState != newStressState)
            {
                // Check if shattered
                if(newStressState == StressState.Shattered && showVFX)
                {
                    // Notification event
                    VisualEventManager.Instance.CreateVisualEvent(() =>
                    {
                        VisualEffectManager.Instance.CreateStatusEffect(view.WorldPosition, "SHATTERED!");
                        view.vfxManager.PlayShattered();
                    }, QueuePosition.Back, 0.5f, 0.5f);

                }
            }

            // Ally stress check event: becoming shattered
            if(allowRecursiveChecks && previousStressState != newStressState && newStressState == StressState.Shattered)
            {
                foreach(HexCharacterModel ally in GetAllAlliesOfCharacter(character, false))
                {
                    CombatController.Instance.CreateStressCheck(ally, StressEventType.AllyShattered, false);
                }
            }

            // Ally stress check event: stress state worsened
            else if (allowRecursiveChecks && (int) previousStressState < (int) newStressState)
            {
                foreach (HexCharacterModel ally in GetAllAlliesOfCharacter(character, false))
                {
                    CombatController.Instance.CreateStressCheck(ally, StressEventType.AllyMoraleStateWorsened, false);
                }
            }

            // Ally stress check event: stress state improved
            else if (allowRecursiveChecks && (int)previousStressState > (int)newStressState)
            {
                foreach (HexCharacterModel ally in GetAllAlliesOfCharacter(character, false))
                {
                    CombatController.Instance.CreateStressCheck(ally, StressEventType.AllyMoraleStateImproved, false);
                }
            }
        }
        private void UpdateStressGUIElements(HexCharacterModel character, int stress)
        {
            Debug.Log("CharacterEntityController.UpdateStressGUIElements() called, stress = " + stress.ToString());

            if (character.hexCharacterView == null) return;

            // Convert health int values to floats
            float currentStressFloat = stress;
            float currentMaxStressFloat = 20f;
            float stressBarFloat = currentStressFloat / currentMaxStressFloat;

            // Modify WORLD space ui
            character.hexCharacterView.stressBarWorld.value = stressBarFloat;
            character.hexCharacterView.stressTextWorld.text = stress.ToString();

            // Update stres state image
            StressState stressState = CombatController.Instance.GetStressStateFromStressAmount((int)stressBarFloat);
            Sprite stressSprite = SpriteLibrary.Instance.GetStressStateSprite(stressState);
            character.hexCharacterView.stressStateIconWorld.sprite = stressSprite;

            // Modify UI elements
            if (TurnController.Instance.EntityActivated == character && !character.characterData.ignoreStress)            
                CombatUIController.Instance.UpdateStressComponents(stress, character);            

        }

        #endregion

        // Modify Character Lists
        #region
        public void AddDefenderToPersistency(HexCharacterModel character)
        {
            Debug.Log("CharacterEntityController.AddDefenderPersistency() called, adding: " + character.myName);
            AllCharacters.Add(character);
            AllDefenders.Add(character);
        }
        public void AddSummonedDefenderToPersistency(HexCharacterModel character)
        {
            Debug.Log("CharacterEntityController.AddDefenderPersistency() called, adding: " + character.myName);
            AllCharacters.Add(character);
            AllSummonedDefenders.Add(character);
        }      
        public void AddEnemyToPersistency(HexCharacterModel character)
        {
            Debug.Log("CharacterEntityController.AddEnemyToPersistency() called, adding: " + character.myName);
            AllCharacters.Add(character);
            AllEnemies.Add(character);
        }
        public void RemoveEnemyFromPersistency(HexCharacterModel character)
        {
            Debug.Log("CharacterEntityController.RemoveEnemyFromPersistency() called, removing: " + character.myName);
            AllCharacters.Remove(character);
            AllEnemies.Remove(character);
        }
        public void RemoveDefenderFromPersistency(HexCharacterModel character)
        {
            Debug.Log("CharacterEntityController.RemoveDefenderFromPersistency() called, removing: " + character.myName);
            AllCharacters.Remove(character);
            AllDefenders.Remove(character);
        }
        public void RemoveSummonedDefenderFromPersistency(HexCharacterModel character)
        {
            Debug.Log("CharacterEntityController.RemoveDefenderFromPersistency() called, removing: " + character.myName);
            AllCharacters.Remove(character);
            AllSummonedDefenders.Remove(character);
        }
        #endregion

        // Trigger Animations
        #region
        public void TriggerMeleeAttackAnimation(HexCharacterView view, Vector2 targetPos, ItemData weaponUsed, CoroutineData cData)
        {
            //AudioManager.Instance.StopSound(Sound.Character_Footsteps);
            StartCoroutine(TriggerMeleeAttackAnimationCoroutine(view, targetPos, weaponUsed, cData));
        }
        
        private IEnumerator TriggerMeleeAttackAnimationCoroutine(HexCharacterView view, Vector2 targetPos, ItemData weaponUsed, CoroutineData cData)
        {
            HexCharacterModel model = view.character;
            if (model == null)
            {
                if (cData != null) cData.MarkAsCompleted();
                yield break;
            }

            Ease moveTowardsEase = Ease.InExpo;
            Ease moveBackEase = Ease.OutSine;
            string animationString = DetermineWeaponAttackAnimationString(model, weaponUsed);
            if (animationString == AnimationEventController.MAIN_HAND_MELEE_ATTACK_OVERHEAD)
            {
                // 60 sample rate
                float offset = -0.04f;
                float frameToMilliseconds = 0.016667f;
                float pauseTimeBeforeInitialMove = 8f * frameToMilliseconds;
                float initialMoveTime = 12f * frameToMilliseconds;
                float postImpactPause = 30f * frameToMilliseconds;
                float moveBackTime = 30f * frameToMilliseconds;
                view.CurrentAnimation = animationString;

                // Start attack animation
                view.ucmAnimator.SetTrigger(animationString);
                yield return new WaitForSeconds(pauseTimeBeforeInitialMove);

                // Trigger SFX weapon swing
                if(weaponUsed != null) AudioManager.Instance.PlaySoundPooled(weaponUsed.swingSFX);

                // Move 50% of the way towards the target position
                Vector2 startPos = view.WorldPosition;
                Vector2 forwardPos = (startPos + targetPos) / 2f;
                view.ucmMovementParent.transform.DOMove(forwardPos, initialMoveTime).SetEase(moveTowardsEase);
                yield return new WaitForSeconds(initialMoveTime + offset);

                // Pause at impact point
                if (cData != null) cData.MarkAsCompleted();
                yield return new WaitForSeconds(postImpactPause);

                // Move back to start point
                view.ucmMovementParent.transform.DOMove(startPos, moveBackTime).SetEase(moveBackEase);
            }
            else if (animationString == AnimationEventController.MAIN_HAND_MELEE_ATTACK_THRUST)
            {
                // 60 sample rate
                float offset = -0.04f;
                float frameToMilliseconds = 0.016667f;
                float pauseTimeBeforeInitialMove = 12f * frameToMilliseconds;
                float initialMoveTime = 11f * frameToMilliseconds;
                float postImpactPause = 19f * frameToMilliseconds;
                float moveBackTime = 20f * frameToMilliseconds;
                view.CurrentAnimation = animationString;

                // Start attack animation
                view.ucmAnimator.SetTrigger(animationString);
                yield return new WaitForSeconds(pauseTimeBeforeInitialMove);

                // Trigger SFX weapon swing
                if (weaponUsed != null) AudioManager.Instance.PlaySoundPooled(weaponUsed.swingSFX);

                // Move 50% of the way towards the target position
                Vector2 startPos = view.WorldPosition;
                Vector2 forwardPos = (startPos + targetPos) / 2f;
                view.ucmMovementParent.transform.DOMove(forwardPos, initialMoveTime).SetEase(moveTowardsEase);
                yield return new WaitForSeconds(initialMoveTime + offset);

                // Pause at impact point
                if (cData != null) cData.MarkAsCompleted();
                yield return new WaitForSeconds(postImpactPause);

                // Move back to start point
                view.ucmMovementParent.transform.DOMove(startPos, moveBackTime).SetEase(moveBackEase);
            }
            else if (animationString == AnimationEventController.OFF_HAND_MELEE_ATTACK_OVERHEAD)
            {
                // 60 sample rate
                float offset = -0.04f;
                float frameToMilliseconds = 0.016667f;
                float pauseTimeBeforeInitialMove = 8f * frameToMilliseconds;
                float initialMoveTime = 12f * frameToMilliseconds;
                float postImpactPause = 30f * frameToMilliseconds;
                float moveBackTime = 30f * frameToMilliseconds;
                view.CurrentAnimation = animationString;

                // Start attack animation
                view.ucmAnimator.SetTrigger(animationString);
                yield return new WaitForSeconds(pauseTimeBeforeInitialMove);

                // to do: trigger SFX weapon swing
                if (weaponUsed != null) AudioManager.Instance.PlaySoundPooled(weaponUsed.swingSFX);

                // Move 50% of the way towards the target position
                Vector2 startPos = view.WorldPosition;
                Vector2 forwardPos = (startPos + targetPos) / 2f;
                view.ucmMovementParent.transform.DOMove(forwardPos, initialMoveTime).SetEase(moveTowardsEase);
                yield return new WaitForSeconds(initialMoveTime + offset);

                // Pause at impact point
                if (cData != null) cData.MarkAsCompleted();
                yield return new WaitForSeconds(postImpactPause);

                // Move back to start point
                view.ucmMovementParent.transform.DOMove(startPos, moveBackTime).SetEase(moveBackEase);
            }
            else if (animationString == AnimationEventController.OFF_HAND_MELEE_ATTACK_THRUST)
            {
                // 60 sample rate
                float offset = -0.04f;
                float frameToMilliseconds = 0.016667f;
                float pauseTimeBeforeInitialMove = 12f * frameToMilliseconds;
                float initialMoveTime = 11f * frameToMilliseconds;
                float postImpactPause = 19f * frameToMilliseconds;
                float moveBackTime = 20f * frameToMilliseconds;
                view.CurrentAnimation = animationString;

                // Start attack animation
                view.ucmAnimator.SetTrigger(animationString);
                yield return new WaitForSeconds(pauseTimeBeforeInitialMove);

                // Trigger SFX weapon swing
                if (weaponUsed != null) AudioManager.Instance.PlaySoundPooled(weaponUsed.swingSFX);

                // Move 50% of the way towards the target position
                Vector2 startPos = view.WorldPosition;
                Vector2 forwardPos = (startPos + targetPos) / 2f;
                view.ucmMovementParent.transform.DOMove(forwardPos, initialMoveTime).SetEase(moveTowardsEase);
                yield return new WaitForSeconds(initialMoveTime + offset);

                // Pause at impact point
                if (cData != null) cData.MarkAsCompleted();
                yield return new WaitForSeconds(postImpactPause);

                // Move back to start point
                view.ucmMovementParent.transform.DOMove(startPos, moveBackTime).SetEase(moveBackEase);
            }
            else if (animationString == AnimationEventController.TWO_HAND_MELEE_ATTACK_OVERHEAD_1 ||
                animationString == AnimationEventController.TWO_HAND_MELEE_ATTACK_OVERHEAD_2)
            {
                // 60 sample rate
                float offset = -0.04f;
                float frameToMilliseconds = 0.016667f;
                float pauseTimeBeforeInitialMove = 8f * frameToMilliseconds;
                float initialMoveTime = 12f * frameToMilliseconds;
                float postImpactPause = 30f * frameToMilliseconds;
                float moveBackTime = 30f * frameToMilliseconds;
                view.CurrentAnimation = animationString;

                // Start attack animation
                view.ucmAnimator.SetTrigger(animationString);
                yield return new WaitForSeconds(pauseTimeBeforeInitialMove);

                // Trigger SFX weapon swing
                if (weaponUsed != null) AudioManager.Instance.PlaySoundPooled(weaponUsed.swingSFX);

                // Move 50% of the way towards the target position
                Vector2 startPos = view.WorldPosition;
                Vector2 forwardPos = (startPos + targetPos) / 2f;
                view.ucmMovementParent.transform.DOMove(forwardPos, initialMoveTime).SetEase(moveTowardsEase);
                yield return new WaitForSeconds(initialMoveTime + offset);

                // Pause at impact point
                if (cData != null) cData.MarkAsCompleted();
                yield return new WaitForSeconds(postImpactPause);

                // Move back to start point
                view.ucmMovementParent.transform.DOMove(startPos, moveBackTime).SetEase(moveBackEase);
            }
            else if (animationString == AnimationEventController.TWO_HAND_MELEE_ATTACK_THRUST)
            {
                // 60 sample rate
                float offset = -0.04f;
                float frameToMilliseconds = 0.016667f;
                float pauseTimeBeforeInitialMove = 12f * frameToMilliseconds;
                float initialMoveTime = 11f * frameToMilliseconds;
                float postImpactPause = 19f * frameToMilliseconds;
                float moveBackTime = 20f * frameToMilliseconds;
                view.CurrentAnimation = animationString;

                // Start attack animation
                view.ucmAnimator.SetTrigger(animationString);
                yield return new WaitForSeconds(pauseTimeBeforeInitialMove);

                // Trigger SFX weapon swing
                if (weaponUsed != null) AudioManager.Instance.PlaySoundPooled(weaponUsed.swingSFX);

                // Move 50% of the way towards the target position
                Vector2 startPos = view.WorldPosition;
                Vector2 forwardPos = (startPos + targetPos) / 2f;
                view.ucmMovementParent.transform.DOMove(forwardPos, initialMoveTime).SetEase(moveTowardsEase);
                yield return new WaitForSeconds(initialMoveTime + offset);

                // Pause at impact point
                if (cData != null) cData.MarkAsCompleted();
                yield return new WaitForSeconds(postImpactPause);

                // Move back to start point
                view.ucmMovementParent.transform.DOMove(startPos, moveBackTime).SetEase(moveBackEase);
            }
            


        }
        
        public void TriggerTackleAnimation(HexCharacterView view, Vector2 targetPos, CoroutineData cData)
        {
            StartCoroutine(TriggerTackleAnimationCoroutine(view, targetPos, cData));
        }
        private IEnumerator TriggerTackleAnimationCoroutine(HexCharacterView view, Vector2 targetPos, CoroutineData cData)
        {
            HexCharacterModel model = view.character;
            if (model == null)
            {
                if (cData != null) cData.MarkAsCompleted();
                yield break;
            }

            float moveSpeedTime = 0.175f;
            view.ucmAnimator.SetTrigger(AnimationEventController.TACKLE);
            view.CurrentAnimation = AnimationEventController.TACKLE;
            Vector2 startPos = view.WorldPosition;

            // Move 66% of the way towards the target position
            Vector2 forwardPos = (startPos + targetPos) / 1.8f;

            view.ucmMovementParent.transform.DOMove(forwardPos, moveSpeedTime);
            yield return new WaitForSeconds(moveSpeedTime / 2);
            
            if (cData != null) cData.MarkAsCompleted();

            if (view.CurrentAnimation == AnimationEventController.TACKLE)
                view.ucmAnimator.SetTrigger(AnimationEventController.TACKLE_END);

            yield return new WaitForSeconds(moveSpeedTime / 2);

            // move back to start pos
            view.ucmMovementParent.transform.DOMove(startPos, moveSpeedTime);
            yield return new WaitForSeconds(moveSpeedTime * 1.5f);

        }
        public void TriggerOffhandPushAnimation(HexCharacterView view, Vector2 targetPos, CoroutineData cData)
        {
            AudioManager.Instance.StopSound(Sound.Character_Footsteps);
            StartCoroutine(TriggerOffhandPushAnimationCoroutine(view, targetPos, cData));
        }
        private IEnumerator TriggerOffhandPushAnimationCoroutine(HexCharacterView view, Vector2 targetPos, CoroutineData cData)
        {
            HexCharacterModel model = view.character;
            if (model == null)
            {
                if (cData != null) cData.MarkAsCompleted();
                yield break;
            }

            Ease moveTowardsEase = Ease.InBack;
            Ease moveBackEase = Ease.OutSine;

            // 60 sample rate
            float offset = -0.04f;
            float frameToMilliseconds = 0.016667f;
            float pauseTimeBeforeInitialMove = 6f * frameToMilliseconds;
            float initialMoveTime = 17f * frameToMilliseconds;
            float postImpactPause = 19f * frameToMilliseconds;
            float moveBackTime = 20f * frameToMilliseconds;
            view.CurrentAnimation = AnimationEventController.OFF_HAND_PUSH;

            // Start attack animation
            view.ucmAnimator.SetTrigger(AnimationEventController.OFF_HAND_PUSH);
            yield return new WaitForSeconds(pauseTimeBeforeInitialMove);

            // Trigger SFX weapon swing
            AudioManager.Instance.PlaySoundPooled(Sound.Weapon_Axe_1H_Swing);

            // Move 66% of the way towards the target position
            Vector2 startPos = view.WorldPosition;
            Vector2 forwardPos = (startPos + targetPos) / 1.8f;
            view.ucmMovementParent.transform.DOMove(forwardPos, initialMoveTime).SetEase(moveTowardsEase);
            yield return new WaitForSeconds(initialMoveTime + offset);

            // Pause at impact point
            if (cData != null) cData.MarkAsCompleted();
            yield return new WaitForSeconds(postImpactPause);

            // Move back to start point
            view.ucmMovementParent.transform.DOMove(startPos, moveBackTime).SetEase(moveBackEase);
        }
        public void TriggerOffhandThrowNetAnimation(HexCharacterView view, Vector2 targetPos, CoroutineData cData)
        {
            AudioManager.Instance.StopSound(Sound.Character_Footsteps);
            StartCoroutine(TriggerOffhandThrowNetAnimationCoroutine(view, targetPos, cData));
        }
        private IEnumerator TriggerOffhandThrowNetAnimationCoroutine(HexCharacterView view, Vector2 targetPos, CoroutineData cData)
        {
            HexCharacterModel model = view.character;
            if (model == null)
            {
                if (cData != null) cData.MarkAsCompleted();
                yield break;
            }

            // 60 sample rate
            float frameToMilliseconds = 0.016667f;
            float pauseTimeBeforeInitialMove = 6f * frameToMilliseconds;
            view.CurrentAnimation = AnimationEventController.OFF_HAND_THROW_NET;

            // Start attack animation
            view.ucmAnimator.SetTrigger(AnimationEventController.OFF_HAND_THROW_NET);
            yield return new WaitForSeconds(pauseTimeBeforeInitialMove);

            // Trigger SFX weapon swing
            AudioManager.Instance.PlaySoundPooled(Sound.Weapon_Axe_1H_Swing);
            if (cData != null) cData.MarkAsCompleted();
        }
        public void TriggerShieldBashAnimation(HexCharacterView view, Vector2 targetPos, CoroutineData cData)
        {
            AudioManager.Instance.StopSound(Sound.Character_Footsteps);
            StartCoroutine(TriggerShieldBashAnimationCoroutine(view, targetPos, cData));
        }
        private IEnumerator TriggerShieldBashAnimationCoroutine(HexCharacterView view, Vector2 targetPos, CoroutineData cData)
        {
            HexCharacterModel model = view.character;
            if (model == null)
            {
                if (cData != null) cData.MarkAsCompleted();
                yield break;
            }

            Ease moveTowardsEase = Ease.InExpo;
            Ease moveBackEase = Ease.OutSine;

            // 60 sample rate
            float offset = -0.04f;
            float frameToMilliseconds = 0.016667f;
            float pauseTimeBeforeInitialMove = 12f * frameToMilliseconds;
            float initialMoveTime = 11f * frameToMilliseconds;
            float postImpactPause = 19f * frameToMilliseconds;
            float moveBackTime = 20f * frameToMilliseconds;

            // Start attack animation
            view.CurrentAnimation = AnimationEventController.SHIELD_BASH;
            view.ucmAnimator.SetTrigger(AnimationEventController.SHIELD_BASH);
            yield return new WaitForSeconds(pauseTimeBeforeInitialMove);

            // Trigger SFX weapon swing
            AudioManager.Instance.PlaySoundPooled(Sound.Weapon_Staff_Swing);

            // Move 50% of the way towards the target position
            Vector2 startPos = view.WorldPosition;
            Vector2 forwardPos = (startPos + targetPos) / 2f;
            view.ucmMovementParent.transform.DOMove(forwardPos, initialMoveTime).SetEase(moveTowardsEase);
            yield return new WaitForSeconds(initialMoveTime + offset);

            // Pause at impact point
            if (cData != null) cData.MarkAsCompleted();
            yield return new WaitForSeconds(postImpactPause);

            // Move back to start point
            view.ucmMovementParent.transform.DOMove(startPos, moveBackTime).SetEase(moveBackEase);

        }
        public void PlayShootCrossbowAnimation(HexCharacterView view, ItemData weaponUsed, CoroutineData cData)
        {
            StartCoroutine(PlayShootCrossbowAnimationCoroutine(view, weaponUsed, cData));
        }
        private IEnumerator PlayShootCrossbowAnimationCoroutine(HexCharacterView view, ItemData weaponUsed, CoroutineData cData)
        {
            float frameToMilliseconds = 0.016667f;
            float shootFrame = 35f * frameToMilliseconds;
            AudioManager.Instance.StopSound(Sound.Character_Footsteps);
            view.CurrentAnimation = AnimationEventController.SHOOT_CROSSBOW;

            // Start anim
            view.ucmAnimator.SetTrigger(AnimationEventController.SHOOT_CROSSBOW);
            yield return new WaitForSeconds(shootFrame);
            AudioManager.Instance.PlaySoundPooled(weaponUsed.swingSFX);
            if (cData != null) cData.MarkAsCompleted();
        }
        public void PlayShootBowAnimation(HexCharacterView view, ItemData weaponUsed, CoroutineData cData)
        {
            StartCoroutine(PlayShootBowAnimationCoroutine(view, weaponUsed, cData));
        }
        private IEnumerator PlayShootBowAnimationCoroutine(HexCharacterView view, ItemData weaponUsed, CoroutineData cData)
        {
            float frameToMilliseconds = 0.016667f;
            float shootFrame = 35f * frameToMilliseconds;
            AudioManager.Instance.StopSound(Sound.Character_Footsteps);
            view.CurrentAnimation = AnimationEventController.SHOOT_BOW;

            // Start anim
            AudioManager.Instance.PlaySoundPooled(Sound.Character_Draw_Bow);
            view.ucmAnimator.SetTrigger(AnimationEventController.SHOOT_BOW);
            yield return new WaitForSeconds(shootFrame);
            AudioManager.Instance.PlaySoundPooled(weaponUsed.swingSFX);
            if (cData != null) cData.MarkAsCompleted();
        }
        public void TriggerKnockedBackIntoObstructionAnimation(HexCharacterView view, Vector2 targetPos, CoroutineData cData)
        {
            AudioManager.Instance.StopSound(Sound.Character_Footsteps);
            StartCoroutine(TriggerKnockedBackIntoObstructionAnimationCoroutine(view, targetPos, cData));
        }
        private IEnumerator TriggerKnockedBackIntoObstructionAnimationCoroutine(HexCharacterView view, Vector2 targetPos, CoroutineData cData)
        {
            HexCharacterModel model = view.character;
            if (model == null)
            {
                if (cData != null) cData.MarkAsCompleted();
                yield break;
            }

            float moveSpeedTime = 0.375f;
            if(view.CurrentAnimation != AnimationEventController.HURT)
            {
                view.CurrentAnimation = AnimationEventController.HURT;
                view.ucmAnimator.SetTrigger(AnimationEventController.HURT);
            }
            
            Vector2 startPos = view.WorldPosition;

            // Move 66% of the way towards the target position
            Vector2 forwardPos = (startPos + targetPos) / 1.8f;

            view.ucmMovementParent.transform.DOMove(forwardPos, moveSpeedTime).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(moveSpeedTime / 2);

            if (cData != null) cData.MarkAsCompleted();

            yield return new WaitForSeconds(moveSpeedTime / 2);

            // move back to start pos
            view.ucmMovementParent.transform.DOMove(startPos, moveSpeedTime);
            yield return new WaitForSeconds(moveSpeedTime);

        }

        public void TriggerShootMagicHandGestureAnimation(HexCharacterView view, CoroutineData cData)
        {
            StartCoroutine(TriggerShootMagicHandGestureAnimationCoroutine(view, cData));
        }
        private IEnumerator TriggerShootMagicHandGestureAnimationCoroutine(HexCharacterView view, CoroutineData cData)
        {
            if (view == null)
            {
                if (cData != null) cData.MarkAsCompleted();
                yield break;
            }
            HexCharacterModel model = view.character;
            if (model == null)
            {
                if (cData != null) cData.MarkAsCompleted();
                yield break;
            }

            float frameToMilliseconds = 0.016667f;
            float framesTillShoot = 16f * frameToMilliseconds;
            string animationString = DetermineShootMagicHandGestureAnimationString(model);
            view.CurrentAnimation = animationString;
            view.ucmAnimator.SetTrigger(animationString);
            AudioManager.Instance.StopSound(Sound.Character_Footsteps);
            yield return new WaitForSeconds(framesTillShoot);
            cData.MarkAsCompleted();

        }
        public void TriggerAoeMeleeAttackAnimation(HexCharacterView view, ItemData weaponUsed)
        {
            if (view == null) return;
            string animationString = DetermineAoeWeaponAttackAnimationString(weaponUsed);
            view.CurrentAnimation = animationString;
            view.ucmAnimator.SetTrigger(animationString);
            AudioManager.Instance.StopSound(Sound.Character_Footsteps);    
        }
        public void PlayChargeAnimation(HexCharacterView view)
        {
            if (view == null) return;
            view.CurrentAnimation = AnimationEventController.CHARGE;
            view.ucmAnimator.SetTrigger(AnimationEventController.CHARGE);            
        }
        public void PlayReloadCrossbowAnimation(HexCharacterView view)
        {
            if (view == null) return;
            view.CurrentAnimation = AnimationEventController.RELOAD_CROSSBOW;
            view.ucmAnimator.SetTrigger(AnimationEventController.RELOAD_CROSSBOW);
        }
        public void PlayChargeEndAnimation(HexCharacterView view)
        {
            if (view == null) return;
            if (view.CurrentAnimation != AnimationEventController.CHARGE) return;
            view.ucmAnimator.SetTrigger(AnimationEventController.CHARGE_END);
            view.CurrentAnimation = AnimationEventController.CHARGE_END;
            AudioManager.Instance.StopSound(Sound.Character_Footsteps);
        }
        public void PlayIdleAnimation(HexCharacterView view)
        {
            if (view == null) return;
            view.ucmAnimator.SetTrigger(AnimationEventController.IDLE);
            view.CurrentAnimation = AnimationEventController.IDLE;
            AudioManager.Instance.StopSound(Sound.Character_Footsteps);
        }
        public void PlaySkillAnimation(HexCharacterView view)
        {
            if (view == null) return;
            view.ucmAnimator.SetTrigger(AnimationEventController.GENERIC_SKILL_1);
            view.CurrentAnimation = AnimationEventController.GENERIC_SKILL_1;
            AudioManager.Instance.StopSound(Sound.Character_Footsteps);
        }
        public void PlayRaiseShieldAnimation(HexCharacterView view)
        {
            if (view == null) return;
            view.ucmAnimator.SetTrigger(AnimationEventController.RAISE_SHIELD);
            view.CurrentAnimation = AnimationEventController.RAISE_SHIELD;
            AudioManager.Instance.StopSound(Sound.Character_Footsteps);
        }
        public void PlayMoveAnimation(HexCharacterView view)
        {
            if (view == null) return;
            AudioManager.Instance.PlaySound(Sound.Character_Footsteps);
            view.ucmAnimator.SetTrigger(AnimationEventController.RUN);
            view.CurrentAnimation = AnimationEventController.RUN;
        }
        public void PlayHurtAnimation(HexCharacterView view)
        {
            if (view == null) return;
            AudioManager.Instance.StopSound(Sound.Character_Footsteps);
            view.ucmAnimator.SetTrigger(AnimationEventController.HURT);
            view.CurrentAnimation = AnimationEventController.HURT;
        }
        public void PlayDuckAnimation(HexCharacterView view)
        {
            Debug.Log("PlayDuckAnimation() called...");
            if (view == null) return;
            AudioManager.Instance.StopSound(Sound.Character_Footsteps);
            view.ucmAnimator.SetTrigger(AnimationEventController.DUCK);
            view.CurrentAnimation = AnimationEventController.DUCK;
        }
        public void PlayDeathAnimation(HexCharacterView view)
        {
            if (view == null) return;
            AudioManager.Instance.StopSound(Sound.Character_Footsteps);
            string deathAnim = "DIE_";
            deathAnim = deathAnim + RandomGenerator.NumberBetween(1, 3).ToString();
            view.ucmAnimator.SetTrigger(deathAnim);
            view.CurrentAnimation = deathAnim;
        }
        public void PlayDecapitateAnimation(HexCharacterView view)
        {
            if (view == null) return;
            AudioManager.Instance.StopSound(Sound.Character_Footsteps);
            string decapitateAnim = "DECAPITATE_";
            decapitateAnim = decapitateAnim + RandomGenerator.NumberBetween(1, 1).ToString();
            view.ucmAnimator.SetTrigger(decapitateAnim);
            view.CurrentAnimation = decapitateAnim;
        }
        public void PlayResurrectAnimation(HexCharacterView view)
        {
            if (view == null) return;
            view.ucmAnimator.SetTrigger(AnimationEventController.RESSURECT);
            view.CurrentAnimation = AnimationEventController.RESSURECT;
        }
        #endregion

        // Turn Events + Sequences
        #region
        public void ApplyCombatStartPerkEffects(HexCharacterModel character)
        {
            // Tough (gain X block)
            if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Tough))
            {
                PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Guard, 1, false);
            }

            // Motivated (gain X focus)
            if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Motivated))
            {
                int stacks = PerkController.Instance.GetStackCountOfPerkOnCharacter(character.pManager, Perk.Motivated);
                PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Focus, stacks, false);
            }

            // Ambusher
            if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Ambusher))
            {
                PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Stealth, 1, false);
            }

            // Thief background (gain 1 stealth)
            if (RandomGenerator.NumberBetween(1, 2) == 1 &&
                CharacterDataController.Instance.DoesCharacterHaveBackground(character.background, CharacterBackground.Thief))
            {
                PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Stealth, 1, false);
            }

            // Elf (gain rune)
            if (character.race == CharacterRace.Elf && character.controller == Controller.Player)
            {
                PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Rune, 1, false);
            }

            // Human (gain flight)
            if (character.race == CharacterRace.Human && character.controller == Controller.Player)
            {
                PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Combo, 1, false);
            }

            // Orc (gain courage)
            if (character.race == CharacterRace.Orc && character.controller == Controller.Player)
            {
                PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Courage, 1, false);
            }

            // Blood Thristy (gain 1 wrath)
            if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.BloodThirsty))
            {
                PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Wrath, 1, false);
            }

            // ITEMS
            ItemController.Instance.ApplyCombatStartPerkEffectsToCharacterFromItemSet(character);
        }
        public async void CharacterOnTurnStart(HexCharacterModel character)
        {
            Debug.Log("HexCharacterController.CharacterOnTurnStart() called for " + character.myName);

            // Brief delay on start
            VisualEventManager.Instance.InsertTimeDelayInQueue(0.25f);

            // Set Phase
            SetCharacterActivationPhase(character, ActivationPhase.StartPhase);
            LevelNode hex = character.currentTile;
            HexCharacterView view = character.hexCharacterView;

            if (character.hasRequestedTurnDelay) character.hasMadeDelayedTurn = true;

            // Reduce ability cooldowns
            if(!character.hasRequestedTurnDelay)
                AbilityController.Instance.ReduceCharacterAbilityCooldownsOnTurnStart(character);

            if (character.controller == Controller.Player && IsCharacterAbleToTakeActions(character))
            {
                // Build combat GUI
                VisualEventManager.Instance.CreateVisualEvent(() => CombatUIController.Instance.BuildAndShowViewsOnTurnStart(character), QueuePosition.Back);
            }              

            if (!character.hasRequestedTurnDelay)
            {
                // Gain AP + update AP views
                int apGain = StatCalculator.GetTotalActionPointRecovery(character);
                ModifyActionPoints(character, apGain, false, false);

                // Recover fatigue+ update fatigue views
                int fatigueRecovered = StatCalculator.GetTotalFatigueRecovery(character);
                ModifyCurrentFatigue(character, -fatigueRecovered, false);
            }              

            // Check effects that are triggered on the first turn only
            if (TurnController.Instance.CurrentTurn == 1 && !character.hasRequestedTurnDelay)
            {   
            }

            // Perk expiries on turn start
            if (!character.hasRequestedTurnDelay)
            {
                // Misc expiries
                character.tilesMovedThisTurn = 0;
                character.skillAbilitiesUsedThisTurn = 0;
                character.meleeAttackAbilitiesUsedThisTurn = 0;
                character.rangedAttackAbilitiesUsedThisTurn = 0;
                character.weaponAbilitiesUsedThisTurn = 0;
                character.spellAbilitiesUsedThisTurn = 0;
                character.charactersKilledThisTurn = 0;
                character.healthLostThisTurn = 0;

                // DOTS
                #region
                // Poisoned
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Poisoned) && character.currentHealth > 0)
                {
                    // Notification event
                    VisualEventManager.Instance.CreateVisualEvent(() => VisualEffectManager.Instance.CreateStatusEffect(view.WorldPosition, "Poisoned!"), QueuePosition.Back, 0, 0.5f);

                    // Calculate and deal Physical damage
                    DamageResult damageResult = CombatController.Instance.GetFinalDamageValueAfterAllCalculations(character, 5 * PerkController.Instance.GetStackCountOfPerkOnCharacter(character.pManager, Perk.Poisoned), DamageType.Physical);
                    VisualEventManager.Instance.CreateVisualEvent(() => VisualEffectManager.Instance.CreateEffectAtLocation(ParticleEffect.ApplyPoisoned, view.WorldPosition));
                    CombatController.Instance.HandleDamage(character, damageResult, DamageType.Physical, true);
                    VisualEventManager.Instance.InsertTimeDelayInQueue(0.5f);

                    // Remove 1 stack of poisoned
                    if (character.currentHealth > 0)
                        PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Poisoned, -1, false);

                }

                // Burning
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Burning) && character.currentHealth > 0)
                {
                    // Notification event
                    VisualEventManager.Instance.CreateVisualEvent(() => VisualEffectManager.Instance.CreateStatusEffect(view.WorldPosition, "Burning!"), QueuePosition.Back, 0, 0.5f);

                    // Calculate and deal Magic damage
                    DamageResult damageResult = CombatController.Instance.GetFinalDamageValueAfterAllCalculations(character, 5 * PerkController.Instance.GetStackCountOfPerkOnCharacter(character.pManager, Perk.Burning), DamageType.Magic);
                    VisualEventManager.Instance.CreateVisualEvent(() => VisualEffectManager.Instance.CreateEffectAtLocation(ParticleEffect.FireExplosion, view.WorldPosition));
                    CombatController.Instance.HandleDamage(character, damageResult, DamageType.Magic, true);
                    VisualEventManager.Instance.InsertTimeDelayInQueue(0.5f);
                    
                    // Remove 1 stack of burning
                    if (character.currentHealth > 0)
                        PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Burning, -1);
                    
                }

                // Cut Artery
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.CutArtery) && character.currentHealth > 0)
                {
                    // Notification event
                    VisualEventManager.Instance.CreateVisualEvent(() => VisualEffectManager.Instance.CreateStatusEffect(view.WorldPosition, "Cut Artery!"), QueuePosition.Back, 0, 0.5f);

                    // Calculate and deal Magic damage
                    DamageResult damageResult = CombatController.Instance.GetFinalDamageValueAfterAllCalculations(character, 2, DamageType.Physical);
                    VisualEventManager.Instance.CreateVisualEvent(() => VisualEffectManager.Instance.CreateEffectAtLocation(ParticleEffect.BloodExplosion, view.WorldPosition));
                    CombatController.Instance.HandleDamage(character, damageResult, DamageType.Physical, true);
                    VisualEventManager.Instance.InsertTimeDelayInQueue(0.5f);
                }

                // Cut Neck Vein
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.CutNeckVein) && character.currentHealth > 0)
                {
                    // Notification event
                    VisualEventManager.Instance.CreateVisualEvent(() => VisualEffectManager.Instance.CreateStatusEffect(view.WorldPosition, "Cut Neck Vein!"), QueuePosition.Back, 0, 0.5f);

                    // Calculate and deal Magic damage
                    DamageResult damageResult = CombatController.Instance.GetFinalDamageValueAfterAllCalculations(character, 4, DamageType.Physical);
                    VisualEventManager.Instance.CreateVisualEvent(() => VisualEffectManager.Instance.CreateEffectAtLocation(ParticleEffect.BloodExplosion, view.WorldPosition));
                    CombatController.Instance.HandleDamage(character, damageResult, DamageType.Physical, true);
                    VisualEventManager.Instance.InsertTimeDelayInQueue(0.5f);
                }

                // Bleeding
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Bleeding) && character.currentHealth > 0)
                {
                    // Notification event
                    VisualEventManager.Instance.CreateVisualEvent(() => VisualEffectManager.Instance.CreateStatusEffect(view.WorldPosition, "Bleeding!"), QueuePosition.Back, 0, 0.5f);

                    // Calculate and deal Magic damage
                    DamageResult damageResult = CombatController.Instance.GetFinalDamageValueAfterAllCalculations(character, 5 * PerkController.Instance.GetStackCountOfPerkOnCharacter(character.pManager, Perk.Bleeding), DamageType.Physical);
                    VisualEventManager.Instance.CreateVisualEvent(() => VisualEffectManager.Instance.CreateEffectAtLocation(ParticleEffect.BloodExplosion, view.WorldPosition));
                    CombatController.Instance.HandleDamage(character, damageResult, DamageType.Physical, true);
                    VisualEventManager.Instance.InsertTimeDelayInQueue(0.5f);

                    // Remove 1 stack of bleeding
                    if (character.currentHealth > 0)
                        PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Bleeding, -1);


                }
                #endregion

                // PASSIVE EXPIRIES
                // Taunt
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Taunt) && character.currentHealth > 0)                
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Taunt, -1, true, 0.5f);

                // Riposte
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Riposte) && character.currentHealth > 0)
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Riposte, -1, true, 0.5f);

                // Shield Wall
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.ShieldWall) && character.currentHealth > 0)
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.ShieldWall, -1, true, 0.5f);

                // Spear Wall
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.SpearWall) && character.currentHealth > 0)                
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.SpearWall, -1, true, 0.5f);              

                // Reload
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Reload) && character.currentHealth > 0)
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Reload, -1, true, 0.5f);

                // 'Boundless' perks
                // Boundless Anger
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.BoundlessAnger) && 
                    character.currentHealth > 0 &&
                    !PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Wrath))
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Wrath, 1, true, 0.5f);

                // Boundless Bravery
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.BoundlessBravery) &&
                    character.currentHealth > 0 &&
                    !PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Courage))
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Courage, 1, true, 0.5f);

                // Boundless Purity
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.BoundlessPurity) &&
                    character.currentHealth > 0 &&
                    !PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Rune))
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Rune, 1, true, 0.5f);

                // Boundless Grit
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.BoundlessGrit) &&
                    character.currentHealth > 0 &&
                    !PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Guard))
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Guard, 1, true, 0.5f);

                // On turn start perk effects from items
                ItemController.Instance.ApplyTurnStartPerkEffectsToCharacterFromItemSet(character);
            }           

            // If shattered, determine result
            if(CombatController.Instance.GetStressStateFromStressAmount(character.currentStress) == StressState.Shattered)
            {
                int roll = RandomGenerator.NumberBetween(1, 4);
                Debug.Log("HexCharacterController.OnTurnStart() resolving shattered stress state event...");
                
                // Rally
                if (roll == 1)
                {
                    Debug.Log("HexCharacterController.OnTurnStart() character rallied from shattered");

                    // Rally notification
                    VisualEventManager.Instance.CreateVisualEvent(() =>
                    {
                        VisualEffectManager.Instance.CreateStatusEffect(view.WorldPosition, "Rallied!");
                        view.vfxManager.StopShattered();
                    }, QueuePosition.Back, 0, 0.5f);

                    // Recover 2 stress
                    ModifyStress(character, -2, true, true);

                    // End turn
                    CharacterOnTurnEnd(character);
                    return;

                }

                // Heart atack => Death
                else if(roll == 2)
                {
                     Debug.Log("HexCharacterController.OnTurnStart() character had a heart attack from being shattered");

                // Rally notification
                VisualEventManager.Instance.CreateVisualEvent(() => VisualEffectManager.Instance.CreateStatusEffect(view.WorldPosition, "HEART ATTACK!"), QueuePosition.Back, 0, 0.25f);

                // Die
                CombatController.Instance.HandleDeathBlow(character, null, true);
                }

                // Pass
                else
                {
                    Debug.Log("HexCharacterController.OnTurnStart() character passing turn due to shattered stress state");

                    // Notification
                    VisualEventManager.Instance.CreateVisualEvent(() => VisualEffectManager.Instance.CreateStatusEffect(view.WorldPosition, "Frozen by Fear..."), QueuePosition.Back, 0, 0.5f);
                }
                
            }

            // Was character killed by a DoT or heart attack?
            if (character.currentHealth <= 0) return;

            // AI Start turn 
            if (character.controller == Controller.AI)
            {
                // Redetermine who has the ranged advantage
                AILogic.UpdateCurrentRangedAdvantage();

                // Do AI turn
                VisualEventManager.Instance.InsertTimeDelayInQueue(1f);
                SetCharacterActivationPhase(character, ActivationPhase.ActivationPhase);
                await AILogic.RunEnemyRoutine(character);

                if(character.livingState == LivingState.Alive &&
                    TurnController.Instance.EntityActivated == character && 
                    character.activationPhase == ActivationPhase.ActivationPhase)
                CharacterOnTurnEnd(character);
            }

            // PLAYER Start turn
            if (character.controller == Controller.Player)
            {
                // Check stunned + unable to take actions
                if (IsCharacterAbleToTakeActions(character))                
                    SetCharacterActivationPhase(character, ActivationPhase.ActivationPhase);                
                else                
                    CharacterOnTurnEnd(character);               
               
            }
        }
        public void CharacterOnTurnEnd(HexCharacterModel character, bool firstTimeDelay = false)
        {
            Debug.Log("HexCharacterController.CharacterOnTurnEnd() called for " + character.myName);

            // Set Activated Phase
            SetCharacterActivationPhase(character, ActivationPhase.EndPhase);

            // Cache refs for visual events
            LevelNode hex = character.currentTile;
            HexCharacterView view = character.hexCharacterView;

            // TO DO: start new hide UI logic here
            // Disable info windows
            if(character.controller == Controller.Player)
            {
                AbilityPopupController.Instance.HidePanel();
                AbilityController.Instance.HideHitChancePopup();
                KeyWordLayoutController.Instance.FadeOutMainView();
                MainModalController.Instance.HideModal();
                EnemyInfoModalController.Instance.HideModal();
            }

            // Stop if combat has ended
            if (CombatController.Instance.CurrentCombatState != CombatGameState.CombatActive)
            {
                Debug.Log("CharacterEntityController.CharacterOnActivationEnd() detected combat state is not active, cancelling early... ");
                return;
            }

            // Do player character exclusive logic
            if (character.controller == Controller.Player)
            {
                // Disable and hide player combat UI
                CombatUIController.Instance.SetInteractability(false);
                CoroutineData fadeOutEvent = new CoroutineData();
                VisualEventManager.Instance.CreateVisualEvent(() => CombatUIController.Instance.HideViewsOnTurnEnd(fadeOutEvent), fadeOutEvent);
            }

            if (!character.hasRequestedTurnDelay)
            {
                // DEBUFF EXPIRIES
                #region

                // Smashed Shield
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.SmashedShield) && character.currentHealth > 0)
                {
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.SmashedShield, -1, true, 0.5f);

                }
                // Rooted
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Rooted) && character.currentHealth > 0)
                {
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Rooted, -1, true, 0.5f);
                }

                // Recently Stunned
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.RecentlyStunned) && character.currentHealth > 0)
                {
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.RecentlyStunned, -1, true, 0.5f);
                }

                // Stunned
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Stunned) && character.currentHealth > 0)
                {
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Stunned, -1, true, 0.5f);
                }

                // Overcharged
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Overcharged) && character.currentHealth > 0)
                {
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Overcharged, -1, false);
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Stunned, 1, true, 0.5f);
                }

                #endregion

                // BUFF EXPIRIES
                #region
                // True Sight
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.TrueSight) && character.currentHealth > 0)                
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.TrueSight, -1, true, 0.5f);                

                // Fortified
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Fortified) && character.currentHealth > 0)                
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Fortified, -1, true, 0.5f);          
                             
                                
                #endregion

                // MISC
                #region
                // Abusive
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Abusive) && character.currentHealth > 0)
                {
                    VisualEventManager.Instance.CreateVisualEvent(() =>
                    VisualEffectManager.Instance.CreateStatusEffect(view.WorldPosition, "Abusive!"), QueuePosition.Back, 0f, 0.5f);

                    List<HexCharacterModel> allies = GetAlliesWithinCharacterAura(character);
                    foreach(HexCharacterModel ally in allies)                    
                        CombatController.Instance.CreateStressCheck(ally, new StressEventData(1, 1, 75), true);
                    
                    VisualEventManager.Instance.InsertTimeDelayInQueue(0.5f);
                }
                // Fearsome
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Fearsome) && character.currentHealth > 0)
                {
                    VisualEventManager.Instance.CreateVisualEvent(() =>
                    VisualEffectManager.Instance.CreateStatusEffect(view.WorldPosition, "Fearsome!"), QueuePosition.Back, 0f, 0.5f);

                    List<HexCharacterModel> enemies = GetAllEnemiesWithinMyAura(character);
                    foreach (HexCharacterModel enemy in enemies)                    
                        CombatController.Instance.CreateStressCheck(enemy, new StressEventData(1, 1, 75), true);
                    
                    VisualEventManager.Instance.InsertTimeDelayInQueue(0.5f);
                }

                // Looming Presence
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.LoomingPresence) && character.currentHealth > 0)
                {
                    VisualEventManager.Instance.CreateVisualEvent(() =>
                    VisualEffectManager.Instance.CreateStatusEffect(view.WorldPosition, "Looming Presence!"), QueuePosition.Back, 0f, 0.5f);
                    int stacks = PerkController.Instance.GetStackCountOfPerkOnCharacter(character.pManager, Perk.LoomingPresence);
                    List<HexCharacterModel> enemies = GetAllEnemiesWithinMyAura(character);
                    foreach (HexCharacterModel enemy in enemies)                    
                        PerkController.Instance.ModifyPerkOnCharacterEntity(enemy.pManager, Perk.Weakened,stacks, true, 0,character.pManager);
                    
                    VisualEventManager.Instance.InsertTimeDelayInQueue(0.5f);
                }

                // Dragon Aspect
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.DragonAspect) && character.currentHealth > 0)
                {
                    // Status Notif 
                    VisualEventManager.Instance.CreateVisualEvent(() =>
                        VisualEffectManager.Instance.CreateStatusEffect(view.WorldPosition, "Dragon Aspect!"), QueuePosition.Back, 0f, 0.5f);

                    // Fire nova VFX
                    VisualEventManager.Instance.CreateVisualEvent(() =>
                        VisualEffectManager.Instance.CreateFireNova(view.WorldPosition), QueuePosition.Back);

                    // Apply 1 burning to all enemies in aura
                    List<HexCharacterModel> enemies = GetAllEnemiesWithinMyAura(character);
                    foreach (HexCharacterModel enemy in enemies)
                    {
                        // Burning VFX
                        HexCharacterView enemyView = enemy.hexCharacterView;
                        VisualEventManager.Instance.CreateVisualEvent(() =>
                            VisualEffectManager.Instance.CreateApplyBurningEffect(enemyView.WorldPosition), QueuePosition.Back);
                        PerkController.Instance.ModifyPerkOnCharacterEntity(enemy.pManager, Perk.Burning, 1, true, 0, character.pManager);

                    }

                    VisualEventManager.Instance.InsertTimeDelayInQueue(0.5f);
                }

                // Fiery Presence
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.FieryPresence) && character.currentHealth > 0)
                {
                    // Status Notif 
                    VisualEventManager.Instance.CreateVisualEvent(() =>
                        VisualEffectManager.Instance.CreateStatusEffect(view.WorldPosition, "Flaming Aspect!"), QueuePosition.Back, 0f, 0.5f);

                    // Fire nova VFX
                    VisualEventManager.Instance.CreateVisualEvent(() =>
                        VisualEffectManager.Instance.CreateFireNova(view.WorldPosition), QueuePosition.Back);

                    // Apply 1 burning to all enemies in aura
                    List<HexCharacterModel> enemies = GetAllEnemiesWithinMyAura(character);
                    foreach (HexCharacterModel enemy in enemies)
                    {
                        // Burning VFX
                        HexCharacterView enemyView = enemy.hexCharacterView;
                        VisualEventManager.Instance.CreateVisualEvent(() =>
                            VisualEffectManager.Instance.CreateApplyBurningEffect(enemyView.WorldPosition), QueuePosition.Back);
                        PerkController.Instance.ModifyPerkOnCharacterEntity(enemy.pManager, Perk.Burning, 1, true, 0, character.pManager);

                    }

                    VisualEventManager.Instance.InsertTimeDelayInQueue(0.5f);
                }

                // Contagious
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Contagious) && character.currentHealth > 0)
                {
                    // Status Notif 
                    VisualEventManager.Instance.CreateVisualEvent(() =>
                        VisualEffectManager.Instance.CreateStatusEffect(view.WorldPosition, "Contagious!"), QueuePosition.Back, 0f, 0.5f);

                    // Fire nova VFX
                    VisualEventManager.Instance.CreateVisualEvent(() =>
                        VisualEffectManager.Instance.CreatePoisonNova(view.WorldPosition), QueuePosition.Back);

                    // Apply 1 poisoned to all enemies in aura
                    List<HexCharacterModel> enemies = GetAllEnemiesWithinMyAura(character);
                    foreach (HexCharacterModel enemy in enemies)
                    {
                        // Poisoned VFX
                        HexCharacterView enemyView = enemy.hexCharacterView;
                        VisualEventManager.Instance.CreateVisualEvent(() =>
                            VisualEffectManager.Instance.CreateApplyPoisonedEffect(enemyView.WorldPosition), QueuePosition.Back);
                        PerkController.Instance.ModifyPerkOnCharacterEntity(enemy.pManager, Perk.Poisoned, 1, true, 0, character.pManager);

                    }

                    VisualEventManager.Instance.InsertTimeDelayInQueue(0.5f);
                }

                // Savage Leader
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.SavageLeader) && character.currentHealth > 0)
                {
                    List<HexCharacterModel> allies = GetAlliesWithinCharacterAura(character);
                    HexCharacterModel ally = allies[RandomGenerator.NumberBetween(0, allies.Count - 1)];

                    if (ally != null)
                    {
                        VisualEventManager.Instance.CreateVisualEvent(() =>
                        VisualEffectManager.Instance.CreateStatusEffect(view.WorldPosition, "Savage Leader!"), QueuePosition.Back, 0f, 0.5f);
                        PerkController.Instance.ModifyPerkOnCharacterEntity(ally.pManager, Perk.Wrath, 1, true, 0, character.pManager);
                        VisualEventManager.Instance.InsertTimeDelayInQueue(0.5f);
                    }                  
                }

                // Encouraging Leader
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.EncouragingLeader) && character.currentHealth > 0)
                {
                    List<HexCharacterModel> allies = GetAlliesWithinCharacterAura(character);
                    HexCharacterModel ally = allies[RandomGenerator.NumberBetween(0, allies.Count - 1)];

                    if (ally != null)
                    {
                        VisualEventManager.Instance.CreateVisualEvent(() =>
                        VisualEffectManager.Instance.CreateStatusEffect(view.WorldPosition, "Encouraging Leader!"), QueuePosition.Back, 0f, 0.5f);
                        PerkController.Instance.ModifyPerkOnCharacterEntity(ally.pManager, Perk.Wrath, 1, true, 0, character.pManager);
                        VisualEventManager.Instance.InsertTimeDelayInQueue(0.5f);
                    }
                }

                // Hymn of Fellowship
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.HymnOfFellowship) && character.currentHealth > 0)
                {
                    VisualEventManager.Instance.CreateVisualEvent(() =>
                       VisualEffectManager.Instance.CreateStatusEffect(view.WorldPosition, "Hymn of Fellowship!"), QueuePosition.Back, 0f, 0.5f);

                    List<HexCharacterModel> allies = GetAlliesWithinCharacterAura(character);
                    foreach(HexCharacterModel ally in allies)
                    {
                        Vector3 pos = ally.hexCharacterView.WorldPosition;
                        PerkController.Instance.ModifyPerkOnCharacterEntity(ally.pManager, Perk.Guard, 1, true, 0, character.pManager);
                        VisualEventManager.Instance.CreateVisualEvent(() =>
                            VisualEffectManager.Instance.CreateGeneralBuffEffect(pos));
                    }               
                                            
                    VisualEventManager.Instance.InsertTimeDelayInQueue(0.5f);
                }

                // Hymn of Wrath
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.HymnOfVengeance) && character.currentHealth > 0)
                {
                    VisualEventManager.Instance.CreateVisualEvent(() =>
                       VisualEffectManager.Instance.CreateStatusEffect(view.WorldPosition, "Hymn of Wrath!"), QueuePosition.Back, 0f, 0.5f);

                    List<HexCharacterModel> allies = GetAlliesWithinCharacterAura(character);
                    foreach (HexCharacterModel ally in allies)
                    {
                        Vector3 pos = ally.hexCharacterView.WorldPosition;
                        PerkController.Instance.ModifyPerkOnCharacterEntity(ally.pManager, Perk.Wrath, 1, true, 0, character.pManager);
                        VisualEventManager.Instance.CreateVisualEvent(() =>
                            VisualEffectManager.Instance.CreateGeneralBuffEffect(pos));
                    }

                    VisualEventManager.Instance.InsertTimeDelayInQueue(0.5f);
                }

                // Hymn of Courage
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.HymnOfCourage) && character.currentHealth > 0)
                {
                    VisualEventManager.Instance.CreateVisualEvent(() =>
                       VisualEffectManager.Instance.CreateStatusEffect(view.WorldPosition, "Hymn of Courage!"), QueuePosition.Back, 0f, 0.5f);

                    List<HexCharacterModel> allies = GetAlliesWithinCharacterAura(character);
                    foreach (HexCharacterModel ally in allies)
                    {
                        Vector3 pos = ally.hexCharacterView.WorldPosition;
                        PerkController.Instance.ModifyPerkOnCharacterEntity(ally.pManager, Perk.Courage, 1, true, 0, character.pManager);
                        VisualEventManager.Instance.CreateVisualEvent(() =>
                            VisualEffectManager.Instance.CreateGeneralBuffEffect(pos));
                    }

                    VisualEventManager.Instance.InsertTimeDelayInQueue(0.5f);
                }

                // Hymn of Purity
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.HymnOfPurity) && character.currentHealth > 0)
                {
                    VisualEventManager.Instance.CreateVisualEvent(() =>
                       VisualEffectManager.Instance.CreateStatusEffect(view.WorldPosition, "Hymn of Purity!"), QueuePosition.Back, 0f, 0.5f);

                    List<HexCharacterModel> allies = GetAlliesWithinCharacterAura(character);
                    foreach (HexCharacterModel ally in allies)
                    {
                        Vector3 pos = ally.hexCharacterView.WorldPosition;

                        // Remove poisoned
                        PerkController.Instance.ModifyPerkOnCharacterEntity
                            (ally.pManager, Perk.Poisoned, -PerkController.Instance.GetStackCountOfPerkOnCharacter(ally.pManager, Perk.Poisoned), false, 0, character.pManager);

                        // Remove burning
                        PerkController.Instance.ModifyPerkOnCharacterEntity
                            (ally.pManager, Perk.Burning, -PerkController.Instance.GetStackCountOfPerkOnCharacter(ally.pManager, Perk.Burning), false, 0, character.pManager);

                        // Remove bleeding
                        PerkController.Instance.ModifyPerkOnCharacterEntity
                            (ally.pManager, Perk.Bleeding, -PerkController.Instance.GetStackCountOfPerkOnCharacter(ally.pManager, Perk.Bleeding), false, 0, character.pManager);

                        VisualEventManager.Instance.CreateVisualEvent(() =>
                            VisualEffectManager.Instance.CreateGeneralBuffEffect(pos));
                    }

                    VisualEventManager.Instance.InsertTimeDelayInQueue(0.5f);
                }

                // Regeneration
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Regeneration) && character.currentHealth > 0)
                {
                    VisualEventManager.Instance.CreateVisualEvent(() =>
                    VisualEffectManager.Instance.CreateStatusEffect(view.WorldPosition, "Regeneration!"), QueuePosition.Back, 0f, 0.5f);

                    int stacks = PerkController.Instance.GetStackCountOfPerkOnCharacter(character.pManager, Perk.Regeneration);
                    VisualEventManager.Instance.CreateVisualEvent(() =>
                        VisualEffectManager.Instance.CreateHealEffect(view.WorldPosition));
                    ModifyHealth(character, stacks);
                    VisualEventManager.Instance.InsertTimeDelayInQueue(0.5f);
                }

                // Grass tile
                if(character.currentTile.tileName == "Grass")
                {
                    // Text Notif
                    VisualEventManager.Instance.CreateVisualEvent(() =>
                    VisualEffectManager.Instance.CreateStatusEffect(view.WorldPosition, "Hiding in Grass!"), QueuePosition.Back, 0f, 0.5f);

                    // Gain stealth
                    PerkController.Instance.ModifyPerkOnCharacterEntity
                        (character.pManager, Perk.Stealth, 1, true, 0, character.pManager);

                    // Poof VFX
                    VisualEventManager.Instance.CreateVisualEvent(() =>
                        VisualEffectManager.Instance.CreateExpendEffect(view.WorldPosition, 15, 0.2f));

                }

                // Rapid Cloaking
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.RapidCloaking) &&
                    !PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Stealth) &&
                    character.currentHealth > 0)
                {
                    // Text Notif
                    VisualEventManager.Instance.CreateVisualEvent(() =>
                    VisualEffectManager.Instance.CreateStatusEffect(view.WorldPosition, "Rapid Cloaking"), QueuePosition.Back, 0f, 0.5f);

                    // Gain stealth
                    PerkController.Instance.ModifyPerkOnCharacterEntity
                        (character.pManager, Perk.Stealth, 1, true, 0, character.pManager);

                    // Poof VFX
                    VisualEventManager.Instance.CreateVisualEvent(() =>
                        VisualEffectManager.Instance.CreateExpendEffect(view.WorldPosition, 15, 0.2f));

                }
                #endregion
            }

            // Do enemy character exclusive logic
            if (character.controller == Controller.AI && character.livingState == LivingState.Alive)
            {
                // Brief pause at the end of enemy action, so player can process whats happened
                VisualEventManager.Instance.InsertTimeDelayInQueue(1f);
            }

            if (firstTimeDelay) character.hasRequestedTurnDelay = true;

            // Activate next character
            if (character != null &&
               character.livingState == LivingState.Alive)
            {
                // Set Activated Phase
                SetCharacterActivationPhase(character, ActivationPhase.NotActivated);

                // Start next entity's activation, or new turn cycle
                TurnController.Instance.ActivateNextEntity();
            }
        }
        public void CharacterOnNewTurnCycleStarted(HexCharacterModel character)
        {
            Debug.Log("CharacterEntityController.CharacterOnNewTurnCycleStarted() called for " + character.myName);

            character.hasMadeTurn = false;
            character.wasSummonedThisTurn = false;
        }
        private void SetCharacterActivationPhase(HexCharacterModel character, ActivationPhase newPhase)
        {
            Debug.Log("CharacterEntityController.SetCharacterActivationPhase() called for " + character.myName + ", new phase = " + newPhase.ToString());
            character.activationPhase = newPhase;
        }
        #endregion

        // Modify Energy + Stamina + Fatigue and Related Views
        #region
        public void ModifyActionPoints(HexCharacterModel character, int energyGainedOrLost, bool showVFX = false, bool updateEnergyGuiInstantly = true)
        {
            Debug.Log("CharacterEntityController.ModifyActionPoints() called for " + character.myName);
            character.currentEnergy += energyGainedOrLost;
            HexCharacterView view = character.hexCharacterView;

            if (character.currentEnergy < 0)
            {
                character.currentEnergy = 0;
            }

            else if (character.currentEnergy > StatCalculator.GetTotalMaxActionPoints(character))
            {
                character.currentEnergy = StatCalculator.GetTotalMaxActionPoints(character);
            }

            if (showVFX && view != null)
            {

                if (energyGainedOrLost > 0)
                {
                    // Status notification
                    VisualEventManager.Instance.CreateVisualEvent(() =>
                    VisualEffectManager.Instance.CreateStatusEffect(view.WorldPosition, "Action Points +" + energyGainedOrLost.ToString()));

                    // Buff sparkle VFX
                    VisualEventManager.Instance.CreateVisualEvent(() => VisualEffectManager.Instance.CreateGeneralBuffEffect(view.WorldPosition));
                }
                else if (energyGainedOrLost < 0)
                {
                    // Status notification
                    VisualEventManager.Instance.CreateVisualEvent(() =>
                    VisualEffectManager.Instance.CreateStatusEffect(view.WorldPosition, "Action Points " + energyGainedOrLost.ToString()));

                    // Debuff sparkle VFX
                    VisualEventManager.Instance.CreateVisualEvent(() => VisualEffectManager.Instance.CreateGeneralDebuffEffect(view.WorldPosition));
                }
            }

            // Update GUI
            if (TurnController.Instance.EntityActivated == character && character.controller == Controller.Player)
            {
                // Update energy bar GUI
                int energyVFX = character.currentEnergy;
                VisualEventManager.Instance.CreateVisualEvent(() => CombatUIController.Instance.EnergyBar.UpdateIcons(energyVFX));

                // Update ability button validity overlays
                foreach (AbilityButton b in CombatUIController.Instance.AbilityButtons)                
                    b.UpdateAbilityButtonUnusableOverlay();
                
            }           

        }
        public void ModifyBaseMaxActionPoints(HexCharacterModel character, int maxEnergyGainedOrLost)
        {
            Debug.Log("CharacterEntityController.ModifyBaseMaxActionPoints() called for " + character.myName);
            character.attributeSheet.apMaximum += maxEnergyGainedOrLost;
            HexCharacterView view = character.hexCharacterView;

            if (character.attributeSheet.apMaximum < 0)            
                character.attributeSheet.apMaximum = 0;      
        }
        public void ModifyCurrentFatigue(HexCharacterModel character, int fatigueGainedOrLost, bool showVFX = false)
        {
            character.currentFatigue += fatigueGainedOrLost;
            HexCharacterView view = character.hexCharacterView;
            int maxFat = StatCalculator.GetTotalMaxFatigue(character);

            if (character.currentFatigue < 0)            
                character.currentFatigue = 0;            

            else if (character.currentFatigue > maxFat)            
                character.currentFatigue = maxFat;            

            if (showVFX && view != null)
            {
                if (fatigueGainedOrLost > 0)
                {
                    // Status notification
                    VisualEventManager.Instance.CreateVisualEvent(() =>
                    VisualEffectManager.Instance.CreateStatusEffect(view.WorldPosition, "Fatigue +" + fatigueGainedOrLost.ToString()));

                    // Buff sparkle VFX
                    VisualEventManager.Instance.CreateVisualEvent(() => VisualEffectManager.Instance.CreateGeneralBuffEffect(view.WorldPosition));
                }
                else if (fatigueGainedOrLost < 0)
                {
                    // Status notification
                    VisualEventManager.Instance.CreateVisualEvent(() =>
                    VisualEffectManager.Instance.CreateStatusEffect(view.WorldPosition, "Fatigue " + fatigueGainedOrLost.ToString()));

                    // Debuff sparkle VFX
                    VisualEventManager.Instance.CreateVisualEvent(() => VisualEffectManager.Instance.CreateGeneralDebuffEffect(view.WorldPosition));
                }
            }

            // Update GUI
            if (TurnController.Instance.EntityActivated == character && character.controller == Controller.Player)
            {
                int currentFat = character.currentFatigue;

                // Modify Screen UI elements
                VisualEventManager.Instance.CreateVisualEvent(() => CombatUIController.Instance.UpdateFatigueComponents(currentFat, maxFat));
                VisualEventManager.Instance.CreateVisualEvent(() => CombatUIController.Instance.UpdateCurrentInitiativeComponents(character));
                
                // Update ability button validity overlays
                foreach (AbilityButton b in CombatUIController.Instance.AbilityButtons)
                    b.UpdateAbilityButtonUnusableOverlay();
            }

        }
        #endregion

        // UI Visual Events
        #region

        public void FadeOutCharacterWorldCanvas(HexCharacterView view, CoroutineData cData, float fadeSpeed = 1f, float endAlpha = 0f)
        {
            if (view == null)
            {
                if (cData != null) cData.MarkAsCompleted();
                return;
            }

            
            view.worldSpaceCanvasParent.DOKill();
            view.worldSpaceCanvasParent.gameObject.SetActive(true);
            //view.worldSpaceCG.alpha = 1;

            Sequence s = DOTween.Sequence();
            s.Append(view.worldSpaceCG.DOFade(endAlpha, fadeSpeed));
            s.OnComplete(() =>
            {
                if (cData != null) cData.MarkAsCompleted();
            });
        }       
        public void FadeInCharacterWorldCanvas(HexCharacterView view, CoroutineData cData = null, float fadeSpeed = 1f)
        {
            if (view == null)
            {
                if (cData != null) cData.MarkAsCompleted();
                return;
            }

            view.worldSpaceCanvasParent.DOKill();
            view.worldSpaceCanvasParent.gameObject.SetActive(true);

            Sequence s = DOTween.Sequence();
            s.Append(view.worldSpaceCG.DOFade(1, fadeSpeed));
            s.OnComplete(() =>
            {
                if (cData != null) cData.MarkAsCompleted();
            });
        }       
        public void FadeOutCharacterModel(UniversalCharacterModel model, float speed = 1f)
        {
            EntityRenderer view = model.myEntityRenderer;
            foreach (SpriteRenderer sr in view.renderers)
            {
                if (sr.gameObject.activeSelf)
                    sr.DOFade(0, speed);
            }

            // Stop particles
            if (model.activeChestParticles != null)
            {
                ParticleSystem[] ps = model.activeChestParticles.GetComponentsInChildren<ParticleSystem>();
                foreach (ParticleSystem p in ps)
                {
                    p.Stop();
                }
            }

        }
        public void FadeInCharacterModel(UniversalCharacterModel model, float speed = 1f)
        {
            EntityRenderer view = model.myEntityRenderer;
            foreach (SpriteRenderer sr in view.renderers)
            {
                if (sr.gameObject.activeSelf)
                    sr.DOKill();
                sr.DOFade(1, speed);
            }

            // Restart particles
            if (model.activeChestParticles != null)
            {
                ParticleSystem[] ps = model.activeChestParticles.GetComponentsInChildren<ParticleSystem>();
                foreach (ParticleSystem p in ps)
                {
                    p.Clear();
                    p.Play();
                }
            }
        }
        public void FadeInCharacterShadow(HexCharacterView view, float speed, System.Action onCompleteCallBack = null)
        {
            view.ucmShadowCg.DOFade(0f, 0f);
            Sequence s = DOTween.Sequence();
            s.Append(view.ucmShadowCg.DOFade(1f, speed));

            if (onCompleteCallBack != null)
            {
                s.OnComplete(() => onCompleteCallBack.Invoke());
            }
        }
        public void FadeOutCharacterShadow(HexCharacterView view, float speed)
        {
            view.ucmShadowCg.DOKill();
            view.ucmShadowCg.DOFade(0f, speed);
        }
        public void ShowFreeStrikeIndicator(HexCharacterView view)
        {
            if (view == null) return;
            view.freeStrikeVisualParent.SetActive(true);
            view.freeStrikeSizingParent.DOKill();
            view.freeStrikeSizingParent.localScale = Vector3.one;
            view.freeStrikeSizingParent.DOScale(1.5f, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }
        public void HideAllFreeStrikeIndicators()
        {
            Debug.Log("HexCharacterController.HideAllFreeStrikeIndicators() called...");
            foreach(HexCharacterModel c in AllCharacters)
            {
                if(c.hexCharacterView != null)
                {
                    c.hexCharacterView.freeStrikeSizingParent.DOKill();
                    c.hexCharacterView.freeStrikeSizingParent.localScale = Vector3.one;
                    c.hexCharacterView.freeStrikeVisualParent.SetActive(false);
                }
            }
        }
        #endregion

        // Character Positioning + Zoning 
        #region
        public List<LevelNode> GetCharacterZoneOfControl(HexCharacterModel character)
        {
            List<LevelNode> zone = new List<LevelNode>();
            if(character.itemSet.mainHandItem != null &&
                character.itemSet.mainHandItem.IsMeleeWeapon)
                  zone = LevelController.Instance.GetAllHexsWithinRange(character.currentTile, 1);

            Debug.Log("HexCharacterController.GetCharacterZoneOfControl() found " + zone.Count.ToString() + " in " + character.myName + " zone of control");
            return zone;
        }
        public List<LevelNode> GetCharacterAura(HexCharacterModel character, bool includeSelfTile = false)
        {
            return LevelController.Instance.GetAllHexsWithinRange(character.currentTile, StatCalculator.GetTotalAuraSize(character), includeSelfTile);
        }
        public LevelNode GetCharacterBackTile(HexCharacterModel character)
        {
            LevelNode ret = null;
            List<LevelNode> mRangeTiles = LevelController.Instance.GetAllHexsWithinRange(character.currentTile, 1);
            foreach (LevelNode h in mRangeTiles)
            {
                if (character.currentFacing == Facing.Left &&
                    h.GridPosition.x > character.currentTile.GridPosition.x &&
                    h.GridPosition.y == character.currentTile.GridPosition.y)
                {
                    ret = h;
                    break;
                }
                else if (character.currentFacing == Facing.Right &&
                  h.GridPosition.x < character.currentTile.GridPosition.x &&
                    h.GridPosition.y == character.currentTile.GridPosition.y)
                {
                    ret = h;
                    break;
                }
            }
            return ret;

        }
        public List<LevelNode> GetCharacterBackArcTiles(HexCharacterModel character)
        {
            List<LevelNode> mRangeTiles = LevelController.Instance.GetAllHexsWithinRange(character.currentTile, 1);
            List<LevelNode> backArcTiles = new List<LevelNode>();

            foreach(LevelNode h in mRangeTiles)
            {
                if(character.currentFacing == Facing.Left &&
                    h.GridPosition.x > character.currentTile.GridPosition.x)
                {
                    backArcTiles.Add(h);
                }
                else if (character.currentFacing == Facing.Right &&
                  h.GridPosition.x < character.currentTile.GridPosition.x)
                {
                    backArcTiles.Add(h);
                }
            }

            return backArcTiles;
        }
        public List<HexCharacterModel> GetAllCharactersWithinMyAura(HexCharacterModel character, bool includeSelf = false)
        {
            List<LevelNode> auraTiles = GetCharacterAura(character, includeSelf);
            List<HexCharacterModel> characters = new List<HexCharacterModel>();
            foreach (LevelNode h in auraTiles)
            {
                if (h.myCharacter != null)
                    characters.Add(h.myCharacter);
            }

            return characters;
        }
        public List<HexCharacterModel> GetAlliesWithinCharacterAura(HexCharacterModel character, bool includeSelf = false)
        {
            List<LevelNode> auraTiles = GetCharacterAura(character, includeSelf);
            List<HexCharacterModel> allies = new List<HexCharacterModel>();
            foreach(LevelNode h in auraTiles)
            {
                if (h.myCharacter != null && IsTargetFriendly(h.myCharacter, character))
                    allies.Add(h.myCharacter);
            }

            return allies;
        }
        public List<HexCharacterModel> GetAllEnemiesWithinMyAura(HexCharacterModel character)
        {
            List<LevelNode> auraTiles = GetCharacterAura(character, false);
            List<HexCharacterModel> enemies = new List<HexCharacterModel>();
            foreach (LevelNode h in auraTiles)
            {
                if (h.myCharacter != null && !IsTargetFriendly(h.myCharacter, character))
                    enemies.Add(h.myCharacter);
            }

            return enemies;
        }
        public bool IsCharacterInTargetsZoneOfControl(HexCharacterModel character, HexCharacterModel target)
        {
            bool bRet = true;
            if (target.itemSet.mainHandItem == null ||
                target.itemSet.mainHandItem.IsMeleeWeapon == false)
            {
                bRet = false;
            }
            else if (GetCharacterZoneOfControl(target).Contains(character.currentTile) == false)
            {
                bRet = false;
            }

            return bRet;
        }
        public bool IsCharacterEngagedInMelee(HexCharacterModel character, int engagingEnemies = 1)
        {
            bool bRet = false;
            int engagedEnemies = 0;
            List<LevelNode> meleeTiles = LevelController.Instance.GetAllHexsWithinRange(character.currentTile, 1);

            foreach(LevelNode h in meleeTiles)
            {
                if(h.myCharacter != null &&
                    h.myCharacter.allegiance != character.allegiance &&
                    h.myCharacter.itemSet.mainHandItem != null &&
                    h.myCharacter.itemSet.mainHandItem.IsMeleeWeapon &&
                    IsCharacterAbleToTakeActions(h.myCharacter))
                {
                    engagedEnemies += 1;
                }
            }
            if (engagedEnemies == 0) bRet = false;
            if (engagedEnemies >= engagingEnemies) bRet = true;
            else bRet = false;
            return bRet;
        }
        public bool IsCharacterTeleportable(HexCharacterModel character)
        {
            if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Implaccable))
            {
                return false;
            }
            else return true;
        }
        public bool IsCharacterKnockBackable(HexCharacterModel character)
        {
            if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Implaccable))
            {
                return false;
            }
            else return true;
        }
        public bool IsCharacterFlanked(HexCharacterModel character)
        {
            int attackers = 0;
            foreach(LevelNode h in LevelController.Instance.GetAllHexsWithinRange(character.currentTile, 1))
            {
                if (h.myCharacter != null &&
                    !IsTargetFriendly(character, h.myCharacter))
                    attackers++;
            }
            if (attackers >= 2) return true;
            else return false;
        }
        public int GetTotalFlankingCharactersOnTarget(HexCharacterModel target)
        {
            int attackers = 0;
            foreach (LevelNode h in LevelController.Instance.GetAllHexsWithinRange(target.currentTile, 1))
            {
                if (h.myCharacter != null &&
                    !IsTargetFriendly(target, h.myCharacter) &&
                    IsCharacterAbleToTakeActions(h.myCharacter) &&
                    h.myCharacter.itemSet.mainHandItem != null &&
                    h.myCharacter.itemSet.mainHandItem.IsMeleeWeapon)
                    attackers++;
            }
            return attackers;
        }
        public int CalculateFlankingHitChanceModifier(HexCharacterModel attacker, HexCharacterModel target)
        {
            int bonusRet = 0;

            // Characters with footwork do not suffer the flanking penalty
            if (PerkController.Instance.DoesCharacterHavePerk(target.pManager, Perk.Footwork))
                return 0;

            // +5 Accuracy from flanking, per ally attacker
            if (IsCharacterFlanked(target))
            {
                bonusRet = 5 * (GetTotalFlankingCharactersOnTarget(target) - 1);

                // Additonal +5 Accuracy for characters with Opportunist perk
                if (PerkController.Instance.DoesCharacterHavePerk(attacker.pManager, Perk.Opportunist))
                    bonusRet += 5;

                // Prevent bonus going negative
                if (bonusRet < 0) bonusRet = 0;
            }              

            return bonusRet;
        }
        public int CalculateBackStrikeHitChanceModifier(HexCharacterModel attacker, HexCharacterModel target)
        {
            int bonusRet = 0;

            if (PerkController.Instance.DoesCharacterHavePerk(target.pManager, Perk.Footwork))
                return 0;

            if (GetCharacterBackArcTiles(target).Contains(attacker.currentTile))
            {
                // Add base back strike bonus
                bonusRet += 10;

                // Bonus is increased for characters with Opportunist perk
                if (PerkController.Instance.DoesCharacterHavePerk(attacker.pManager, Perk.Opportunist) &&
                    !IsCharacterFlanked(target)) // prevent doubling of opportunist buff if target is both flanked and backstruck
                    bonusRet += 5;
            }

            return bonusRet;
        }
        public int CalculateElevationAccuracyModifier(HexCharacterModel attacker, HexCharacterModel target)
        {
            int bonusRet = 0;

            // +10 Accuracy attacking from elevation
            if (!PerkController.Instance.DoesCharacterHavePerk(target.pManager, Perk.Footwork) &&
                attacker.currentTile.Elevation == TileElevation.Elevated &&
                target.currentTile.Elevation == TileElevation.Ground)
            {
                bonusRet += 10;
            }

            // -10 Accuracy attacking from ground to elevated target
            else if (attacker.currentTile.Elevation == TileElevation.Ground &&
                target.currentTile.Elevation == TileElevation.Elevated)
            {
                bonusRet -= 10;
            }
            

            return bonusRet;
        }
        #endregion

        // Move Character Visual Events
        #region
        public void MoveEntityToNodeCentre(HexCharacterView view, LevelNode node, CoroutineData data, Action onCompleteCallback = null, float startDelay = 0.25f)
        {
            Debug.Log("CharacterEntityController.MoveEntityToNodeCentre() called...");
            StartCoroutine(MoveEntityToNodeCentreCoroutine(view, node, data, onCompleteCallback, startDelay));
        }
        private IEnumerator MoveEntityToNodeCentreCoroutine(HexCharacterView view, LevelNode node, CoroutineData cData, Action onCompleteCallback, float startDelay)
        {
            // Brief yield here (incase melee attack anim played and character hasn't returned to attack pos )
            yield return new WaitForSeconds(startDelay);
            float distance = Vector2.Distance(view.ucmMovementParent.transform.position, view.WorldPosition);

            // Face direction of destination node
            LevelController.Instance.FaceCharacterTowardsHex(view.character, node);

            // Play movement animation
            PlayMoveAnimation(view);

            // Move
            Sequence s = DOTween.Sequence();
            s.Append(view.ucmMovementParent.transform.DOMove(view.WorldPosition, 1.5f * (distance * 0.1f)));
            s.OnComplete(() =>
            {
                // Reset facing, depending on living entity type
                if (view.character != null)
                {
                    if (view.character.allegiance == Allegiance.Player)                    
                        LevelController.Instance.SetCharacterFacing(view.character, Facing.Right);                    
                    else if (view.character.allegiance == Allegiance.Enemy)                    
                        LevelController.Instance.SetCharacterFacing(view.character, Facing.Left);                    
                }

                // Idle anim
                PlayIdleAnimation(view);

                // Resolve event
                if (cData != null) cData.MarkAsCompleted();   
                if (onCompleteCallback != null) onCompleteCallback.Invoke();             
            });
        }
        public void MoveAllCharactersToOffScreenPosition()
        {
            Debug.Log("HexCharacterController.MoveAllCharactersToOffScreenPosition() called...");
            foreach (HexCharacterModel character in AllCharacters)
            {
                MoveEntityToOffScreenPosition(character);
            }
        }
        private void MoveEntityToOffScreenPosition(HexCharacterModel entity)
        {
            if (entity.allegiance == Allegiance.Player)            
                entity.hexCharacterView.ucmMovementParent.transform.position = LevelController.Instance.DefenderOffScreenNode.transform.position;
            
            else if (entity.allegiance == Allegiance.Enemy)            
                entity.hexCharacterView.ucmMovementParent.transform.position = LevelController.Instance.EnemyOffScreenNode.transform.position;
            
        }
        public void MoveAllCharactersToStartingNodes(CoroutineData data)
        {
            Debug.Log("HexCharacterController.MoveAllCharactersToStartingNodes() called...");
            StartCoroutine(MoveAllCharactersToStartingNodesCoroutine(data));
        }
        private IEnumerator MoveAllCharactersToStartingNodesCoroutine(CoroutineData data)
        {
            List<CoroutineData> events = new List<CoroutineData>();
            foreach (HexCharacterModel character in AllCharacters)
            {
                CoroutineData cData = new CoroutineData();
                events.Add(cData);
                MoveEntityToNodeCentre(character.hexCharacterView, character.currentTile, cData, () => { events.Remove(cData); });
            }

            yield return new WaitUntil(() => events.Count == 0);

            if (data != null) data.MarkAsCompleted();
        }
        public void MoveCharactersToOffScreenRight(List<HexCharacterModel> characters, CoroutineData cData)
        {
            StartCoroutine(MoveCharactersToOffScreenRightCoroutine(characters, cData));
        }
        private IEnumerator MoveCharactersToOffScreenRightCoroutine(List<HexCharacterModel> characters, CoroutineData cData)
        {
            List<CoroutineData> events = new List<CoroutineData>();
            foreach (HexCharacterModel character in characters)
            {
                CoroutineData cd = new CoroutineData();
                events.Add(cd);
                MoveEntityToNodeCentre(character.hexCharacterView, LevelController.Instance.EnemyOffScreenNode, cd, () => { events.Remove(cd); });
            }

            yield return new WaitUntil(() => events.Count == 0);

            if (cData != null) cData.MarkAsCompleted();   
        }
        #endregion

        // Determine a Character's Allies and Enemies Logic
        #region
        public bool IsTargetFriendly(HexCharacterModel character, HexCharacterModel target)
        {
            Debug.Log("CharacterEntityController.IsTargetFriendly() called, comparing " +
                character.myName + " to " + target.myName);

            return character.allegiance == target.allegiance;
        }
        public List<HexCharacterModel> GetAllEnemiesOfCharacter(HexCharacterModel character)
        {
            Debug.Log("CharacterEntityController.GetAllEnemiesOfCharacter() called...");

            List<HexCharacterModel> listReturned = new List<HexCharacterModel>();

            foreach (HexCharacterModel entity in AllCharacters)
            {
                if (!IsTargetFriendly(character, entity))
                {
                    listReturned.Add(entity);
                }
            }

            return listReturned;
        }
        public List<HexCharacterModel> GetAllAlliesOfCharacter(HexCharacterModel character, bool includeSelfInSearch = true)
        {
            Debug.Log("CharacterEntityController.GetAllEnemiesOfCharacter() called...");

            List<HexCharacterModel> listReturned = new List<HexCharacterModel>();

            foreach (HexCharacterModel entity in AllCharacters)
            {
                if (IsTargetFriendly(character, entity))
                {
                    listReturned.Add(entity);
                }
            }

            if (includeSelfInSearch == false &&
                listReturned.Contains(character))
            {
                listReturned.Remove(character);
            }

            return listReturned;
        }
        #endregion

        // Destroy models and views logic
        #region
        public void DisconnectModelFromView(HexCharacterModel character)
        {
            Debug.Log("CharacterEntityController.DisconnectModelFromView() called for: " + character.myName);

            character.hexCharacterView.character = null;
            character.hexCharacterView = null;
        }
        public void DestroyCharacterView(HexCharacterView view, bool disconnectUCM = false)
        {
            Debug.Log("CharacterEntityController.DestroyCharacterView() called...");
            if (disconnectUCM)
            {
                view.ucm.gameObject.AddComponent<DestroyOnSceneChange>();
                view.ucm.transform.SetParent(null, true);
            }
            Destroy(view.gameObject,0.1f);
        }
        public void HandleTearDownCombatScene()
        {
            foreach(HexCharacterModel h in AllCharacters)
            {
                DestroyCharacterView(h.hexCharacterView);
            }

            AllCharacters.Clear();
            AllEnemies.Clear();
            AllSummonedDefenders.Clear();
            AllDefenders.Clear();
            Graveyard.Clear();

            var destroyables = FindObjectsOfType<DestroyOnSceneChange>();
            destroyables.ForEach(x => Destroy(x.gameObject));


        }
        #endregion

        // Conditional Checks on Characters
        #region
        public bool IsCharacterAbleToMove(HexCharacterModel c)
        {
            bool bRet = true;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Rooted) ||
                    PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Stunned) ||
                    PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.TurtleAspect))
                bRet = false;

            return bRet;
        }
        public bool IsCharacterAbleToTakeActions(HexCharacterModel c)
        {
            bool bRet = true;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Stunned) ||
                CombatController.Instance.GetStressStateFromStressAmount(c.currentStress) == StressState.Shattered)
                bRet = false;

            return bRet;
        }
        public bool IsCharacterAbleToMakeFreeStrikes(HexCharacterModel c)
        {
            if ((c.itemSet.mainHandItem == null ||
                 (c.itemSet.mainHandItem != null  && !c.itemSet.mainHandItem.IsRangedWeapon)) &&
                IsCharacterAbleToTakeActions(c))
                return true;

            else return false;
        }
        public bool IsCharacterAbleToMakeRiposteAttack(HexCharacterModel c)
        {
            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Riposte) &&
                c.itemSet.mainHandItem != null &&
                c.itemSet.mainHandItem.IsMeleeWeapon &&
                IsCharacterAbleToTakeActions(c))
                return true;

            else return false;
        }
        public bool IsCharacterAbleToMakeSpearWallAttack(HexCharacterModel c)
        {
            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.SpearWall) &&
                c.itemSet.mainHandItem != null &&
                c.itemSet.mainHandItem.IsMeleeWeapon &&
                IsCharacterAbleToTakeActions(c))
                return true;

            else return false;
        }
        public bool DoesCharacterHaveEnoughFatigue(HexCharacterModel caster, int fatigueCost)
        {
            return StatCalculator.GetTotalMaxFatigue(caster) - caster.currentFatigue >= fatigueCost;
        }
        public bool CanCharacterBeStunnedNow(HexCharacterModel c)
        {
            if (!PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Stunned) &&
                !PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.RecentlyStunned) &&
                !PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.StunImmunity) &&
                !PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Implaccable))
                return true;

            else return false;
        }
        #endregion

        // Determine 
        private string DetermineWeaponAttackAnimationString(HexCharacterModel character, ItemData weaponUsed)
        {
            string ret = AnimationEventController.MAIN_HAND_MELEE_ATTACK_OVERHEAD;

            // 1H 
            if (weaponUsed != null && weaponUsed.IsMeleeWeapon && weaponUsed.handRequirement == HandRequirement.OneHanded)
            {
                // Main hand overhead
                if(weaponUsed == character.itemSet.mainHandItem && weaponUsed.weaponAttackAnimationType == WeaponAttackAnimationType.Overhead)                
                    ret = AnimationEventController.MAIN_HAND_MELEE_ATTACK_OVERHEAD;

                // Main hand thrust
                else if (weaponUsed == character.itemSet.mainHandItem && weaponUsed.weaponAttackAnimationType == WeaponAttackAnimationType.Thrust)
                    ret = AnimationEventController.MAIN_HAND_MELEE_ATTACK_THRUST;

                // Off hand overhead
                if (weaponUsed == character.itemSet.offHandItem && weaponUsed.weaponAttackAnimationType == WeaponAttackAnimationType.Overhead)
                    ret = AnimationEventController.OFF_HAND_MELEE_ATTACK_OVERHEAD;

                // Off hand thrust
                else if (weaponUsed == character.itemSet.offHandItem && weaponUsed.weaponAttackAnimationType == WeaponAttackAnimationType.Thrust)
                    ret = AnimationEventController.OFF_HAND_MELEE_ATTACK_THRUST;
            }

            // 2h
            else if (weaponUsed != null && 
                weaponUsed.IsMeleeWeapon && 
                weaponUsed.handRequirement == HandRequirement.TwoHanded)
            {
                // Overhead
                if (weaponUsed.weaponAttackAnimationType == WeaponAttackAnimationType.Overhead)
                {
                    int random = RandomGenerator.NumberBetween(0, 2);
                    if (random == 0) ret = AnimationEventController.TWO_HAND_MELEE_ATTACK_OVERHEAD_2;
                    else ret = AnimationEventController.TWO_HAND_MELEE_ATTACK_OVERHEAD_1;
                }

                // Thrust
                else if (weaponUsed.weaponAttackAnimationType == WeaponAttackAnimationType.Thrust)
                    ret = AnimationEventController.TWO_HAND_MELEE_ATTACK_THRUST;
            }

            return ret;
        }
        public string DetermineAoeWeaponAttackAnimationString(ItemData weaponUsed)
        {
            string ret = AnimationEventController.MAIN_HAND_MELEE_ATTACK_CLEAVE;

            // 2h
            if (weaponUsed != null &&
                weaponUsed.IsMeleeWeapon &&
                weaponUsed.handRequirement == HandRequirement.TwoHanded)
            {
                ret = AnimationEventController.TWO_HAND_MELEE_ATTACK_CLEAVE;
            }

            Debug.Log("HexCharacterController.DetermineAoeWeaponAttackAnimationString() returning: " + ret);
            return ret;
        }
        public string DetermineShootMagicHandGestureAnimationString(HexCharacterModel character)
        {
            string ret = AnimationEventController.LEFT_HAND_SHOOT_MAGIC;

            // 2h
            if (character.itemSet.mainHandItem != null &&
                character.itemSet.mainHandItem.IsMeleeWeapon &&
                character.itemSet.mainHandItem.handRequirement == HandRequirement.TwoHanded)
            {
                ret = AnimationEventController.TWO_HAND_SHOOT_MAGIC;
            }

            Debug.Log("HexCharacterController.DetermineShootMagicAnimationString() returning: " + ret);
            return ret;
        }
    }
}