using DG.Tweening;
using HexGameEngine.Abilities;
using HexGameEngine.CameraSystems;
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
using HexGameEngine.JourneyLogic;
using HexGameEngine.Pathfinding;
using HexGameEngine.UI;

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
                    LevelNode spawnPos = null;
                    foreach(CharacterWithSpawnData cw in RunController.Instance.CurrentDeployedCharacters)
                    {
                        LevelNode n = LevelController.Instance.GetHexAtGridPosition(cw.spawnPosition);
                        if (cw != null && 
                            cw.characterData == c && 
                            n != null &&
                            Pathfinder.IsHexSpawnable(n))
                        {
                            spawnPos = n;
                            break;
                        }
                    }
                    if (spawnPos == null)
                        spawnPos = LevelController.Instance.GetRandomSpawnableLevelNode(LevelController.Instance.GetPlayerSpawnZone());
                    CreatePlayerHexCharacter(c, spawnPos);
                }
            }
        }
        public void CreatePlayerHexCharacter(HexCharacterData data, LevelNode startPosition)
        {
            // Create GO + View
            HexCharacterView vm = CreateCharacterEntityView().GetComponent<HexCharacterView>();            

            // Create data object
            HexCharacterModel model = new HexCharacterModel();

            // Connect model to view
            model.hexCharacterView = vm;
            vm.character = model;

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
            HexCharacterView vm = CreateCharacterEntityView().GetComponent<HexCharacterView>();

            // Create data object
            HexCharacterModel model = new HexCharacterModel();

            // Connect model to view
            model.hexCharacterView = vm;
            vm.character = model;

            // Set up positioning in world
            LevelController.Instance.PlaceCharacterOnHex(model, startPosition, true);

            // Face enemies
            LevelController.Instance.SetCharacterFacing(model, Facing.Left);

            // Set type + allegiance
            model.controller = Controller.AI;
            model.allegiance = Allegiance.Enemy;

            // set enemy view model size

            // Set up view
            SetCharacterViewStartingState(model);

            // Copy data from enemy data into new model
            SetupCharacterFromEnemyData(model, data);

            // Add to persistency
            AddEnemyToPersistency(model);

            return model;


        }
        private GameObject CreateCharacterEntityView()
        {
            return Instantiate(PrefabHolder.Instance.CharacterEntityModel, transform.position, Quaternion.identity);
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
            HexCharacterView view = character.hexCharacterView;

            // Get camera references
            view.uiCanvas.worldCamera = CameraController.Instance.MainCamera;

            // Disable main UI canvas + card UI stuff
            view.uiCanvasParent.SetActive(false);

            if (character.controller == Controller.Player) view.stressBarWorld.gameObject.SetActive(true);
            
        }
        private void SetupCharacterFromCharacterData(HexCharacterModel character, HexCharacterData data)
        {
            // Set general info
            character.characterData = data;
            character.myName = data.myName;
            character.race = data.race;
            //character.audioProfile = data.audioProfile;

            // Setup stats
            character.attributeSheet = new AttributeSheet();
            data.attributeSheet.CopyValuesIntoOther(character.attributeSheet);      

            // Set up passive traits
            PerkController.Instance.BuildPlayerCharacterEntityPassivesFromCharacterData(character, data);

            // Set up health + energy + stress with views
            ModifyBaseMaxEnergy(character, 0); // modify by 0 just to update views
            ModifyMaxHealth(character, 0); // modify by 0 just to update views
            ModifyHealth(character, data.currentHealth, false);
            ModifyStress(character, data.currentStress, false);

            // Misc UI setup
            character.hexCharacterView.characterNameTextUI.text = character.myName;

            // Build UCM
            CharacterModeller.BuildModelFromStringReferences(character.hexCharacterView.ucm, data.modelParts);
            CharacterModeller.BuildModelFromStringReferences(character.hexCharacterView.uiPotraitUCM, data.modelParts);
            SetCharacterModelSize(character.hexCharacterView, data.modelSize);

            // Build activation window
            TurnController.Instance.CreateActivationWindow(character);

            // Set up items
            Items.ItemController.Instance.RunItemSetupOnHexCharacterFromItemSet(character, data.itemSet);

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
            //character.audioProfile = data.audioProfile;

            // Setup stats
            character.attributeSheet = new AttributeSheet();
            data.attributeSheet.CopyValuesIntoOther(character.attributeSheet);

            // Set up passive traits
            PerkController.Instance.BuildPlayerCharacterEntityPassivesFromCharacterData(character, data);

            // Set up health + energy views
            ModifyBaseMaxEnergy(character, 0);
            ModifyMaxHealth(character, 0);
            ModifyHealth(character, StatCalculator.GetTotalMaxHealth(character));

            // AI Logic
            character.aiTurnRoutine = data.aiTurnRoutine;
            if (character.aiTurnRoutine == null)
                Debug.LogWarning("Routine is null...");

            // Build UCM
            CharacterModeller.BuildModelFromStringReferences(character.hexCharacterView.ucm, data.modelParts);
            SetCharacterModelSize(character.hexCharacterView, data.modelSize);

            // Build activation window
            TurnController.Instance.CreateActivationWindow(character);

            // Set up items
            Items.ItemController.Instance.RunItemSetupOnHexCharacterFromItemSet(character, data.itemSet);

            // Setup abilities
            AbilityController.Instance.BuildHexCharacterAbilityBookFromData(character, data.abilityBook);
        }
        private void SetupCharacterFromEnemyData(HexCharacterModel character, EnemyTemplateSO data)
        {
            // Set general info
            character.myName = data.myName;
            character.race = data.race;
            //character.audioProfile = data.audioProfile;

            // Setup stats
            character.attributeSheet = new AttributeSheet();
            data.attributeSheet.CopyValuesIntoOther(character.attributeSheet);

            // Set up passive traits
            PerkController.Instance.BuildEnemyCharacterEntityPassivesFromEnemyData(character, data);

            // Set up items
            Items.ItemSet itemSet = new Items.ItemSet();
            Items.ItemController.Instance.CopySerializedItemManagerIntoStandardItemManager(data.itemSet, itemSet);
            Items.ItemController.Instance.RunItemSetupOnHexCharacterFromItemSet(character, itemSet);

            // Set up randomized health (if marked)           
            if (data.randomizeHealth)
                character.attributeSheet.maxHealth = RandomGenerator.NumberBetween(data.lowerHealthLimit, data.upperHealthLimit);

            // Set up health + energy views
            ModifyBaseMaxEnergy(character, 0);
            ModifyMaxHealth(character, 0);
            ModifyHealth(character, StatCalculator.GetTotalMaxHealth(character));

            // AI Logic
            character.aiTurnRoutine = data.aiTurnRoutine;
            if (character.aiTurnRoutine == null)
                Debug.LogWarning("Routine is null...");

            // Build UCM
            CharacterModeller.BuildModelFromStringReferences(character.hexCharacterView.ucm, data.modelParts);
            SetCharacterModelSize(character.hexCharacterView, data.modelSize);

            // Build activation window
            TurnController.Instance.CreateActivationWindow(character);

            // Setup abilities
            AbilityController.Instance.BuildHexCharacterAbilityBookFromData(character,
                AbilityController.Instance.ConvertSerializedAbilityBookToUnserialized(data.abilityBook));
        }
        public void SpawnEnemyEncounter(EnemyEncounterData encounterData)
        {
            Debug.Log("SpawnEnemyEncounter() Called....");

            List<LevelNode> spawnLocations = LevelController.Instance.GetEnemySpawnZone();
          
            // Create all enemies in wave
            foreach (CharacterWithSpawnData dataSet in encounterData.enemiesInEncounter)
            {
                LevelNode spawnNode = LevelController.Instance.GetHexAtGridPosition(dataSet.spawnPosition);
                if (Pathfinder.IsHexSpawnable(spawnNode) == false) 
                    spawnNode = LevelController.Instance.GetRandomSpawnableLevelNode(spawnLocations, false);
                CreateEnemyHexCharacter(dataSet.characterData, spawnNode);
            }

        }
        
        public HexCharacterModel CreateSummonedHexCharacter(EnemyTemplateSO data, LevelNode startPosition, Allegiance allegiance)
        {
            // Create GO + View
            HexCharacterView vm = CreateCharacterEntityView().GetComponent<HexCharacterView>();

            // Create data object
            HexCharacterModel model = new HexCharacterModel();

            // Connect model to view
            model.hexCharacterView = vm;
            vm.character = model;

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
            SetupCharacterFromEnemyData(model, data);

            // Add to persistency
            AddDefenderToPersistency(model);

            // Prevent character taking its turn this turn cycle
            model.wasSummonedThisTurn = true;

            return model;
        }      


        #endregion

        // Modify Health
        #region
        public void ModifyHealth(HexCharacterModel character, int healthGainedOrLost, bool relayToCharacterData = true)
        {
            Debug.Log("CharacterEntityController.ModifyHealth() called for " + character.myName);

            //int originalHealth = character.currentHealth;
            int finalHealthValue = character.currentHealth;

            finalHealthValue += healthGainedOrLost;

            // prevent health increasing over maximum
            if (finalHealthValue > StatCalculator.GetTotalMaxHealth(character))
            {
                finalHealthValue = StatCalculator.GetTotalMaxHealth(character);
            }

            // prevent health going less then 0
            if (finalHealthValue < 0)
            {
                finalHealthValue = 0;
            }

            // Set health after calculation
            character.currentHealth = finalHealthValue;

            // relay changes to character data
            if (character.characterData != null && relayToCharacterData)
            {
                CharacterDataController.Instance.SetCharacterHealth(character.characterData, character.currentHealth);
            }

            Debug.Log(character.myName + " health value = " + character.currentHealth.ToString());

            VisualEventManager.Instance.CreateVisualEvent(() => UpdateHealthGUIElements(character, finalHealthValue, StatCalculator.GetTotalMaxHealth(character)), QueuePosition.Back, 0, 0, character.GetLastStackEventParent());
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
            character.hexCharacterView.healthBarWorld.value = healthBarFloat;
            character.hexCharacterView.healthTextWorld.text = health.ToString();
            //character.hexCharacterView.maxHealthTextWorld.text = maxHealth.ToString();

            // Modify UI elements
            character.hexCharacterView.healthBarUI.value = healthBarFloat;
            character.hexCharacterView.healthTextUI.text = health.ToString();
            character.hexCharacterView.maxHealthTextUI.text = maxHealth.ToString();

        }
        #endregion

        // Modify Stress
        #region
        public void ModifyStress(HexCharacterModel character, int stressGainedOrLost, bool relayToCharacterData = true, bool showVFX = false)
        {
            Debug.Log("CharacterEntityController.ModifyStress() called for " + character.myName);

            // Enemy characters do not suffer from stress
            if (character.allegiance == Allegiance.Enemy) return;

            HexCharacterView view = character.hexCharacterView;
            int originalStress = character.currentStress;
            int finalStress = character.currentStress;
            StressState previousStressState = CombatController.Instance.GetStressStateFromStressAmount(character.currentStress);

            finalStress += stressGainedOrLost;
            StressState newStressState = CombatController.Instance.GetStressStateFromStressAmount(character.currentStress);

            // prevent health increasing over maximum
            if (finalStress > 100)
            {
                finalStress = 100;
            }

            // prevent health going less then 0
            if (finalStress < 0)
            {
                finalStress = 0;
            }

            // Set stress after calculation
            character.stressGainedThisCombat += finalStress - originalStress;
            character.currentStress = finalStress;

            // relay changes to character data
            if (character.characterData != null && relayToCharacterData)
            {
                CharacterDataController.Instance.SetCharacterStress(character.characterData, finalStress);
            }

            // Print
            Debug.Log(character.myName + " stress value = " + character.currentStress.ToString());

            // Stress VFX
            if (stressGainedOrLost > 0 && showVFX)
            {
                if (character.eventStacks.Count > 0)
                {
                    VisualEventManager.Instance.CreateVisualEvent(() =>
                        VisualEffectManager.Instance.CreateStressGainedEffect(view.WorldPosition, stressGainedOrLost), QueuePosition.Back, 0, 0f, character.GetLastStackEventParent());
                }
                else
                {
                    VisualEventManager.Instance.CreateVisualEvent(() =>
                    VisualEffectManager.Instance.CreateStressGainedEffect(view.WorldPosition, stressGainedOrLost), QueuePosition.Back, 0, 0);
                }

            }

            // Update stress related GUI
            VisualEventManager.Instance.CreateVisualEvent(() => UpdateStressGUIElements(character, finalStress), QueuePosition.Back, 0, 0);

            // Check if there was a change of stress state, up or down
            if(previousStressState != newStressState)
            {

            }
        }
        private void UpdateStressGUIElements(HexCharacterModel character, int stress)
        {
            Debug.Log("CharacterEntityController.UpdateStressGUIElements() called, stress = " + stress.ToString());

            if (character.hexCharacterView == null) return;

            // Convert health int values to floats
            float currentStressFloat = stress;
            float currentMaxHealthFloat = 100f;
            float stressBarFloat = currentStressFloat / currentMaxHealthFloat;

            // Modify WORLD space ui
            character.hexCharacterView.stressBarWorld.value = stressBarFloat;
            character.hexCharacterView.stressTextWorld.text = stress.ToString();
            //character.hexCharacterView.maxStressTextWorld.text = "100";

            // Modify UI elements
            character.hexCharacterView.stressBarUI.value = stressBarFloat;
            character.hexCharacterView.stressTextUI.text = stress.ToString();
            character.hexCharacterView.maxStressTextUI.text = "100";

            //myActivationWindow.myHealthBar.value = finalValue;

            character.hexCharacterView.stressPanel.BuildPanelViews(character);

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
        public void TriggerMeleeAttackAnimation(HexCharacterView view, Vector2 targetPos, CoroutineData cData)
        {
            AudioManager.Instance.StopSound(Sound.Character_Footsteps);
            StartCoroutine(TriggerMeleeAttackAnimationCoroutine(view, targetPos, cData));
        }
        private IEnumerator TriggerMeleeAttackAnimationCoroutine(HexCharacterView view, Vector2 targetPos, CoroutineData cData)
        {
            view.ucmAnimator.SetTrigger("Melee Attack");
            Vector2 startPos = view.WorldPosition;
            Vector2 forwardPos = (startPos + targetPos) / 2;
            float moveSpeedTime = 0.25f;

            HexCharacterModel model = view.character;
            if (model != null)
            {
                // slight movement forward
                view.ucmMovementParent.transform.DOMove(forwardPos, moveSpeedTime);
                yield return new WaitForSeconds(moveSpeedTime / 2);

                if (cData != null)
                {
                    cData.MarkAsCompleted();
                }

                yield return new WaitForSeconds(moveSpeedTime / 2);

                // move back to start pos
                view.ucmMovementParent.transform.DOMove(startPos, moveSpeedTime);
                yield return new WaitForSeconds(moveSpeedTime);
            }

        }
        public void PlayShootBowAnimation(HexCharacterView view, CoroutineData cData)
        {
            Debug.Log("CharacterEntityController.PlayRangedAttackAnimation() called...");
            AudioManager.Instance.StopSound(Sound.Character_Footsteps);
            AudioManager.Instance.PlaySoundPooled(Sound.Character_Draw_Bow);
            StartCoroutine(PlayShootBowAnimationCoroutine(view, cData));
        }
        private IEnumerator PlayShootBowAnimationCoroutine(HexCharacterView view, CoroutineData cData)
        {
            view.ucmAnimator.SetTrigger("Shoot Bow");
            AudioManager.Instance.StopSound(Sound.Character_Footsteps);
            yield return new WaitForSeconds(0.5f);

            // Resolve
            if (cData != null)
            {
                cData.MarkAsCompleted();
            }
        }
        public void TriggerShootProjectileAnimation(HexCharacterView view)
        {
            if (view == null) return;
            AudioManager.Instance.StopSound(Sound.Character_Footsteps);
            view.ucmAnimator.SetTrigger("Melee Attack");
        }
        public void TriggerAoeMeleeAttackAnimation(HexCharacterView view)
        {
            if (view == null) return;
            view.ucmAnimator.SetTrigger("Melee Attack");
            AudioManager.Instance.StopSound(Sound.Character_Footsteps);
        }
        public void PlayIdleAnimation(HexCharacterView view)
        {
            if (view == null) return;
            view.ucmAnimator.SetTrigger("Idle");
            AudioManager.Instance.StopSound(Sound.Character_Footsteps);
        }
        public void PlaySkillAnimation(HexCharacterView view)
        {
            if (view == null) return;
            view.ucmAnimator.SetTrigger("Skill Two");
            AudioManager.Instance.StopSound(Sound.Character_Footsteps);

        }
        public void PlayMoveAnimation(HexCharacterView view)
        {
            if (view == null) return;
            AudioManager.Instance.PlaySound(Sound.Character_Footsteps);
            view.ucmAnimator.SetTrigger("Move");
        }
        public void PlayHurtAnimation(HexCharacterView view)
        {
            if (view == null) return;
            AudioManager.Instance.StopSound(Sound.Character_Footsteps);
            view.ucmAnimator.SetTrigger("Hurt");
        }
        public void PlayDuckAnimation(HexCharacterView view)
        {
            Debug.Log("PlayDuckAnimation() called...");
            if (view == null) return;
            AudioManager.Instance.StopSound(Sound.Character_Footsteps);
            view.ucmAnimator.SetTrigger("Duck");
        }
        public void PlayDeathAnimation(HexCharacterView view)
        {
            if (view == null) return;
            AudioManager.Instance.StopSound(Sound.Character_Footsteps);
            view.ucmAnimator.SetTrigger("Die");
        }
        public void PlayResurrectAnimation(HexCharacterView view)
        {
            if (view == null) return;
            view.ucmAnimator.SetTrigger("Resurrect");
        }
        #endregion

        // Turn Events + Sequences
        #region
        public void CharacterOnTurnStart(HexCharacterModel character)
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
                // Build ability bar
                AbilityController.Instance.BuildHexCharacterAbilityBar(character);
                VisualEventManager.Instance.CreateVisualEvent(() => FadeInCharacterUICanvas(character.hexCharacterView, null), QueuePosition.Back);
            }              

            if (!character.hasRequestedTurnDelay)
            {
                // Gain Energy + update energy views
                int energyGain = StatCalculator.GetTotalEnergyRecovery(character);
                
                // Check pyromania perk bonus
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Eager) && TurnController.Instance.CurrentTurn == 1)
                    energyGain += 1;                

                ModifyEnergy(character, energyGain, false, false);
            }              

            // Check effects that are triggered on the first turn only
            if (TurnController.Instance.CurrentTurn == 1 && !character.hasRequestedTurnDelay)
            {                
                // TO DO: characters should START combat with these effects, not gain them on the start of their
                // first turn. move this code somewhere else.

                // Tough (gain X block)
                if(PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Tough))
                {
                    int stacks = PerkController.Instance.GetStackCountOfPerkOnCharacter(character.pManager, Perk.Tough);
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Block, stacks, true, 0.5f);
                }

                // Motivated (gain X focus)
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Motivated))
                {
                    int stacks = PerkController.Instance.GetStackCountOfPerkOnCharacter(character.pManager, Perk.Motivated);
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Focus, stacks, true, 0.5f);
                }

                // Elf (gain rune)
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.BloodOfTheAncients))
                {
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Rune, 1, true, 0.5f);
                }

                // Blood Thristy (gain 2 wrath)
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.BloodThirsty))
                {
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Wrath, 2, true, 0.5f);
                }
            }

            // Perk expiries on turn start
            if (!character.hasRequestedTurnDelay)
            {
                // Misc expiries
                character.tilesMovedThisTurn = 0;
                character.skillAbilitiesUsedThisTurn = 0;
                character.meleeAttackAbilitiesUsedThisTurn = 0;
                character.rangedAttackAbilitiesUsedThisTurn = 0;
                character.charactersKilledThisTurn = 0;

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
                    CombatController.Instance.HandleDamage(character, damageResult, DamageType.Physical);
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
                    CombatController.Instance.HandleDamage(character, damageResult, DamageType.Magic);
                    VisualEventManager.Instance.InsertTimeDelayInQueue(0.5f);
                    
                    // Remove 1 stack of burning
                    if (character.currentHealth > 0)
                        PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Burning, -1, false);
                    
                }

                // Bleeding
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Bleeding) && character.currentHealth > 0)
                {
                    // Notification event
                    VisualEventManager.Instance.CreateVisualEvent(() => VisualEffectManager.Instance.CreateStatusEffect(view.WorldPosition, "Bleeding!"), QueuePosition.Back, 0, 0.5f);

                    // Calculate and deal Magic damage
                    DamageResult damageResult = CombatController.Instance.GetFinalDamageValueAfterAllCalculations(character, 5 * PerkController.Instance.GetStackCountOfPerkOnCharacter(character.pManager, Perk.Bleeding), DamageType.Physical);
                    VisualEventManager.Instance.CreateVisualEvent(() => VisualEffectManager.Instance.CreateEffectAtLocation(ParticleEffect.BloodExplosion, view.WorldPosition));
                    CombatController.Instance.HandleDamage(character, damageResult, DamageType.Physical);
                    VisualEventManager.Instance.InsertTimeDelayInQueue(0.5f);

                    // Remove 1 stack of bleeding
                    if (character.currentHealth > 0)
                        PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Bleeding, -1, false);


                }
                #endregion

                // PASSIVE EXPIRIES
                // Taunt
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Taunt) && character.currentHealth > 0)
                {
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Taunt, -1, true, 0.5f);
                }

                // Divine Favour
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.DivineFavour) && character.currentHealth > 0)
                {
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.DivineFavour, -1, true, 0.5f);
                }
            }

            // was character killed by a DoT?
            if(character.currentHealth <= 0)
            {
                return;
            }

            // ENEMY Start turn 
            if (character.controller == Controller.AI)
            {
                VisualEventManager.Instance.InsertTimeDelayInQueue(1f);
                SetCharacterActivationPhase(character, ActivationPhase.ActivationPhase);
                AILogic.RunEnemyRoutine(character);

                if(character.livingState == LivingState.Alive &&
                    TurnController.Instance.EntityActivated == character)
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

            // Disable end turn button clickability
            TurnController.Instance.DisableEndTurnButtonInteractions();
            TurnController.Instance.DisableDelayTurnButtonInteractions();

            // Disable info windows
            if(character.controller == Controller.Player)
            {
                AbilityPopupController.Instance.HidePanel();
                AbilityController.Instance.HideHitChancePopup();
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
                // Fade out view
                CoroutineData fadeOutEvent = new CoroutineData();
                VisualEventManager.Instance.CreateVisualEvent(() => FadeOutCharacterUICanvas(character.hexCharacterView, fadeOutEvent), fadeOutEvent);
            }

            if (!character.hasRequestedTurnDelay)
            {
                // DEBUFF EXPIRIES
                #region

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

                #endregion

                // BUFF EXPIRIES
                #region
                // Eagle Eye
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.EagleEye) && character.currentHealth > 0)
                {
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.EagleEye, -1, true, 0.5f);
                }

                // Concealing Clouds
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.ConcealingClouds) && character.currentHealth > 0)
                {
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.ConcealingClouds, -1, true, 0.5f);
                }

                // Cleansing Waters
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.CleaningWaters) && character.currentHealth > 0)
                {
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.CleaningWaters, -1, true, 0.5f);
                }

                // Siphon Soul + Enriched Soul
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.SiphonedSoul) && character.currentHealth > 0)
                {
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.SiphonedSoul, -1, true, 0.5f);
                }
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.EnrichedSoul) && character.currentHealth > 0)
                {
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.EnrichedSoul, -1, true, 0.5f);
                }

                // Ignited Weapon
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.FlamingWeapon) && character.currentHealth > 0)
                {
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.FlamingWeapon, -1, true, 0.5f);
                }
                #endregion

                // MISC
                #region
                // Abusive
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Abusive) && character.currentHealth > 0)
                {
                    VisualEventManager.Instance.CreateVisualEvent(() =>
                    VisualEffectManager.Instance.CreateStatusEffect(view.WorldPosition, "Abusive!"), QueuePosition.Back, 0f, 0.5f);

                    List<HexCharacterModel> allies = GetAlliesWithinMyAura(character);
                    foreach(HexCharacterModel ally in allies)
                    {
                        CombatController.Instance.CreateStressCheck(ally, new StressEventData(3, 5, 50), true);
                    }
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
                    {
                        PerkController.Instance.ModifyPerkOnCharacterEntity(enemy.pManager, Perk.Weakened,stacks, true, 0,character.pManager);
                    }
                    VisualEventManager.Instance.InsertTimeDelayInQueue(0.5f);
                }

                // Savage Leader
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.SavageLeader) && character.currentHealth > 0)
                {
                    List<HexCharacterModel> allies = GetAlliesWithinMyAura(character);
                    HexCharacterModel ally = allies[RandomGenerator.NumberBetween(0, allies.Count - 1)];

                    if(ally != null)
                    {
                        int stacks = PerkController.Instance.GetStackCountOfPerkOnCharacter(character.pManager, Perk.SavageLeader);
                        VisualEventManager.Instance.CreateVisualEvent(() =>
                        VisualEffectManager.Instance.CreateStatusEffect(view.WorldPosition, "Savage Leader!"), QueuePosition.Back, 0f, 0.5f);
                        PerkController.Instance.ModifyPerkOnCharacterEntity(ally.pManager, Perk.Wrath, stacks, true, 0, character.pManager);
                        VisualEventManager.Instance.InsertTimeDelayInQueue(0.5f);
                    }
                  
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
        public void ModifyEnergy(HexCharacterModel character, int energyGainedOrLost, bool showVFX = false, bool updateEnergyGuiInstantly = true)
        {
            Debug.Log("CharacterEntityController.ModifyEnergy() called for " + character.myName);
            character.currentEnergy += energyGainedOrLost;
            HexCharacterView view = character.hexCharacterView;

            if (character.currentEnergy < 0)
            {
                character.currentEnergy = 0;
            }

            else if (character.currentEnergy > StatCalculator.GetTotalMaxEnergy(character))
            {
                character.currentEnergy = StatCalculator.GetTotalMaxEnergy(character);
            }

            if (showVFX && view != null)
            {

                if (energyGainedOrLost > 0)
                {
                    // Status notification
                    VisualEventManager.Instance.CreateVisualEvent(() =>
                    VisualEffectManager.Instance.CreateStatusEffect(view.WorldPosition, "Energy +" + energyGainedOrLost.ToString()));

                    // Buff sparkle VFX
                    VisualEventManager.Instance.CreateVisualEvent(() => VisualEffectManager.Instance.CreateGeneralBuffEffect(view.WorldPosition));
                }
                else if (energyGainedOrLost < 0)
                {
                    // Status notification
                    VisualEventManager.Instance.CreateVisualEvent(() =>
                    VisualEffectManager.Instance.CreateStatusEffect(view.WorldPosition, "Energy " + energyGainedOrLost.ToString()));

                    // Debuff sparkle VFX
                    VisualEventManager.Instance.CreateVisualEvent(() => VisualEffectManager.Instance.CreateGeneralDebuffEffect(view.WorldPosition));
                }
            }

            QueuePosition qPos = QueuePosition.Back;
            if (updateEnergyGuiInstantly) qPos = QueuePosition.Front;

            int energyVfxValue = character.currentEnergy;
            int maxEnergyVfxValue = StatCalculator.GetTotalMaxEnergy(character);
            VisualEventManager.Instance.CreateVisualEvent(() => UpdateEnergyGUIElements(character, energyVfxValue, maxEnergyVfxValue), qPos, 0, 0);

            //CardController.Instance.AutoUpdateCardsInHandGlowOutlines(character);
            // to do maybe: auto update ability bar glows??
        }
        public void ModifyBaseMaxEnergy(HexCharacterModel character, int maxEnergyGainedOrLost, bool updateEnergyGuiInstantly = true)
        {
            Debug.Log("CharacterEntityController.ModifyBaseMaxEnergy() called for " + character.myName);
            character.attributeSheet.maxEnergy += maxEnergyGainedOrLost;
            HexCharacterView view = character.hexCharacterView;

            if (character.attributeSheet.maxEnergy < 0)
            {
                character.attributeSheet.maxEnergy = 0;
            }

            // int energyVfxValue = StatCalculator.GetTotalMaxEnergy(character);
            // VisualEventManager.Instance.CreateVisualEvent(() => UpdateMaxEnergyText(view, energyVfxValue), QueuePosition.Front, 0, 0);

            QueuePosition qPos = QueuePosition.Back;
            if (updateEnergyGuiInstantly) qPos = QueuePosition.Front;

            int energyVfxValue = character.currentEnergy;
            int maxEnergyVfxValue = StatCalculator.GetTotalMaxEnergy(character);
            VisualEventManager.Instance.CreateVisualEvent(() => UpdateEnergyGUIElements(character, energyVfxValue, maxEnergyVfxValue), qPos, 0, 0);

        }             
        private void UpdateEnergyGUIElements(HexCharacterModel character, int energy, int maxEnergy)
        {
            // Convert energy int values to floats
            float currentHealthFloat = energy;
            float currentMaxHealthFloat = maxEnergy;
            float healthBarFloat = currentHealthFloat / currentMaxHealthFloat;

            // Modify text + slider ui
            character.hexCharacterView.energyBar.value = healthBarFloat;
            character.hexCharacterView.energyTextUI.text = energy.ToString();
            character.hexCharacterView.maxEnergyTextUI.text = maxEnergy.ToString();

        }
        #endregion

        // UI Visual Events
        #region
        public void FadeInCharacterUICanvas(HexCharacterView view, CoroutineData cData)
        {
            if (view == null)
            {
                if (cData != null) cData.MarkAsCompleted();
                return;
            }
            view.uiCanvasParent.SetActive(true);
            view.uiCanvasCg.alpha = 0;
            Sequence s = DOTween.Sequence();
            s.Append(view.uiCanvasCg.DOFade(1, 0.75f));
            s.OnComplete(() =>
            {
                // Resolve
                if (cData != null)
                    cData.MarkAsCompleted();
            });
        }
        public void FadeOutCharacterUICanvas(HexCharacterView view, CoroutineData cData)
        {
            if (view == null)
            {
                if (cData != null) cData.MarkAsCompleted();
                return;
            }
            view.uiCanvasParent.SetActive(true);
            Sequence s = DOTween.Sequence();
            s.Append(view.uiCanvasCg.DOFade(0, 0.75f));
            s.OnComplete(() =>
            {
                view.uiCanvasParent.SetActive(false);

                // Resolve
                if (cData != null)
                {
                    cData.MarkAsCompleted();
                }
            });
        }
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
            view.ucmShadowCg.DOFade(1f, 0f);
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
        public List<HexCharacterModel> GetAlliesWithinMyAura(HexCharacterModel character, bool includeSelf = false)
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
        public bool IsCharacterEngagedInMelee(HexCharacterModel character)
        {
            bool bRet = false;
            List<LevelNode> meleeTiles = LevelController.Instance.GetAllHexsWithinRange(character.currentTile, 1);

            foreach(LevelNode h in meleeTiles)
            {
                if(h.myCharacter != null &&
                    h.myCharacter.allegiance != character.allegiance &&
                    h.myCharacter.itemSet.mainHandItem != null &&
                    h.myCharacter.itemSet.mainHandItem.IsMeleeWeapon)
                {
                    bRet = true;
                    break;
                }
            }

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
                    !IsTargetFriendly(target, h.myCharacter))
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

            // +10 Accuracy from flanking
            if (IsCharacterFlanked(target))
            {
                bonusRet = 10 * (GetTotalFlankingCharactersOnTarget(target) - 1);

                // Bonus is doubled for characters with opportunist perk
                if (PerkController.Instance.DoesCharacterHavePerk(attacker.pManager, Perk.Opportunist))
                    bonusRet *= 2;

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

                // Bonus is doubled for characters with opportunist perk
                if (PerkController.Instance.DoesCharacterHavePerk(attacker.pManager, Perk.Opportunist))
                    bonusRet *= 2;

                // Backstriked characters dont benefit from dodge stat
                bonusRet += StatCalculator.GetTotalDodge(target);
            }

            return bonusRet;
        }
        public int CalculateElevationAccuracyModifier(HexCharacterModel attacker, HexCharacterModel target)
        {
            int bonusRet = 0;

            /*
            // +10 Accuracy attacking from elevation
            if (attacker.myCurrentHex.elevation == TileElevation.Elevated &&
                target.myCurrentHex.elevation == TileElevation.Ground)
            {
                bonusRet += 10;
            }

            // -10 Accuracy attacking from ground to elevated target
            else if (attacker.myCurrentHex.elevation == TileElevation.Ground &&
                target.myCurrentHex.elevation == TileElevation.Elevated)
            {
                bonusRet -= 10;
            }
            */

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
            {
                entity.hexCharacterView.ucmMovementParent.transform.position = LevelController.Instance.DefenderOffScreenNode.transform.position;
            }
            else if (entity.allegiance == Allegiance.Enemy)
            {
                entity.hexCharacterView.ucmMovementParent.transform.position = LevelController.Instance.EnemyOffScreenNode.transform.position;
            }
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
        public void DestroyCharacterView(HexCharacterView view)
        {
            Debug.Log("CharacterEntityController.DestroyCharacterView() called...");
            Destroy(view.gameObject);
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


        }
        #endregion

        // Conditional Checks on Characters
        #region
        public bool IsCharacterAbleToMove(HexCharacterModel c)
        {
            bool bRet = true;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Rooted) ||
                    PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Stunned))
                bRet = false;

            return bRet;
        }
        public bool IsCharacterAbleToTakeActions(HexCharacterModel c)
        {
            bool bRet = true;

            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Stunned) )
                bRet = false;

            return bRet;
        }
        public bool IsCharacterAbleToMakeFreeStrikes(HexCharacterModel c)
        {
            if (c.itemSet.mainHandItem != null &&
                c.itemSet.mainHandItem.IsMeleeWeapon &&
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
        #endregion
    }
}