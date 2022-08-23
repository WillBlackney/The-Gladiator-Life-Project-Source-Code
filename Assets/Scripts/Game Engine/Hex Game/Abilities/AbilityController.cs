using HexGameEngine.Characters;
using HexGameEngine.Combat;
using HexGameEngine.HexTiles;
using HexGameEngine.TurnLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using HexGameEngine.Perks;
using HexGameEngine.Pathfinding;
using HexGameEngine.Utilities;
using HexGameEngine.UI;
using HexGameEngine.VisualEvents;
using HexGameEngine.UCM;
using UnityEngine.UI;
using System.Linq;
using System;

namespace HexGameEngine.Abilities
{
    public class AbilityController : Singleton<AbilityController>
    {
        // Properties + Components
        #region
        [Header("Ability Data Files")]
        [SerializeField] private AbilityDataSO[] allAbilityDataSOs;
        [SerializeField] private AbilityDataSO freeStrikeAbilityData;
        [SerializeField] private AbilityDataSO riposteAbilityData;
        private AbilityData[] allAbilities;

        [Header("Hit Chance Pop Up Components")]
        [SerializeField] GameObject hitChanceVisualParent;
        [SerializeField] GameObject hitChancePositionParent;
        [SerializeField] CanvasGroup hitChanceCg;
        [SerializeField] TextMeshProUGUI hitChanceText;
        [SerializeField] ModalDottedRow[] hitChanceBoxes;
        [SerializeField] RectTransform[] hitChanceLayouts;
        private float popupDelay = 0.5f;

        // Ability caching properties
        private AbilityData currentAbilityAwaiting;
        private AbilitySelectionPhase currentSelectionPhase;
        private HexCharacterModel firstSelectionCharacter;
        #endregion

        // Getters + Accessors
        #region
        public AbilityData FreeStrikeAbility
        {
            get; private set;
        }
        public AbilityData RiposteAbility
        {
            get; private set;
        }
        public AbilityData[] AllAbilities
        {
            get { return allAbilities; }
            private set { allAbilities = value; }
        }
        public AbilityDataSO[] AllAbilityDataSOs
        {
            get { return allAbilityDataSOs; }
            private set { allAbilityDataSOs = value; }
        }
        public AbilityData CurrentAbilityAwaiting
        {
            get { return currentAbilityAwaiting; }
        }
        public AbilitySelectionPhase CurrentSelectionPhase
        {
            get { return currentSelectionPhase; }
        }
        #endregion

        // Initialization + Build Library
        #region
        protected override void Awake()
        {
            base.Awake();
            BuildAbilityLibrary();
        }
        private void BuildAbilityLibrary()
        {
            Debug.Log("AbilityController.BuildAbilityLibrary() called...");

            List<AbilityData> tempList = new List<AbilityData>();

            foreach (AbilityDataSO dataSO in allAbilityDataSOs)
            {
                if (dataSO.includeInGame)
                    tempList.Add(BuildAbilityDataFromScriptableObjectData(dataSO));
            }

            AllAbilities = tempList.ToArray();

            // Setup free strike + globally shared abilities
            FreeStrikeAbility = BuildAbilityDataFromScriptableObjectData(freeStrikeAbilityData);
            RiposteAbility = BuildAbilityDataFromScriptableObjectData(riposteAbilityData);

        }
        #endregion

        // Search Logic
        #region
        public AbilityData GetCharacterAbilityByName(HexCharacterModel character, string name)
        {
            AbilityData abilityReturned = null;
            foreach (AbilityData a in character.abilityBook.activeAbilities)
            {
                if (a.abilityName == name)
                {
                    abilityReturned = a;
                    break;
                }
            }

            if (abilityReturned == null)
                Debug.LogWarning("GetCharacterAbilityByName() could not find an ability on character with the name " + name + ", returning null...");

            return abilityReturned;
        }
        public AbilityData GetRandomAbilityTomeAbility()
        {
            List<TalentSchool> talentSchools = new List<TalentSchool> { TalentSchool.Divinity, TalentSchool.Guardian, TalentSchool.Manipulation,
            TalentSchool.Naturalism, TalentSchool.Pyromania, TalentSchool.Ranger, TalentSchool.Scoundrel, TalentSchool.Shadowcraft, TalentSchool.Warfare, TalentSchool.Metamorph };

            talentSchools.Shuffle();
            TalentSchool ts = talentSchools[0];
            List<AbilityData> validAbilities = new List<AbilityData>();
            foreach (AbilityData a in AllAbilities)
            {
                if (a.talentRequirementData.talentSchool == ts)
                    validAbilities.Add(a);
            }

            validAbilities.Shuffle();
            return validAbilities[0];
        }
        public List<AbilityData> GetAllAbilitiesOfTalent(TalentSchool ts)
        {
            List<AbilityData> ret = new List<AbilityData>();

            for(int i = 0; i < allAbilities.Length; i++)
            {
                if (allAbilities[i].talentRequirementData != null &&
                    allAbilities[i].talentRequirementData.talentSchool == ts)
                    ret.Add(allAbilities[i]);
            }

            return ret;
        }
        public AbilityData FindAbilityData(string abilityName)
        {
            AbilityData ret = null;
            foreach(AbilityData a in allAbilities)
            {
                if (a.abilityName == abilityName)
                {
                    ret = a;
                    break;
                }
            }
               
            return ret;
        }
        #endregion

        // Data Conversion
        #region
        public AbilityData BuildAbilityDataFromScriptableObjectData(AbilityDataSO d)
        {
            AbilityData a = new AbilityData();

            a.abilityName = d.abilityName;
            a.baseAbilityDescription = d.baseAbilityDescription;

            a.abilityType = d.abilityType;
            a.doesNotBreakStealth = d.doesNotBreakStealth;
            a.weaponAbilityType = d.weaponAbilityType;
            a.derivedFromWeapon = d.derivedFromWeapon;
            a.derivedFromItemLoadout = d.derivedFromItemLoadout;
            a.weaponClass = d.weaponClass;
            a.targetRequirement = d.targetRequirement;
            a.weaponRequirement = d.weaponRequirement;
            a.talentRequirementData = d.talentRequirementData;

            a.energyCost = d.energyCost;
            a.baseCooldown = d.baseCooldown;

            a.baseRange = d.baseRange;
            a.gainRangeBonusFromVision = d.gainRangeBonusFromVision;
            a.hitChanceModifier = d.hitChanceModifier;
            a.accuracyPenaltyFromMelee = d.accuracyPenaltyFromMelee;
            a.secondaryTargetRequirement = d.secondaryTargetRequirement;
            a.rangeFromTarget = d.rangeFromTarget;

            a.chainLoops = d.chainLoops;

            // Ability  effects
            a.abilityEffects = new List<AbilityEffect>();
            foreach (AbilityEffect effect in d.abilityEffects)
            {
                a.abilityEffects.Add(ObjectCloner.CloneJSON(effect));
            }
            a.onHitEffects = new List<AbilityEffect>();
            foreach (AbilityEffect effect in d.onHitEffects)
            {
                a.onHitEffects.Add(ObjectCloner.CloneJSON(effect));
            }
            a.onCritEffects = new List<AbilityEffect>();
            foreach (AbilityEffect effect in d.onCritEffects)
            {
                a.onCritEffects.Add(ObjectCloner.CloneJSON(effect));
            }
            a.onPerkAppliedSuccessEffects = new List<AbilityEffect>();
            foreach (AbilityEffect effect in d.onPerkAppliedSuccessEffects)
            {
                a.onPerkAppliedSuccessEffects.Add(ObjectCloner.CloneJSON(effect));
            }
            a.onCollisionEffects = new List<AbilityEffect>();
            foreach (AbilityEffect effect in d.onCollisionEffects)
            {
                a.onCollisionEffects.Add(ObjectCloner.CloneJSON(effect));
            }
            a.chainedEffects = new List<AbilityEffect>();
            foreach (AbilityEffect effect in d.chainedEffects)
            {
                a.chainedEffects.Add(ObjectCloner.CloneJSON(effect));
            }

            // Keyword Model Data
            a.keyWords = new List<KeyWordModel>();
            foreach (KeyWordModel kwdm in d.keyWords)
            {
                a.keyWords.Add(ObjectCloner.CloneJSON(kwdm));
            }

            // Custom string Data
            a.dynamicDescription = new List<CustomString>();
            foreach (CustomString cs in d.dynamicDescription)
            {
                a.dynamicDescription.Add(ObjectCloner.CloneJSON(cs));
            }

            // Requirements
            a.abilitySubRequirements = new List<AbilityRequirement>();
            foreach (AbilityRequirement ar in d.abilitySubRequirements)
            {
                a.abilitySubRequirements.Add(ObjectCloner.CloneJSON(ar));
            }


            return a;
        }
        
        public AbilityBook ConvertSerializedAbilityBookToUnserialized(SerializedAbilityBook data)
        {
            AbilityBook a = new AbilityBook();

            foreach (AbilityDataSO d in data.activeAbilities)
            {
                AbilityData ab = BuildAbilityDataFromScriptableObjectData(d);
                a.HandleLearnNewAbility(ab);
            }

            return a;
        }

        #endregion

        // Learn + Unlearn Abilities
        #region
        public void BuildHexCharacterAbilityBookFromData(HexCharacterModel character, AbilityBook data)
        {
            // Copy ability book from data character into hex character
            character.abilityBook = new AbilityBook(data);

            // Link hex character to ability data
            foreach (AbilityData a in character.abilityBook.activeAbilities)
            {
                a.myCharacter = character;
            }            
        }
        #endregion

       

        // Ability Usage Logic
        #region     
        public void UseAbility(HexCharacterModel character, AbilityData ability, HexCharacterModel target = null, LevelNode tileTarget = null)
        {
            // Status effect for enemies + AI characters
            if(character.controller == Controller.AI)
            {
                HexCharacterView view = character.hexCharacterView;
                VisualEventManager.Instance.CreateVisualEvent(() =>
                VisualEffectManager.Instance.CreateStatusEffect(view.WorldPosition, ability.abilityName), QueuePosition.Back, 0, 0.5f);
            }

            // Hide pop up
            HideHitChancePopup();

            // Update UI energy bar
            if (TurnController.Instance.EntityActivated == character && character.controller == Controller.Player)
                CombatUIController.Instance.EnergyBar.UpdateIcons(TurnController.Instance.EntityActivated.currentEnergy);

            OnAbilityUsedStart(character, ability, target);

            // Face target
            if (target != null && target != character && ability.abilityType != AbilityType.Skill)
                LevelController.Instance.FaceCharacterTowardsTargetCharacter(character, target);
            else if (tileTarget != null && ability.abilityType != AbilityType.Skill)
                LevelController.Instance.FaceCharacterTowardsHex(character, tileTarget);

            foreach (AbilityEffect e in ability.abilityEffects)
            {
                TriggerAbilityEffect(ability, e, character, target, tileTarget);
            }

            OnAbilityUsedFinish(character, ability);

            // Check for removal of damage/accuracy related tokens
            if(ability.abilityType == AbilityType.MeleeAttack ||
                ability.abilityType == AbilityType.RangedAttack)
            {
                if(PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Wrath))                
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Wrath, -1);                
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Weakened))                
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Weakened, -1);
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Focus))
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Focus, -1);
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Blinded))
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Blinded, -1);
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Combo))
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Combo, -1);
            }


            if(ability.abilityName == "Riposte")
            {
                Debug.Log("Removing riposte");
                // Remove a riposte stack
                PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Riposte, -1);
            }

        }
        private void TriggerAbilityEffect(AbilityData ability, AbilityEffect abilityEffect, HexCharacterModel caster, HexCharacterModel target, LevelNode tileTarget = null, HexCharacterModel previousChainTarget = null)
        {

            //bool effectSuccessful = false;
            bool triggerEffectEndEvents = true;

            // Stop and return if effect requires a target and that target is dying/dead/null/no longer valid      
            if ((target == null || target.livingState == LivingState.Dead) &&
                (
                abilityEffect.effectType == AbilityEffectType.DamageTarget ||
                abilityEffect.effectType == AbilityEffectType.ApplyPassiveTarget ||
                abilityEffect.effectType == AbilityEffectType.RemovePassiveTarget ||
                abilityEffect.effectType == AbilityEffectType.KnockBack
                )
                )
            {
                Debug.Log("AbilityController.TriggerAbilityEffect() cancelling: target is no longer valid");
                return;
            }

            // Stop and return if caster of the ability is dead or null  
            if (caster == null || caster.livingState == LivingState.Dead)
            {
                return;
            }

            // Check effect sub requirements
            if (!DoesAbilityEffectMeetAllRequirements(ability, abilityEffect, caster, target, tileTarget)) return;

            // Queue starting anims and particles
            foreach (AnimationEventData vEvent in abilityEffect.visualEventsOnStart)
            {
                // if effect is not chained
                if (abilityEffect.chainedEffect == false)
                    AnimationEventController.Instance.PlayAnimationEvent(vEvent, caster, target, tileTarget);

                // if effect is chained, the effect starts from previous chain target
                else if (abilityEffect.chainedEffect == true)
                    AnimationEventController.Instance.PlayAnimationEvent(vEvent, previousChainTarget, target, tileTarget);
            }

            // RESOLVE EFFECT LOGIC START!

            // Damage Target
            if (abilityEffect.effectType == AbilityEffectType.DamageTarget)
            {
                triggerEffectEndEvents = false;
                Items.ItemData weaponUsed = caster.itemSet.mainHandItem;

                HitRoll hitResult = CombatController.Instance.RollForHit(caster, target, ability);
                bool didCrit = CombatController.Instance.RollForCrit(caster, target, ability, abilityEffect);

               // VisualEventManager.Instance.CreateStackParentVisualEvent(target);

                if (hitResult.Result == HitRollResult.Hit)
                {
                    DamageResult damageResult = null;
                    damageResult = CombatController.Instance.GetFinalDamageValueAfterAllCalculations(caster, target, ability, abilityEffect, didCrit);
                    damageResult.didCrit = didCrit;       
                    
                    // Resolve bonus armour damage
                    if(abilityEffect.bonusArmourDamage > 0)                    
                        HexCharacterController.Instance.ModifyArmour(target, abilityEffect.bonusArmourDamage);                    

                    // Do on hit visual effects for this ability
                    foreach (AnimationEventData vEvent in abilityEffect.visualEventsOnHit)                    
                        AnimationEventController.Instance.PlayAnimationEvent(vEvent, caster, target, null, target.GetLastStackEventParent());
                    
                    // On crit stress check
                    if (didCrit)                    
                        CombatController.Instance.CreateStressCheck(caster, StressEventType.LandedCriticalStrike);

                    // Cache hex position, in case target is killed before chain effect triggers
                    LevelNode lastChainHex = target.currentTile;

                    // Deal damage
                    CombatController.Instance.HandleDamage(caster, target, damageResult, ability, abilityEffect, false, target.GetLastStackEventParent());

                    // On ability effect completed VFX
                    if (CombatController.Instance.CurrentCombatState == CombatGameState.CombatActive &&
                        caster.livingState == LivingState.Alive)
                    {
                        if (target != null && target.livingState == LivingState.Alive)
                        {
                            foreach (AnimationEventData vEvent in abilityEffect.visualEventsOnEffectFinish)
                            {
                                AnimationEventController.Instance.PlayAnimationEvent(vEvent, caster, target, null, target.GetLastStackEventParent());
                            }
                        }
                    }

                    // Trigger on hit/crit effects
                    if (abilityEffect.chainedEffect == false)
                    {
                        if (didCrit)
                        {
                            foreach (AbilityEffect e in ability.onCritEffects)
                            {
                                TriggerAbilityEffect(ability, e, caster, target, tileTarget);
                            }
                        }
                        else
                        {
                            foreach (AbilityEffect e in ability.onHitEffects)
                            {
                                TriggerAbilityEffect(ability, e, caster, target, tileTarget);
                            }
                        }
                       
                    }

                    // trigger chain effect here?
                    if (ability.chainedEffects.Count > 0 && abilityEffect.chainedEffect == false && abilityEffect.triggersChainEffectSequence == true)
                    {
                        Allegiance targetAllegiance = Allegiance.Player;
                        if (caster.allegiance == Allegiance.Player)
                            targetAllegiance = Allegiance.Enemy;
                        else if (caster.allegiance == Allegiance.Enemy)
                            targetAllegiance = Allegiance.Player;

                        // TO DO ABOVE: chain effects can only hit enemies now, if there is a chain effect that targets allies of the caster (e.g. a chain heal or buff)
                        // then we need to write some more code to be specific about whether allies or enemies are effected by the chain sequence.

                        HexCharacterModel lastChainTargetHit = target;
                        HexCharacterModel nextChainTarget = null;
                        int totalLoopsCompleted = 0;

                        for (int i = 0; i < ability.chainLoops; i++)
                        {
                            // try find a chain target
                            List<LevelNode> hexs = LevelController.Instance.GetAllHexsWithinRange(lastChainHex, 1);
                            List<HexCharacterModel> possibleTargets = new List<HexCharacterModel>();
                            foreach (LevelNode h in hexs)
                            {
                                if (h.myCharacter != null && h.myCharacter.allegiance == targetAllegiance)
                                {
                                    possibleTargets.Add(h.myCharacter);
                                }
                            }

                            // randomly decide the next target to be chained onto
                            if (possibleTargets.Count == 1)
                                nextChainTarget = possibleTargets[0];
                            else if (possibleTargets.Count > 1)
                                nextChainTarget = possibleTargets[RandomGenerator.NumberBetween(0, possibleTargets.Count - 1)];

                            // found a valid target to chain onto
                            if (nextChainTarget != null)
                            {
                                lastChainHex = nextChainTarget.currentTile;
                                foreach (AbilityEffect ef in ability.chainedEffects)
                                {
                                    TriggerAbilityEffect(ability, ef, caster, nextChainTarget, tileTarget, lastChainTargetHit);
                                }

                                lastChainTargetHit = nextChainTarget;
                                totalLoopsCompleted++;
                            }
                        }

                        Debug.Log("Total loops of chain effect completed = " + totalLoopsCompleted.ToString());
                    }


                }
                else if (hitResult.Result == HitRollResult.Miss)
                {
                    // Miss notification
                    VisualEventManager.Instance.CreateVisualEvent(() =>
                        VisualEffectManager.Instance.CreateStatusEffect(target.hexCharacterView.WorldPosition, "MISS"), QueuePosition.Back
                        , 0, 0, target.GetLastStackEventParent());

                    // Duck animation on miss
                    VisualEventManager.Instance.CreateVisualEvent(() =>
                    HexCharacterController.Instance.PlayDuckAnimation(target.hexCharacterView), QueuePosition.Back, 0f, 0.5f, target.GetLastStackEventParent());

                    if (HexCharacterController.Instance.IsCharacterAbleToMakeRiposteAttack(target) &&
                        ability.abilityType == AbilityType.MeleeAttack &&
                        target.currentTile.Distance(caster.currentTile) <= 1 &&
                        // Cant riposte against another riposte, or free strike
                        ability.abilityName != RiposteAbility.abilityName && 
                        ability.abilityName != FreeStrikeAbility.abilityName)
                    {
                        // Miss notification
                        VisualEventManager.Instance.CreateVisualEvent(() =>
                            VisualEffectManager.Instance.CreateStatusEffect(target.hexCharacterView.WorldPosition, "Riposte!"), QueuePosition.Back,0.5f);

                        // Start free strike attack
                        UseAbility(target, RiposteAbility, caster);
                        VisualEventManager.Instance.InsertTimeDelayInQueue(0.5f);

                        
                    }                       
                }
                
            }

            // Damage Aoe
            else if (abilityEffect.effectType == AbilityEffectType.DamageAoe)
            {
                triggerEffectEndEvents = false;
                List<LevelNode> tilesEffected = new List<LevelNode>();
                List<HexCharacterModel> charactersEffected = new List<HexCharacterModel>();
                List<HexCharacterModel> charactersHit = new List<HexCharacterModel>();

                // Get Aoe Area
                if (abilityEffect.aoeType == AoeType.Aura)
                {
                    tilesEffected = HexCharacterController.Instance.GetCharacterAura(caster, true);
                }
                else if (abilityEffect.aoeType == AoeType.ZoneOfControl)
                {
                    tilesEffected = HexCharacterController.Instance.GetCharacterZoneOfControl(caster);
                }
                else if (abilityEffect.aoeType == AoeType.AtTarget)
                {
                    tilesEffected = LevelController.Instance.GetAllHexsWithinRange(tileTarget, abilityEffect.aoeSize, true);
                }
                else if (abilityEffect.aoeType == AoeType.Global)
                {
                    tilesEffected.AddRange(LevelController.Instance.AllLevelNodes.ToList());
                }

                // Remove centre point, if needed
                if (abilityEffect.includeCentreTile == false)
                {
                    if ((abilityEffect.aoeType == AoeType.Aura || abilityEffect.aoeType == AoeType.Global) &&
                        tilesEffected.Contains(caster.currentTile))
                        tilesEffected.Remove(caster.currentTile);

                    else if (abilityEffect.aoeType == AoeType.AtTarget &&
                        tilesEffected.Contains(tileTarget))
                        tilesEffected.Remove(tileTarget);
                }

                Debug.Log("Tiles effected = " + tilesEffected.Count.ToString());

                // Filter out enemies or allies where applicable
                foreach (LevelNode h in tilesEffected)
                {
                    if (h.myCharacter != null)
                    {
                        if ((h.myCharacter.allegiance == caster.allegiance && abilityEffect.effectsAllies) ||
                            (h.myCharacter.allegiance != caster.allegiance && abilityEffect.effectsEnemies))
                        {
                            charactersEffected.Add(h.myCharacter);
                        }
                    }
                }
                Debug.Log("Characters effected = " + charactersEffected.Count.ToString());

                // Roll for hits + play on Hit animations or on miss animations
                foreach (HexCharacterModel character in charactersEffected)
                {
                    //character.eventStacks.Add(VisualEventManager.Instance.CreateStackParentVisualEvent(character));
                    VisualEventManager.Instance.CreateStackParentVisualEvent(character);

                    // Roll for hit
                    HitRoll hitRoll = CombatController.Instance.RollForHit(caster, character, ability);                   

                    if (hitRoll.Result == HitRollResult.Hit)
                    {
                        charactersHit.Add(character);

                        // Do on hit visual effects for this ability
                        foreach (AnimationEventData vEvent in abilityEffect.visualEventsOnHit)
                        {
                            AnimationEventController.Instance.PlayAnimationEvent(vEvent, caster, character, null, character.GetLastStackEventParent());
                        }
                    }
                    else if (hitRoll.Result == HitRollResult.Miss)
                    {
                        // Miss notification
                        Vector3 pos = character.hexCharacterView.WorldPosition;
                        VisualEventManager.Instance.CreateVisualEvent(() =>
                        {
                            if (character.hexCharacterView != null) pos = character.hexCharacterView.WorldPosition;
                            VisualEffectManager.Instance.CreateStatusEffect(pos, "MISS");
                        }, QueuePosition.Back, 0, 0, character.GetLastStackEventParent());


                        // Duck animation on miss
                        VisualEventManager.Instance.CreateVisualEvent(() =>
                       HexCharacterController.Instance.PlayDuckAnimation(character.hexCharacterView), QueuePosition.Back, 0, 0, character.GetLastStackEventParent());
                    }                    

                }

                // Calcuate and deal damage
                foreach (HexCharacterModel character in charactersHit)
                {
                    Items.ItemData weaponUsed = caster.itemSet.mainHandItem;
                    bool didCrit = CombatController.Instance.RollForCrit(caster, character, ability, abilityEffect);
                    DamageResult dResult = null;
                    dResult = CombatController.Instance.GetFinalDamageValueAfterAllCalculations(caster, character, ability, abilityEffect, didCrit);

                    dResult.didCrit = didCrit;

                    // On crit stress check
                    if (didCrit)
                    {
                        CombatController.Instance.CreateStressCheck(caster, StressEventType.LandedCriticalStrike);
                    }

                    // Resolve bonus armour damage
                    if (abilityEffect.bonusArmourDamage > 0)
                        HexCharacterController.Instance.ModifyArmour(target, abilityEffect.bonusArmourDamage);

                    // Deal damage
                    CombatController.Instance.HandleDamage(caster, character, dResult, ability, abilityEffect, false, character.GetLastStackEventParent());

                    // On ability effect completed VFX
                    if (CombatController.Instance.CurrentCombatState == CombatGameState.CombatActive &&
                        caster.livingState == LivingState.Alive)
                    {
                        if (character != null && character.livingState == LivingState.Alive)
                        {
                            foreach (AnimationEventData vEvent in abilityEffect.visualEventsOnEffectFinish)
                            {
                                AnimationEventController.Instance.PlayAnimationEvent(vEvent, caster, character, null, character.GetLastStackEventParent());
                            }
                        }
                    }

                    // Trigger on hit/crit effects
                    if (didCrit)
                    {
                        foreach (AbilityEffect e in ability.onCritEffects)
                        {
                            TriggerAbilityEffect(ability, e, caster, character, tileTarget);
                        }
                    }
                    else
                    {
                        foreach (AbilityEffect e in ability.onHitEffects)
                        {
                            TriggerAbilityEffect(ability, e, caster, character, tileTarget);
                        }
                    }
                }

            }

            // Stress Check
            else if (abilityEffect.effectType == AbilityEffectType.StressCheck)
            {
                CombatController.Instance.CreateStressCheck(target, abilityEffect.stressEventData, true);
            }

            // Apply passive to target
            else if (abilityEffect.effectType == AbilityEffectType.ApplyPassiveTarget)
            {
                int stacks = abilityEffect.perkPairing.passiveStacks;
                bool roll = RandomGenerator.NumberBetween(1, 100) <= abilityEffect.perkApplicationChance;
                bool success = false;

                if(roll)
                success = PerkController.Instance.ModifyPerkOnCharacterEntity(target.pManager, abilityEffect.perkPairing.perkTag, stacks, true, 0.5f, caster.pManager);

                if (success)
                {
                    foreach (AbilityEffect e in ability.onPerkAppliedSuccessEffects)
                    {
                        triggerEffectEndEvents = false;
                        TriggerAbilityEffect(ability, e, caster, target, tileTarget);
                    }
                }
            }

            // Apply passive AoE
            else if (abilityEffect.effectType == AbilityEffectType.ApplyPassiveAoe)
            {
                triggerEffectEndEvents = false;
                List<LevelNode> tilesEffected = new List<LevelNode>();
                List<HexCharacterModel> charactersEffected = new List<HexCharacterModel>();
                List<HexCharacterModel> charactersHit = new List<HexCharacterModel>();

                // Get Aoe Area
                if (abilityEffect.aoeType == AoeType.Aura)
                {
                    tilesEffected = HexCharacterController.Instance.GetCharacterAura(caster, true);
                }
                else if (abilityEffect.aoeType == AoeType.ZoneOfControl)
                {
                    tilesEffected = HexCharacterController.Instance.GetCharacterZoneOfControl(caster);
                }
                else if (abilityEffect.aoeType == AoeType.AtTarget)
                {
                    tilesEffected = LevelController.Instance.GetAllHexsWithinRange(tileTarget, abilityEffect.aoeSize, true);
                }
                else if (abilityEffect.aoeType == AoeType.Global)
                {
                    tilesEffected.AddRange(LevelController.Instance.AllLevelNodes.ToList());
                }

                // Remove centre point, if needed
                if (abilityEffect.includeCentreTile == false)
                {
                    if ((abilityEffect.aoeType == AoeType.Aura || abilityEffect.aoeType == AoeType.Global) &&
                        tilesEffected.Contains(caster.currentTile))
                        tilesEffected.Remove(caster.currentTile);

                    else if (abilityEffect.aoeType == AoeType.AtTarget &&
                        tilesEffected.Contains(tileTarget))
                        tilesEffected.Remove(tileTarget);
                }

                Debug.Log("Tiles effected = " + tilesEffected.Count.ToString());

                // Filter out enemies or allies where applicable
                foreach (LevelNode h in tilesEffected)
                {
                    if (h.myCharacter != null)
                    {
                        if ((h.myCharacter.allegiance == caster.allegiance && abilityEffect.effectsAllies) ||
                            (h.myCharacter.allegiance != caster.allegiance && abilityEffect.effectsEnemies))
                        {
                            charactersEffected.Add(h.myCharacter);
                        }
                    }
                }
                Debug.Log("Characters effected = " + charactersEffected.Count.ToString());

                // Roll for hits + play on Hit animations or on miss animations
                foreach (HexCharacterModel character in charactersEffected)
                {
                    //character.eventStacks.Add(VisualEventManager.Instance.CreateStackParentVisualEvent(character));
                    VisualEventManager.Instance.CreateStackParentVisualEvent(character);
                }

                // Apply Passive
                foreach (HexCharacterModel character in charactersEffected)
                {
                    bool roll = RandomGenerator.NumberBetween(1, 100) <= abilityEffect.perkApplicationChance;
                    bool success = false;

                    if (roll)
                        success = PerkController.Instance.ModifyPerkOnCharacterEntity
                         (character.pManager, abilityEffect.perkPairing.perkTag, abilityEffect.perkPairing.passiveStacks, true, 0f, caster.pManager);

                    if (success) charactersHit.Add(character);
                }

                // On passive succesfully applied visual events + effects
                foreach (HexCharacterModel character in charactersHit)
                {
                    if (CombatController.Instance.CurrentCombatState == CombatGameState.CombatActive &&
                        caster.livingState == LivingState.Alive)
                    {
                        if (character != null && character.livingState == LivingState.Alive)
                        {
                            foreach (AnimationEventData vEvent in abilityEffect.visualEventsOnHit)
                            {
                                AnimationEventController.Instance.PlayAnimationEvent(vEvent, caster, character, null, character.GetLastStackEventParent());
                            }
                        }
                    }

                    foreach (AbilityEffect e in ability.onPerkAppliedSuccessEffects)
                    {
                        triggerEffectEndEvents = false;
                        TriggerAbilityEffect(ability, e, caster, character, tileTarget);
                    }
                }
            }

            // Apply passive to self
            else if (abilityEffect.effectType == AbilityEffectType.ApplyPassiveSelf)
            {
                int stacks = abilityEffect.perkPairing.passiveStacks;

                // Do probability roll
                bool roll = RandomGenerator.NumberBetween(1, 100) <= abilityEffect.perkApplicationChance;

                if(roll) PerkController.Instance.ModifyPerkOnCharacterEntity(caster.pManager, abilityEffect.perkPairing.perkTag, stacks, true, 0.5f, caster.pManager);
              
            }

            // Apply passive in line
            else if (abilityEffect.effectType == AbilityEffectType.ApplyPassiveInLine)
            {
                HexDirection direction = LevelController.Instance.GetDirectionToTargetHex(caster.currentTile, tileTarget);
                List<LevelNode> hexsOnPath = new List<LevelNode>();
                LevelNode previousHex = caster.currentTile;
                LevelNode nextHex = null;
                List<HexCharacterModel> charactersEffected = new List<HexCharacterModel>();
                List<HexCharacterModel> charactersHit = new List<HexCharacterModel>();

                for (int i = 0; i < abilityEffect.lineLength; i++)
                {
                    nextHex = LevelController.Instance.GetAdjacentHexByDirection(previousHex, direction);
                    if (nextHex == null)
                    {
                        break;
                    }
                    else
                    {
                        // Tile in direction from current tile is valid to move over
                        hexsOnPath.Add(nextHex);
                        previousHex = nextHex;
                    }
                }

                // Filter out enemies or allies where applicable
                foreach (LevelNode h in hexsOnPath)
                {
                    if (h.myCharacter != null)
                    {
                        charactersEffected.Add(h.myCharacter);
                        /*
                        if ((h.myCharacter.allegiance == caster.allegiance && abilityEffect.effectsAllies) ||
                            (h.myCharacter.allegiance != caster.allegiance && abilityEffect.effectsEnemies))
                        {
                            charactersEffected.Add(h.myCharacter);
                        }*/
                    }
                }
                Debug.Log("Characters effected = " + charactersEffected.Count.ToString());

                // Roll for hits + play on Hit animations or on miss animations
                foreach (HexCharacterModel character in charactersEffected)
                {
                    //character.eventStacks.Add(VisualEventManager.Instance.CreateStackParentVisualEvent(character));
                    VisualEventManager.Instance.CreateStackParentVisualEvent(character);
                }

                // Apply Passive
                foreach (HexCharacterModel character in charactersEffected)
                {
                    bool roll = RandomGenerator.NumberBetween(1, 100) <= abilityEffect.perkApplicationChance;
                    bool success = false;

                    if(roll) success = PerkController.Instance.ModifyPerkOnCharacterEntity
                         (character.pManager, abilityEffect.perkPairing.perkTag, abilityEffect.perkPairing.passiveStacks, true, 0f, caster.pManager);

                    if (success) charactersHit.Add(character);
                }

                // On passive succesfully applied visual events
                foreach (HexCharacterModel character in charactersHit)
                {
                    if (CombatController.Instance.CurrentCombatState == CombatGameState.CombatActive &&
                        caster.livingState == LivingState.Alive)
                    {
                        if (character != null && character.livingState == LivingState.Alive)
                        {
                            foreach (AnimationEventData vEvent in abilityEffect.visualEventsOnHit)
                            {
                                AnimationEventController.Instance.PlayAnimationEvent(vEvent, caster, character, null, character.GetLastStackEventParent());
                            }
                        }
                    }
                }
            }

            // Remove passive from target
            else if (abilityEffect.effectType == AbilityEffectType.RemovePassiveTarget)
            {
                int stacks = PerkController.Instance.GetStackCountOfPerkOnCharacter(target.pManager, abilityEffect.perkPairing.perkTag);
                if(stacks > 0)
                    PerkController.Instance.ModifyPerkOnCharacterEntity(target.pManager, abilityEffect.perkPairing.perkTag, -stacks, true, 0.5f, caster.pManager);
            }

            // Gain Energy Target
            else if (abilityEffect.effectType == AbilityEffectType.GainEnergyTarget)
            {
                HexCharacterController.Instance.ModifyEnergy(target, abilityEffect.energyGained, true);
            }

            // Gain Energy Self
            else if (abilityEffect.effectType == AbilityEffectType.GainEnergy)
            {
                HexCharacterController.Instance.ModifyEnergy(caster, abilityEffect.energyGained, true);
            }

            // Lose Health Self
            else if (abilityEffect.effectType == AbilityEffectType.LoseHealthSelf)
            {
                DamageResult damageResult = CombatController.Instance.GetFinalDamageValueAfterAllCalculations(caster, abilityEffect.healthLost, DamageType.None);
                CombatController.Instance.HandleDamage(caster, damageResult, DamageType.None, true);
                //CombatController.Instance.HandleDamage(caster, target, damageResult, null, abilityEffect, caster.myCurrentEventStack);
            }

            // Heal Self
            else if (abilityEffect.effectType == AbilityEffectType.HealSelf)
            {
                // Setup
                HexCharacterView view = caster.hexCharacterView;

                // Gain health
                HexCharacterController.Instance.ModifyHealth(caster, abilityEffect.healthGained);

                // Heal VFX
                VisualEventManager.Instance.CreateVisualEvent(()=> 
                { 
                    VisualEffectManager.Instance.CreateHealEffect(view.WorldPosition);
                    VisualEffectManager.Instance.CreateDamageTextEffect(view.WorldPosition, abilityEffect.healthGained, false, true);
                });

            }

            // Teleport Self
            else if (abilityEffect.effectType == AbilityEffectType.TeleportSelf)
            {
                LevelController.Instance.HandleTeleportCharacter(caster, tileTarget);
            }

            // Teleport Target
            else if (abilityEffect.effectType == AbilityEffectType.TeleportTargetToTile)
            {
                LevelController.Instance.HandleTeleportCharacter(target, tileTarget);
            }

            // Teleport Switch With Target
            else if (abilityEffect.effectType == AbilityEffectType.TeleportSwitchWithTarget)
            {
                LevelController.Instance.HandleTeleportSwitchTwoCharacters(caster, target, abilityEffect.normalTeleportVFX);
            }

            // Teleport Behind Target
            else if (abilityEffect.effectType == AbilityEffectType.TeleportSelfBehindTarget)
            {
                // Try find a valid back arc tile to teleport to
                /*
                LevelNode destination = HexCharacterController.Instance.GetCharacterBackTile(target);
                if (destination)
                {
                    LevelController.Instance.HandleTeleportCharacter(caster, destination);
                    LevelController.Instance.FaceCharacterTowardsHex(caster, target.currentTile);
                }
                */
                
                LevelNode destination = null;
                List<LevelNode> targetBackTiles = HexCharacterController.Instance.GetCharacterBackArcTiles(target);
                List<LevelNode> validTiles = new List<LevelNode>();

                foreach (LevelNode h in targetBackTiles)
                {
                    if (Pathfinder.CanHexBeOccupied(h))
                        validTiles.Add(h);
                }

                if (validTiles.Count == 0) return;
                else if (validTiles.Count == 1) destination = validTiles[0];
                else destination = validTiles[RandomGenerator.NumberBetween(0, validTiles.Count - 1)];

                LevelController.Instance.HandleTeleportCharacter(caster, destination);
                LevelController.Instance.FaceCharacterTowardsHex(caster, target.currentTile);
                
            }

            // Move in line
            else if (abilityEffect.effectType == AbilityEffectType.MoveInLine)
            {
                HexDirection direction = LevelController.Instance.GetDirectionToTargetHex(caster.currentTile, tileTarget);
                LevelNode obstructionHex = null;
                List<LevelNode> hexsOnPath = new List<LevelNode>();
                LevelNode previousHex = caster.currentTile;
                LevelNode nextHex = null;
                //Hex finalDestination = null;

                for (int i = 0; i < abilityEffect.tilesMoved; i++)
                {
                    nextHex = LevelController.Instance.GetAdjacentHexByDirection(previousHex, direction);
                    if (nextHex != null && !Pathfinder.CanHexBeOccupied(nextHex))
                    {
                        // Collision end point found
                        obstructionHex = nextHex;
                        break;
                    }
                    else if (nextHex != null)
                    {
                        // Tile in direction from current tile is valid to move over
                        hexsOnPath.Add(nextHex);
                        previousHex = nextHex;
                    }
                }

                // Move down path
                if (hexsOnPath.Count > 0)
                    LevelController.Instance.HandleMoveDownPath(caster, hexsOnPath);

                // Trigger on collision effects
                if (abilityEffect.chainedEffect == false && obstructionHex != null)
                {
                    foreach (AbilityEffect e in ability.onCollisionEffects)
                    {
                        TriggerAbilityEffect(ability, e, caster, obstructionHex.myCharacter, obstructionHex);
                    }
                }


            }

            // Knock Back
            else if (abilityEffect.effectType == AbilityEffectType.KnockBack)
            {
                // calculate direction of knock back
                HexDirection dir = LevelController.Instance.GetDirectionToTargetHex(caster.currentTile, target.currentTile);

                LevelNode startingTile = target.currentTile;
                LevelNode previousTile = target.currentTile;

                // get knock back tiles
                for (int i = 0; i < abilityEffect.knockBackDistance; i++)
                {                    
                    bool forceBreak = false;
                    foreach (LevelNode h in LevelController.Instance.GetAllHexsWithinRange(previousTile, 1))
                    {
                        if (LevelController.Instance.GetDirectionToTargetHex(previousTile, h) == dir)
                        {
                            // Found next tile in direction
                            if (Pathfinder.CanHexBeOccupied(h))                            
                                previousTile = h;                            
                            else
                            {
                                forceBreak = true;
                                break;
                            }
                        }
                    }

                    if (forceBreak)
                        break;
                }

                // found a hex that is valid to be knocked on to?
                if (startingTile != previousTile)
                {
                    LevelController.Instance.HandleKnockBackCharacter(target, previousTile);
                }
                // Characters that are knocked into on obstacle/player become stunned.
                else
                {
                    Debug.Log("TriggerAbilityEffect() detected collision during knock back effect, applying Stunned to target...");
                    PerkController.Instance.ModifyPerkOnCharacterEntity(target.pManager, Perk.Stunned, 1, true, 0.5f);
                }
            }

            // Summon Character
            else if (abilityEffect.effectType == AbilityEffectType.SummonCharacter)
            {
                // Create character
                HexCharacterModel newSummon = 
                    HexCharacterController.Instance.CreateSummonedHexCharacter(abilityEffect.characterSummoned, tileTarget, caster.allegiance);
                              

                // Disable activation window until ready
                HexCharacterView view = newSummon.hexCharacterView;
                view.myActivationWindow.gameObject.SetActive(false);
                TurnController.Instance.DisablePanelSlotAtIndex(TurnController.Instance.ActivationOrder.IndexOf(newSummon));

                // Hide GUI
                HexCharacterController.Instance.FadeOutCharacterWorldCanvas(view, null, 0);

                // Hide model
                HexCharacterController.Instance.FadeOutCharacterModel(view.ucm, 0);
                HexCharacterController.Instance.FadeOutCharacterShadow(view, 0);
                //view.blockMouseOver = true;

                // Enable activation window
                int windowIndex = TurnController.Instance.ActivationOrder.IndexOf(newSummon);
                VisualEventManager.Instance.CreateVisualEvent(() =>
                {
                    view.myActivationWindow.gameObject.SetActive(true);
                    view.myActivationWindow.Show();
                    TurnController.Instance.EnablePanelSlotAtIndex(windowIndex);
                }, QueuePosition.Back, 0f, 0.1f);

                // Update all window slot positions + activation pointer arrow
                HexCharacterModel entityActivated = TurnController.Instance.EntityActivated;
                VisualEventManager.Instance.CreateVisualEvent(() => TurnController.Instance.UpdateWindowPositions());
                VisualEventManager.Instance.CreateVisualEvent(() => TurnController.Instance.MoveActivationArrowTowardsEntityWindow(entityActivated), QueuePosition.Back);

                // Fade in model + UI
                VisualEventManager.Instance.CreateVisualEvent(() => HexCharacterController.Instance.FadeInCharacterWorldCanvas(view, null, abilityEffect.uiFadeInSpeed));
                VisualEventManager.Instance.CreateVisualEvent(() =>
                {
                    CharacterModeller.FadeInCharacterModel(view.ucm, abilityEffect.modelFadeInSpeed);
                    // CharacterModeller.FadeInCharacterShadow(view, 1f, () => view.blockMouseOver = false);
                    CharacterModeller.FadeInCharacterShadow(view, 1f);
                });

                // Resolve visual events
                foreach (AnimationEventData vEvent in abilityEffect.summonedCreatureVisualEvents)
                {
                    AnimationEventController.Instance.PlayAnimationEvent(vEvent, newSummon, newSummon);
                }


            }


            if (triggerEffectEndEvents)
            {
                // On ability effect completed VFX
                if (CombatController.Instance.CurrentCombatState == CombatGameState.CombatActive &&
                    caster.livingState == LivingState.Alive)
                {
                    if (target != null && target.livingState == LivingState.Alive)
                    {
                        foreach (AnimationEventData vEvent in abilityEffect.visualEventsOnEffectFinish)
                        {
                            AnimationEventController.Instance.PlayAnimationEvent(vEvent, caster, target, null, target.GetLastStackEventParent());
                        }
                    }
                }
            }

        }
        private void OnAbilityUsedStart(HexCharacterModel character, AbilityData ability, HexCharacterModel target = null)
        {
            // Pay Energy Cost
            HexCharacterController.Instance.ModifyEnergy(character, -GetAbilityEnergyCost(character, ability));           

            // Check shed perk: remove if ability used was an aspect ability
            if (ability.abilityName.Contains("Aspect") && PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Shed))
                PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Shed, -1);

            // Increment skills used this turn
            if (ability.abilityType == AbilityType.Skill)            
                character.skillAbilitiesUsedThisTurn++;
            else if (ability.abilityType == AbilityType.RangedAttack)
                character.rangedAttackAbilitiesUsedThisTurn++;
            else if (ability.abilityType == AbilityType.MeleeAttack)
                character.meleeAttackAbilitiesUsedThisTurn++;

        }
        private void OnAbilityUsedFinish(HexCharacterModel character, AbilityData ability, HexCharacterModel target = null)
        {
            SetAbilityOnCooldown(ability);
            if(PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Stealth) &&
                ability.doesNotBreakStealth == false)
            {
                PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Stealth, -1);
            }
        }
        private void SetAbilityOnCooldown(AbilityData ability)
        {
            ability.currentCooldown = ability.baseCooldown;
            if (ability.myCharacter != null &&
                TurnController.Instance.EntityActivated == ability.myCharacter &&
                ability.myCharacter.controller == Controller.Player)
            { 
                foreach(AbilityButton b in CombatUIController.Instance.AbilityButtons)
                {
                    if(b.MyAbilityData == ability)
                    {
                        b.UpdateAbilityButtonCooldownView();
                        break;
                    }
                }
            }
            
        }
        public void ReduceCharacterAbilityCooldownsOnTurnStart(HexCharacterModel character)
        {
            foreach (AbilityData a in character.abilityBook.activeAbilities)
            {
                if (a.currentCooldown > 0)
                {
                    a.currentCooldown += -1;
                }
            }
        }


        #endregion

        // Input
        #region
        public void OnAbilityButtonClicked(AbilityButton b)
        {
            if (b.MyAbilityData == null) return;
            if (IsAbilityUseable(b.MyAbilityData.myCharacter, b.MyAbilityData))
            {
                // clear move selection state + views
                MoveActionController.Instance.ResetSelectionState(false);

                // start ability usage part 1 !!
                HandleAbilityButtonClicked(b);
            }
        }
        private void HandleAbilityButtonClicked(AbilityButton b)
        {
            HexCharacterModel caster = b.MyAbilityData.myCharacter;
            AbilityData ability = b.MyAbilityData;

            if (currentAbilityAwaiting != null || ability.targetRequirement == TargetRequirement.NoTarget)
            {
                // player clicked a different ability, handle new selection
                LevelController.Instance.UnmarkAllTiles();
            }

            // cache ability, get ready for second click, or for instant use
            currentAbilityAwaiting = b.MyAbilityData;
            currentSelectionPhase = AbilitySelectionPhase.None;

            // Highlight tiles in range of ability
            if (ability.targetRequirement != TargetRequirement.NoTarget)
            {
                bool neutral = true;
                if (ability.targetRequirement == TargetRequirement.Enemy) neutral = false;
                LevelController.Instance.MarkTilesInRange(GetTargettableTilesOfAbility(ability, caster), neutral);
            }
            else if (ability.targetRequirement == TargetRequirement.NoTarget)
            {
                UseAbility(caster, currentAbilityAwaiting);
                HandleCancelCurrentAbilityOrder();
            }
        }
        public void HandleTargetSelectionMade(HexCharacterModel target)
        {
            HexCharacterModel caster = currentAbilityAwaiting.myCharacter;
            if (IsTargetOfAbilityValid(caster, target, currentAbilityAwaiting))
            {
                // multiple target selection abilities (e.g. telekinesis, phase shift, etc)
                if (currentAbilityAwaiting.secondaryTargetRequirement != SecondaryTargetRequirement.None &&
                    currentSelectionPhase == AbilitySelectionPhase.None)
                {
                    // Player has made their first selection, and the selection is valid, prepare for the 2nd selection
                    if (currentAbilityAwaiting.secondaryTargetRequirement == SecondaryTargetRequirement.UnoccupiedHexWithinRangeOfTarget)
                    {
                        List<LevelNode> validHexs = new List<LevelNode>();
                        foreach (LevelNode h in LevelController.Instance.GetAllHexsWithinRange(target.currentTile, currentAbilityAwaiting.rangeFromTarget))
                        {
                            if (Pathfinder.CanHexBeOccupied(h))
                                validHexs.Add(h);
                        }

                        // If player selected a target with no valid adjacent tiles, cancel the ability selection process
                        if (validHexs.Count == 0)
                        {
                            return;
                        }
                        else
                        {
                            // Get ready for second selection
                            currentSelectionPhase = AbilitySelectionPhase.First;
                            LevelController.Instance.UnmarkAllTiles();
                            bool neutral = true;
                            if (currentAbilityAwaiting.targetRequirement == TargetRequirement.Enemy) neutral = false;
                            LevelController.Instance.MarkTilesInRange(validHexs, neutral);
                            firstSelectionCharacter = target;
                        }
                    }

                }

                // single target selection abilities
                else
                {
                    UseAbility(caster, currentAbilityAwaiting, target);
                    HandleCancelCurrentAbilityOrder();
                }

            }
        }
        public void HandleTargetSelectionMade(LevelNode target)
        {
            // overload function for abilities that target a hex, not a character (e.g. teleport to location, throw grenade at location, etc)
            HexCharacterModel caster = currentAbilityAwaiting.myCharacter;
            if (currentSelectionPhase == AbilitySelectionPhase.None && IsTargetOfAbilityValid(caster, target, currentAbilityAwaiting))
            {
                UseAbility(caster, currentAbilityAwaiting, null, target);
                HandleCancelCurrentAbilityOrder();
            }
            else if (currentSelectionPhase == AbilitySelectionPhase.First)
            {
                // Secondary target that must be an unoccupied hex with range of the first character selection
                if (currentAbilityAwaiting.secondaryTargetRequirement == SecondaryTargetRequirement.UnoccupiedHexWithinRangeOfTarget)
                {
                    // is the tile selected valid?
                    if (Pathfinder.CanHexBeOccupied(target) && LevelController.Instance.GetAllHexsWithinRange(firstSelectionCharacter.currentTile, currentAbilityAwaiting.rangeFromTarget).Contains(target))
                    {
                        // it is, use the ability

                        // make sure that the target is teleportable if the effect is a teleport
                        if (currentAbilityAwaiting.abilityEffects.Count > 0 &&
                            currentAbilityAwaiting.abilityEffects[0].effectType == AbilityEffectType.TeleportTargetToTile &&
                            !HexCharacterController.Instance.IsCharacterTeleportable(firstSelectionCharacter))
                        {
                            return;
                        }

                        UseAbility(caster, currentAbilityAwaiting, firstSelectionCharacter, target);
                        HandleCancelCurrentAbilityOrder();
                    }
                }
            }
        }
        private void HandleCancelCurrentAbilityOrder()
        {
            currentAbilityAwaiting = null;
            firstSelectionCharacter = null;
            currentSelectionPhase = AbilitySelectionPhase.None;
            LevelController.Instance.UnmarkAllTiles();
        }
        private void Update()
        {
            if (GameController.Instance.GameState != GameState.CombatActive) return;

            if (Input.GetKeyDown(KeyCode.Mouse1))
                HandleCancelCurrentAbilityOrder();

            if (hitChanceVisualParent.activeSelf == true &&
               hitChancePositionParent.transform.position != LevelController.HexMousedOver.WorldPosition)
                hitChancePositionParent.transform.position = LevelController.HexMousedOver.WorldPosition;
        }
        #endregion

        // Get Ability Calculations
        #region
        public int GetAbilityEnergyCost(HexCharacterModel character, AbilityData ability)
        {
            int energyCost = ability.energyCost;

            // Check shed perk: aspect ability costs 0
            if (ability.abilityName.Contains("Aspect") && PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Shed))
                return 0;

            // Check resolute passive
            if (character != null &&
                ability.abilityType == AbilityType.Skill &&
                character.skillAbilitiesUsedThisTurn == 0 &&
                PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Resolute))
            {
                energyCost -= 2;
            }

            // Check Quick Draw passive
            else if (character != null &&
                ability.abilityType == AbilityType.RangedAttack &&
                character.rangedAttackAbilitiesUsedThisTurn == 0 &&
                PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.QuickDraw))
            {
                energyCost -= PerkController.Instance.GetStackCountOfPerkOnCharacter(character.pManager, Perk.QuickDraw);
            }

            // Check Finesse passive
            else if (character != null &&
                ability.abilityType == AbilityType.MeleeAttack &&
                character.meleeAttackAbilitiesUsedThisTurn == 0 &&
                PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Finesse))
            {
                energyCost -= PerkController.Instance.GetStackCountOfPerkOnCharacter(character.pManager, Perk.Finesse);
            }

            // Check Arms Master passive
            else if (character != null &&
                (ability.derivedFromWeapon || ability.derivedFromItemLoadout) &&
                PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.ArmsMaster))
            {
                energyCost -= PerkController.Instance.GetStackCountOfPerkOnCharacter(character.pManager, Perk.ArmsMaster);
            }

            // prevent cost going negative
            if (energyCost < 0)
                energyCost = 0;

            return energyCost;
        }
        public List<LevelNode> GetTargettableTilesOfAbility(AbilityData ability, HexCharacterModel caster)
        {
            List<LevelNode> targettableTiles = new List<LevelNode>();
            bool includeSelfTile = false;
            if (ability.targetRequirement == TargetRequirement.AllyOrSelf || ability.targetRequirement == TargetRequirement.AllCharacters)            
                includeSelfTile = true;            

            targettableTiles = LevelController.Instance.GetAllHexsWithinRange(caster.currentTile, CalculateFinalRangeOfAbility(ability, caster), includeSelfTile);

            // Remove occupied tiles for self teleportation effects or summon character effects
            AbilityEffect freeTileRequirementEffect = null;
            foreach (AbilityEffect e in ability.abilityEffects)
            {
                if (e.effectType == AbilityEffectType.TeleportSelf ||
                    e.effectType == AbilityEffectType.SummonCharacter)
                {
                    freeTileRequirementEffect = e;
                    break;
                }
            }
            if (freeTileRequirementEffect != null)
            {
                List<LevelNode> newValidTiles = new List<LevelNode>();
                foreach (LevelNode h in targettableTiles)
                {
                    if (Pathfinder.CanHexBeOccupied(h))
                    {
                        newValidTiles.Add(h);
                    }
                }

                targettableTiles = newValidTiles;
            }

            return targettableTiles;
        }
        public int CalculateFinalRangeOfAbility(AbilityData ability, HexCharacterModel caster)
        {
            Debug.Log("AbilityController.CalculateFinalRangeOfAbility() called, calculating range of ability '" + ability.abilityName +
                "' used by character " + caster.myName);

            int rangeReturned = ability.baseRange;

            if ((ability.abilityType == AbilityType.RangedAttack || ability.abilityType == AbilityType.Skill) && ability.gainRangeBonusFromVision)
            {
                rangeReturned += StatCalculator.GetTotalVision(caster);

                // Check elevation range bonus
                if (caster.currentTile.Elevation == TileElevation.Elevated)
                   rangeReturned += 1;
            }

            if (rangeReturned < 1) rangeReturned = 1;

            Debug.Log("Final calculated range of '" + ability.abilityName + "' is " + rangeReturned.ToString());
            return rangeReturned;
        }
        #endregion

        // Ability Useability + Validation Logic
        #region
        private bool DoesAbilityEffectMeetAllRequirements(AbilityData abilityUsed, AbilityEffect effect, HexCharacterModel character, HexCharacterModel target = null, LevelNode location = null)
        {
            bool bRet = true;

            foreach (AbilityEffectRequirement req in effect.requirements)
            {
                if (req.requirementType == AbilityEffectRequirementType.BackStrike)
                {
                    if (!HexCharacterController.Instance.GetCharacterBackArcTiles(target).Contains(character.currentTile))
                    {
                        Debug.Log("AbilityController.DoesAbilityEffectMeetAllRequirements() failed back strike requirement check...");
                        bRet = false;
                        break;
                    }

                }
                else if (req.requirementType == AbilityEffectRequirementType.TargetHasPerk)
                {
                    if (!PerkController.Instance.DoesCharacterHavePerk(target.pManager, req.perk))
                    {
                        Debug.Log("AbilityController.DoesAbilityEffectMeetAllRequirements() failed 'Target Has Perk' requirement check (required" + req.perk.ToString() +").");
                        bRet = false;
                        break;
                    }
                }

                else if (req.requirementType == AbilityEffectRequirementType.CasterHasPerk)
                {
                    if (!PerkController.Instance.DoesCharacterHavePerk(character.pManager, req.perk))
                    {
                        Debug.Log("AbilityController.DoesAbilityEffectMeetAllRequirements() failed 'Caster Has Perk' requirement check (required" + req.perk.ToString() + ").");
                        bRet = false;
                        break;
                    }
                }

                else if (req.requirementType == AbilityEffectRequirementType.TargetDoesNotHavePerk)
                {
                    if (PerkController.Instance.DoesCharacterHavePerk(target.pManager, req.perk))
                    {
                        Debug.Log("AbilityController.DoesAbilityEffectMeetAllRequirements() failed 'Target Does Not Have Perk' requirement check (required" + req.perk.ToString() + ").");
                        bRet = false;
                        break;
                    }
                }
                else if (req.requirementType == AbilityEffectRequirementType.CasterDoesNotHavePerk)
                {
                    if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, req.perk))
                    {
                        Debug.Log("AbilityController.DoesAbilityEffectMeetAllRequirements() failed 'Caster Does Not Have Perk' requirement check (required" + req.perk.ToString() + ").");
                        bRet = false;
                        break;
                    }
                }
            }

            return bRet;
        }
        public bool IsTargetOfAbilityValid(HexCharacterModel caster, HexCharacterModel target, AbilityData ability)
        {
            if (ability.targetRequirement == TargetRequirement.NoTarget) return true;

            Debug.Log("AbilityController.IsTargetOfAbilityValid() called, validating '" + ability.abilityName + "' usage by character " + caster.myName +
                " against target " + target.myName);           

            // Function used AFTER "IsAbilityUseable" to check if the selected target of the ability is valid.
            bool bRet = false;

            // check target is in range
            if (IsTargetOfAbilityInRange(caster, target, ability) &&
                DoesTargetOfAbilityMeetAllegianceRequirement(caster, target, ability) &&
                DoesTargetOfAbilityMeetSubRequirements(caster, target, ability))
            {
                bRet = true;
            }

            // check target allegiance validity (e.g. friendly spell cant target enemy, damage spell cant target ally, etc)

            Debug.Log("AbilityController.IsTargetOfAbilityValid() returning " + bRet.ToString());
            return bRet;
        }
        private bool IsTargetOfAbilityValid(HexCharacterModel caster, LevelNode target, AbilityData ability)
        {
            if (ability.targetRequirement == TargetRequirement.NoTarget) return true;

            Debug.Log("AbilityController.IsTargetOfAbilityValid() called, validating '" + ability.abilityName + "' usage by character " + caster.myName +
                " on hex tile " + target.GridPosition.x.ToString() + ", " + target.GridPosition.y.ToString());        

            // Function used AFTER "IsAbilityUseable" to check if the selected target of the ability is valid.
            bool bRet = false;

            // check target is in range
            if (IsTargetOfAbilityInRange(caster, target, ability))
            {
                bRet = true;
            }

            Debug.Log("AbilityController.IsTargetOfAbilityValid() returning " + bRet.ToString());
            return bRet;
        }
        public bool IsAbilityUseable(HexCharacterModel character, AbilityData ability)
        {
            // This called when the player first clicks an ability button
            Debug.Log("AbilityController.IsAbilityUseable() called, validating '" + ability.abilityName + "' usage by character " + character.myName);

            bool bRet = true;

            // check its actually characters turn
            if (TurnController.Instance.EntityActivated != character ||
                (TurnController.Instance.EntityActivated == character && character.activationPhase != ActivationPhase.ActivationPhase))
            {
                Debug.Log("IsAbilityUseable() returning false: cannot use abilities when it is not your turn");
                bRet = false;
            }

            // Check movement impairment
            foreach (AbilityEffect ef in ability.abilityEffects)
            {
                if ((ef.effectType == AbilityEffectType.MoveInLine ||
                   ef.effectType == AbilityEffectType.MoveToTile) &&
                   !HexCharacterController.Instance.IsCharacterAbleToMove(character))
                {
                    Debug.Log("IsAbilityUseable() returning false: cannot take movement actions while rooted");
                    bRet = false;
                }
            }

            // check character is alive
            if (character.currentHealth <= 0 || character.livingState == LivingState.Dead)
            {
                Debug.Log("IsAbilityUseable() returning false: character is dead or in the death sequence");
                bRet = false;
            }

            // check has enough energy
            if (!DoesCharacterHaveEnoughEnergy(character, ability))
            {
                Debug.Log("IsAbilityUseable() returning false: not enough energy");
                bRet =  false;
            }

            // check cooldown
            if (ability.currentCooldown != 0 )
            {
                Debug.Log("IsAbilityUseable() returning false: ability is on cooldown");
                bRet = false;
            }

            // check weapon requirement
            if (!DoesCharacterMeetAbilityWeaponRequirement(character, ability))
            {
                Debug.Log("IsAbilityUseable() returning false: character does not meet the ability's weapon requirement");
                bRet = false;
            }

            


            return bRet;
        }
        public bool DoesCharacterMeetAbilityWeaponRequirement(HexCharacterModel character, AbilityData ability)
        {
            bool bRet = false;

            if (ability.weaponRequirement == WeaponRequirement.None)
                bRet = true;

            else if (ability.weaponRequirement == WeaponRequirement.MeleeWeapon &&
                character.itemSet.mainHandItem != null &&
                character.itemSet.mainHandItem.IsMeleeWeapon)
                bRet = true;

            else if (ability.weaponRequirement == WeaponRequirement.RangedWeapon &&
                character.itemSet.mainHandItem != null &&
                character.itemSet.mainHandItem.IsRangedWeapon)
                bRet = true;

            else if (ability.weaponRequirement == WeaponRequirement.Shield &&
               character.itemSet.offHandItem != null &&
               character.itemSet.offHandItem.weaponClass == WeaponClass.Shield)
                bRet = true;

            else if (ability.weaponRequirement == WeaponRequirement.Bow &&
              character.itemSet.mainHandItem != null &&
              character.itemSet.mainHandItem.weaponClass == WeaponClass.Bow)
                bRet = true;

            if(bRet == false)
            {
                Debug.Log("AbilityController.DoesCharacterMeetAbilityWeaponRequirement() returning false: '" + ability.abilityName +
                    "' has requirement of " + ability.weaponRequirement.ToString() + ", character '" + character.myName + "' does not meet this requirement");
            }

            return bRet;
        }
        private bool DoesCharacterHaveEnoughEnergy(HexCharacterModel caster, AbilityData ability)
        {
            return caster.currentEnergy >= GetAbilityEnergyCost(caster, ability);
        }
        private bool IsTargetOfAbilityInRange(HexCharacterModel caster, HexCharacterModel target, AbilityData ability)
        {
            bool bRet = false;
            bRet = GetTargettableTilesOfAbility(ability, caster).Contains(target.currentTile);

            int stealthDistance = StatCalculator.GetTotalVision(caster) + 1;
            if (stealthDistance < 1) stealthDistance = 1;

            // Check stealth + eagle eye (EE ignores stealth)
            if (caster.currentTile.Distance(target.currentTile) > stealthDistance &&
                HexCharacterController.Instance.IsTargetFriendly(caster, target) == false &&
                PerkController.Instance.DoesCharacterHavePerk(target.pManager, Perk.Stealth) &&                
                !PerkController.Instance.DoesCharacterHavePerk(caster.pManager, Perk.TrueSight))
                bRet = false;

            return bRet;
        }
        private bool IsTargetOfAbilityInRange(HexCharacterModel caster, LevelNode target, AbilityData ability)
        {
            return GetTargettableTilesOfAbility(ability, caster).Contains(target);
        }
        private bool DoesTargetOfAbilityMeetAllegianceRequirement(HexCharacterModel caster, HexCharacterModel target, AbilityData ability)
        {
            Debug.Log("AbilityController.DoesTargetOfAbilityMeetAllegianceRequirement() called, validating '" + ability.abilityName + "' usage by character " + caster.myName +
                " against target " + target.myName);

            bool bRet = false;

            List<HexCharacterModel> allies = HexCharacterController.Instance.GetAllAlliesOfCharacter(caster, false);
            List<HexCharacterModel> enemies = HexCharacterController.Instance.GetAllEnemiesOfCharacter(caster);

            if(ability.targetRequirement == TargetRequirement.Ally &&
                allies.Contains(target))
            {
                bRet = true;
            }
            else if (ability.targetRequirement == TargetRequirement.AllyOrSelf &&
               (allies.Contains(target) || caster == target))
            {
                bRet = true;
            }
            else if(ability.targetRequirement == TargetRequirement.Enemy &&
                enemies.Contains(target))
            {
                bRet = true;
            }
            else if (ability.targetRequirement == TargetRequirement.AllCharacters &&
                target != null)
            {
                bRet = true;
            }
            else if (ability.targetRequirement == TargetRequirement.AllCharactersExceptSelf &&
               target != caster)
            {
                bRet = true;
            }

            Debug.Log("AbilityController.DoesTargetOfAbilityMeetAllegianceRequirement() returning " + bRet);
            return bRet;

        }
        private bool DoesTargetOfAbilityMeetSubRequirements(HexCharacterModel caster, HexCharacterModel target, AbilityData ability)
        {
            bool bRet = false;

            if(ability.abilitySubRequirements.Count == 0)
            {
                Debug.Log("DoesTargetOfAbilityMeetSubRequirements() ability has n sub requirements, auto-passing...");
                bRet = true;
            }
            else
            {
                bool pass = false;

                foreach (AbilityRequirement ar in ability.abilitySubRequirements)
                {
                    if (ar.type == AbilityRequirementType.TargetHasUnoccupiedBackTile)
                    {
                        foreach (LevelNode n in HexCharacterController.Instance.GetCharacterBackArcTiles(target))
                        {
                            if (Pathfinder.CanHexBeOccupied(n))
                            {
                                Debug.Log("DoesTargetOfAbilityMeetSubRequirements() passed 'TargetHasAnUnoccupiedBackTile' check");
                                pass = true;
                                break;
                            }
                        }
                    }

                    else if (ar.type == AbilityRequirementType.TargetIsTeleportable &&
                        HexCharacterController.Instance.IsCharacterTeleportable(target))
                    {
                        pass = true;
                    }

                    else if (ar.type == AbilityRequirementType.CasterIsTeleportable &&
                        HexCharacterController.Instance.IsCharacterTeleportable(caster))
                    {
                        pass = true;
                    }
                    else if (ar.type == AbilityRequirementType.CasterHasEnoughHealth &&
                       caster.currentHealth >= ar.healthRequired)
                    {
                        pass = true;
                    }

                    // if failed a check, dont bother checking the other requirements
                    if (pass == false) break;                    
                }

                if (!pass) bRet = false;
                else if (pass) bRet = true;
            }

            Debug.Log("AbilityController.DoesTargetOfAbilityMeetSubRequirements() returning " + bRet);
            return bRet;
        }
        public bool AwaitingAbilityOrder()
        {
            return currentAbilityAwaiting != null;
        }
        #endregion

        // Hit Chance Popup Logic
        #region
        public void ShowHitChancePopup(HexCharacterModel caster, HexCharacterModel target, AbilityData ability)
        {

            if (target != null &&
                target.allegiance != caster.allegiance &&
                caster.activationPhase == ActivationPhase.ActivationPhase &&
                ability != null &&
                ability.targetRequirement == TargetRequirement.Enemy &&
                (ability.abilityType == AbilityType.MeleeAttack || ability.abilityType == AbilityType.RangedAttack))
            {
                hitChanceCg.alpha = 0;
                hitChanceVisualParent.SetActive(true);
                hitChanceCg.DOFade(1, 0.5f);
                HitChanceDataSet hitChanceData = CombatController.Instance.GetHitChance(caster, target, ability);
                hitChanceData.details = hitChanceData.details.OrderByDescending(x => x.accuracyMod).ToList();
                BuildHitChancePopup(hitChanceData);
                foreach(RectTransform t in hitChanceLayouts)
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(t);
                }
                // Obstruction Indicator Logic
                /*
                if(ability.abilityType == AbilityType.RangedAttack)
                {
                    Hex obstructionHex = LevelController.Instance.IsTargetObstructed(caster, target);
                    if(obstructionHex != null)
                    {
                        LevelController.Instance.ShowObstructionIndicator(target.myCurrentHex, obstructionHex);
                    }
                }
                */
            }

          
        }
        private void BuildHitChancePopup(HitChanceDataSet data)
        {
            // Header text
            hitChanceText.text = TextLogic.ReturnColoredText(data.FinalHitChance.ToString() + "%", TextLogic.neutralYellow) + " chance to hit";

            // Reset tabs
            for(int i = 0; i < hitChanceBoxes.Length; i++)
            {
                hitChanceBoxes[i].gameObject.SetActive(false);
            }
            for(int i = 0; i < data.details.Count; i++)
            {
                string extra = data.details[i].accuracyMod > 0 ? "+" : "";
                if (i == hitChanceBoxes.Length - 1) break;
                hitChanceBoxes[i].Build(data.details[i].reason + ": " + 
                    TextLogic.ReturnColoredText(extra + data.details[i].accuracyMod.ToString(), 
                    TextLogic.neutralYellow), data.details[i].accuracyMod > 0 ? DotStyle.Green : DotStyle.Red);
            }
        }
        public void HideHitChancePopup()
        {
            hitChanceCg.alpha = 1;
            hitChanceVisualParent.SetActive(false);
            hitChanceCg.alpha = 0;
        }
        #endregion

        // Dynamic Description Logic
        #region
        public string GetDynamicDescriptionFromAbility(AbilityData ability)
        {
            string sRet = "";
            HexCharacterModel character = ability.myCharacter;

            foreach(CustomString cs in ability.dynamicDescription)
            {
                // Does the custom string even have a dynamic value?
                if (cs.getPhraseFromAbilityValue == false)
                    sRet += TextLogic.ConvertCustomStringToString(cs);
                else
                {
                    // It does, start searching for an ability effect that
                    // matches the effect value of the custom string

                    AbilityEffect matchingEffect = null;
                    foreach (AbilityEffect effect in ability.abilityEffects)
                    {
                        if (effect.effectType == cs.abilityEffectType)
                        {
                            // Found a match, cache it and break
                            matchingEffect = effect;
                            break;
                        }
                    }

                    // Check on hit effects for matching effects if couldnt find them in "abilities effects" list
                    if(matchingEffect == null)
                    {
                        foreach (AbilityEffect effect in ability.onHitEffects)
                        {
                            if (effect.effectType == cs.abilityEffectType)
                            {
                                // Found a match, cache it and break
                                matchingEffect = effect;
                                break;
                            }
                        }
                    }

                    // Check on hit effects for matching effects if couldnt find them in "abilities effects" list
                    if (matchingEffect == null)
                    {
                        foreach (AbilityEffect effect in ability.onCritEffects)
                        {
                            if (effect.effectType == cs.abilityEffectType)
                            {
                                // Found a match, cache it and break
                                matchingEffect = effect;
                                break;
                            }
                        }
                    }
                    // Check on collision effects for matching effects if couldnt find them in "abilities effects" list
                    if (matchingEffect == null)
                    {
                        foreach (AbilityEffect effect in ability.onCollisionEffects)
                        {
                            if (effect.effectType == cs.abilityEffectType)
                            {
                                // Found a match, cache it and break
                                matchingEffect = effect;
                                break;
                            }
                        }


                    }

                    // Damage Target
                    if (cs.abilityEffectType == AbilityEffectType.DamageTarget ||
                        cs.abilityEffectType == AbilityEffectType.DamageAoe)
                    {
                        if (character != null)
                        {
                            DamageResult dr = CombatController.Instance.GetFinalDamageValueAfterAllCalculations(character, null, ability, matchingEffect, false);
                            sRet += TextLogic.ReturnColoredText(dr.damageLowerLimit.ToString(), TextLogic.blueNumber) + " - " +
                                TextLogic.ReturnColoredText(dr.damageUpperLimit.ToString(), TextLogic.blueNumber);

                        }
                        else
                        {
                            sRet += TextLogic.ReturnColoredText(matchingEffect.minBaseDamage.ToString(), TextLogic.blueNumber) + " - " +
                                TextLogic.ReturnColoredText(matchingEffect.maxBaseDamage.ToString(), TextLogic.blueNumber);
                        }
                    }
                }

               
            }

            return sRet;
        }
        #endregion
    }
}