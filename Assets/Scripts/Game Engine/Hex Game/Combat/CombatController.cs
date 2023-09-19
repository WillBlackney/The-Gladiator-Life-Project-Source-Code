using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using WeAreGladiators.Abilities;
using WeAreGladiators.Audio;
using WeAreGladiators.CameraSystems;
using WeAreGladiators.Characters;
using WeAreGladiators.CombatLog;
using WeAreGladiators.HexTiles;
using WeAreGladiators.Items;
using WeAreGladiators.JourneyLogic;
using WeAreGladiators.Perks;
using WeAreGladiators.Player;
using WeAreGladiators.Scoring;
using WeAreGladiators.TurnLogic;
using WeAreGladiators.UCM;
using WeAreGladiators.UI;
using WeAreGladiators.Utilities;
using WeAreGladiators.VisualEvents;

namespace WeAreGladiators.Combat
{
    public class CombatController : Singleton<CombatController>
    {

        // Getters + Accessors
        #region

        public CombatGameState CurrentCombatState { get; private set; }
        
        public MoraleStateData GetMoraleStateData(MoraleState moraleState)
        {
            return Array.Find(moraleStateData, data => data.moraleState == moraleState);
        }

        #endregion

        // Combat State
        #region

        public void SetCombatState(CombatGameState newState)
        {
            Debug.Log("CombatController.SetCombatState() called, new state: " + newState);
            CurrentCombatState = newState;
        }

        #endregion

        // Damage Type Calculation
        #region

        public DamageType GetFinalFinalDamageTypeOfAbility(HexCharacterModel entity, AbilityEffect abilityEffect, AbilityData ability)
        {
            Debug.Log("CombatController.CalculateFinalDamageTypeOfAttack() called...");

            DamageType damageTypeReturned = abilityEffect.damageType;

            // Check overload perk passive
            if (ability.abilityType.Contains(AbilityType.WeaponAttack) &&
                PerkController.Instance.DoesCharacterHavePerk(entity.pManager, Perk.Overload))
            {
                Debug.Log("CalculateFinalDamageTypeOfAttack() attacker has 'Overload' perk, damage dealt converted to Magic damage");
                damageTypeReturned = DamageType.Magic;
            }

            // Check 'Flaming Weapon' buff
            /*
            if (ability.weaponRequirement != WeaponRequirement.None &&
                PerkController.Instance.DoesCharacterHavePerk(entity.pManager, Perk.FlamingWeapon))
            {
                Debug.Log("CalculateFinalDamageTypeOfAttack() attacker has 'Flaming Weapon' buff, damage dealt converted to Magic damage");
                damageTypeReturned = DamageType.Magic;
            }
            */

            Debug.Log("CombatController.GetFinalFinalDamageTypeOfAbility() final damage type returned: " + damageTypeReturned);
            return damageTypeReturned;
        }

        #endregion

        // Injuries Logic
        #region

        private void CheckAndHandleInjuryOnHealthLost(HexCharacterModel character, DamageResult damageResult, HexCharacterModel attacker, AbilityData ability, AbilityEffect effect)
        {
            // INJURY CALCULATION RULES
            // 25% Chance to be injured (base chance).
            // Injury chance REDUCED by 1% for each point of injury resistance.
            // Injury chance INCREASED by 1% for each 1% of health lost from max health in the attack
            // (e.g. if the character lost 17% of their health, the injury chance is increased by 17% to 42%.

            // Can only be injured by ability based attacks (e.g. cant be injured from Bleeding, Poisoned, etc)
            if (ability == null || effect == null)
            {
                return;
            }

            if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.InjuryImmunity))
            {
                return;
            }

            int roll = RandomGenerator.NumberBetween(1, 1000);
            float baseInjuryChance = 25;
            float healthLostModifier = StatCalculator.GetPercentage(damageResult.totalHealthLost, StatCalculator.GetTotalMaxHealth(character));
            float injuryResistanceMod = StatCalculator.GetTotalInjuryResistance(character);
            float injuryChanceActual = (baseInjuryChance + healthLostModifier - injuryResistanceMod) * 10;
            int mildThreshold = (int) (StatCalculator.GetTotalMaxHealth(character) * 0.1f);
            int severeThreshold = (int) (StatCalculator.GetTotalMaxHealth(character) * 0.25f);

            Debug.Log("CheckAndHandleInjuryOnHealthLost() called on character " + character.myName +
                ", Rolled = : " + roll + "/1000" +
                ", injury probability = " + injuryChanceActual / 10 + "%" +
                ", health lost = " + damageResult.totalHealthLost +
                ", injury thresholds: Mild = " + mildThreshold + " health or more, Severe = " + severeThreshold + " health or more.");

            // Did character lose enough health to trigger an injury?
            if (damageResult.totalHealthLost < mildThreshold)
            {
                return;
            }

            // Character successfully resisted the injury
            if (roll > injuryChanceActual)
            {
                Debug.Log("CheckAndHandleInjuryOnHealthLost() character successfully resisted the injury, rolled " + roll +
                    ", needed less than " + injuryChanceActual);
                return;
            }
            if (roll <= injuryChanceActual)
            {
                Debug.Log("CheckAndHandleInjuryOnHealthLost() character failed to resist the injury, rolled " + roll +
                    ", needed more than " + injuryChanceActual);

                // Determine injury severity
                InjurySeverity severity = InjurySeverity.None;

                // Injury is severe if character lost at least 25% health, or if the attack was a critical
                if (damageResult.totalHealthLost >= severeThreshold ||
                    damageResult.didCrit)
                {
                    severity = InjurySeverity.Severe;
                }
                else if (damageResult.totalHealthLost >= mildThreshold)
                {
                    severity = InjurySeverity.Mild;
                }

                // Determine injury type based on the weapon used, or the ability (if it doesnt require a weapon e.g. fireball)
                InjuryType injuryType = InjuryType.Blunt;
                if (ability.weaponRequirement == WeaponRequirement.None &&
                    effect != null)
                {
                    // Get injury type from ability effect
                    if (effect.injuryTypesCaused.Length == 1)
                    {
                        injuryType = effect.injuryTypesCaused[0];
                    }
                    else if (effect.injuryTypesCaused.Length > 1)
                    {
                        injuryType = effect.injuryTypesCaused[RandomGenerator.NumberBetween(0, effect.injuryTypesCaused.Length - 1)];
                    }
                }
                else if (ability.weaponRequirement != WeaponRequirement.None)
                {
                    // Get injury type from weapon used
                    ItemData weaponUsed = attacker.itemSet.mainHandItem;

                    // Check if injury type should be drawn from the shield instead
                    if (ability.weaponRequirement == WeaponRequirement.Shield)
                    {
                        weaponUsed = attacker.itemSet.offHandItem;
                    }

                    // If weapon causes multiple injury types, pick one randomly.
                    if (weaponUsed != null)
                    {
                        if (weaponUsed.injuryTypesCaused.Length == 1)
                        {
                            injuryType = weaponUsed.injuryTypesCaused[0];
                        }
                        else if (weaponUsed.injuryTypesCaused.Length > 1)
                        {
                            injuryType = weaponUsed.injuryTypesCaused[RandomGenerator.NumberBetween(0, weaponUsed.injuryTypesCaused.Length - 1)];
                        }
                    }
                }

                Debug.Log("CheckAndHandleInjuryOnHealthLost() injury determinations: Severity = " + severity +
                    ", Injury Type = " + injuryType);

                // Succesfully made all calculations??
                if (severity != InjurySeverity.None && injuryType != InjuryType.None)
                {
                    // Get a random viable injury
                    PerkIconData injuryGained = PerkController.Instance.GetRandomValidInjury(character.pManager, severity, injuryType);

                    // Apply the injury
                    if (injuryGained != null)
                    {
                        int injuryStacks = RandomGenerator.NumberBetween(injuryGained.minInjuryDuration, injuryGained.maxInjuryDuration);
                        //VisualEventManager.InsertTimeDelayInQueue(0.5f, character.GetLastStackEventParent());
                        PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, injuryGained.perkTag, injuryStacks, true, 1f);
                        //VisualEventManager.InsertTimeDelayInQueue(0.5f, character.GetLastStackEventParent());

                        CombatLogController.Instance.CreateInjuryEntry(character, injuryGained.passiveName, (int) (roll * 0.1f), (int) (injuryChanceActual * 0.1f));

                        // In case injury affects max health or max fatigue, update current health and current fatigue                   
                        HexCharacterController.Instance.ModifyMaxHealth(character, 0);
                        //HexCharacterController.Instance.ModifyCurrentFatigue(character, 0);

                        // Stress Check events on injury applied
                        CreateMoraleCheck(character, StressEventType.InjuryGained);

                        // Check ally injured stress check
                        foreach (HexCharacterModel c in HexCharacterController.Instance.GetAllAlliesOfCharacter(character))
                        {
                            CreateMoraleCheck(c, StressEventType.AllyInjured);
                        }

                        // Check enemy injured stress check
                        foreach (HexCharacterModel c in HexCharacterController.Instance.GetAllEnemiesOfCharacter(character))
                        {
                            // Check squeamish trait
                            if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Squeamish))
                            {
                                CreateMoraleCheck(c, StressEventType.AllyInjured);
                            }
                            else
                            {
                                CreateMoraleCheck(c, StressEventType.EnemyInjured);
                            }
                        }

                        // Check 'What Doesn't Kill Me Perk': gain permanent stats
                        if (character.characterData != null &&
                            character.controller == Controller.Player &&
                            PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.WhatDoesntKillMe))
                        {
                            PerkController.Instance.ModifyPerkOnCharacterData(character.pManager, Perk.WhatDoesntKillMe, 1);
                        }

                    }
                }

            }

        }

        #endregion

        // Handle Death
        #region

        public void HandleDeathBlow(HexCharacterModel character, VisualEvent parentEvent = null, bool guaranteedDeath = false, ItemData killingWeapon = null)
        {
            Debug.Log("CombatLogic.HandleDeathBlow() started for " + character.myName);

            // Cache relevant references for visual events
            HexCharacterView view = character.hexCharacterView;
            LevelNode hex = character.currentTile;
            TurnWindow window = view.myActivationWindow;

            // Mark as dead
            character.livingState = LivingState.Dead;
            HexCharacterController.Instance.Graveyard.Add(character);

            // Roll for death or survival
            DeathRollResult result = RollForDeathResist(character);

            // Remove from persitency
            if (character.allegiance == Allegiance.Enemy)
            {
                HexCharacterController.Instance.RemoveEnemyFromPersistency(character);
            }

            else if (character.allegiance == Allegiance.Player && HexCharacterController.Instance.AllSummonedPlayerCharacters.Contains(character))
            {
                HexCharacterController.Instance.RemoveSummonedDefenderFromPersistency(character);
            }

            else if (character.allegiance == Allegiance.Player)
            {
                HexCharacterController.Instance.RemoveDefenderFromPersistency(character);
            }

            // Remove from activation order
            TurnController.Instance.RemoveEntityFromActivationOrder(character);

            // Disable and hide player combat UI
            if (character.controller == Controller.Player && TurnController.Instance.EntityActivated == character)
            {
                VisualEventManager.CreateVisualEvent(() =>
                {
                    CombatUIController.Instance.SetInteractability(false);
                    CombatUIController.Instance.HideViewsOnTurnEnd();
                });
            }

            // Fade out world space GUI
            VisualEventManager.CreateVisualEvent(() =>
            {
                // to do: big crowd cheer SFX
                HexCharacterController.Instance.FadeOutCharacterWorldCanvas(view, null, 0.5f);
                view.vfxManager.StopAllEffects();
            }, parentEvent);

            // Play death animation
            // Randomize animation 
            int randomDeathAnim = RandomGenerator.NumberBetween(0, 1);

            // Randomize final rotation
            int randomDeathRotation = RandomGenerator.NumberBetween(1, 15);
            if (RandomGenerator.NumberBetween(0, 1) == 1)
            {
                randomDeathRotation = -randomDeathRotation;
            }

            // Randomize death position
            float randY = RandomGenerator.NumberBetween(1, 15) / 100f;
            float randX = RandomGenerator.NumberBetween(1, 22) / 100f;
            if (RandomGenerator.NumberBetween(0, 1) == 0)
            {
                randY = -randY;
            }
            if (RandomGenerator.NumberBetween(0, 1) == 0)
            {
                randX = -randX;
            }

            // Fade out UCM
            VisualEventManager.CreateVisualEvent(() => CharacterModeller.FadeOutCharacterShadow(view, 1f), parentEvent);

            // UCM die animation + blood effects
            VisualEventManager.CreateVisualEvent(() =>
            {
                int spatters = 2;
                character.hexCharacterView.model.RootSortingGroup.sortingOrder = character.hexCharacterView.model.RootSortingGroup.sortingOrder - 1;
                AudioManager.Instance.PlaySound(character.AudioProfile, AudioSet.Die);

                Vector3 finalPos = new Vector3(view.ucmMovementParent.transform.position.x + randX, view.ucmMovementParent.transform.position.y + randY, view.ucmMovementParent.transform.position.z);
                view.ucmMovementParent.transform.DOMove(finalPos, 0.5f);
                view.model.transform.DORotate(new Vector3(0, 0, randomDeathRotation), 0.5f);

                // Normal Death anim
                if (randomDeathAnim == 0 ||
                    view.model.TotalDecapitationAnims == 0 ||
                    killingWeapon == null ||
                    killingWeapon != null && killingWeapon.canDecapitate == false ||
                    result.pass && character.controller == Controller.Player)
                {
                    HexCharacterController.Instance.PlayDeathAnimation(view);
                }

                // Decapitation anim
                else
                {
                    spatters = 3;
                    VisualEffectManager.Instance.CreateEffectAtLocation(ParticleEffect.BloodExplosion, view.WorldPosition);
                    HexCharacterController.Instance.PlayDecapitateAnimation(view);
                }

                // Create random blood spatters
                for (int i = 0; i < spatters; i++)
                {
                    VisualEffectManager.Instance.CreateGroundBloodSpatter(view.WorldPosition);
                }

            }, parentEvent).SetEndDelay(1f);

            // Destroy characters activation window and update other window positions
            HexCharacterModel currentlyActivatedEntity = TurnController.Instance.EntityActivated;
            List<HexCharacterModel> cachedOrder = TurnController.Instance.ActivationOrder.ToList();
            VisualEventManager.CreateVisualEvent(() =>
                TurnController.Instance.OnCharacterKilledVisualEvent(window, currentlyActivatedEntity, cachedOrder), parentEvent).SetEndDelay(1f);

            // Roll for death or knock down on player characters
            if (character.controller == Controller.Player)
            {
                if (result.pass && !guaranteedDeath)
                {
                    // Gain permanent injury
                    PerkIconData permInjury = PerkController.Instance.GetRandomValidPermanentInjury(character);
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, permInjury.perkTag, 1, false);

                    // Move health to 1 (since 0 is invalid and they're not dead)
                    CharacterDataController.Instance.SetCharacterHealth(character.characterData, 1);
                    CombatLogController.Instance.CreatePermanentInjuryEntry(character, result, TextLogic.SplitByCapitals(permInjury.passiveName));
                }
                else
                {
                    CharacterDataController.Instance.RemoveCharacterFromRoster(character.characterData);
                    CombatLogController.Instance.CreateCharacterDiedEntry(character, result);
                }
            }
            else
            {
                CombatLogController.Instance.CreateCharacterDiedEntry(character, null);
            }

            // Break references
            LevelController.Instance.DisconnectCharacterFromTheirHex(character);

            // Destroy view and break references
            VisualEventManager.CreateVisualEvent(() =>
            {
                // Destroy view gameobject
                HexCharacterController.Instance.DisconnectModelFromView(character);
                HexCharacterController.Instance.DestroyCharacterView(view, true);
            }, parentEvent);

            // Lich death, kill all summoned skeletons
            if (character.myName == "Lich")
            {
                List<HexCharacterModel> skeletons = new List<HexCharacterModel>();
                foreach (HexCharacterModel c in HexCharacterController.Instance.GetAllAlliesOfCharacter(character, false))
                {
                    if (PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.FragileBinding))
                    {
                        HandleDeathBlow(c, c.GetLastStackEventParent(), true);
                    }
                }
            }

            // Volatile (explode on death)
            if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Volatile))
            {
                Debug.Log("CombatLogic.HandleDeathBlow() character killed has Volatile perk, applying Poisoned to nearby characters...");

                // Poison Explosion VFX on character killed
                VisualEventManager.CreateVisualEvent(() =>
                {
                    VisualEffectManager.Instance.CreatePoisonNova(view.WorldPosition);
                }, parentEvent);

                // Poison all nearby characters
                foreach (LevelNode tile in LevelController.Instance.GetAllHexsWithinRange(hex, 1))
                {
                    if (tile.myCharacter != null &&
                        tile.myCharacter.livingState == LivingState.Alive &&
                        tile.myCharacter.currentHealth > 0)
                    {
                        PerkController.Instance.ModifyPerkOnCharacterEntity(tile.myCharacter.pManager, Perk.Poisoned, 3, true, 0, character.pManager);
                    }
                }

                VisualEventManager.InsertTimeDelayInQueue(0.5f);
            }

            // Check if the combat defeat event should be triggered
            if (HexCharacterController.Instance.AllPlayerCharacters.Count == 0 &&
                CurrentCombatState == CombatGameState.CombatActive)
            {
                SetCombatState(CombatGameState.CombatInactive);
                // Game over? or just normal defeat?
                if (RunController.Instance.CurrentCombatContractData.enemyEncounterData.difficulty == CombatDifficulty.Boss)
                {
                    GameController.Instance.HandleGameOverBossCombatDefeat();
                }
                else
                {
                    GameController.Instance.StartCombatDefeatSequence();
                }
            }

            // Check if the combat victory event should be triggered
            else if (HexCharacterController.Instance.AllEnemies.Count == 0 &&
                     CurrentCombatState == CombatGameState.CombatActive)
            {
                SetCombatState(CombatGameState.CombatInactive);
                HandleOnCombatVictoryEffects();
                if (RunController.Instance.CurrentCombatContractData.enemyEncounterData.difficulty == CombatDifficulty.Boss)
                {
                    GameController.Instance.HandleGameOverBossCombatVictory();
                }
                else
                {
                    GameController.Instance.StartCombatVictorySequence();
                }
            }

            // If this character died during their turn (but no during end turn phase), 
            // resolve the transition to next character activation
            if (character == TurnController.Instance.EntityActivated)
            {
                TurnController.Instance.ActivateNextEntity();
            }
        }

        #endregion
        // Properties + Variables
        #region

        [Header("Stress Data")]
        [SerializeField] private StressEventSO[] allStressEventData;
        [SerializeField] private MoraleStateData[] moraleStateData;

        #endregion

        // Damage Calculation
        #region

        public DamageResult GetFinalDamageValueAfterAllCalculations(HexCharacterModel character, HexCharacterModel target, AbilityData ability, AbilityEffect effect, bool didCrit)
        {
            // Entry point for abilities not using a weapon
            return ExecuteGetFinalDamageValueAfterAllCalculations(character, target, 0, ability, effect, null,
                GetFinalFinalDamageTypeOfAbility(character, effect, ability), didCrit);

        }
        public DamageResult GetFinalDamageValueAfterAllCalculations(HexCharacterModel target, int baseDamage, DamageType damageType, bool ignoreBlock = true)
        {
            // Entry for attacker-less damage like bleeding, burning, etc
            return ExecuteGetFinalDamageValueAfterAllCalculations(null, target, baseDamage, null, null, null, damageType, false, ignoreBlock);

        }
        private DamageResult ExecuteGetFinalDamageValueAfterAllCalculations(HexCharacterModel attacker, HexCharacterModel target, int baseDamage, AbilityData ability, AbilityEffect effect, ItemData weaponUsed, DamageType damageType, bool didCrit = false, bool ignoreBlock = false)
        {
            int baseDamageFinal = baseDamage;
            int lowerDamageFinal = baseDamage;
            int upperDamageFinal = baseDamage;

            if (effect != null)
            {
                lowerDamageFinal = effect.minBaseDamage;
                upperDamageFinal = effect.maxBaseDamage;
            }

            float damageModPercentageAdditive = 1f;
            DamageResult resultReturned = new DamageResult();

            // Calculate base damage
            if (effect != null)
            {
                baseDamageFinal = RandomGenerator.NumberBetween(effect.minBaseDamage, effect.maxBaseDamage);
                Debug.Log("ExecuteGetFinalDamageValueAfterAllCalculations() Base damage drawn from ABILITY, roll result between " + effect.minBaseDamage + " and " + effect.maxBaseDamage +
                    " = " + baseDamageFinal);
            }

            // Calculate might, magic and physical damage modifiers
            if (effect != null)
            {
                damageModPercentageAdditive += StatCalculator.GetTotalMight(attacker) * 0.01f;
            }

            if (effect != null && damageType == DamageType.Physical)
            {
                damageModPercentageAdditive += StatCalculator.GetTotalPhysicalDamageBonus(attacker) * 0.01f;
            }

            else if (effect != null && damageType == DamageType.Magic)
            {
                damageModPercentageAdditive += StatCalculator.GetTotalMagicDamageBonus(attacker) * 0.01f;
            }

            if (effect != null && weaponUsed != null && ability != null && ability.abilityType.Contains(AbilityType.WeaponAttack))
            {
                damageModPercentageAdditive += StatCalculator.GetTotalWeaponDamageBonus(attacker) * 0.01f;
            }

            // Add critical modifier to damage mod
            if (didCrit && attacker != null && effect != null)
            {
                damageModPercentageAdditive += StatCalculator.GetTotalCriticalModifier(attacker, effect) / 100f;
                Debug.Log("ExecuteGetFinalDamageValueAfterAllCalculations() Additive damage modifier after adding in critical strike modifier = " + damageModPercentageAdditive);
            }

            // Check 'bully' bonus
            if (attacker != null &&
                PerkController.Instance.DoesCharacterHavePerk(attacker.pManager, Perk.Bully) &&
                target != null &&
                (PerkController.Instance.DoesCharacterHavePerk(target.pManager, Perk.Stunned) ||
                    PerkController.Instance.DoesCharacterHavePerk(target.pManager, Perk.Blinded) ||
                    PerkController.Instance.IsCharacteInjured(target.pManager)))
            {
                damageModPercentageAdditive += 0.20f;
                Debug.Log("ExecuteGetFinalDamageValueAfterAllCalculations() Additive damage modifier after adding in Bully perk modifier = " + damageModPercentageAdditive);
            }

            // Check 'Berserk' bonus
            if (attacker != null &&
                PerkController.Instance.DoesCharacterHavePerk(attacker.pManager, Perk.Berserk) &&
                target != null)
            {
                float berserkMod = StatCalculator.GetCurrentHealthAsPercentageOfMaxHealth(attacker) * 0.01f;
                damageModPercentageAdditive += berserkMod;
                Debug.Log("ExecuteGetFinalDamageValueAfterAllCalculations() Additive damage modifier after adding in Berserk perk modifier = " + damageModPercentageAdditive);
            }

            // Check Locomotion bonus
            if (attacker != null &&
                PerkController.Instance.DoesCharacterHavePerk(attacker.pManager, Perk.Locomotion) &&
                target != null)
            {
                float init = StatCalculator.GetTotalInitiative(attacker);
                damageModPercentageAdditive += 0.01f * init;
                Debug.Log("ExecuteGetFinalDamageValueAfterAllCalculations() Additive damage modifier after adding in Locomotion perk modifier = " + damageModPercentageAdditive);
            }

            // Calculate core additive multipliers
            // Lover's Scorn
            if (effect != null && attacker != null && PerkController.Instance.DoesCharacterHavePerk(attacker.pManager, Perk.LoversScorn))
            {
                damageModPercentageAdditive += 0.20f;
                Debug.Log("ExecuteGetFinalDamageValueAfterAllCalculations() Additive damage modifier after adding in Lovers Rage modifier = " + damageModPercentageAdditive);
            }
            // Wrath
            if (effect != null && attacker != null && PerkController.Instance.DoesCharacterHavePerk(attacker.pManager, Perk.Wrath))
            {
                damageModPercentageAdditive += 0.3f;
                Debug.Log("ExecuteGetFinalDamageValueAfterAllCalculations() Additive damage modifier after adding in Wrath modifier = " + damageModPercentageAdditive);
            }

            // Weakened
            if (effect != null && attacker != null && PerkController.Instance.DoesCharacterHavePerk(attacker.pManager, Perk.Weakened))
            {
                damageModPercentageAdditive -= 0.3f;
                Debug.Log("ExecuteGetFinalDamageValueAfterAllCalculations() Additive damage modifier after adding in Weakened modifier = " + damageModPercentageAdditive);
            }

            // Vulnerable
            if (target != null && effect != null && attacker != null && PerkController.Instance.DoesCharacterHavePerk(target.pManager, Perk.Vulnerable))
            {
                damageModPercentageAdditive += 0.3f;
                Debug.Log("ExecuteGetFinalDamageValueAfterAllCalculations() Additive damage modifier after adding in Vulnerable modifier = " + damageModPercentageAdditive);
            }

            // Block
            if (target != null && effect != null && attacker != null && PerkController.Instance.DoesCharacterHavePerk(target.pManager, Perk.Guard) && effect.ignoresGuard == false)
            {
                damageModPercentageAdditive -= 0.3f;
                Debug.Log("ExecuteGetFinalDamageValueAfterAllCalculations() Additive damage modifier after adding in Block modifier = " + damageModPercentageAdditive);
            }

            // Stoneskin
            if (target != null && effect != null && attacker != null && PerkController.Instance.DoesCharacterHavePerk(target.pManager, Perk.StoneSkin))
            {
                damageModPercentageAdditive -= 0.2f;
                Debug.Log("ExecuteGetFinalDamageValueAfterAllCalculations() Additive damage modifier after adding in Stone Skin modifier = " + damageModPercentageAdditive);
            }

            // Turtle Aspect
            if (target != null && PerkController.Instance.DoesCharacterHavePerk(target.pManager, Perk.TurtleAspect))
            {
                damageModPercentageAdditive -= 0.15f;
                Debug.Log("ExecuteGetFinalDamageValueAfterAllCalculations() Additive damage modifier after adding in Turtle Aspect modifier = " + damageModPercentageAdditive);
            }

            // Calculate talent additive multipliers
            // Pyromania
            if (effect != null && attacker != null && target != null &&
                CharacterDataController.Instance.DoesCharacterHaveTalent(attacker.talentPairings, TalentSchool.Pyromania, 1) &&
                PerkController.Instance.DoesCharacterHavePerk(target.pManager, Perk.Burning))
            {
                damageModPercentageAdditive += CharacterDataController.Instance.GetCharacterTalentLevel(attacker.talentPairings, TalentSchool.Pyromania) * 0.1f;
                Debug.Log("ExecuteGetFinalDamageValueAfterAllCalculations() Additive damage modifier after adding in pyromania modifier = " + damageModPercentageAdditive);
            }

            // Check damage mod effect from ability effect
            if (effect != null)
            {
                foreach (DamageEffectModifier dMod in effect.damageEffectModifiers)
                {
                    // Health missing self
                    if (dMod.type == DamageEffectModifierType.AddHealthMissingOnSelfToDamage && attacker != null && target != null)
                    {
                        // Get total max health missing percentage
                        float missingHealthDamageMod = (100 - StatCalculator.GetCurrentHealthAsPercentageOfMaxHealth(attacker)) * dMod.bonusDamageModifier;
                        damageModPercentageAdditive += missingHealthDamageMod;
                        Debug.Log("ExecuteGetFinalDamageValueAfterAllCalculations() Additive damage modifier after adding in health missing self modifier = " + damageModPercentageAdditive);
                    }

                    // Health missing target
                    if (dMod.type == DamageEffectModifierType.AddHealthMissingOnTargetToDamage && attacker != null && target != null)
                    {
                        // Get total max health missing percentage
                        float missingHealthDamageMod = (100 - StatCalculator.GetCurrentHealthAsPercentageOfMaxHealth(target)) * dMod.bonusDamageModifier;
                        damageModPercentageAdditive += missingHealthDamageMod;
                        Debug.Log("ExecuteGetFinalDamageValueAfterAllCalculations() Additive damage modifier after adding in health missing target modifier = " + damageModPercentageAdditive);
                    }

                    // Physical resistance added to damage
                    if (dMod.type == DamageEffectModifierType.AddPhysicalResistanceToDamage && attacker != null)
                    {
                        float physicalResistance = StatCalculator.GetTotalPhysicalResistance(attacker);
                        damageModPercentageAdditive += physicalResistance * 0.01f;
                        Debug.Log("ExecuteGetFinalDamageValueAfterAllCalculations() Additive damage modifier after adding 'physical resistance added to damage' modifier = " + damageModPercentageAdditive);
                    }

                    // Caster perks added to damage
                    if (dMod.type == DamageEffectModifierType.ExtraDamageIfCasterHasSpecificPerk && attacker != null)
                    {
                        float perkStacks = PerkController.Instance.GetStackCountOfPerkOnCharacter(attacker.pManager, dMod.perk);
                        damageModPercentageAdditive += dMod.bonusDamageModifier * perkStacks;
                        Debug.Log("ExecuteGetFinalDamageValueAfterAllCalculations() Additive damage modifier after adding 'ExtraDamageIfCasterHasSpecificPerk' modifier = " + damageModPercentageAdditive);
                    }

                    // Target perks added to damage
                    if (dMod.type == DamageEffectModifierType.ExtraDamageIfTargetHasSpecificPerk && target != null && attacker != null)
                    {
                        float perkStacks = PerkController.Instance.GetStackCountOfPerkOnCharacter(target.pManager, dMod.perk);
                        damageModPercentageAdditive += dMod.bonusDamageModifier * perkStacks;
                        Debug.Log("ExecuteGetFinalDamageValueAfterAllCalculations() Additive damage modifier after adding 'ExtraDamageIfTargetHasSpecificPerk' modifier = " + damageModPercentageAdditive);
                    }

                    // Caster perks added to damage
                    if (dMod.type == DamageEffectModifierType.ExtraCriticalDamage && attacker != null && didCrit)
                    {
                        damageModPercentageAdditive += dMod.extraCriticalDamage;
                        Debug.Log("ExecuteGetFinalDamageValueAfterAllCalculations() Additive damage modifier after adding 'ExtraCriticalDamage' modifier = " + damageModPercentageAdditive);

                    }
                }
            }

            // Physical + Magical Resistance
            if (target != null && damageType == DamageType.Physical)
            {
                damageModPercentageAdditive -= StatCalculator.GetTotalPhysicalResistance(target) * 0.01f;
            }

            else if (target != null && damageType == DamageType.Magic)
            {
                damageModPercentageAdditive -= StatCalculator.GetTotalMagicResistance(target) * 0.01f;
            }

            // Apply additive damage modifier to base damage (+min and max limits for ability description panels)
            if (damageModPercentageAdditive < 0f)
            {
                damageModPercentageAdditive = 0f;
            }
            baseDamageFinal = (int) (baseDamageFinal * damageModPercentageAdditive);
            lowerDamageFinal = (int) (lowerDamageFinal * damageModPercentageAdditive);
            upperDamageFinal = (int) (upperDamageFinal * damageModPercentageAdditive);

            Debug.Log("ExecuteGetFinalDamageValueAfterAllCalculations() Base damage AFTER applying final multiplicative modifiers + resistance: " + baseDamageFinal);

            // Metamorph talent passive
            if (target != null &&
                CharacterDataController.Instance.DoesCharacterHaveTalent(target.talentPairings, TalentSchool.Metamorph, 1))
            {
                int reduction = CharacterDataController.Instance.GetCharacterTalentLevel(target.talentPairings, TalentSchool.Metamorph);
                baseDamageFinal -= reduction;
                lowerDamageFinal -= reduction;
                upperDamageFinal -= reduction;
            }

            // Prevent damage gong negative
            if (baseDamageFinal < 0)
            {
                baseDamageFinal = 0;
            }
            if (lowerDamageFinal < 0)
            {
                lowerDamageFinal = 0;
            }
            if (upperDamageFinal < 0)
            {
                upperDamageFinal = 0;
            }

            resultReturned.totalDamage = baseDamageFinal;
            resultReturned.damageLowerLimit = lowerDamageFinal;
            resultReturned.damageUpperLimit = upperDamageFinal;

            Debug.Log("ExecuteGetFinalDamageValueAfterAllCalculations() Final damage = " + resultReturned.totalDamage);

            return resultReturned;
        }

        #endregion

        // Stress Events Logic
        #region

        public void CreateMoraleCheck(HexCharacterModel character, StressEventType eventType, bool allowRecursiveChecks = true)
        {
            // Non player characters dont use the stress mechanic
            if (character.characterData.ignoreStress || PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Fearless))
            {
                return;
            }

            Debug.Log("CombatController.CreateMoraleCheck() called, character = " + character.myName + ", type = " + TextLogic.SplitByCapitals(eventType.ToString()));
            StressEventSO data = null;

            // Generate a roll
            int roll = RandomGenerator.NumberBetween(1, 100);

            // Get data
            foreach (StressEventSO d in allStressEventData)
            {
                if (d.Type == eventType)
                {
                    data = d;
                    break;
                }
            }

            // Check data was actually found
            if (data == null)
            {
                return;
            }

            // Calculate total resolve + roll for 'Irrational' perk.
            int characterStressResistance = StatCalculator.GetTotalBravery(character);
            if (data.NegativeEvent && PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Irrational))
            {
                int irrationalRoll = RandomGenerator.NumberBetween(1, 2);
                if (irrationalRoll == 1)
                {
                    characterStressResistance -= 15;
                }
                else
                {
                    characterStressResistance += 15;
                }
            }

            // Determine roll required to pass the stress check
            int requiredRoll = 0;
            if (data.NegativeEvent)
            {
                requiredRoll = Mathf.Clamp(data.SuccessChance - characterStressResistance, 5, 95);
            }
            else
            {
                requiredRoll = Mathf.Clamp(data.SuccessChance + characterStressResistance, 5, 95);
            }

            Debug.Log("CreateStressCheck() Stress event stats: Roll = " + roll + ", Stress Resistance: " + characterStressResistance +
                ", Required roll to pass: " + requiredRoll);

            // Negative event roll failure + positive event roll success
            if (roll <= requiredRoll)
            {
                Debug.Log("Character rolled below the required roll threshold, applying effects of stress event...");
                int finalStateChangeAmount = RandomGenerator.NumberBetween(data.MoraleChangeMin, data.MoraleChangeMax);
                HexCharacterController.Instance.ModifyMoraleState(character, finalStateChangeAmount, true, true, allowRecursiveChecks);
            }

        }
        public void CreateMoraleCheck(HexCharacterModel character, StressEventData data, bool showVFX)
        {
            Debug.Log("CombatController.CreateMoraleCheck() called, character = " + character.myName);

            // Non player characters dont use the stress mechanic
            if (character.characterData.ignoreStress || PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Fearless))
            {
                return;
            }

            // Generate a roll
            int roll = RandomGenerator.NumberBetween(1, 100);

            // Calculate total resolve + roll for 'Irrational' perk.
            int characterStressResistance = StatCalculator.GetTotalBravery(character);
            if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Irrational))
            {
                int irrationalRoll = RandomGenerator.NumberBetween(1, 2);
                if (irrationalRoll == 1)
                {
                    characterStressResistance -= 15;
                }
                else
                {
                    characterStressResistance += 15;
                }
            }

            // Determine roll required to pass the stress check
            int requiredRoll = Mathf.Clamp(data.successChance - characterStressResistance, 5, 95);

            Debug.Log("CreateStressCheck() Stress event stats: Roll = " + roll + ", Stress Resistance: " + characterStressResistance +
                ", Required roll to pass: " + requiredRoll);

            // negative event roll failure + positice event roll success
            if (roll <= requiredRoll)
            {
                Debug.Log("Character rolled below the required roll threshold, applying effects of stress event...");
                int finalStressAmount = RandomGenerator.NumberBetween(data.moraleChangeMin, data.moraleChangeMax);
                HexCharacterController.Instance.ModifyMoraleState(character, finalStressAmount, true, showVFX);
            }

        }
        public int GetStatMultiplierFromStressState(MoraleState stressState, HexCharacterModel character)
        {
            int multiplier = 0;
            // Enemies dont interact with stress system
            if (character.characterData.ignoreStress || PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Fearless))
            {
                Debug.Log(string.Format("GetStatMultiplierFromStressState() character {0} does not benefit/suffer from stress state, returning 0...", character.myName));
                return 0;
            }

            if (stressState == MoraleState.Confident && !CharacterDataController.Instance.DoesCharacterHaveBackground(character.background, CharacterBackground.Slave))
            {
                multiplier = 5;
            }
            else if (stressState == MoraleState.Steady)
            {
                multiplier = 0;
            }
            else if (stressState == MoraleState.Nervous)
            {
                multiplier = -5;
            }
            else if (stressState == MoraleState.Panicking)
            {
                multiplier = -10;
            }
            else if (stressState == MoraleState.Shattered)
            {
                multiplier = -15;
            }
            return multiplier;
        }
        public int[] GetStressStateRanges(MoraleState state)
        {
            if (state == MoraleState.Confident)
            {
                return new int[2]
                {
                    0, 4
                };
            }
            if (state == MoraleState.Steady)
            {
                return new int[2]
                {
                    5, 9
                };
            }
            if (state == MoraleState.Nervous)
            {
                return new int[2]
                {
                    10, 14
                };
            }
            if (state == MoraleState.Panicking)
            {
                return new int[2]
                {
                    15, 19
                };
            }
            if (state == MoraleState.Shattered)
            {
                return new int[2]
                {
                    20, 20
                };
            }
            return new int[2]
            {
                0, 0
            };
        }

        #endregion

        // Rolls + Critical Logic
        #region

        public HitChanceDataSet GetHitChance(HexCharacterModel attacker, HexCharacterModel target, AbilityData ability = null, ItemData weaponUsed = null)
        {
            HitChanceDataSet ret = new HitChanceDataSet();

            // Check turtle aspect => unable to dodge attacks
            if (PerkController.Instance.DoesCharacterHavePerk(target.pManager, Perk.TurtleAspect))
            {
                ret.details.Add(new HitChanceDetailData("Turtle Aspect", 100));
                ret.guaranteedHit = true;
                return ret;
            }

            // Calculate stress state mod
            MoraleState attackerStressState = attacker.currentMoraleState;
            int attackerStressMod = GetStatMultiplierFromStressState(attackerStressState, attacker);
            MoraleState targetStressState = target.currentMoraleState;
            int targetStressMod = GetStatMultiplierFromStressState(targetStressState, target);

            // Warfare talent bonus
            int warfareBonus = 0;
            if (ability != null &&
                ability.abilityType.Contains(AbilityType.MeleeAttack) &&
                CharacterDataController.Instance.DoesCharacterHaveTalent(attacker.talentPairings, TalentSchool.Warfare, 1))
            {
                warfareBonus = CharacterDataController.Instance.GetCharacterTalentLevel(attacker.talentPairings, TalentSchool.Warfare) * 5;
            }

            // ranger talent bonus
            int rangerBonus = 0;
            if (ability != null &&
                ability.abilityType.Contains(AbilityType.RangedAttack) &&
                CharacterDataController.Instance.DoesCharacterHaveTalent(attacker.talentPairings, TalentSchool.Ranger, 1))
            {
                rangerBonus = CharacterDataController.Instance.GetCharacterTalentLevel(attacker.talentPairings, TalentSchool.Ranger) * 5;
            }

            // Assassin background bonus
            int assassinBonus = 0;
            if (ability != null &&
                CharacterDataController.Instance.DoesCharacterHaveBackground(attacker.background, CharacterBackground.Assassin) &&
                StatCalculator.GetCurrentHealthAsPercentageOfMaxHealth(target) < 50)
            {
                assassinBonus = 5;
            }

            // Lumberjack background bonus
            int lumberjackBonus = 0;
            if (ability != null &&
                ability.abilityType.Contains(AbilityType.WeaponAttack) &&
                CharacterDataController.Instance.DoesCharacterHaveBackground(attacker.background, CharacterBackground.Lumberjack) &&
                ItemController.Instance.IsCharacterUsingWeaponClass(attacker.itemSet, WeaponClass.Axe)
               )
            {
                lumberjackBonus = 5;
            }

            // Poacher background bonus
            int poacherBonus = 0;
            if (ability != null &&
                ability.abilityType.Contains(AbilityType.WeaponAttack) &&
                CharacterDataController.Instance.DoesCharacterHaveBackground(attacker.background, CharacterBackground.Poacher) &&
                ItemController.Instance.IsCharacterUsingWeaponClass(attacker.itemSet, WeaponClass.Bow)
               )
            {
                poacherBonus = 5;
            }

            // Brawny
            int onehandedFinesseBonus = 0;
            if (ability != null &&
                ability.abilityType.Contains(AbilityType.WeaponAttack) &&
                PerkController.Instance.DoesCharacterHavePerk(attacker.pManager, Perk.OneHandedExpertise))
            {
                onehandedFinesseBonus = 10;
            }

            // Dead Eye
            int deadEyeBonus = 0;
            if (ability != null &&
                ability.abilityType.Contains(AbilityType.RangedAttack) &&
                PerkController.Instance.DoesCharacterHavePerk(attacker.pManager, Perk.DeadEye))
            {
                deadEyeBonus = 5;
            }

            // Brawny
            int brawnyBonus = 0;
            if (ability != null &&
                ability.abilityType.Contains(AbilityType.MeleeAttack) &&
                PerkController.Instance.DoesCharacterHavePerk(attacker.pManager, Perk.Brawny))
            {
                brawnyBonus = 5;
            }

            // Base hit chance
            int baseHitMod = GlobalSettings.Instance.BaseHitChance;
            if (baseHitMod != 0)
            {
                ret.details.Add(new HitChanceDetailData("Base hit chance", baseHitMod));
            }

            // Attacker Accuracy
            int accuracyMod = StatCalculator.GetTotalAccuracy(attacker) - attackerStressMod;
            accuracyMod += brawnyBonus;
            accuracyMod += deadEyeBonus;
            accuracyMod += poacherBonus;
            accuracyMod += lumberjackBonus;
            accuracyMod += assassinBonus;
            accuracyMod += rangerBonus;
            accuracyMod += warfareBonus;
            accuracyMod += onehandedFinesseBonus;

            if (accuracyMod != 0)
            {
                ret.details.Add(new HitChanceDetailData("Attacker accuracy", accuracyMod));
            }

            // Target dual wield finesse
            int dualWieldMod = 0;
            if (ability != null && ability.abilityType.Contains(AbilityType.MeleeAttack) &&
                target.itemSet.IsDualWieldingMeleeWeapons() &&
                PerkController.Instance.DoesCharacterHavePerk(target.pManager, Perk.DualWieldFinesse))
            {
                dualWieldMod = 10;
            }

            // Target Dodge
            int dodgeMod = -(StatCalculator.GetTotalDodge(target) + dualWieldMod - targetStressMod);
            if (dodgeMod != 0)
            {
                ret.details.Add(new HitChanceDetailData("Target dodge", dodgeMod));
            }

            // Stress State            
            if (attackerStressMod != 0)
            {
                ret.details.Add(new HitChanceDetailData("Attacker " + attackerStressState, attackerStressMod));
            }
            if (targetStressMod != 0)
            {
                ret.details.Add(new HitChanceDetailData("Target " + targetStressState, -targetStressMod));
            }

            // Strong Starter perk
            if (TurnController.Instance.CurrentTurn == 1 &&
                (ability.abilityType.Contains(AbilityType.WeaponAttack) || ability.abilityType.Contains(AbilityType.RangedAttack) || ability.abilityType.Contains(AbilityType.MeleeAttack)) &&
                attacker.weaponAbilitiesUsedThisTurn == 0 &&
                attacker.rangedAttackAbilitiesUsedThisTurn == 0 &&
                attacker.meleeAttackAbilitiesUsedThisTurn == 0 &&
                PerkController.Instance.DoesCharacterHavePerk(attacker.pManager, Perk.StrongStarter))
            {
                ret.details.Add(new HitChanceDetailData("Strong Starter", 35));
            }

            // Melee modifiers
            if (ability != null && ability.abilityType.Contains(AbilityType.MeleeAttack))
            {
                // Check back strike bonus
                int backStrikeMod = HexCharacterController.Instance.CalculateBackStrikeHitChanceModifier(attacker, target);
                if (backStrikeMod != 0)
                {
                    ret.details.Add(new HitChanceDetailData("Backstrike bonus", backStrikeMod));
                }

                // Check flanking bonus
                int flankingMod = HexCharacterController.Instance.CalculateFlankingHitChanceModifier(attacker, target);
                if (flankingMod != 0)
                {
                    ret.details.Add(new HitChanceDetailData("Flanking bonus", flankingMod));
                }
            }

            // Check elevation bonus
            int elevationMod = HexCharacterController.Instance.CalculateElevationAccuracyModifier(attacker, target);
            if (elevationMod != 0)
            {
                if (elevationMod < 0)
                {
                    ret.details.Add(new HitChanceDetailData("Elevation penalty", elevationMod));
                }
                else if (elevationMod > 0)
                {
                    ret.details.Add(new HitChanceDetailData("Elevation bonus", elevationMod));
                }
            }

            // Check shooting at engaged target or shooting from melee
            if (ability != null &&
                ability.abilityType.Contains(AbilityType.RangedAttack) &&
                !PerkController.Instance.DoesCharacterHavePerk(target.pManager, Perk.MeticulousAim) &&
                ability.accuracyPenaltyFromMelee)
            {
                if (HexCharacterController.Instance.IsCharacterEngagedInMelee(attacker))
                {
                    ret.details.Add(new HitChanceDetailData("Shooting from melee", -10));
                }

                if (HexCharacterController.Instance.IsCharacterEngagedInMelee(target))
                {
                    ret.details.Add(new HitChanceDetailData("Shooting into melee", -10));
                }
            }

            // Check ability innate hit bonus
            if (ability != null)
            {
                int innateBonus = ability.hitChanceModifier;
                string bOrP = innateBonus > 0 ? "bonus" : "penalty";
                if (innateBonus != 0)
                {
                    ret.details.Add(new HitChanceDetailData("Ability " + bOrP, innateBonus));
                }
            }

            // Check ability + weapon adjacent bonus/penalty
            if (ability != null && attacker.currentTile.Distance(target.currentTile) == 1)
            {
                int innateBonus = ability.hitChanceModifierAgainstAdjacent;

                // Check weapon innate accuracy bonus/penalty against adjacent
                if (weaponUsed != null && ability.abilityType.Contains(AbilityType.WeaponAttack))
                {
                    Debug.Log("Innate weapon accuracy against adjacent!");
                    innateBonus += ItemController.Instance.GetInnateModifierFromWeapon(InnateItemEffectType.InnateAccuracyAgainstAdjacentModifier, weaponUsed);
                }

                string bOrP = innateBonus > 0 ? "Bonus" : "Penalty";
                if (innateBonus != 0)
                {
                    ret.details.Add(new HitChanceDetailData("Adjacent target " + bOrP, innateBonus));
                }
            }

            // Check weapon innate accuracy bonus/penalty
            if (weaponUsed != null && ability != null && ability.abilityType.Contains(AbilityType.WeaponAttack))
            {
                Debug.Log("Innate weapon accuracy!");
                int innateBonus = ItemController.Instance.GetInnateModifierFromWeapon(InnateItemEffectType.InnateAccuracyModifier, weaponUsed);
                string bOrP = innateBonus > 0 ? "Bonus" : "Penalty";
                if (innateBonus != 0)
                {
                    ret.details.Add(new HitChanceDetailData("Weapon " + bOrP, innateBonus));
                }
            }

            // Ranged attack distance penalty
            if (!PerkController.Instance.DoesCharacterHavePerk(attacker.pManager, Perk.MeticulousAim) &&
                ability != null && ability.abilityType.Contains(AbilityType.RangedAttack))
            {
                int distanceMod = -(attacker.currentTile.Distance(target.currentTile) - 1) * 5;
                if (distanceMod != 0)
                {
                    ret.details.Add(new HitChanceDetailData("Distance penalty", distanceMod));
                }
            }

            // Check guaranteed hit effect for abilities
            if (ability != null && ability.abilityEffects[0] != null &&
                ability.abilityEffects[0].guaranteedHit)
            {
                ret.guaranteedHit = true;
            }

            return ret;
        }
        public HitRoll RollForHit(HexCharacterModel attacker, HexCharacterModel target, AbilityData ability = null, ItemData weaponUsed = null)
        {
            HitRollResult result;
            int hitRoll = RandomGenerator.NumberBetween(1, 100);
            HitChanceDataSet hitData = GetHitChance(attacker, target, ability, weaponUsed);
            int requiredRoll = hitData.FinalHitChance;
            if (hitData.guaranteedHit)
            {
                requiredRoll = 100;
            }

            if (hitRoll <= requiredRoll)
            {
                result = HitRollResult.Hit;
            }
            else
            {
                result = HitRollResult.Miss;
            }

            HitRoll rollReturned = new HitRoll(hitRoll, requiredRoll, result, attacker, target);

            return rollReturned;
        }
        public bool RollForCrit(HexCharacterModel attacker, HexCharacterModel target, AbilityData ability = null, AbilityEffect effect = null)
        {
            bool didCrit = false;
            int critRoll = RandomGenerator.NumberBetween(1, 1000);
            float critChance = StatCalculator.GetTotalCriticalChance(attacker);

            // Check combo
            if (PerkController.Instance.DoesCharacterHavePerk(attacker.pManager, Perk.Combo) &&
                ability != null)
            {
                return true;
            }

            // Check Point Blank bonus            
            if (ability != null &&
                ability.abilityType.Contains(AbilityType.RangedAttack) &&
                PerkController.Instance.DoesCharacterHavePerk(attacker.pManager, Perk.PointBlank) &&
                LevelController.Instance.GetAllHexsWithinRange(attacker.currentTile, 1).Contains(target.currentTile))
            {
                critChance += 25;
            }

            // Check Strong Starter perk
            if (TurnController.Instance.CurrentTurn == 1 &&
                (ability.abilityType.Contains(AbilityType.WeaponAttack) || ability.abilityType.Contains(AbilityType.RangedAttack) || ability.abilityType.Contains(AbilityType.MeleeAttack)) &&
                attacker.weaponAbilitiesUsedThisTurn == 0 &&
                attacker.rangedAttackAbilitiesUsedThisTurn == 0 &&
                attacker.meleeAttackAbilitiesUsedThisTurn == 0 &&
                PerkController.Instance.DoesCharacterHavePerk(attacker.pManager, Perk.StrongStarter))
            {
                critChance += 35;
            }

            // Check Assssin background
            if (ability != null &&
                StatCalculator.GetCurrentHealthAsPercentageOfMaxHealth(target) < 50 &&
                CharacterDataController.Instance.DoesCharacterHaveBackground(attacker.background, CharacterBackground.Assassin))
            {
                critChance += 5;
            }

            // Check Opportunist
            if (ability != null &&
                PerkController.Instance.DoesCharacterHavePerk(attacker.pManager, Perk.Opportunist) &&
                (HexCharacterController.Instance.GetCharacterBackArcTiles(target).Contains(attacker.currentTile) ||
                    HexCharacterController.Instance.IsCharacterFlanked(target)))
            {
                critChance += 10;
            }

            // Check Dead Eye
            if (PerkController.Instance.DoesCharacterHavePerk(attacker.pManager, Perk.DeadEye) &&
                ability != null &&
                ability.abilityType.Contains(AbilityType.RangedAttack))
            {
                critChance += 5;
            }

            // Check Brawny
            else if (PerkController.Instance.DoesCharacterHavePerk(attacker.pManager, Perk.Brawny) &&
                     ability != null &&
                     ability.abilityType.Contains(AbilityType.MeleeAttack))
            {
                critChance += 5;
            }

            // Check Tiger Aspect
            else if (PerkController.Instance.DoesCharacterHavePerk(attacker.pManager, Perk.TigerAspect) &&
                     ability != null &&
                     ability.abilityType.Contains(AbilityType.MeleeAttack))
            {
                critChance += 15;
            }

            if (effect != null)
            {
                foreach (DamageEffectModifier d in effect.damageEffectModifiers)
                {
                    // target has specific perk
                    if (d.type == DamageEffectModifierType.ExtraCriticalChanceIfTargetHasSpecificPerk &&
                        target != null &&
                        PerkController.Instance.DoesCharacterHavePerk(target.pManager, d.perk))
                    {
                        critChance += d.bonusCriticalChance;
                    }

                    // target is of specific race
                    else if (d.type == DamageEffectModifierType.ExtraCriticalChanceAgainstRace &&
                             target != null &&
                             target.race == d.targetRace)
                    {
                        critChance += d.bonusCriticalChance;
                    }
                }
            }

            critChance *= 10f;
            Debug.Log("RollForCrit() critical chance = " + critChance / 10f + "%. Roll result from 1-1000: " + critRoll);

            if (critRoll <= critChance)
            {
                didCrit = true;
            }

            return didCrit;
        }
        public bool RollForDebuffResist(HexCharacterModel attacker, HexCharacterModel target, PerkIconData perkData = null)
        {
            Debug.Log("CombatLogic.RollForDebuffResist() called...");
            bool didResist = false;
            int totalResistance = StatCalculator.GetTotalDebuffResistance(target);

            // Check Clotter perk
            if (perkData != null && perkData.perkTag == Perk.Bleeding &&
                PerkController.Instance.DoesCharacterHavePerk(target.pManager, Perk.Clotter))
            {
                totalResistance += 50;
            }

            // Check Innoculated perk
            if (perkData != null && perkData.perkTag == Perk.Poisoned &&
                PerkController.Instance.DoesCharacterHavePerk(target.pManager, Perk.Inoculated))
            {
                totalResistance += 50;
            }

            // Check attacker has shadowcraft talent passive
            if (attacker != null &&
                CharacterDataController.Instance.DoesCharacterHaveTalent(attacker.talentPairings, TalentSchool.Shadowcraft, 1))
            {
                totalResistance -= CharacterDataController.Instance.GetCharacterTalentLevel(attacker.talentPairings, TalentSchool.Shadowcraft) * 15;
            }

            // Roll for resist
            int roll = RandomGenerator.NumberBetween(1, 100);
            if (roll <= totalResistance)
            {
                didResist = true;
            }

            Debug.Log("Target rolled " + roll + " and needed " + totalResistance + " or less. Resisted = " + didResist);
            return didResist;
        }
        public HitChanceDataSet GetDebuffChance(HexCharacterModel attacker, HexCharacterModel target, AbilityData abilityUsed, AbilityEffect effect)
        {
            HitChanceDataSet ret = new HitChanceDataSet();
            ret.clampResult = false;
            PerkIconData perkApplied = PerkController.Instance.GetPerkIconDataByTag(effect.perkPairing.perkTag);
            ret.perk = perkApplied;

            int totalResistance = StatCalculator.GetTotalDebuffResistance(target);
            int baseChance = effect.perkApplicationChance;

            // Check for rune
            if (perkApplied.runeBlocksIncrease && PerkController.Instance.DoesCharacterHavePerk(target.pManager, Perk.Rune))
            {
                ret.details.Add(new HitChanceDetailData("Blocked by Rune", -100, true));
                return ret;
            }

            // Check for immunity from 'perksThatBlockThis'
            foreach (Perk blocker in perkApplied.perksThatBlockThis)
            {
                if (PerkController.Instance.DoesCharacterHavePerk(target.pManager, blocker))
                {
                    ret.details.Add(new HitChanceDetailData("Blocked by " + TextLogic.SplitByCapitals(blocker.ToString()), -100, true));
                    return ret;
                }
            }

            // Check for racial immunity
            if (perkApplied.racesThatBlockThis.Contains(target.race))
            {
                ret.details.Add(new HitChanceDetailData("Racial Immunity", -100, true));
                return ret;
            }

            // Check Clotter perk
            if (perkApplied.perkTag == Perk.Bleeding &&
                PerkController.Instance.DoesCharacterHavePerk(target.pManager, Perk.Clotter))
            {
                ret.details.Add(new HitChanceDetailData("Target Clotter", -50));
            }

            // Check Innoculated perk
            if (perkApplied.perkTag == Perk.Poisoned &&
                PerkController.Instance.DoesCharacterHavePerk(target.pManager, Perk.Inoculated))
            {
                ret.details.Add(new HitChanceDetailData("Target Inoculated", -50));
            }

            // Check for shadowcraft passive
            if (CharacterDataController.Instance.DoesCharacterHaveTalent(attacker.talentPairings, TalentSchool.Shadowcraft, 1))
            {
                ret.details.Add(new HitChanceDetailData("Shadowcraft bonus", 15));
            }

            // Check base application chance from ability
            ret.details.Add(new HitChanceDetailData("Base chance", baseChance));

            // check target debuff resistance
            ret.details.Add(new HitChanceDetailData("Target Debuff Resistance", -totalResistance));

            return ret;
        }

        #endregion

        // Handle Damage + Entry Points
        #region

        public void HandleDamage(HexCharacterModel attacker, HexCharacterModel target, DamageResult damageResult, AbilityData ability, AbilityEffect effect, ItemData weaponUsed, bool ignoreArmour = false, VisualEvent parentEvent = null)
        {
            // Normal ability damage entry point
            ExecuteHandleDamage(attacker, target, damageResult, ability, effect, weaponUsed, ignoreArmour, parentEvent);
        }
        public void HandleDamage(HexCharacterModel target, DamageResult damageResult, DamageType damageType, bool ignoreArmour)
        {
            // Non ability and attacker damage source entry point
            ExecuteHandleDamage(null, target, damageResult, null, null, null, ignoreArmour);
        }
        private void ExecuteHandleDamage(HexCharacterModel attacker, HexCharacterModel target, DamageResult damageResult, AbilityData ability = null, AbilityEffect effect = null, ItemData weaponUsed = null, bool ignoreArmour = false, VisualEvent parentEvent = null)
        {
            // Final health and armour loss calculations
            int totalDamage = damageResult.totalDamage;
            int totalHealthLost = 0;
            int totalArmourLost = 0;

            float healthDamageMod = 1f;
            float armourDamageMod = 1f;
            float penetrationMod = 0.1f;
            int finalPenetration = 0;

            if (ability != null)
            {
                if (weaponUsed != null && ability.abilityType.Contains(AbilityType.WeaponAttack))
                {
                    healthDamageMod = weaponUsed.healthDamage;
                    armourDamageMod = weaponUsed.armourDamage;
                    penetrationMod = weaponUsed.armourPenetration;

                    // Check innate weapon effect: Backstab Penetration
                    if (attacker != null && HexCharacterController.Instance.GetCharacterBackArcTiles(target).Contains(attacker.currentTile))
                    {
                        penetrationMod += ItemController.Instance.GetInnateModifierFromWeapon(InnateItemEffectType.PenetrationBonusOnBackstab, weaponUsed) * 0.01f;
                    }

                    // Check two handed dominance perk: bonus penetration
                    if (attacker != null &&
                        attacker.itemSet.IsWieldingTwoHandMeleeWeapon() &&
                        PerkController.Instance.DoesCharacterHavePerk(attacker.pManager, Perk.TwoHandedDominance))
                    {
                        penetrationMod += 0.1f;
                    }

                    // Check axe vulnerability on target
                    if (attacker != null &&
                        weaponUsed.weaponClass == WeaponClass.Axe &&
                        PerkController.Instance.DoesCharacterHavePerk(target.pManager, Perk.AxeVulnerability))
                    {
                        healthDamageMod += 0.5f;
                    }
                }

                // Check Point Blank bonus            
                if (ability.abilityType.Contains(AbilityType.RangedAttack) &&
                    PerkController.Instance.DoesCharacterHavePerk(attacker.pManager, Perk.PointBlank) &&
                    LevelController.Instance.GetAllHexsWithinRange(attacker.currentTile, 1).Contains(target.currentTile))
                {
                    penetrationMod += 0.25f;
                }

                Debug.Log("XX ExecuteHandleDamage() Damage modifiers: " +
                    "Health damage = " + healthDamageMod * 100f + "%" +
                    ", Armour Damage = " + armourDamageMod * 100f + "%" +
                    ", Penetration = " + penetrationMod * 100f + "%");

                // Target is unarmoured, or attack ignores armour for whatever reason
                if (target.currentArmour <= 0 ||
                    ignoreArmour ||
                    effect != null && effect.ignoresArmour ||
                    attacker != null && PerkController.Instance.DoesCharacterHavePerk(attacker.pManager, Perk.HeartSeeker) &&
                    ability != null &&
                    ability.abilityType.Contains(AbilityType.WeaponAttack))
                {
                    totalHealthLost = (int) (damageResult.totalDamage * healthDamageMod);
                    totalArmourLost = 0;
                    totalDamage = totalHealthLost;

                    damageResult.totalHealthLost = totalHealthLost;
                    damageResult.totalArmourLost = 0;
                }

                // Target is armoured
                else
                {
                    // Will damage exceed and destroy armour?
                    int maxPossibleArmourDamage = (int) (totalDamage * armourDamageMod);

                    // Damage will exceed armour
                    if (maxPossibleArmourDamage > target.currentArmour)
                    {
                        Debug.Log("XX ExecuteHandleDamage() damage will exceed armour...");
                        int initialDifference = maxPossibleArmourDamage - target.currentArmour;
                        float adjustedDifference = initialDifference / armourDamageMod;
                        int overflowHealthDamage = (int) (adjustedDifference * healthDamageMod);

                        totalArmourLost = target.currentArmour;
                        totalHealthLost = overflowHealthDamage;
                        damageResult.totalHealthLost = totalHealthLost;
                        damageResult.totalArmourLost = totalArmourLost;

                        // Calculate penetration damage to health
                        float x = target.currentArmour / armourDamageMod;
                        int finalPenetrationHealthDamage = (int) (x * penetrationMod);
                        if (finalPenetrationHealthDamage < 0)
                        {
                            finalPenetrationHealthDamage = 0;
                        }

                        damageResult.totalHealthLost += finalPenetrationHealthDamage;
                        totalHealthLost += finalPenetrationHealthDamage;

                        finalPenetration = finalPenetrationHealthDamage;
                    }

                    // Damage will NOT exceed armour
                    else
                    {
                        Debug.Log("XX ExecuteHandleDamage() damage will NOT exceed armour...");
                        totalArmourLost = maxPossibleArmourDamage;
                        totalHealthLost = 0;
                        damageResult.totalHealthLost = totalHealthLost;
                        damageResult.totalArmourLost = totalArmourLost;

                        // Calculate health damage from armour penetration
                        float armourPenDamage = totalDamage * penetrationMod;
                        //float remainingArmourPenalty = (target.currentArmour - totalArmourLost) * 0.1f;
                        //int finalPenetrationHealthDamage = (int)(armourPenDamage - remainingArmourPenalty);
                        int finalPenetrationHealthDamage = (int) armourPenDamage;
                        if (finalPenetrationHealthDamage < 0)
                        {
                            finalPenetrationHealthDamage = 0;
                        }

                        damageResult.totalHealthLost += finalPenetrationHealthDamage;
                        totalHealthLost += finalPenetrationHealthDamage;

                        finalPenetration = finalPenetrationHealthDamage;
                    }

                    totalDamage = totalHealthLost + totalArmourLost;

                }
            }

            // Non ability damage ignoring armour (bleeding, poisoned, etc)
            else if (ignoreArmour)
            {
                damageResult.totalHealthLost = totalDamage;
                totalHealthLost = totalDamage;
                damageResult.totalArmourLost = 0;
                totalArmourLost = 0;
            }

            // Non abiity damage that damages armour
            else
            {
                // Damage will exceed armour
                if (totalDamage > target.currentArmour)
                {
                    damageResult.totalHealthLost = totalDamage - target.currentArmour;
                    totalHealthLost = totalDamage - target.currentArmour;

                    damageResult.totalArmourLost = target.currentArmour;
                    totalArmourLost = target.currentArmour;
                }
                else
                {
                    damageResult.totalHealthLost = 0;
                    totalHealthLost = 0;

                    damageResult.totalArmourLost = totalDamage;
                    totalArmourLost = totalDamage;
                }
            }

            // Check Sturdy Defense
            if (ability != null &&
                PerkController.Instance.DoesCharacterHavePerk(target.pManager, Perk.SturdyDefense))
            {
                damageResult.totalArmourLost = (int) (damageResult.totalArmourLost * 0.75f);
                totalArmourLost = (int) (damageResult.totalArmourLost * 0.75f);
            }

            /*
            // Check Agile Defense
            if (ability != null &&
                PerkController.Instance.DoesCharacterHavePerk(target.pManager, Perk.AgileDefense))
            {
                float adMod = 0.25f;
                float itemFatPenaltyMod = ItemController.Instance.GetTotalMaximumFatiguePenaltyFromHeadAndBodyItems(target.itemSet) * 0.01f;
                adMod = 1f - (adMod - itemFatPenaltyMod);
                damageResult.totalHealthLost = (int)(damageResult.totalHealthLost * adMod);
                totalHealthLost = (int)(damageResult.totalHealthLost * adMod);
            }*/

            Debug.Log("XX ExecuteHandleDamage() results: " +
                "Base damage = " + totalDamage +
                ", Health lost = " + totalHealthLost +
                ", Armour Lost = " + totalArmourLost +
                ", Penetration health damage = " + finalPenetration +
                ", Final total damage dealt = " + (totalHealthLost + totalArmourLost));

            bool removedBarrier = false;

            // Check for Bring It On perk
            if (target.hasTriggeredBringItOn == false &&
                (totalHealthLost > 0 || totalArmourLost > 0) &&
                PerkController.Instance.DoesCharacterHavePerk(target.pManager, Perk.BringItOn))
            {
                totalHealthLost = 0;
                totalArmourLost = 0;
                damageResult.totalHealthLost = 0;
                damageResult.totalArmourLost = 0;
                damageResult.totalDamage = 0;
                target.hasTriggeredBringItOn = true;

                // Status notification
                VisualEventManager.CreateVisualEvent(() =>
                {
                    AudioManager.Instance.PlaySound(Sound.Ability_Cheeky_Laugh);
                    VisualEffectManager.Instance.CreateStatusEffect(attacker.hexCharacterView.WorldPosition, "Bring It On!",
                        PerkController.Instance.GetPerkIconDataByTag(Perk.BringItOn).passiveSprite, StatusFrameType.CircularBrown);
                }, attacker.GetLastStackEventParent()).SetEndDelay(0.5f);
            }

            // Check for barrier
            if (PerkController.Instance.DoesCharacterHavePerk(target.pManager, Perk.Barrier) &&
                totalHealthLost > 0)
            {
                totalHealthLost = 0;
                damageResult.totalHealthLost = 0;
                removedBarrier = true;
            }

            // Combat log entry for damage result
            if (totalArmourLost > 0 || totalHealthLost > 0)
            {
                CombatLogController.Instance.CreateCharacterDamageEntry(target, totalHealthLost, totalArmourLost);
            }

            // Set crit result
            bool didCrit = false;
            if (damageResult != null && damageResult.didCrit)
            {
                didCrit = true;
            }

            // Create impact effect 
            Vector3 pos = target.hexCharacterView.WorldPosition;
            VisualEventManager.CreateVisualEvent(() =>
            {
                if (target.hexCharacterView != null)
                {
                    pos = target.hexCharacterView.WorldPosition;
                }
                VisualEffectManager.Instance.CreateSmallMeleeImpact(pos);
            }, parentEvent);

            // Create screen shake
            if (totalHealthLost > 0 && attacker != null)
            {
                if (totalHealthLost < 35)
                {
                    VisualEventManager.CreateVisualEvent(() => CameraController.Instance.CreateCameraShake(CameraShakeType.Small), parentEvent);
                }
                else if (totalHealthLost >= 35)
                {
                    VisualEventManager.CreateVisualEvent(() => CameraController.Instance.CreateCameraShake(CameraShakeType.Medium), parentEvent);
                }
            }

            // On health lost animations
            if ((totalHealthLost > 0 || totalArmourLost > 0) &&
                target.currentHealth - totalHealthLost > 0)
            {
                VisualEventManager.CreateVisualEvent(() =>
                {
                    HexCharacterController.Instance.PlayHurtAnimation(target.hexCharacterView);
                    AudioManager.Instance.PlaySound(target.AudioProfile, AudioSet.Hurt);
                }, parentEvent);
            }

            // Create damage text effect            
            VisualEventManager.CreateVisualEvent(() =>
                VisualEffectManager.Instance.CreateDamageTextEffect(target.hexCharacterView.WorldPosition, totalDamage, didCrit), parentEvent);

            // Create blood in ground VFX  
            if (totalHealthLost > 0)
            {
                VisualEventManager.CreateVisualEvent(() =>
                    VisualEffectManager.Instance.CreateGroundBloodSpatter(target.hexCharacterView.WorldPosition), parentEvent);
            }

            // Animate crowd
            if (attacker != null)
            {
                VisualEventManager.CreateVisualEvent(() => LevelController.Instance.AnimateCrowdOnHit(), parentEvent);
            }

            // Reduce health + armour
            if (totalHealthLost != 0)
            {
                HexCharacterController.Instance.ModifyHealth(target, -totalHealthLost);
            }
            if (totalArmourLost != 0)
            {
                HexCharacterController.Instance.ModifyArmour(target, -totalArmourLost);
            }

            // Increment damage dealt tracking
            if (attacker != null)
            {
                attacker.damageDealtThisCombat += totalHealthLost + totalArmourLost;
            }

            // Check for barrier
            if (removedBarrier)
            {
                PerkController.Instance.ModifyPerkOnCharacterEntity(target.pManager, Perk.Barrier, -1, true, 0.5f);
            }

            // Check and handle death
            LevelNode targetTile = target.currentTile;
            if (target.currentHealth <= 0 && target.livingState == LivingState.Alive)
            {
                HandleDeathBlow(target, parentEvent, false, weaponUsed);
            }

            // Combat Token Expiries >>
            if (target.livingState == LivingState.Alive)
            {
                // Check Block
                if (ability != null &&
                    !removedBarrier &&
                    totalDamage > 0 &&
                    PerkController.Instance.DoesCharacterHavePerk(target.pManager, Perk.Guard))
                {
                    PerkController.Instance.ModifyPerkOnCharacterEntity(target.pManager, Perk.Guard, -1);
                }

                // Check Vulnerable
                if (ability != null &&
                    !removedBarrier &&
                    totalDamage > 0 &&
                    PerkController.Instance.DoesCharacterHavePerk(target.pManager, Perk.Vulnerable))
                {
                    PerkController.Instance.ModifyPerkOnCharacterEntity(target.pManager, Perk.Vulnerable, -1);
                }

                // Check Evasion
                if (ability != null &&
                    PerkController.Instance.DoesCharacterHavePerk(target.pManager, Perk.Evasion))
                {
                    PerkController.Instance.ModifyPerkOnCharacterEntity(target.pManager, Perk.Evasion, -1);
                }

                // Check Crippled
                if (ability != null &&
                    PerkController.Instance.DoesCharacterHavePerk(target.pManager, Perk.Crippled))
                {
                    PerkController.Instance.ModifyPerkOnCharacterEntity(target.pManager, Perk.Crippled, -1);
                }

                // Check Stealth
                if (ability != null &&
                    PerkController.Instance.DoesCharacterHavePerk(target.pManager, Perk.Stealth))
                {
                    PerkController.Instance.ModifyPerkOnCharacterEntity(target.pManager, Perk.Stealth, -1);
                }
            }

            // On hit effects
            // Flaming weapon => apply burning
            if (target.currentHealth > 0 &&
                target.livingState == LivingState.Alive &&
                attacker != null &&
                PerkController.Instance.DoesCharacterHavePerk(attacker.pManager, Perk.FlamingWeapon) &&
                ability != null &&
                (ability.weaponRequirement == WeaponRequirement.MeleeWeapon || ability.weaponRequirement == WeaponRequirement.RangedWeapon || ability.weaponRequirement == WeaponRequirement.Bow || ability.weaponRequirement == WeaponRequirement.Crossbow || ability.weaponRequirement == WeaponRequirement.BowOrCrossbow))
            {
                Debug.Log("ExecuteHandleDamage() attacker has ignited weapon, applying burning on target");
                PerkController.Instance.ModifyPerkOnCharacterEntity(target.pManager, Perk.Burning, 1, true, 0.5f, attacker.pManager);
            }

            // Poisoned weapon => apply poisoned
            if (target.currentHealth > 0 &&
                target.livingState == LivingState.Alive &&
                attacker != null &&
                PerkController.Instance.DoesCharacterHavePerk(attacker.pManager, Perk.PoisonedWeapon) &&
                ability != null &&
                (ability.weaponRequirement == WeaponRequirement.MeleeWeapon || ability.weaponRequirement == WeaponRequirement.RangedWeapon || ability.weaponRequirement == WeaponRequirement.Bow || ability.weaponRequirement == WeaponRequirement.Crossbow || ability.weaponRequirement == WeaponRequirement.BowOrCrossbow))
            {
                Debug.Log("ExecuteHandleDamage() attacker has poisoned weapon, applying poisoned on target");
                VisualEventManager.CreateVisualEvent(() => AudioManager.Instance.PlaySound(Sound.Ability_Poison_Debuff), target.GetLastStackEventParent());
                PerkController.Instance.ModifyPerkOnCharacterEntity(target.pManager, Perk.Poisoned, 1, true, 0.5f, attacker.pManager);
            }

            // Tiger Aspect => apply bleeding

            if (target.currentHealth > 0 &&
                target.livingState == LivingState.Alive &&
                attacker != null &&
                PerkController.Instance.DoesCharacterHavePerk(attacker.pManager, Perk.TigerAspect) &&
                ability != null &&
                ability.abilityType.Contains(AbilityType.MeleeAttack))
            {
                Debug.Log("ExecuteHandleDamage() attacker has Tiger Aspect, applying bleeding on target");
                VisualEventManager.CreateVisualEvent(() => AudioManager.Instance.PlaySound(Sound.Ability_Bloody_Stab), target.GetLastStackEventParent());
                PerkController.Instance.ModifyPerkOnCharacterEntity(target.pManager, Perk.Bleeding, 1, true, 0.5f, attacker.pManager);
            }

            // Thorns
            if (target.livingState == LivingState.Alive &&
                PerkController.Instance.DoesCharacterHavePerk(target.pManager, Perk.Thorns) &&
                attacker != null &&
                attacker.currentHealth > 0 &&
                attacker.livingState == LivingState.Alive &&
                ability != null &&
                ability.abilityType.Contains(AbilityType.MeleeAttack))
            {
                // Take 10 damage
                DamageResult dr = GetFinalDamageValueAfterAllCalculations(attacker, 10, DamageType.Physical, false);
                HandleDamage(attacker, dr, DamageType.Physical, false);

                // Remove a stack of thorns from target
                PerkController.Instance.ModifyPerkOnCharacterEntity(target.pManager, Perk.Thorns, -1);
            }

            // Storm Shield
            if (target.livingState == LivingState.Alive &&
                PerkController.Instance.DoesCharacterHavePerk(target.pManager, Perk.StormShield) &&
                attacker != null &&
                attacker.currentHealth > 0 &&
                attacker.livingState == LivingState.Alive &&
                ability != null)
            {
                // Take 10 damage
                DamageResult dr = GetFinalDamageValueAfterAllCalculations(attacker, 10, DamageType.Magic, false);
                HandleDamage(attacker, dr, DamageType.Magic, false);

                // Remove a stack of thorns from target
                PerkController.Instance.ModifyPerkOnCharacterEntity(target.pManager, Perk.StormShield, -1);
            }

            // Item 'on hit' target effects
            if (attacker != null &&
                attacker.currentHealth > 0 &&
                attacker.livingState == LivingState.Alive &&
                target.currentHealth > 0 &&
                target.livingState == LivingState.Alive &&
                ability != null &&
                ability.abilityType.Contains(AbilityType.WeaponAttack) &&
                (ability.weaponRequirement == WeaponRequirement.MeleeWeapon || ability.weaponRequirement == WeaponRequirement.RangedWeapon || ability.weaponRequirement == WeaponRequirement.Bow || ability.weaponRequirement == WeaponRequirement.Crossbow || ability.weaponRequirement == WeaponRequirement.BowOrCrossbow) &&
                weaponUsed != null)
            {
                foreach (ItemEffect ie in weaponUsed.itemEffects)
                {
                    if (ie.effectType == ItemEffectType.OnHitEffect)
                    {
                        // roll for success chance
                        int roll = RandomGenerator.NumberBetween(1, 100);
                        if (roll <= ie.effectChance)
                        {
                            PerkController.Instance.ModifyPerkOnCharacterEntity(target.pManager, ie.perkApplied.perkTag, ie.perkApplied.passiveStacks, true, 0.5f, attacker.pManager);
                        }
                    }
                }
            }

            // On health lost events
            if (totalHealthLost > 0 && target.currentHealth > 0)
            {
                // Vengeful perk
                if (target.livingState == LivingState.Alive &&
                    PerkController.Instance.DoesCharacterHavePerk(target.pManager, Perk.Vengeful))
                {
                    VisualEventManager.CreateVisualEvent(() => AudioManager.Instance.PlaySound(Sound.Ability_Enrage), target.GetLastStackEventParent());
                    PerkController.Instance.ModifyPerkOnCharacterEntity(target.pManager, Perk.Combo, 1, true, 0.5f);
                }

                // Punch drunk perk (25% chance to become stunned)
                if (target.livingState == LivingState.Alive &&
                    ability != null &&
                    attacker != target &&
                    PerkController.Instance.DoesCharacterHavePerk(target.pManager, Perk.PunchDrunk))
                {
                    if (RandomGenerator.NumberBetween(1, 100) <= 25)
                    {
                        PerkController.Instance.ModifyPerkOnCharacterEntity(target.pManager, Perk.Stunned, 1, true, 0.5f);
                    }
                }
            }

            // Stress Events on health lost
            if (totalHealthLost > 0 && target.currentHealth > 0 && target.livingState == LivingState.Alive)
            {
                // Target 'On Health Lost' stress check
                CreateMoraleCheck(target, StressEventType.HealthLost);

                // ALlies' 'On Ally Health Lost' stress check
                foreach (HexCharacterModel c in HexCharacterController.Instance.GetAllAlliesOfCharacter(target, false))
                {
                    CreateMoraleCheck(c, StressEventType.AllyLosesHealth);
                }
            }

            // Check and roll for injury + handle stress check from injury
            if (target.currentHealth > 0 && target.livingState == LivingState.Alive)
            {
                // Handle injury
                CheckAndHandleInjuryOnHealthLost(target, damageResult, attacker, ability, effect);
            }

            // Check and handle death
            if (target.currentHealth <= 0 && target.livingState == LivingState.Dead)
            {
                //HandleDeathBlow(target, parentEvent);

                // Attacker 'on killed an enemy' events
                if (attacker != null &&
                    attacker.currentHealth > 0 &&
                    attacker.livingState == LivingState.Alive)
                {
                    // Exectioner perk: attacker gains 6 energy on kill
                    if (PerkController.Instance.DoesCharacterHavePerk(attacker.pManager, Perk.Executioner) && attacker.charactersKilledThisTurn == 0)
                    {
                        // Status notification
                        VisualEventManager.CreateVisualEvent(() =>
                        {
                            AudioManager.Instance.PlaySound(Sound.Ability_Enrage);
                            VisualEffectManager.Instance.CreateStatusEffect(attacker.hexCharacterView.WorldPosition, "Executioner!",
                                PerkController.Instance.GetPerkIconDataByTag(Perk.Executioner).passiveSprite, StatusFrameType.CircularBrown);
                        }, attacker.GetLastStackEventParent()).SetEndDelay(0.5f);

                        HexCharacterController.Instance.ModifyActionPoints(attacker, 6);
                    }

                    // Gladiator background: become confident on kill
                    if (CharacterDataController.Instance.DoesCharacterHaveBackground(attacker.background, CharacterBackground.Gladiator))
                    {
                        HexCharacterController.Instance.ModifyMoraleState(attacker, 1000, true, true);
                    }

                    // Perk Soul Collector: permanently gain 1 constitution
                    if (PerkController.Instance.DoesCharacterHavePerk(attacker.pManager, Perk.SoulCollector) &&
                        attacker.characterData != null)
                    {
                        // Status notification
                        VisualEventManager.CreateVisualEvent(() =>
                            VisualEffectManager.Instance.CreateStatusEffect(attacker.hexCharacterView.WorldPosition, "Soul Collector!", PerkController.Instance.GetPerkIconDataByTag(Perk.SoulCollector).passiveSprite, StatusFrameType.CircularBrown), attacker.GetLastStackEventParent()).SetEndDelay(0.5f);

                        // Increment stats
                        attacker.characterData.attributeSheet.constitution.value += 1;
                        attacker.attributeSheet.constitution.value += 1;

                        // Update health UI
                        HexCharacterController.Instance.ModifyMaxHealth(attacker, 0);
                    }

                    // Perk Soul Devourer: permanently gain 1 might
                    if (PerkController.Instance.DoesCharacterHavePerk(attacker.pManager, Perk.SoulDevourer) &&
                        attacker.characterData != null)
                    {
                        // Status notification
                        VisualEventManager.CreateVisualEvent(() =>
                            VisualEffectManager.Instance.CreateStatusEffect(attacker.hexCharacterView.WorldPosition, "Soul Devourer!", PerkController.Instance.GetPerkIconDataByTag(Perk.SoulDevourer).passiveSprite, StatusFrameType.CircularBrown), attacker.GetLastStackEventParent()).SetEndDelay(0.5f);

                        // Increment stats
                        attacker.characterData.attributeSheet.might.value += 1;
                        attacker.attributeSheet.might.value += 1;
                    }

                    // Mercenary background
                    if (CharacterDataController.Instance.DoesCharacterHaveBackground(attacker.background, CharacterBackground.Mercenary) &&
                        attacker.characterData != null)
                    {
                        // Status notification
                        VisualEventManager.CreateVisualEvent(() =>
                            VisualEffectManager.Instance.CreateStatusEffect(attacker.hexCharacterView.WorldPosition, "Mercenary!", CharacterDataController.Instance.GetBackgroundData(CharacterBackground.Mercenary).BackgroundSprite, StatusFrameType.CircularBrown), attacker.GetLastStackEventParent()).SetEndDelay(0.5f);

                        // Gain gold
                        PlayerDataController.Instance.ModifyPlayerGold(10);
                    }

                    // Increment kills this turn
                    attacker.charactersKilledThisTurn++;
                    attacker.totalKills++;
                }

                // Check nearby gnoll enemies: gnolls heal when a character is killed within 1 of them
                List<HexCharacterModel> characters = HexCharacterController.Instance.AllCharacters;
                List<LevelNode> tiles = LevelController.Instance.GetAllHexsWithinRange(targetTile, 1);
                foreach (HexCharacterModel possibleGnoll in characters)
                {
                    if (possibleGnoll.race == CharacterRace.Gnoll &&
                        tiles.Contains(possibleGnoll.currentTile))
                    {
                        // Gain health
                        HexCharacterController.Instance.ModifyHealth(possibleGnoll, 5);

                        // Status notification
                        VisualEventManager.CreateVisualEvent(() =>
                        {
                            AudioManager.Instance.PlaySound(Sound.Ability_Feast);
                            VisualEffectManager.Instance.CreateStatusEffect(possibleGnoll.hexCharacterView.WorldPosition, "Cannibalism!", CharacterDataController.Instance.GetRaceData(CharacterRace.Gnoll).racialSprite, StatusFrameType.CircularBrown);
                        }, possibleGnoll.GetLastStackEventParent()).SetEndDelay(0.5f);

                    }
                }

                // Stress Events on death
                // Enemy Killed (Positive)
                foreach (HexCharacterModel c in HexCharacterController.Instance.GetAllEnemiesOfCharacter(target))
                {
                    CreateMoraleCheck(c, StressEventType.EnemyKilled);
                }

                // Ally Killed (Negative)
                foreach (HexCharacterModel c in HexCharacterController.Instance.GetAllAlliesOfCharacter(target, false))
                {
                    CreateMoraleCheck(c, StressEventType.AllyKilled);
                }

                //HandleDeathBlow(target, parentEvent);
            }

        }
        private DeathRollResult RollForDeathResist(HexCharacterModel c)
        {
            DeathRollResult ret = new DeathRollResult();
            int resistance = StatCalculator.GetTotalDeathResistance(c);
            ret.roll = RandomGenerator.NumberBetween(1, 100);
            ret.required = resistance;
            if (ret.roll <= resistance)
            {
                ret.pass = true;
            }
            else
            {
                ret.pass = false;
            }

            // Catch check: in the extremely rare circumstance where a character has every single permanent injury, kill them
            // automatically to prevent bugs
            if (PerkController.Instance.GetAllPermanentInjuriesOnCharacter(c.characterData).Count >=
                PerkController.Instance.GetAllPermanentInjuries().Count)
            {
                ret.pass = false;
            }
            return ret;

        }

        #endregion

        // Combat Victory, Defeat + Game Over Logic
        #region

        private void HandleOnCombatVictoryEffects()
        {

        }
        public void UpdateScoreDataPostCombat(bool victory = true)
        {
            PlayerScoreTracker scoreData = ScoreController.Instance.CurrentScoreData;
            EnemyEncounterData encounterData = RunController.Instance.CurrentCombatContractData.enemyEncounterData;
            List<HexCharacterModel> charactersKilled = HexCharacterController.Instance.Graveyard.FindAll(
                c => c.allegiance == Allegiance.Player &&
                    c.controller == Controller.Player &&
                    c.characterData != null &&
                    c.characterData.currentHealth <= 0);

            scoreData.playerCharactersKilled += charactersKilled.Count;

            if (!victory)
            {
                scoreData.combatDefeats += 1;
            }

            if (encounterData.difficulty == CombatDifficulty.Basic)
            {
                if (victory)
                {
                    scoreData.basicCombatsCompleted += 1;
                }
                if (victory && charactersKilled.Count == 0)
                {
                    scoreData.basicCombatsCompletedWithoutDeath += 1;
                }
            }
            else if (encounterData.difficulty == CombatDifficulty.Elite)
            {
                if (victory)
                {
                    scoreData.eliteCombatsCompleted += 1;
                }
                if (victory && charactersKilled.Count == 0)
                {
                    scoreData.eliteCombatsCompletedWithoutDeath += 1;
                }
            }
            else if (encounterData.difficulty == CombatDifficulty.Boss)
            {
                if (victory)
                {
                    scoreData.bossCombatsCompleted += 1;
                }
                if (victory && charactersKilled.Count == 0)
                {
                    scoreData.bossCombatsCompletedWithoutDeath += 1;
                }
            }
        }

        #endregion
    }

    public class HitRoll
    {

        public HitRoll(int roll, int required, HitRollResult result, HexCharacterModel attacker, HexCharacterModel target)
        {
            Roll = roll;
            RequiredRoll = required;
            Result = result;
            Attacker = attacker;
            Target = target;
        }

        public int Roll { get; }

        public int RequiredRoll { get; }

        public HitRollResult Result { get; }

        public HexCharacterModel Attacker { get; }

        public HexCharacterModel Target { get; }
    }

    [Serializable]
    public class StressEventData
    {
        public int moraleChangeMin;
        public int moraleChangeMax;
        public int successChance;

        public StressEventData(int min, int max, int successChance)
        {
            moraleChangeMin = min;
            moraleChangeMax = max;
            this.successChance = successChance;
        }
        public StressEventData() { }
    }

    public class DamageResult
    {
        public int damageLowerLimit;
        public int damageUpperLimit;
        public bool didCrit;
        public int totalArmourLost;
        public int totalDamage;
        public int totalHealthLost;
        public ItemData weaponUsed;
    }
    public class DeathRollResult
    {
        public bool pass;
        public int required;
        public int roll;
    }

    [Serializable]
    public class MoraleStateData
    {
        public Sprite icon;
        public MoraleState moraleState;
        public List<CustomString> description;
        public List<ModalDotRowBuildData> effectDescriptions;
    }
}
