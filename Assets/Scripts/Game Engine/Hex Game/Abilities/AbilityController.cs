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
using System.Linq;
using System;
using HexGameEngine.Items;
using HexGameEngine.Audio;
using UnityEngine.TextCore.Text;

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
        [SerializeField] private AbilityDataSO spearWallStrikeAbilityData;
        private AbilityData[] allAbilities;

        [Header("Hit Chance Pop Up Components")]
        [SerializeField] Canvas hitChanceRootCanvas;
        [SerializeField] GameObject hitChancePositionParent;
        [SerializeField] CanvasGroup hitChanceCg;
        [SerializeField] RectTransform[] hitChanceLayouts;
        [Space(10)]
        [SerializeField] GameObject hitChanceHeaderParent;
        [SerializeField] TextMeshProUGUI hitChanceText;
        [SerializeField] GameObject hitChanceBoxesParent;
        [SerializeField] ModalDottedRow[] hitChanceBoxes;
        [Space(10)]
        [SerializeField] GameObject applyPassiveHeaderParent;
        [SerializeField] TextMeshProUGUI applyPassiveText;
        [SerializeField] UIPerkIcon applyPassiveIcon;
        [SerializeField] GameObject applyPassiveBoxesParent;
        [SerializeField] ModalDottedRow[] applyPassiveBoxes;

        

        // Ability caching properties
        private AbilityData currentAbilityAwaiting;
        private AbilitySelectionPhase currentSelectionPhase;
        private HexCharacterModel firstSelectionCharacter;
        #endregion

        // Getters + Accessors
        #region
        public bool HitChanceModalIsVisible
        {
            get { return hitChanceRootCanvas.isActiveAndEnabled; }
        }
        public AbilityData SpearWallStrikeAbility
        {
            get; private set;
        }
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
                if (dataSO != null && dataSO.includeInGame)
                    tempList.Add(BuildAbilityDataFromScriptableObjectData(dataSO));
            }

            AllAbilities = tempList.ToArray();

            // Setup free strike + globally shared abilities
            FreeStrikeAbility = BuildAbilityDataFromScriptableObjectData(freeStrikeAbilityData);
            RiposteAbility = BuildAbilityDataFromScriptableObjectData(riposteAbilityData);
            SpearWallStrikeAbility = BuildAbilityDataFromScriptableObjectData(spearWallStrikeAbilityData);

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
            a.displayedName = d.displayedName;
            a.baseAbilityDescription = d.baseAbilityDescription;

            a.abilityType = d.abilityType;
            a.doesNotBreakStealth = d.doesNotBreakStealth;
            a.guidanceInstruction = d.guidanceInstruction;
            a.guidanceInstructionTwo = d.guidanceInstructionTwo;
            a.weaponAbilityType = d.weaponAbilityType;
            a.derivedFromWeapon = d.derivedFromWeapon;
            a.derivedFromItemLoadout = d.derivedFromItemLoadout;
            a.weaponClass = d.weaponClass;
            a.targetRequirement = d.targetRequirement;
            a.weaponRequirement = d.weaponRequirement;
            a.talentRequirementData = d.talentRequirementData;

            a.energyCost = d.energyCost;
            a.fatigueCost = d.fatigueCost;
            a.baseCooldown = d.baseCooldown;

            a.baseRange = d.baseRange;
            a.gainRangeBonusFromVision = d.gainRangeBonusFromVision;
            a.hitChanceModifier = d.hitChanceModifier;
            a.hitChanceModifierAgainstAdjacent = d.hitChanceModifierAgainstAdjacent;
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
                a.myCharacter = character;         
            

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
                VisualEventManager.CreateVisualEvent(() =>
                VisualEffectManager.Instance.CreateStatusEffect(view.WorldPosition, ability.abilityName, ability.AbilitySprite, StatusFrameType.SquareBrown)).SetEndDelay(1f);
            }

            // Hide pop up
            HideHitChancePopup();

            // Update UI energy bar
            if (TurnController.Instance.EntityActivated == character && character.controller == Controller.Player)
            {
                CombatUIController.Instance.EnergyBar.UpdateIcons(TurnController.Instance.EntityActivated.currentEnergy);
                CombatUIController.Instance.ResetFatigueCostPreview();
            }                

            OnAbilityUsedStart(character, ability, target);

            // Face target
            if (target != null && target != character && ability.abilityType.Contains(AbilityType.Skill) == false)
                LevelController.Instance.FaceCharacterTowardsTargetCharacter(character, target);
            else if (tileTarget != null && ability.abilityType.Contains(AbilityType.Skill) == false)
                LevelController.Instance.FaceCharacterTowardsHex(character, tileTarget);

            foreach (AbilityEffect e in ability.abilityEffects)
            {
                TriggerAbilityEffect(ability, e, character, target, tileTarget);
            }

            
            OnAbilityUsedFinish(character, ability);

            // Check for removal of damage/accuracy related tokens
            if(ability.abilityType.Contains(AbilityType.MeleeAttack) ||
                ability.abilityType.Contains(AbilityType.RangedAttack) ||
                ability.abilityType.Contains(AbilityType.WeaponAttack))
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

                // Weapon attack specific
                if (ability.abilityType.Contains(AbilityType.WeaponAttack))
                {
                    if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.PoisonedWeapon))
                        PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.PoisonedWeapon, -1);
                    if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.FlamingWeapon))
                        PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.FlamingWeapon, -1);
                }

                // Check and apply furiously assault to target
                if(target != null && PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.FuriousAssault))                
                    PerkController.Instance.ModifyPerkOnCharacterEntity(target.pManager, Perk.FuriouslyAssaulted, 1);                
            }

            // Item 'on use' apply perk to self effects
            if (ability != null &&
                ability.abilityType.Contains(AbilityType.WeaponAttack) &&
                character.itemSet.mainHandItem != null)
            {
                foreach (ActivePerk perk in ItemController.Instance.GetInnateOnUseActivePerksFromItem(character.itemSet.mainHandItem))
                {
                    Debug.Log("Should apply perk on innate weapon usage");
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, perk.perkTag, perk.stacks);
                }
            }

            // Tie off and block any stack visual events on all characters
            foreach (HexCharacterModel c in HexCharacterController.Instance.AllCharacters)
            {
                VisualEvent stack = c.GetLastStackEventParent();
                if (stack != null)
                {
                    Debug.Log("Closing stack event parent");
                    stack.isClosed = true;
                }
            }

            // Update Ability buttons: show/hide if useable or unuseable
            if (TurnController.Instance.EntityActivated == character && character.controller == Controller.Player)
            {
                // Update ability button validity overlays
                foreach (AbilityButton b in CombatUIController.Instance.AbilityButtons)
                    b.UpdateAbilityButtonUnusableOverlay();
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

            // Determine weapon used
            ItemData weaponUsed = null;
            if (abilityEffect.weaponUsed == WeaponSlot.Offhand &&
                caster.itemSet.offHandItem != null)
                weaponUsed = caster.itemSet.offHandItem;

            else if (abilityEffect.weaponUsed == WeaponSlot.MainHand &&
                caster.itemSet.mainHandItem != null)
                weaponUsed = caster.itemSet.mainHandItem;

            // Dynamic weapon used check: make sure the ability actually belongs to the main hand weapon if directed.
            if (ability.abilityType.Contains(AbilityType.WeaponAttack) &&
                weaponUsed == caster.itemSet.mainHandItem &&
                caster.itemSet.offHandItem != null &&
                ability.derivedFromWeapon &&
                ability.weaponRequirement != WeaponRequirement.None)
            {
                bool foundMatch = false;
                foreach (AbilityData weaponAbility in caster.itemSet.mainHandItem.grantedAbilities)
                {
                    if (weaponAbility.abilityName == ability.abilityName)
                    {
                        foundMatch = true;
                        break;
                    }
                }

                if (!foundMatch)
                {
                    Debug.Log(String.Format("AbilityController.TriggerAbilityEffect() weapon derived ability '{0}' does not exist on weapon '{1}', using off hand weapon instead", ability.abilityName, caster.itemSet.mainHandItem.itemName));
                    weaponUsed = caster.itemSet.offHandItem;
                }
            }

            // Queue starting anims and particles
            foreach (AnimationEventData vEvent in abilityEffect.visualEventsOnStart)
            {
                // if effect is not chained
                if (abilityEffect.chainedEffect == false)
                    AnimationEventController.Instance.PlayAnimationEvent(vEvent, caster, target, tileTarget, weaponUsed);

                // if effect is chained, the effect starts from previous chain target
                else if (abilityEffect.chainedEffect == true)
                    AnimationEventController.Instance.PlayAnimationEvent(vEvent, previousChainTarget, target, tileTarget, weaponUsed);
            }

            // RESOLVE EFFECT LOGIC START!

            // Damage Target
            if (abilityEffect.effectType == AbilityEffectType.DamageTarget)
            {
                triggerEffectEndEvents = false;
                HitRoll hitResult = CombatController.Instance.RollForHit(caster, target, ability, weaponUsed);
                bool didCrit = CombatController.Instance.RollForCrit(caster, target, ability, abilityEffect);

                if (hitResult.Result == HitRollResult.Hit)
                {
                    // Gain 5 fatigue from being hit.
                    HexCharacterController.Instance.ModifyCurrentFatigue(target, 5);

                    DamageResult damageResult = null;
                    damageResult = CombatController.Instance.GetFinalDamageValueAfterAllCalculations(caster, target, ability, abilityEffect, didCrit);
                    damageResult.didCrit = didCrit;

                    // Resolve bonus armour damage
                    if (abilityEffect.bonusArmourDamage > 0)
                        HexCharacterController.Instance.ModifyArmour(target, -abilityEffect.bonusArmourDamage);

                    // Do on hit visual effects for this ability
                    VisualEventManager.CreateVisualEvent(() => AudioManager.Instance.PlaySoundPooled(Sound.Crowd_Cheer_1), target.GetLastStackEventParent());
                    foreach (AnimationEventData vEvent in abilityEffect.visualEventsOnHit)
                        AnimationEventController.Instance.PlayAnimationEvent(vEvent, caster, target, null, weaponUsed, target.GetLastStackEventParent());

                    // Cache hex position, in case target is killed before chain effect triggers
                    LevelNode lastChainHex = target.currentTile;

                    // Deal damage
                    CombatController.Instance.HandleDamage(caster, target, damageResult, ability, abilityEffect, weaponUsed, false, target.GetLastStackEventParent());

                    // On ability effect completed VFX
                    if (CombatController.Instance.CurrentCombatState == CombatGameState.CombatActive &&
                        caster.livingState == LivingState.Alive)
                    {
                        if (target != null && target.livingState == LivingState.Alive)
                        {
                            foreach (AnimationEventData vEvent in abilityEffect.visualEventsOnEffectFinish)
                            {
                                AnimationEventController.Instance.PlayAnimationEvent(vEvent, caster, target, null, weaponUsed, target.GetLastStackEventParent());
                            }
                        }
                    }

                    // On crit stress check
                    if (didCrit &&
                        CombatController.Instance.CurrentCombatState == CombatGameState.CombatActive &&
                        caster.livingState == LivingState.Alive)
                        CombatController.Instance.CreateStressCheck(caster, StressEventType.LandedCriticalStrike);

                    // Trigger on hit/crit effects
                    if (abilityEffect.chainedEffect == false)
                    {
                        if (didCrit && ability.onCritEffects.Count > 0)
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

                    // Trigger chain effect here
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
                    // Gain 3 fatigue from being hit.
                    HexCharacterController.Instance.ModifyCurrentFatigue(target, 2);

                    // Miss notification
                    VisualEventManager.CreateVisualEvent(() =>
                    {
                        VisualEffectManager.Instance.CreateStatusEffect(target.hexCharacterView.WorldPosition, "MISS");
                        AudioManager.Instance.PlaySoundPooled(Sound.Crowd_Ooh_1);
                        LevelController.Instance.AnimateCrowdOnMiss();
                    }, target.GetLastStackEventParent());

                    // Check Evasion
                    if (PerkController.Instance.DoesCharacterHavePerk(target.pManager, Perk.Evasion))
                        PerkController.Instance.ModifyPerkOnCharacterEntity(target.pManager, Perk.Evasion, -1);

                    // Check Crippled
                    if (PerkController.Instance.DoesCharacterHavePerk(target.pManager, Perk.Crippled))
                        PerkController.Instance.ModifyPerkOnCharacterEntity(target.pManager, Perk.Crippled, -1);

                    // Duck animation on miss
                    VisualEventManager.CreateVisualEvent(() =>
                    HexCharacterController.Instance.PlayDuckAnimation(target.hexCharacterView), target.GetLastStackEventParent()).SetEndDelay(0.5f);

                    if (!PerkController.Instance.DoesCharacterHavePerk(caster.pManager, Perk.Slippery) &&
                        HexCharacterController.Instance.IsCharacterAbleToMakeRiposteAttack(target) &&
                        ability.abilityType.Contains(AbilityType.MeleeAttack) &&
                        target.currentTile.Distance(caster.currentTile) <= 1 &&
                        // Cant riposte against another riposte, or free strike
                        ability.abilityName != RiposteAbility.abilityName &&
                        ability.abilityName != FreeStrikeAbility.abilityName &&
                         ability.abilityName != SpearWallStrikeAbility.abilityName)
                    {
                        // Start free strike attack
                        UseAbility(target, RiposteAbility, caster);
                        VisualEventManager.InsertTimeDelayInQueue(0.5f);
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
                if (abilityEffect.aoeType == AoeType.Aura) tilesEffected = HexCharacterController.Instance.GetCharacterAura(caster, true);
                else if (abilityEffect.aoeType == AoeType.ZoneOfControl) tilesEffected = HexCharacterController.Instance.GetCharacterZoneOfControl(caster);
                else if (abilityEffect.aoeType == AoeType.AtTarget) tilesEffected = LevelController.Instance.GetAllHexsWithinRange(tileTarget, abilityEffect.aoeSize, true);
                else if (abilityEffect.aoeType == AoeType.Global) tilesEffected.AddRange(LevelController.Instance.AllLevelNodes.ToList());
                else if (abilityEffect.aoeType == AoeType.Line)
                {
                    LevelNode firstHexHit = null;
                    if (target != null) firstHexHit = target.currentTile;
                    else if (tileTarget != null) firstHexHit = tileTarget;

                    HexDirection direction = LevelController.Instance.GetDirectionToTargetHex(caster.currentTile, firstHexHit);
                    LevelNode previousHex = firstHexHit;
                    LevelNode nextHex = null;
                    tilesEffected.Add(firstHexHit);

                    for (int i = 0; i < abilityEffect.aoeSize; i++)
                    {
                        nextHex = LevelController.Instance.GetAdjacentHexByDirection(previousHex, direction);
                        if (nextHex == null)
                        {
                            break;
                        }
                        else
                        {
                            // Tile in direction from current tile is valid to move over
                            tilesEffected.Add(nextHex);
                            previousHex = nextHex;
                            nextHex = null;
                        }
                    }
                }

                // Remove centre point, if needed
                if (abilityEffect.includeCentreTile == false && abilityEffect.aoeType != AoeType.Line)
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
                    VisualEventManager.CreateStackParentVisualEvent(character);

                    // Roll for hit
                    HitRoll hitRoll = CombatController.Instance.RollForHit(caster, character, ability, weaponUsed);

                    if (hitRoll.Result == HitRollResult.Hit)
                    {
                        charactersHit.Add(character);

                        // Gain 5 fatigue from being hit.
                        HexCharacterController.Instance.ModifyCurrentFatigue(character, 5);

                        // Do on hit visual effects for this ability
                        VisualEventManager.CreateVisualEvent(() => AudioManager.Instance.PlaySoundPooled(Sound.Crowd_Cheer_1), character.GetLastStackEventParent());
                        foreach (AnimationEventData vEvent in abilityEffect.visualEventsOnHit)
                        {
                            AnimationEventController.Instance.PlayAnimationEvent(vEvent, caster, character, null, weaponUsed, character.GetLastStackEventParent());
                        }
                    }
                    else if (hitRoll.Result == HitRollResult.Miss)
                    {
                        // Gain 1 fatigue for dodging.
                        HexCharacterController.Instance.ModifyCurrentFatigue(character, 2);

                        // Miss notification
                        Vector3 pos = character.hexCharacterView.WorldPosition;
                        VisualEventManager.CreateVisualEvent(() =>
                        {
                            if (character.hexCharacterView != null) pos = character.hexCharacterView.WorldPosition;
                            VisualEffectManager.Instance.CreateStatusEffect(pos, "MISS");
                            AudioManager.Instance.PlaySoundPooled(Sound.Crowd_Ooh_1);
                            LevelController.Instance.AnimateCrowdOnMiss();
                        }, character.GetLastStackEventParent());

                        // Check Evasion
                        if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Evasion))
                            PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Evasion, -1);

                        // Check Crippled
                        if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Crippled))
                            PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Crippled, -1);

                        // Duck animation on miss
                        VisualEventManager.CreateVisualEvent(() =>
                            HexCharacterController.Instance.PlayDuckAnimation(character.hexCharacterView), character.GetLastStackEventParent());
                    }

                }

                // Calcuate and deal damage
                foreach (HexCharacterModel character in charactersHit)
                {
                    bool didCrit = CombatController.Instance.RollForCrit(caster, character, ability, abilityEffect);
                    DamageResult dResult = null;
                    dResult = CombatController.Instance.GetFinalDamageValueAfterAllCalculations(caster, character, ability, abilityEffect, didCrit);
                    dResult.didCrit = didCrit;                                      

                    // Resolve bonus armour damage
                    if (abilityEffect.bonusArmourDamage > 0)
                        HexCharacterController.Instance.ModifyArmour(target, -abilityEffect.bonusArmourDamage);

                    // Deal damage
                    CombatController.Instance.HandleDamage(caster, character, dResult, ability, abilityEffect, weaponUsed, false, character.GetLastStackEventParent());

                    // On ability effect completed VFX
                    if (CombatController.Instance.CurrentCombatState == CombatGameState.CombatActive &&
                        caster.livingState == LivingState.Alive)
                    {
                        if (character != null && character.livingState == LivingState.Alive)
                        {
                            foreach (AnimationEventData vEvent in abilityEffect.visualEventsOnEffectFinish)
                            {
                                AnimationEventController.Instance.PlayAnimationEvent(vEvent, caster, character, null, weaponUsed, character.GetLastStackEventParent());
                            }
                        }
                    }

                    // On crit stress check
                    if (didCrit &&
                        CombatController.Instance.CurrentCombatState == CombatGameState.CombatActive &&
                        caster.livingState == LivingState.Alive)
                        CombatController.Instance.CreateStressCheck(caster, StressEventType.LandedCriticalStrike);

                    // Trigger on hit/crit effects
                    if (didCrit && ability.onCritEffects.Count > 0)
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

            // Stress Check AoE
            else if (abilityEffect.effectType == AbilityEffectType.StressCheckAoe)
            {
                triggerEffectEndEvents = false;
                List<LevelNode> tilesEffected = new List<LevelNode>();
                List<HexCharacterModel> charactersEffected = new List<HexCharacterModel>();
                List<HexCharacterModel> charactersHit = new List<HexCharacterModel>();

                // Get Aoe Area
                if (abilityEffect.aoeType == AoeType.Aura)                
                    tilesEffected = HexCharacterController.Instance.GetCharacterAura(caster, true);                
                else if (abilityEffect.aoeType == AoeType.ZoneOfControl)                
                    tilesEffected = HexCharacterController.Instance.GetCharacterZoneOfControl(caster);                
                else if (abilityEffect.aoeType == AoeType.AtTarget)                
                    tilesEffected = LevelController.Instance.GetAllHexsWithinRange(tileTarget, abilityEffect.aoeSize, true);                
                else if (abilityEffect.aoeType == AoeType.Global)                
                    tilesEffected.AddRange(LevelController.Instance.AllLevelNodes.ToList());                

                // Determine targets to roll against.
                foreach (LevelNode h in tilesEffected)                
                    if (h.myCharacter != null && 
                        !HexCharacterController.Instance.IsTargetFriendly(h.myCharacter, caster))                    
                        charactersEffected.Add(h.myCharacter);                   
                
                // Roll for hits + play on Hit animations or on miss animations
                foreach (HexCharacterModel character in charactersEffected)
                {
                    VisualEventManager.CreateStackParentVisualEvent(character);
                    CombatController.Instance.CreateStressCheck(character, abilityEffect.stressEventData, true);
                }                                          
            }

            // Apply passive to target
            else if (abilityEffect.effectType == AbilityEffectType.ApplyPassiveTarget)
            {
                int stacks = abilityEffect.perkPairing.passiveStacks;
                bool roll = RandomGenerator.NumberBetween(1, 100) <= abilityEffect.perkApplicationChance;
                bool success = false;

                if (roll)
                    success = PerkController.Instance.ModifyPerkOnCharacterEntity(target.pManager, abilityEffect.perkPairing.perkTag, stacks, true, 0.5f, caster.pManager);

                if (success)
                {
                    foreach (AnimationEventData vEvent in abilityEffect.visualEventsOnHit)
                        AnimationEventController.Instance.PlayAnimationEvent(vEvent, caster, target, null, null, target.GetLastStackEventParent());

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
                    VisualEventManager.CreateStackParentVisualEvent(character);
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
                                AnimationEventController.Instance.PlayAnimationEvent(vEvent, caster, character, null, weaponUsed, character.GetLastStackEventParent());
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

                if (roll) PerkController.Instance.ModifyPerkOnCharacterEntity(caster.pManager, abilityEffect.perkPairing.perkTag, stacks, true, 0.5f, caster.pManager);

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
                        nextHex = null;
                    }
                }

                // Filter out enemies or allies where applicable
                foreach (LevelNode h in hexsOnPath)
                {
                    if (h.myCharacter != null)
                    {
                        charactersEffected.Add(h.myCharacter);
                    }
                }
                Debug.Log("Characters effected = " + charactersEffected.Count.ToString());

                // Roll for hits + play on Hit animations or on miss animations
                foreach (HexCharacterModel character in charactersEffected)
                {
                    //character.eventStacks.Add(VisualEventManager.Instance.CreateStackParentVisualEvent(character));
                    VisualEventManager.CreateStackParentVisualEvent(character);
                }

                // Apply Passive
                foreach (HexCharacterModel character in charactersEffected)
                {
                    bool roll = RandomGenerator.NumberBetween(1, 100) <= abilityEffect.perkApplicationChance;
                    bool success = false;

                    if (roll) success = PerkController.Instance.ModifyPerkOnCharacterEntity
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
                                AnimationEventController.Instance.PlayAnimationEvent(vEvent, caster, character, null, weaponUsed, character.GetLastStackEventParent());
                            }
                        }
                    }
                }
            }

            // Remove passive from target
            else if (abilityEffect.effectType == AbilityEffectType.RemovePassiveTarget)
            {
                int stacks = PerkController.Instance.GetStackCountOfPerkOnCharacter(target.pManager, abilityEffect.perkPairing.perkTag);
                if (stacks > 0)
                    PerkController.Instance.ModifyPerkOnCharacterEntity(target.pManager, abilityEffect.perkPairing.perkTag, -stacks, true, 0.5f, caster.pManager);
            }

            // Gain Energy Target
            else if (abilityEffect.effectType == AbilityEffectType.GainActionPointsTarget)
            {
                HexCharacterController.Instance.ModifyActionPoints(target, abilityEffect.energyGained, true);
            }

            // Gain Energy Self
            else if (abilityEffect.effectType == AbilityEffectType.GainActionPoints)
            {
                HexCharacterController.Instance.ModifyActionPoints(caster, abilityEffect.energyGained, true);
            }

            // Lose Health Self
            else if (abilityEffect.effectType == AbilityEffectType.LoseHealthSelf)
            {
                DamageResult damageResult = CombatController.Instance.GetFinalDamageValueAfterAllCalculations(caster, abilityEffect.healthLost, DamageType.None);
                CombatController.Instance.HandleDamage(caster, damageResult, DamageType.None, true);
            }

            // Heal Self
            else if (abilityEffect.effectType == AbilityEffectType.HealSelf)
            {
                // Setup
                HexCharacterView view = caster.hexCharacterView;

                // Gain health
                HexCharacterController.Instance.ModifyHealth(caster, abilityEffect.healthGained);

                // Heal VFX
                VisualEventManager.CreateVisualEvent(() =>
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
                LevelNode knockbackHex = null;
                HexCharacterModel knockedBackTarget = null;

                for (int i = 0; i < abilityEffect.tilesMoved; i++)
                {
                    nextHex = LevelController.Instance.GetAdjacentHexByDirection(previousHex, direction);

                    // Hitting an enemy
                    if (nextHex != null && 
                        nextHex.myCharacter != null && 
                        !HexCharacterController.Instance.IsTargetFriendly(nextHex.myCharacter, caster))
                    {
                        // Collision end point found
                        knockedBackTarget = nextHex.myCharacter;
                        obstructionHex = nextHex;
                        knockbackHex = LevelController.Instance.GetAdjacentHexByDirection(obstructionHex, direction);
                        break;
                    }
                    // Hitting an friendly character or obstruction
                    else if (nextHex != null && !Pathfinder.CanHexBeOccupied(nextHex))
                    {
                        // Collision end point found
                        //obstructionHex = nextHex;
                        break;
                    }
                    else if (nextHex != null)
                    {
                        // Tile in direction from current tile is valid to move over
                        hexsOnPath.Add(nextHex);
                        previousHex = nextHex;
                        nextHex = null;
                    }
                }

                // Move down path
                if (hexsOnPath.Count > 0)
                {
                    // to do: animation type and speed (charge, dash, run, etc, and speed)
                    LevelController.Instance.ChargeDownPath(caster, hexsOnPath);
                }                    

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
                LevelNode obstruction = null;
                bool shouldStun = false;

                // get knock back tiles
                for (int i = 0; i < abilityEffect.knockBackDistance; i++)
                {
                    bool forceBreak = false;
                    foreach (LevelNode h in LevelController.Instance.GetAllHexsWithinRange(previousTile, 1))
                    {
                        if (LevelController.Instance.GetDirectionToTargetHex(previousTile, h) == dir)
                        {
                            // Found next tile in direction
                            if (Pathfinder.CanHexBeOccupied(h)) previousTile = h;
                            else
                            {
                                obstruction = h;
                                forceBreak = true;
                                shouldStun = true;
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
                if(shouldStun)
                {
                    // Knock back and forth towards obstruction
                    if(obstruction != null)
                    {
                        TaskTracker tracker = new TaskTracker();
                        VisualEventManager.CreateVisualEvent(() => HexCharacterController.Instance.TriggerKnockedBackIntoObstructionAnimation
                            (target.hexCharacterView, obstruction.WorldPosition, tracker)).SetCoroutineData(tracker);

                    }
                    PerkController.Instance.ModifyPerkOnCharacterEntity(target.pManager, Perk.Stunned, 1, true, 0.5f);
                }
            }

            // Summon Character
            else if (abilityEffect.effectType == AbilityEffectType.SummonCharacter)
            {
                for (int i = 0; i < abilityEffect.amountSummoned; i++)
                {
                    LevelNode spawnLocation = tileTarget;
                    if (spawnLocation == null)
                    {
                        // Get a random available tile.
                        List<LevelNode> possibleTiles = new List<LevelNode>();
                        foreach (LevelNode h in LevelController.Instance.AllLevelNodes)
                        {
                            if (Pathfinder.CanHexBeOccupied(h))
                                possibleTiles.Add(h);
                        }

                        if (possibleTiles.Count > 0)
                        {
                            possibleTiles.Shuffle();
                            spawnLocation = possibleTiles[0];
                        }
                    }

                    if (!spawnLocation) break;

                    // Create character
                    HexCharacterModel newSummon =
                        HexCharacterController.Instance.CreateSummonedHexCharacter(abilityEffect.CharacterSummoned, spawnLocation, caster.allegiance);

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
                    VisualEventManager.CreateVisualEvent(() =>
                    {
                        view.myActivationWindow.gameObject.SetActive(true);
                        view.myActivationWindow.Show();
                        TurnController.Instance.EnablePanelSlotAtIndex(windowIndex);
                    }).SetEndDelay(0.1f);

                    // Update all window slot positions + activation pointer arrow
                    HexCharacterModel entityActivated = TurnController.Instance.EntityActivated;
                    var cachedOrder = TurnController.Instance.ActivationOrder.ToList();
                    VisualEventManager.CreateVisualEvent(() =>
                    {
                        TurnController.Instance.UpdateWindowPositions(cachedOrder);
                        TurnController.Instance.MoveActivationArrowTowardsEntityWindow(entityActivated);
                        HexCharacterController.Instance.FadeInCharacterWorldCanvas(view, null, abilityEffect.uiFadeInSpeed);
                        CharacterModeller.FadeInCharacterModel(view.ucm, abilityEffect.modelFadeInSpeed);
                        CharacterModeller.FadeInCharacterShadow(view, 1f);
                    });                    

                    // Resolve visual events
                    foreach (AnimationEventData vEvent in abilityEffect.summonedCreatureVisualEvents)
                    {
                        AnimationEventController.Instance.PlayAnimationEvent(vEvent, newSummon, newSummon);
                    }
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
                            AnimationEventController.Instance.PlayAnimationEvent(vEvent, caster, target, null, weaponUsed, target.GetLastStackEventParent());
                        }
                    }
                }
            }

        }
        private void OnAbilityUsedStart(HexCharacterModel character, AbilityData ability, HexCharacterModel target = null)
        {
            // Pay Energy Cost
            HexCharacterController.Instance.ModifyActionPoints(character, -GetAbilityActionPointCost(character, ability));
            HexCharacterController.Instance.ModifyCurrentFatigue(character, GetAbilityFatigueCost(character, ability));

            // Check shed perk: remove if ability used was an aspect ability
            if (ability.abilityType.Contains(AbilityType.Aspect) && PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Shed))
                PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Shed, -1);

            // Increment skills used this turn
            character.abilitiesUsedThisCombat++;
            if (ability.abilityType.Contains(AbilityType.Skill))            
                character.skillAbilitiesUsedThisTurn++;
            else if (ability.abilityType.Contains(AbilityType.RangedAttack))
                character.rangedAttackAbilitiesUsedThisTurn++;
            else if (ability.abilityType.Contains(AbilityType.MeleeAttack))
                character.meleeAttackAbilitiesUsedThisTurn++;
            else if (ability.abilityType.Contains(AbilityType.WeaponAttack))
                character.weaponAbilitiesUsedThisTurn++;
            else if (ability.abilityType.Contains(AbilityType.Spell))
                character.spellAbilitiesUsedThisTurn++;
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
            // Set cooldown count to max
            ability.currentCooldown = ability.baseCooldown;

            // Update ability bar UI if activated player character
            if (ability.myCharacter != null &&
                TurnController.Instance.EntityActivated == ability.myCharacter &&
                ability.myCharacter.controller == Controller.Player)
            { 
                foreach(AbilityButton b in CombatUIController.Instance.AbilityButtons)
                {
                    if(b.MyAbilityData == ability)
                    {
                        b.UpdateAbilityButtonUnusableOverlay();
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
                AudioManager.Instance.PlaySound(Sound.UI_Button_Click);

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
                TargetGuidanceController.Instance.Hide(0);
                LevelController.Instance.UnmarkAllTiles();
                LevelController.Instance.UnmarkAllSubTargetMarkers();
                var selectedButton = CombatUIController.Instance.FindAbilityButton(currentAbilityAwaiting);
                if (selectedButton != null) selectedButton.SetSelectedGlow(false);
            }

            // cache ability, get ready for second click, or for instant use
            currentAbilityAwaiting = b.MyAbilityData;           
            currentSelectionPhase = AbilitySelectionPhase.None;

            // Set cursor
            CursorController.Instance.SetFallbackCursor(CursorType.TargetClick);
            CursorController.Instance.SetCursor(CursorType.TargetClick);

            // Highlight tiles in range of ability
            if (ability.targetRequirement != TargetRequirement.NoTarget)
            {
                bool neutral = true;
                if (ability.targetRequirement == TargetRequirement.Enemy) neutral = false;
                CombatUIController.Instance.FindAbilityButton(currentAbilityAwaiting).SetSelectedGlow(true);
                LevelController.Instance.MarkTilesInRange(GetTargettableTilesOfAbility(ability, caster), neutral);
                TargetGuidanceController.Instance.BuildAndShow(ability.guidanceInstruction, 0.5f);
            }
            else if (ability.targetRequirement == TargetRequirement.NoTarget)
            {
                UseAbility(caster, currentAbilityAwaiting);
                HandleCancelCurrentAbilityOrder();
            }

            // Handle keyboard pressed to trigger ability click, and hit chance
            // modal is open while moused over the previous target
            if (AwaitingAbilityOrder() && currentAbilityAwaiting.targetRequirement == TargetRequirement.Enemy &&
                LevelController.HexMousedOver != null && LevelController.HexMousedOver.myCharacter != null)
            {
                ShowHitChancePopup(TurnController.Instance.EntityActivated, LevelController.HexMousedOver.myCharacter,
                    CurrentAbilityAwaiting, TurnController.Instance.EntityActivated.itemSet.mainHandItem);
            }
            else HideHitChancePopup();
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
                            LevelController.Instance.UnmarkAllSubTargetMarkers();
                            TargetGuidanceController.Instance.Hide(0);
                            bool neutral = true;
                            if (currentAbilityAwaiting.targetRequirement == TargetRequirement.Enemy) neutral = false;
                            LevelController.Instance.MarkTilesInRange(validHexs, neutral);
                            TargetGuidanceController.Instance.BuildAndShow(currentAbilityAwaiting.guidanceInstructionTwo);
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
            var selectedButton = CombatUIController.Instance.FindAbilityButton(currentAbilityAwaiting);
            if (selectedButton != null) selectedButton.SetSelectedGlow(false);
            currentAbilityAwaiting = null;
            firstSelectionCharacter = null;
            currentSelectionPhase = AbilitySelectionPhase.None;
            LevelController.Instance.UnmarkAllSubTargetMarkers();
            LevelController.Instance.UnmarkAllTiles();
            TargetGuidanceController.Instance.Hide();
            CursorController.Instance.SetFallbackCursor(CursorType.NormalPointer);
            CursorController.Instance.SetCursor(CursorType.NormalPointer);
            HideHitChancePopup();
        }
        private void Update()
        {
            if (GameController.Instance.GameState != GameState.CombatActive) return;

            if (Input.GetKeyDown(KeyCode.Mouse1))
                HandleCancelCurrentAbilityOrder();

            if (hitChanceRootCanvas.isActiveAndEnabled == true &&
                LevelController.HexMousedOver != null &&
               hitChancePositionParent.transform.position != LevelController.HexMousedOver.WorldPosition)
                hitChancePositionParent.transform.position = LevelController.HexMousedOver.WorldPosition;
        }
        #endregion

        // Get Ability Calculations
        #region
        public int GetAbilityActionPointCost(HexCharacterModel character, AbilityData ability)
        {
            int apCost = ability.energyCost;

            // Check shed perk: aspect ability costs 0
            if (ability.abilityType.Contains(AbilityType.Aspect) && PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Shed))
                return 0;

            // Gifted Perk
            if (character != null &&
                character.abilitiesUsedThisCombat == 0 &&
                PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Gifted))
            {
                apCost -= 3;
            }

            // MASTERY PERKS
            // Master of Archer
            if (character != null &&
                (ability.abilityName == "Load Bolt" || ability.abilityName == "Plink") && 
                PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.MasterOfArchery))
            {
                apCost -= 1;
            }

            // Weapon mastery 
            if (character != null &&
                ability.abilityType.Contains(AbilityType.WeaponAttack) &&
                character.weaponAbilitiesUsedThisTurn == 0 &&
                PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.WeaponMastery))
            {
                apCost -= 1;
            }

            // Skill Mastery
            if (character != null &&
                ability.abilityType.Contains(AbilityType.Skill) &&
                character.skillAbilitiesUsedThisTurn == 0 &&
                PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Resourceful))
            {
                apCost -= 1;
            }

            // Ranged Mastery 
            if (character != null &&
                ability.abilityType.Contains(AbilityType.RangedAttack) &&
                character.rangedAttackAbilitiesUsedThisTurn == 0 &&
                PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.RangedMastery))
            {
                apCost -= 1;
            }

            // Melee Mastery
            if (character != null &&
                ability.abilityType.Contains(AbilityType.MeleeAttack) &&
                character.meleeAttackAbilitiesUsedThisTurn == 0 &&
                PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.MeleeMastery))
            {
                apCost -= 1;
            }

            // Arcane Mastery
            if (character != null &&
                ability.abilityType.Contains(AbilityType.Spell) &&
                character.spellAbilitiesUsedThisTurn == 0 &&
                PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.ArcaneMastery))
            {
                apCost -= 1;
            }

            // Spell Mastery
            if (character != null &&
                ability.abilityType.Contains(AbilityType.Spell) &&
                character.spellAbilitiesUsedThisTurn == 0 &&
                PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.SpellMastery))
            {
                apCost -= 1;
            }

            // prevent cost going negative
            if (apCost < 0)
                apCost = 0;

            return apCost;
        }
        public int GetAbilityFatigueCost(HexCharacterModel character, AbilityData ability)
        {
            int fatigueCost = ability.fatigueCost;
            float mod = 1f;

            // Muscles memories perk
            if (fatigueCost > 0 &&
                ability.abilityType.Contains(AbilityType.WeaponAttack) &&
                PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.MusclesMemories))
                return 0;

            // Well Drilled perk
            if (fatigueCost > 0 && PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.WellDrilled))
                mod -= 0.5f;

            // WEAPON SPECIALIZATIONS
            // Shield specialist perk
            if (ability.abilityName == "Raise Shield" &&
                fatigueCost > 0 && PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.ShieldSpecialist))
                mod -= 0.25f;

            // Dual wield finesse
            if (character.itemSet.IsDualWieldingMeleeWeapons() &&
                ability.abilityType.Contains(AbilityType.WeaponAttack) &&
                PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.DualWieldFinesse))
                mod -= 0.25f;

            // Two hand dominance
            if (character.itemSet.IsWieldingTwoHandMeleeWeapon() &&
                ability.abilityType.Contains(AbilityType.WeaponAttack) &&
                PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.TwoHandedDominance))
                mod -= 0.25f;

            // Master of Archery
            if (character.itemSet.IsWieldingBowOrCrossbow() &&
                ability.abilityType.Contains(AbilityType.WeaponAttack) &&
                PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.MasterOfArchery))
                mod -= 0.25f;

            // One hand expertise
            if (character.itemSet.IsWieldingOneHandMeleeWeaponWithEmptyOffhand() &&
                ability.abilityType.Contains(AbilityType.WeaponAttack) &&
                PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.OneHandedExpertise))
                mod -= 0.25f;

            // Arcane Mastery
            if (ability.abilityType.Contains(AbilityType.Spell) &&
                PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.ArcaneMastery))
                mod -= 0.25f;

            fatigueCost = (int) (fatigueCost * mod);

            // prevent cost going negative
            if (fatigueCost < 0)
                fatigueCost = 0;

            return fatigueCost;
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

            if ((ability.abilityType.Contains(AbilityType.RangedAttack) || ability.abilityType.Contains(AbilityType.Skill) || ability.abilityType.Contains(AbilityType.Spell)) &&
                ability.gainRangeBonusFromVision)
            {
                rangeReturned += StatCalculator.GetTotalVision(caster);

                // Check elevation range bonus
                if (caster.currentTile.Elevation == TileElevation.Elevated)
                   rangeReturned += 1;

                if (ability.abilityType.Contains(AbilityType.Spell) && PerkController.Instance.DoesCharacterHavePerk(caster.pManager, Perk.SpellSight))
                    rangeReturned += 1;
            }

            if(caster.itemSet.mainHandItem != null && ability.abilityType.Contains(AbilityType.WeaponAttack))
            {
                rangeReturned += ItemController.Instance.GetInnateModifierFromWeapon(InnateItemEffectType.BonusMeleeRange, caster.itemSet.mainHandItem);
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
                DoesAbilityMeetSubRequirements(caster, ability, target))
            {
                bRet = true;
            }

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
            else
            {
                ActionErrorGuidanceController.Instance.ShowErrorMessage(caster, "Target is out of range");
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
                ActionErrorGuidanceController.Instance.ShowErrorMessage(character, "It is not this character's turn");
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
                    ActionErrorGuidanceController.Instance.ShowErrorMessage(character, "Character is unable to move right now");
                    bRet = false;
                }
            }

            // Check character is actually alive
            if (character.currentHealth <= 0 || character.livingState == LivingState.Dead)
            {
                Debug.Log("IsAbilityUseable() returning false: character is dead or in the death sequence");
                bRet = false;
            }

            // check has enough energy
            if (!DoesCharacterHaveEnoughActionPoints(character, ability))
            {
                Debug.Log("IsAbilityUseable() returning false: not enough energy");
                ActionErrorGuidanceController.Instance.ShowErrorMessage(character, "Not enough Action Points");
                bRet =  false;
            }

            // Check sub reqs
            if (!DoesAbilityMeetSubRequirements(character, ability, null))
            {
                Debug.Log("IsAbilityUseable() returning false: did not meet sub requirements");
                bRet = false;
            }

            // check has enough fatigue
            if (!DoesCharacterHaveEnoughFatigue(character, ability))
            {
                Debug.Log("IsAbilityUseable() returning false: not enough fatigue");
                ActionErrorGuidanceController.Instance.ShowErrorMessage(character, "Character is too fatigued");
                bRet = false;
            }

            // check cooldown
            if (ability.currentCooldown != 0 )
            {
                Debug.Log("IsAbilityUseable() returning false: ability is on cooldown");
                ActionErrorGuidanceController.Instance.ShowErrorMessage(character, "Ability is on cooldown");
                bRet = false;
            }

            // check weapon requirement
            if (!DoesCharacterMeetAbilityWeaponRequirement(character.itemSet, ability.weaponRequirement))
            {
                Debug.Log("IsAbilityUseable() returning false: character does not meet the ability's weapon requirement");
                bRet = false;
            }

            // Check smashed shield
            if(ability.weaponRequirement == WeaponRequirement.Shield &&
                PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.SmashedShield))
            {
                Debug.Log("IsAbilityUseable() returning false: character has 'Smashed Shield' perk");
                bRet = false;
            }

            // check unloaded crossbow
            if(ability.abilityType.Contains(AbilityType.WeaponAttack) && 
                ability.abilityType.Contains(AbilityType.RangedAttack) &&
                PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Reload))
            {
                Debug.Log("IsAbilityUseable() returning false: character trying to use a ranged weapon attack with 'Reload' status...");
                ActionErrorGuidanceController.Instance.ShowErrorMessage(character, "Character needs to reload first");
                bRet = false;
            }           

            return bRet;
        }
        public bool DoesCharacterMeetAbilityWeaponRequirement(ItemSet itemSet, WeaponRequirement weaponReq)
        {
            bool bRet = false;

            if (weaponReq == WeaponRequirement.None)
                bRet = true;

            else if (weaponReq == WeaponRequirement.MeleeWeapon &&
                itemSet.mainHandItem != null &&
                itemSet.mainHandItem.IsMeleeWeapon)
                bRet = true;

            else if (weaponReq == WeaponRequirement.RangedWeapon &&
                itemSet.mainHandItem != null &&
                itemSet.mainHandItem.IsRangedWeapon)
                bRet = true;

            else if (weaponReq == WeaponRequirement.Shield &&
               itemSet.offHandItem != null &&
               itemSet.offHandItem.weaponClass == WeaponClass.Shield)
                bRet = true;

            else if (weaponReq == WeaponRequirement.Bow &&
              itemSet.mainHandItem != null &&
              itemSet.mainHandItem.weaponClass == WeaponClass.Bow)
                bRet = true;

            else if (weaponReq == WeaponRequirement.Crossbow &&
            itemSet.mainHandItem != null &&
            itemSet.mainHandItem.weaponClass == WeaponClass.Crossbow)
                bRet = true;

            else if (weaponReq == WeaponRequirement.BowOrCrossbow &&
            itemSet.mainHandItem != null &&
            (itemSet.mainHandItem.weaponClass == WeaponClass.Crossbow || itemSet.mainHandItem.weaponClass == WeaponClass.Bow))
                bRet = true;

            else if (weaponReq == WeaponRequirement.ThrowingNet &&
              itemSet.offHandItem != null &&
              itemSet.offHandItem.weaponClass == WeaponClass.ThrowingNet)
                bRet = true;

            else if (weaponReq == WeaponRequirement.EmptyOffhand &&
            itemSet.offHandItem == null)
                bRet = true;

            return bRet;
        }
        private bool DoesCharacterHaveEnoughActionPoints(HexCharacterModel caster, AbilityData ability)
        {
            return caster.currentEnergy >= GetAbilityActionPointCost(caster, ability);
        }
        private bool DoesCharacterHaveEnoughFatigue(HexCharacterModel caster, AbilityData ability)
        {
            return StatCalculator.GetTotalMaxFatigue(caster) - caster.currentFatigue >= GetAbilityFatigueCost(caster, ability);
        }
        private bool IsTargetOfAbilityInRange(HexCharacterModel caster, HexCharacterModel target, AbilityData ability)
        {
            bool bRet = false;
            bRet = GetTargettableTilesOfAbility(ability, caster).Contains(target.currentTile);
            bool showErrorPanel = false;
            if (TurnController.Instance.EntityActivated != null && 
                TurnController.Instance.EntityActivated.controller == Controller.Player) 
                showErrorPanel = true;

            if (!bRet)
            {
                if (showErrorPanel) ActionErrorGuidanceController.Instance.ShowErrorMessage(caster, "Target is out of range");
                return false;
            }

            int stealthDistance = StatCalculator.GetTotalVision(caster) + 1;
            bool ignoreStealth = PerkController.Instance.DoesCharacterHavePerk(caster.pManager, Perk.TrueSight) ||
                PerkController.Instance.DoesCharacterHavePerk(caster.pManager, Perk.EyelessSight);

            if (stealthDistance < 1) stealthDistance = 1;

            // Check stealth + true eye / sniper (ignores stealth)
            if (caster.currentTile.Distance(target.currentTile) > stealthDistance &&
                HexCharacterController.Instance.IsTargetFriendly(caster, target) == false &&
                PerkController.Instance.DoesCharacterHavePerk(target.pManager, Perk.Stealth) &&
                !ignoreStealth)
            {
                if (showErrorPanel) ActionErrorGuidanceController.Instance.ShowErrorMessage(caster, "Character needs to reload first");
                bRet = false;
            }

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

            if (bRet == false &&
                TurnController.Instance.EntityActivated != null &&
                TurnController.Instance.EntityActivated.controller == Controller.Player)
            {
                ActionErrorGuidanceController.Instance.ShowErrorMessage(caster, "Invalid target for this ability");
            }

            Debug.Log("AbilityController.DoesTargetOfAbilityMeetAllegianceRequirement() returning " + bRet);
            return bRet;

        }
        private bool DoesAbilityMeetSubRequirements(HexCharacterModel caster, AbilityData ability, HexCharacterModel target = null)
        {
            bool bRet = true;

            // check its actually characters turn
            if (TurnController.Instance.EntityActivated != caster ||
                (TurnController.Instance.EntityActivated == caster && caster.activationPhase != ActivationPhase.ActivationPhase))
            {
                Debug.Log("IsAbilityUseable() returning false: cannot use abilities when it is not your turn");
                ActionErrorGuidanceController.Instance.ShowErrorMessage(caster, "It is not this character's turn");
                bRet = false;
            }

            foreach (AbilityRequirement ar in ability.abilitySubRequirements)
            {
                if (ar.type == AbilityRequirementType.TargetHasUnoccupiedBackTile)
                {
                    if (target == null) continue;

                    bool shouldContinue = false;
                    foreach (LevelNode n in HexCharacterController.Instance.GetCharacterBackArcTiles(target))
                    {
                        if (Pathfinder.CanHexBeOccupied(n))
                        {
                            Debug.Log("DoesTargetOfAbilityMeetSubRequirements() passed 'TargetHasAnUnoccupiedBackTile' check");
                            shouldContinue = true;
                            break;
                        }
                    }
                    if (shouldContinue) continue;
                    else
                    {
                        ActionErrorGuidanceController.Instance.ShowErrorMessage(caster, "Target's back tiles are obstructed.");
                    }
                }

                else if (ar.type == AbilityRequirementType.TargetHasRace)
                {
                    if (target == null || (target != null && target.race == ar.race)) continue;
                }

                else if (ar.type == AbilityRequirementType.TargetHasPerk)
                {
                    if (target == null || (target != null && PerkController.Instance.DoesCharacterHavePerk(target.pManager, ar.perk))) continue;
                }

                else if (ar.type == AbilityRequirementType.CasterHasPerk && 
                    PerkController.Instance.DoesCharacterHavePerk(caster.pManager, ar.perk))
                {
                    continue;
                }

                else if (ar.type == AbilityRequirementType.CasterDoesNotHavePerk &&
                    !PerkController.Instance.DoesCharacterHavePerk(caster.pManager, ar.perk))
                {
                    continue;
                }                 
                
                else if (ar.type == AbilityRequirementType.TargetDoesNotHavePerk)
                {
                    if (target == null || (target != null && !PerkController.Instance.DoesCharacterHavePerk(target.pManager, ar.perk))) continue;
                }

                else if (ar.type == AbilityRequirementType.TargetHasShield)
                {
                    if (target == null || (target != null && target.itemSet.offHandItem != null && target.itemSet.offHandItem.weaponClass == WeaponClass.Shield)) continue;
                    else ActionErrorGuidanceController.Instance.ShowErrorMessage(caster, "Target does not have a shield");
                }

                else if (ar.type == AbilityRequirementType.TargetIsTeleportable)
                {
                    if (target == null || (target != null && HexCharacterController.Instance.IsCharacterTeleportable(target))) continue;
                    else ActionErrorGuidanceController.Instance.ShowErrorMessage(caster, "Target cannot be teleported");
                }

                else if (ar.type == AbilityRequirementType.CasterIsTeleportable &&
                    HexCharacterController.Instance.IsCharacterTeleportable(caster))
                {
                    continue;
                }

                else if (ar.type == AbilityRequirementType.CasterHasEnoughHealth &&
                   caster.currentHealth >= ar.healthRequired)
                {
                    continue;
                }

                bRet = false;
                break;
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
        public void ShowHitChancePopup(HexCharacterModel caster, HexCharacterModel target, AbilityData ability, ItemData weaponUsed = null)
        {            
            hitChanceBoxesParent.SetActive(false);
            hitChanceHeaderParent.SetActive(false);
            applyPassiveBoxesParent.SetActive(false);
            applyPassiveHeaderParent.SetActive(false);

            EnemyInfoModalController.Instance.HideModal();

            if (target != null &&
                target.allegiance != caster.allegiance &&
                caster.activationPhase == ActivationPhase.ActivationPhase &&
                ability != null &&
                ability.targetRequirement == TargetRequirement.Enemy)
            {
                if (ability.abilityType.Contains(AbilityType.MeleeAttack) || ability.abilityType.Contains(AbilityType.RangedAttack) || ability.abilityType.Contains(AbilityType.WeaponAttack))
                {
                    HitChanceDataSet hitChanceData = CombatController.Instance.GetHitChance(caster, target, ability, weaponUsed);
                    hitChanceData.details = hitChanceData.details.OrderByDescending(x => x.accuracyMod).ToList();

                    // to do: try find passive apply effect and add to hit chance modal display
                    AbilityEffect effect = FindDebuffEffect(ability);                    
                    BuildHitChancePopup(hitChanceData);                       
                    
                    if(effect != null)
                    {
                        HitChanceDataSet applyDebuffData = CombatController.Instance.GetDebuffChance(caster, target, ability, effect);
                        applyDebuffData.details = applyDebuffData.details.OrderByDescending(x => x.accuracyMod).ToList();
                        BuildApplyPassiveChancePopup(applyDebuffData, applyDebuffData.perk.passiveSprite);
                    }
                    hitChanceCg.DOKill();
                    hitChanceCg.alpha = 0;
                    hitChanceRootCanvas.enabled = true;
                    hitChanceCg.DOFade(1, 0.5f);
                    TransformUtils.RebuildLayouts(hitChanceLayouts);
                }
                
                else if(ability.abilityType.Contains(AbilityType.Spell) || ability.abilityType.Contains(AbilityType.Skill))
                { 
                    AbilityEffect effect = FindDebuffEffect(ability);
                    if (effect == null) return;

                    applyPassiveBoxesParent.SetActive(true);
                    applyPassiveHeaderParent.SetActive(true);
                    hitChanceCg.DOKill();
                    hitChanceCg.alpha = 0;
                    hitChanceRootCanvas.enabled = true;
                    hitChanceCg.DOFade(1, 0.5f);

                    HitChanceDataSet applyDebuffData = CombatController.Instance.GetDebuffChance(caster, target, ability, effect);
                    applyDebuffData.details = applyDebuffData.details.OrderByDescending(x => x.accuracyMod).ToList();
                    BuildApplyPassiveChancePopup(applyDebuffData, applyDebuffData.perk.passiveSprite);
                    TransformUtils.RebuildLayouts(hitChanceLayouts);
                }
            }          
        }
        private void BuildHitChancePopup(HitChanceDataSet data)
        {
            hitChanceBoxesParent.SetActive(true);
            hitChanceHeaderParent.SetActive(true);

            // Header text
            hitChanceText.text = TextLogic.ReturnColoredText(data.FinalHitChance.ToString() + "%", TextLogic.neutralYellow) + " chance to hit";

            // Reset tabs
            for(int i = 0; i < hitChanceBoxes.Length; i++)            
                hitChanceBoxes[i].gameObject.SetActive(false);
            
            for(int i = 0; i < data.details.Count; i++)
            {
                string extra = data.details[i].accuracyMod > 0 ? "+" : "";
                if (i == hitChanceBoxes.Length - 1) break;
                hitChanceBoxes[i].Build(data.details[i].reason + ": " + 
                    TextLogic.ReturnColoredText(extra + data.details[i].accuracyMod.ToString(), 
                    TextLogic.neutralYellow), data.details[i].accuracyMod > 0 ? DotStyle.Green : DotStyle.Red);
            }
        }
        private void BuildApplyPassiveChancePopup(HitChanceDataSet data, Sprite iconSprite)
        {
            applyPassiveBoxesParent.SetActive(true);
            applyPassiveHeaderParent.SetActive(true);

            // Header text
            applyPassiveText.text = TextLogic.ReturnColoredText(data.FinalHitChance.ToString() + "%", TextLogic.neutralYellow) + " chance";
            applyPassiveIcon.PerkImage.sprite = iconSprite;

            // Reset tabs
            for (int i = 0; i < applyPassiveBoxes.Length; i++)            
                applyPassiveBoxes[i].gameObject.SetActive(false);
            
            for (int i = 0; i < data.details.Count; i++)
            {
                string extra = data.details[i].accuracyMod > 0 ? "+" : "";
                if (i == applyPassiveBoxes.Length - 1) break;

                string accuracyModText = ": " + TextLogic.ReturnColoredText(extra + data.details[i].accuracyMod.ToString(), TextLogic.neutralYellow);
                if (data.details[i].hideAccuracyMod) accuracyModText = "";
                applyPassiveBoxes[i].Build(data.details[i].reason + accuracyModText, data.details[i].accuracyMod > 0 ? DotStyle.Green : DotStyle.Red);
            }
        }
        public void HideHitChancePopup()
        {
            hitChanceCg.alpha = 1;
            hitChanceCg.DOKill();
            hitChanceCg.DOFade(0f, 0.15f).OnComplete(() => hitChanceRootCanvas.enabled = false);
            //hitChanceRootCanvas.enabled = false;
           // hitChanceCg.alpha = 0;
        }
        private AbilityEffect FindDebuffEffect(AbilityData ability)
        {
            AbilityEffect ret = null;

            foreach(AbilityEffect e in ability.abilityEffects)
            {
                if(e.effectType == AbilityEffectType.ApplyPassiveTarget)
                {
                    ret = e;
                    break;
                }
            }
            foreach (AbilityEffect e in ability.onHitEffects)
            {
                if (e.effectType == AbilityEffectType.ApplyPassiveTarget)
                {
                    ret = e;
                    break;
                }
            }
            foreach (AbilityEffect e in ability.onCritEffects)
            {
                if (e.effectType == AbilityEffectType.ApplyPassiveTarget)
                {
                    ret = e;
                    break;
                }
            }
            foreach (AbilityEffect e in ability.onPerkAppliedSuccessEffects)
            {
                if (e.effectType == AbilityEffectType.ApplyPassiveTarget)
                {
                    ret = e;
                    break;
                }
            }
            foreach (AbilityEffect e in ability.onCollisionEffects)
            {
                if (e.effectType == AbilityEffectType.ApplyPassiveTarget)
                {
                    ret = e;
                    break;
                }
            }

            return ret;
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