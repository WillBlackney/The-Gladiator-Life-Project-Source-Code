using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using WeAreGladiators.Audio;
using WeAreGladiators.Characters;
using WeAreGladiators.Combat;
using WeAreGladiators.CombatLog;
using WeAreGladiators.HexTiles;
using WeAreGladiators.Items;
using WeAreGladiators.Pathfinding;
using WeAreGladiators.Perks;
using WeAreGladiators.TurnLogic;
using WeAreGladiators.UCM;
using WeAreGladiators.UI;
using WeAreGladiators.Utilities;
using WeAreGladiators.VisualEvents;

namespace WeAreGladiators.Abilities
{
    public class AbilityController : Singleton<AbilityController>
    {
        #region Properties + Components
        [Header("Ability Data Files")]
        [SerializeField] private AbilityDataSO[] allAbilityDataSOs;
        [SerializeField] private AbilityDataSO freeStrikeAbilityData;
        [SerializeField] private AbilityDataSO riposteAbilityData;
        [SerializeField] private AbilityDataSO spearWallStrikeAbilityData;
        [Space(20)]

        [Header("Hit Chance Pop Up Components")]
        [SerializeField] private Canvas hitChanceRootCanvas;
        [SerializeField] private GameObject hitChancePositionParent;
        [SerializeField] private CanvasGroup hitChanceCg;
        [SerializeField] private RectTransform[] hitChanceLayouts;
        [Space(10)]
        [SerializeField] private GameObject hitChanceHeaderParent;
        [SerializeField] private TextMeshProUGUI hitChanceText;
        [SerializeField] private GameObject hitChanceBoxesParent;
        [SerializeField] private ModalDottedRow[] hitChanceBoxes;
        [Space(10)]
        [SerializeField] private GameObject applyPassiveHeaderParent;
        [SerializeField] private TextMeshProUGUI applyPassiveText;
        [SerializeField] private UIPerkIcon applyPassiveIcon;
        [SerializeField] private GameObject applyPassiveBoxesParent;
        [SerializeField] private ModalDottedRow[] applyPassiveBoxes;

        private HexCharacterModel firstSelectionCharacter;
        public Dictionary<HexCharacterModel, Action> onAbilityEndVisualEventQueue = new Dictionary<HexCharacterModel, Action>();

        #endregion

        #region Getters + Accessors
        public bool HitChanceModalIsVisible => hitChanceRootCanvas.isActiveAndEnabled;
        public AbilityData SpearWallStrikeAbility
        {
            get;
            private set;
        }
        public AbilityData FreeStrikeAbility
        {
            get;
            private set;
        }
        public AbilityData RiposteAbility
        {
            get;
            private set;
        }
        public AbilityData[] AllAbilities { get; private set; }
        public AbilityDataSO[] AllAbilityDataSOs
        {
            get => allAbilityDataSOs;
            private set => allAbilityDataSOs = value;
        }
        public AbilityData CurrentAbilityAwaiting { get; private set; }
        public AbilitySelectionPhase CurrentSelectionPhase { get; private set; }
        #endregion

        #region Initialization + Build Library
        protected override void Awake()
        {
            base.Awake();
            BuildAbilityLibrary();
        }
        private void BuildAbilityLibrary()
        {
            List<AbilityData> tempList = new List<AbilityData>();

            foreach (AbilityDataSO dataSO in allAbilityDataSOs)
            {
                if (dataSO != null && dataSO.includeInGame)
                {
                    tempList.Add(BuildAbilityDataFromScriptableObjectData(dataSO));
                }
            }

            AllAbilities = tempList.ToArray();

            // Setup free strike + globally shared abilities
            FreeStrikeAbility = BuildAbilityDataFromScriptableObjectData(freeStrikeAbilityData);
            RiposteAbility = BuildAbilityDataFromScriptableObjectData(riposteAbilityData);
            SpearWallStrikeAbility = BuildAbilityDataFromScriptableObjectData(spearWallStrikeAbilityData);

        }
        #endregion

        #region Learn + Unlearn Abilities
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

        #region Dynamic Description Logic
        public string GetDynamicDescriptionFromAbility(AbilityData ability)
        {
            string sRet = "";
            HexCharacterModel character = ability.myCharacter;

            foreach (CustomString cs in ability.dynamicDescription)
            {
                // Does the custom string even have a dynamic value?
                if (cs.getPhraseFromAbilityValue == false)
                {
                    sRet += TextLogic.ConvertCustomStringToString(cs);
                }
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
                    if (matchingEffect == null)
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

        #region Search Logic
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
            List<TalentSchool> talentSchools = new List<TalentSchool>
            {
                TalentSchool.Divinity,
                TalentSchool.Guardian,
                TalentSchool.Manipulation,
                TalentSchool.Naturalism,
                TalentSchool.Pyromania,
                TalentSchool.Ranger,
                TalentSchool.Scoundrel,
                TalentSchool.Shadowcraft,
                TalentSchool.Warfare,
                TalentSchool.Metamorph
            };

            talentSchools.Shuffle();
            TalentSchool ts = talentSchools[0];
            List<AbilityData> validAbilities = new List<AbilityData>();
            foreach (AbilityData a in AllAbilities)
            {
                if (a.talentRequirementData.talentSchool == ts)
                {
                    validAbilities.Add(a);
                }
            }

            validAbilities.Shuffle();
            return validAbilities[0];
        }
        public List<AbilityData> GetAllAbilitiesOfTalent(TalentSchool ts)
        {
            List<AbilityData> ret = new List<AbilityData>();

            for (int i = 0; i < AllAbilities.Length; i++)
            {
                if (AllAbilities[i].talentRequirementData != null &&
                    AllAbilities[i].talentRequirementData.talentSchool == ts)
                {
                    ret.Add(AllAbilities[i]);
                }
            }

            return ret;
        }
        public AbilityData FindAbilityData(string abilityName)
        {
            AbilityData ret = null;
            foreach (AbilityData a in AllAbilities)
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

        #region Data Conversion

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
                a.abilityEffects.Add(effect.CloneJSON());
            }
            a.onHitEffects = new List<AbilityEffect>();
            foreach (AbilityEffect effect in d.onHitEffects)
            {
                a.onHitEffects.Add(effect.CloneJSON());
            }
            a.onCritEffects = new List<AbilityEffect>();
            foreach (AbilityEffect effect in d.onCritEffects)
            {
                a.onCritEffects.Add(effect.CloneJSON());
            }
            a.onPerkAppliedSuccessEffects = new List<AbilityEffect>();
            foreach (AbilityEffect effect in d.onPerkAppliedSuccessEffects)
            {
                a.onPerkAppliedSuccessEffects.Add(effect.CloneJSON());
            }
            a.onCollisionEffects = new List<AbilityEffect>();
            foreach (AbilityEffect effect in d.onCollisionEffects)
            {
                a.onCollisionEffects.Add(effect.CloneJSON());
            }
            a.chainedEffects = new List<AbilityEffect>();
            foreach (AbilityEffect effect in d.chainedEffects)
            {
                a.chainedEffects.Add(effect.CloneJSON());
            }

            // Keyword Model Data
            a.keyWords = new List<KeyWordModel>();
            foreach (KeyWordModel kwdm in d.keyWords)
            {
                a.keyWords.Add(kwdm.CloneJSON());
            }

            // Custom string Data
            a.dynamicDescription = new List<CustomString>();
            foreach (CustomString cs in d.dynamicDescription)
            {
                a.dynamicDescription.Add(cs.CloneJSON());
            }

            // Requirements
            a.abilitySubRequirements = new List<AbilityRequirement>();
            foreach (AbilityRequirement ar in d.abilitySubRequirements)
            {
                a.abilitySubRequirements.Add(ar.CloneJSON());
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

        #region Ability Usage Logic
        public void UseAbility(HexCharacterModel character, AbilityData ability, HexCharacterModel target = null, LevelNode tileTarget = null)
        {
            CombatLogController.Instance.CreateCharacterUsedAbilityEntry(character, ability, target);

            // Status effect for enemies + AI characters
            if (character.controller == Controller.AI)
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
                CombatUIController.Instance.EnergyBar.UpdateIcons(TurnController.Instance.EntityActivated.currentActionPoints);
            }

            OnAbilityUsedStart(character, ability, target);

            // Face target
            if (target != null && target != character && ability.abilityType.Contains(AbilityType.Skill) == false)
            {
                LevelController.Instance.FaceCharacterTowardsTargetCharacter(character, target);
            }
            else if (tileTarget != null && ability.abilityType.Contains(AbilityType.Skill) == false)
            {
                LevelController.Instance.FaceCharacterTowardsHex(character, tileTarget);
            }

            foreach (AbilityEffect e in ability.abilityEffects)
            {
                TriggerAbilityEffect(ability, e, character, target, tileTarget);
            }

            OnAbilityUsedFinish(character, ability);

            foreach (KeyValuePair<HexCharacterModel, Action> kwp in onAbilityEndVisualEventQueue)
            {
                if(CombatController.Instance.CurrentCombatState == CombatGameState.CombatActive &&
                    kwp.Key != null && 
                    kwp.Key.hexCharacterView != null && 
                    kwp.Key.livingState == LivingState.Alive)
                {
                    kwp.Value.Invoke();
                }
            }
            onAbilityEndVisualEventQueue.Clear();

            // Check for removal of damage/accuracy related tokens
            if (ability.abilityType.Contains(AbilityType.MeleeAttack) ||
                ability.abilityType.Contains(AbilityType.RangedAttack) ||
                ability.abilityType.Contains(AbilityType.WeaponAttack))
            {
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Wrath))
                {
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Wrath, -1);
                }
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Weakened))
                {
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Weakened, -1);
                }
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Focus))
                {
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Focus, -1);
                }
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Blinded))
                {
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Blinded, -1);
                }
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Combo))
                {
                    PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Combo, -1);
                }

                // Check and apply furiously assault to target
                if (target != null &&
                    target.livingState == LivingState.Alive &&
                    PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.FuriousAssault))
                {
                    PerkController.Instance.ModifyPerkOnCharacterEntity(target.pManager, Perk.FuriouslyAssaulted, 1);
                }
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
                    stack.isClosed = true;
                }
            }

            // Update Ability buttons: show/hide if useable or unuseable
            if (TurnController.Instance.EntityActivated == character && character.controller == Controller.Player)
            {
                // Update ability button validity overlays
                foreach (AbilityButton b in CombatUIController.Instance.AbilityButtons)
                {
                    b.UpdateAbilityButtonUnusableOverlay();
                }
            }
        }
        private void TriggerAbilityEffect(AbilityData ability, AbilityEffect abilityEffect, HexCharacterModel caster, HexCharacterModel target, LevelNode tileTarget = null, HexCharacterModel previousChainTarget = null)
        {

            bool triggerEffectEndEvents = true;

            // Stop and return if effect requires a target and that target is dying/dead/null/no longer valid      
            if ((target == null || target.livingState == LivingState.Dead) &&
                (
                    abilityEffect.effectType == AbilityEffectType.DamageTarget ||
                    abilityEffect.effectType == AbilityEffectType.ApplyPassiveTarget ||
                    abilityEffect.effectType == AbilityEffectType.RemovePassiveTarget ||
                    abilityEffect.effectType == AbilityEffectType.KnockBack ||
                    abilityEffect.effectType == AbilityEffectType.MoraleCheck
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
            if (!DoesAbilityEffectMeetAllRequirements(ability, abilityEffect, caster, target, tileTarget))
            {
                return;
            }

            // Determine weapon used
            ItemData weaponUsed = null;
            if (abilityEffect.weaponUsed == WeaponSlot.Offhand &&
                caster.itemSet.offHandItem != null)
            {
                weaponUsed = caster.itemSet.offHandItem;
            }

            else if (abilityEffect.weaponUsed == WeaponSlot.MainHand &&
                     caster.itemSet.mainHandItem != null)
            {
                weaponUsed = caster.itemSet.mainHandItem;
            }

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
                    Debug.Log(string.Format("AbilityController.TriggerAbilityEffect() weapon derived ability '{0}' does not exist on weapon '{1}', using off hand weapon instead", ability.abilityName, caster.itemSet.mainHandItem.itemName));
                    weaponUsed = caster.itemSet.offHandItem;
                }
            }

            // Queue starting anims and particles
            foreach (AnimationEventData vEvent in abilityEffect.visualEventsOnStart)
            {
                // if effect is not chained
                if (abilityEffect.chainedEffect == false)
                {
                    AnimationEventController.Instance.PlayAnimationEvent(vEvent, caster, target, tileTarget, weaponUsed);
                }

                // if effect is chained, the effect starts from previous chain target
                else if (abilityEffect.chainedEffect)
                {
                    AnimationEventController.Instance.PlayAnimationEvent(vEvent, previousChainTarget, target, tileTarget, weaponUsed);
                }
            }

            // RESOLVE EFFECT LOGIC START!
            Debug.Log("AbilityController.TriggerAbilityEffect() resolving effect " + abilityEffect.effectType.ToString() + " for ability " + ability.abilityName);

            // Damage Target
            if (abilityEffect.effectType == AbilityEffectType.DamageTarget)
            {
                triggerEffectEndEvents = false;
                HitRoll hitResult = CombatController.Instance.RollForHit(caster, target, ability, weaponUsed);
                bool didCrit = CombatController.Instance.RollForCrit(caster, target, ability, abilityEffect);

                CombatLogController.Instance.CreateCharacterHitResultEntry(caster, target, hitResult, didCrit);

                if (hitResult.Result == HitRollResult.Hit)
                {
                    // Gain 5 fatigue from being hit.
                    //HexCharacterController.Instance.ModifyCurrentFatigue(target, 5);

                    DamageResult damageResult = null;
                    damageResult = CombatController.Instance.GetFinalDamageValueAfterAllCalculations(caster, target, ability, abilityEffect, didCrit);
                    damageResult.didCrit = didCrit;

                    // Resolve bonus armour damage
                    if (abilityEffect.bonusArmourDamage > 0)
                    {
                        HexCharacterController.Instance.ModifyArmour(target, -abilityEffect.bonusArmourDamage);
                    }

                    // Do on hit visual effects for this ability
                    VisualEventManager.CreateVisualEvent(() => AudioManager.Instance.PlaySound(Sound.Crowd_Cheer_1), target.GetLastStackEventParent());
                    foreach (AnimationEventData vEvent in abilityEffect.visualEventsOnHit)
                    {
                        AnimationEventController.Instance.PlayAnimationEvent(vEvent, caster, target, null, weaponUsed, target.GetLastStackEventParent());
                    }

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
                    {
                        CombatController.Instance.CreateMoraleCheck(caster, caster.currentTile, StressEventType.LandedCriticalStrike);
                    }

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
                    if (ability.chainedEffects.Count > 0 && abilityEffect.chainedEffect == false && abilityEffect.triggersChainEffectSequence)
                    {
                        Allegiance targetAllegiance = Allegiance.Player;
                        if (caster.allegiance == Allegiance.Player)
                        {
                            targetAllegiance = Allegiance.Enemy;
                        }
                        else if (caster.allegiance == Allegiance.Enemy)
                        {
                            targetAllegiance = Allegiance.Player;
                        }

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
                            {
                                nextChainTarget = possibleTargets[0];
                            }
                            else if (possibleTargets.Count > 1)
                            {
                                nextChainTarget = possibleTargets[RandomGenerator.NumberBetween(0, possibleTargets.Count - 1)];
                            }

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

                        Debug.Log("Total loops of chain effect completed = " + totalLoopsCompleted);
                    }

                }
                else if (hitResult.Result == HitRollResult.Miss)
                {
                    // Gain 3 fatigue from being hit.
                    //HexCharacterController.Instance.ModifyCurrentFatigue(target, 2);

                    // Miss notification
                    VisualEventManager.CreateVisualEvent(() =>
                    {
                        VisualEffectManager.Instance.CreateStatusEffect(target.hexCharacterView.WorldPosition, "MISS");
                        AudioManager.Instance.PlaySound(Sound.Crowd_Ooh_1);
                        LevelController.Instance.AnimateCrowdOnMiss();
                    }, target.GetLastStackEventParent());

                    // Check Evasion
                    if (PerkController.Instance.DoesCharacterHavePerk(target.pManager, Perk.Evasion))
                    {
                        PerkController.Instance.ModifyPerkOnCharacterEntity(target.pManager, Perk.Evasion, -1);
                    }

                    // Check Crippled
                    if (PerkController.Instance.DoesCharacterHavePerk(target.pManager, Perk.Crippled))
                    {
                        PerkController.Instance.ModifyPerkOnCharacterEntity(target.pManager, Perk.Crippled, -1);
                    }

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
                Dictionary<HexCharacterModel, HitRoll> charactersHit = new Dictionary<HexCharacterModel, HitRoll>();

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
                else if (abilityEffect.aoeType == AoeType.Line)
                {
                    LevelNode firstHexHit = null;
                    if (target != null)
                    {
                        firstHexHit = target.currentTile;
                    }
                    else if (tileTarget != null)
                    {
                        firstHexHit = tileTarget;
                    }

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
                        // Tile in direction from current tile is valid to move over
                        tilesEffected.Add(nextHex);
                        previousHex = nextHex;
                        nextHex = null;
                    }
                }

                // Remove centre point, if needed
                if (abilityEffect.includeCentreTile == false && abilityEffect.aoeType != AoeType.Line)
                {
                    if ((abilityEffect.aoeType == AoeType.Aura || abilityEffect.aoeType == AoeType.Global) &&
                        tilesEffected.Contains(caster.currentTile))
                    {
                        tilesEffected.Remove(caster.currentTile);
                    }

                    else if (abilityEffect.aoeType == AoeType.AtTarget &&
                             tilesEffected.Contains(tileTarget))
                    {
                        tilesEffected.Remove(tileTarget);
                    }
                }

                Debug.Log("Tiles effected = " + tilesEffected.Count);

                // Filter out enemies or allies where applicable
                foreach (LevelNode h in tilesEffected)
                {
                    if (h.myCharacter != null)
                    {
                        if (h.myCharacter.allegiance == caster.allegiance && abilityEffect.effectsAllies ||
                            h.myCharacter.allegiance != caster.allegiance && abilityEffect.effectsEnemies)
                        {
                            charactersEffected.Add(h.myCharacter);
                        }
                    }
                }
                Debug.Log("Characters effected = " + charactersEffected.Count);

                // Roll for hits + play on Hit animations or on miss animations
                foreach (HexCharacterModel character in charactersEffected)
                {
                    VisualEventManager.CreateStackParentVisualEvent(character);

                    // Roll for hit
                    HitRoll hitRoll = CombatController.Instance.RollForHit(caster, character, ability, weaponUsed);

                    if (hitRoll.Result == HitRollResult.Hit)
                    {
                        charactersHit.Add(character, hitRoll);

                        // Gain 5 fatigue from being hit.
                        //HexCharacterController.Instance.ModifyCurrentFatigue(character, 5);

                        // Do on hit visual effects for this ability
                        VisualEventManager.CreateVisualEvent(() => AudioManager.Instance.PlaySound(Sound.Crowd_Cheer_1), character.GetLastStackEventParent());
                        foreach (AnimationEventData vEvent in abilityEffect.visualEventsOnHit)
                        {
                            AnimationEventController.Instance.PlayAnimationEvent(vEvent, caster, character, null, weaponUsed, character.GetLastStackEventParent());
                        }
                    }
                    else if (hitRoll.Result == HitRollResult.Miss)
                    {
                        CombatLogController.Instance.CreateCharacterHitResultEntry(caster, character, hitRoll);

                        // Gain 1 fatigue for dodging.
                        //HexCharacterController.Instance.ModifyCurrentFatigue(character, 2);

                        // Miss notification
                        Vector3 pos = character.hexCharacterView.WorldPosition;
                        VisualEventManager.CreateVisualEvent(() =>
                        {
                            if (character.hexCharacterView != null)
                            {
                                pos = character.hexCharacterView.WorldPosition;
                            }
                            VisualEffectManager.Instance.CreateStatusEffect(pos, "MISS");
                            AudioManager.Instance.PlaySound(Sound.Crowd_Ooh_1);
                            LevelController.Instance.AnimateCrowdOnMiss();
                        }, character.GetLastStackEventParent());

                        // Check Evasion
                        if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Evasion))
                        {
                            PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Evasion, -1);
                        }

                        // Check Crippled
                        if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Crippled))
                        {
                            PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Crippled, -1);
                        }

                        // Duck animation on miss
                        VisualEventManager.CreateVisualEvent(() =>
                            HexCharacterController.Instance.PlayDuckAnimation(character.hexCharacterView), character.GetLastStackEventParent());
                    }

                }

                // Calcuate and deal damage
                foreach (KeyValuePair<HexCharacterModel, HitRoll> character in charactersHit)
                {
                    bool didCrit = CombatController.Instance.RollForCrit(caster, character.Key, ability, abilityEffect);
                    DamageResult dResult = null;
                    dResult = CombatController.Instance.GetFinalDamageValueAfterAllCalculations(caster, character.Key, ability, abilityEffect, didCrit);
                    dResult.didCrit = didCrit;

                    CombatLogController.Instance.CreateCharacterHitResultEntry(caster, character.Key, character.Value, didCrit);

                    // Resolve bonus armour damage
                    if (abilityEffect.bonusArmourDamage > 0)
                    {
                        HexCharacterController.Instance.ModifyArmour(target, -abilityEffect.bonusArmourDamage);
                    }

                    // Deal damage
                    CombatController.Instance.HandleDamage(caster, character.Key, dResult, ability, abilityEffect, weaponUsed, false, character.Key.GetLastStackEventParent());

                    // On ability effect completed VFX
                    if (CombatController.Instance.CurrentCombatState == CombatGameState.CombatActive &&
                        caster.livingState == LivingState.Alive)
                    {
                        if (character.Key != null && character.Key.livingState == LivingState.Alive)
                        {
                            foreach (AnimationEventData vEvent in abilityEffect.visualEventsOnEffectFinish)
                            {
                                AnimationEventController.Instance.PlayAnimationEvent(vEvent, caster, character.Key, null, weaponUsed, character.Key.GetLastStackEventParent());
                            }
                        }
                    }

                    // On crit stress check
                    if (didCrit &&
                        CombatController.Instance.CurrentCombatState == CombatGameState.CombatActive &&
                        caster.livingState == LivingState.Alive)
                    {
                        CombatController.Instance.CreateMoraleCheck(caster, caster.currentTile, StressEventType.LandedCriticalStrike);
                    }

                    // Trigger on hit/crit effects
                    if (didCrit && ability.onCritEffects.Count > 0)
                    {
                        foreach (AbilityEffect e in ability.onCritEffects)
                        {
                            TriggerAbilityEffect(ability, e, caster, character.Key, tileTarget);
                        }
                    }
                    else
                    {
                        foreach (AbilityEffect e in ability.onHitEffects)
                        {
                            TriggerAbilityEffect(ability, e, caster, character.Key, tileTarget);
                        }
                    }
                }

            }

            // Morale Check
            else if (abilityEffect.effectType == AbilityEffectType.MoraleCheck)
            {
                CombatController.Instance.CreateMoraleCheck(target, abilityEffect.stressEventData, true, true);
            }

            // Morale Check AoE
            else if (abilityEffect.effectType == AbilityEffectType.MoraleCheckAoe)
            {
                triggerEffectEndEvents = false;
                List<LevelNode> tilesEffected = new List<LevelNode>();
                List<HexCharacterModel> charactersEffected = new List<HexCharacterModel>();
                List<HexCharacterModel> charactersHit = new List<HexCharacterModel>();

                // Get Aoe Area
                if (abilityEffect.aoeType == AoeType.Aura)
                {
                    tilesEffected = HexCharacterController.Instance.GetCharacterAura(caster, abilityEffect.includeCentreTile);
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

                // Filter out enemies or allies where applicable
                foreach (LevelNode h in tilesEffected)
                {
                    if (h.myCharacter != null)
                    {
                        if (h.myCharacter.allegiance == caster.allegiance && abilityEffect.effectsAllies ||
                            h.myCharacter.allegiance != caster.allegiance && abilityEffect.effectsEnemies)
                        {
                            charactersEffected.Add(h.myCharacter);
                        }
                    }
                }

                // Roll for hits + play on Hit animations or on miss animations
                foreach (HexCharacterModel character in charactersEffected)
                {
                    VisualEventManager.CreateStackParentVisualEvent(character);
                    bool hit = CombatController.Instance.CreateMoraleCheck(character, abilityEffect.stressEventData, false, true);
                    if(hit) charactersHit.Add(character);
                }

                // On morale change succesful events + effects
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

            // Apply passive to target
            else if (abilityEffect.effectType == AbilityEffectType.ApplyPassiveTarget)
            {
                int stacks = abilityEffect.perkPairing.passiveStacks;
                bool roll = RandomGenerator.NumberBetween(1, 100) <= abilityEffect.perkApplicationChance;
                bool success = false;

                if (roll)
                {
                    success = PerkController.Instance.ModifyPerkOnCharacterEntity(target.pManager, abilityEffect.perkPairing.perkTag, stacks, true, 0.5f, caster.pManager);
                }

                if (success)
                {
                    foreach (AnimationEventData vEvent in abilityEffect.visualEventsOnHit)
                    {
                        AnimationEventController.Instance.PlayAnimationEvent(vEvent, caster, target, null, null, target.GetLastStackEventParent());
                    }

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
                    {
                        tilesEffected.Remove(caster.currentTile);
                    }

                    else if (abilityEffect.aoeType == AoeType.AtTarget &&
                             tilesEffected.Contains(tileTarget))
                    {
                        tilesEffected.Remove(tileTarget);
                    }
                }

                Debug.Log("Tiles effected = " + tilesEffected.Count);

                // Filter out enemies or allies where applicable
                foreach (LevelNode h in tilesEffected)
                {
                    if (h.myCharacter != null)
                    {
                        if (h.myCharacter.allegiance == caster.allegiance && abilityEffect.effectsAllies ||
                            h.myCharacter.allegiance != caster.allegiance && abilityEffect.effectsEnemies)
                        {
                            charactersEffected.Add(h.myCharacter);
                        }
                    }
                }
                Debug.Log("Characters effected = " + charactersEffected.Count);

                // Roll for hits + play on Hit animations or on miss animations
                foreach (HexCharacterModel character in charactersEffected)
                {
                    VisualEventManager.CreateStackParentVisualEvent(character);
                }

                // Apply Passive
                foreach (HexCharacterModel character in charactersEffected)
                {
                    bool roll = RandomGenerator.NumberBetween(1, 100) <= abilityEffect.perkApplicationChance;
                    bool success = false;

                    if (roll)
                    {
                        success = PerkController.Instance.ModifyPerkOnCharacterEntity
                            (character.pManager, abilityEffect.perkPairing.perkTag, abilityEffect.perkPairing.passiveStacks, true, 0f, caster.pManager);
                    }

                    if (success)
                    {
                        charactersHit.Add(character);
                    }
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

                if (roll)
                {
                    PerkController.Instance.ModifyPerkOnCharacterEntity(caster.pManager, abilityEffect.perkPairing.perkTag, stacks, true, 0.5f, caster.pManager);
                }

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
                    // Tile in direction from current tile is valid to move over
                    hexsOnPath.Add(nextHex);
                    previousHex = nextHex;
                    nextHex = null;
                }

                // Filter out enemies or allies where applicable
                foreach (LevelNode h in hexsOnPath)
                {
                    if (h.myCharacter != null)
                    {
                        charactersEffected.Add(h.myCharacter);
                    }
                }
                Debug.Log("Characters effected = " + charactersEffected.Count);

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
                    {
                        success = PerkController.Instance.ModifyPerkOnCharacterEntity
                            (character.pManager, abilityEffect.perkPairing.perkTag, abilityEffect.perkPairing.passiveStacks, true, 0f, caster.pManager);
                    }

                    if (success)
                    {
                        charactersHit.Add(character);
                    }
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
                {
                    PerkController.Instance.ModifyPerkOnCharacterEntity(target.pManager, abilityEffect.perkPairing.perkTag, -stacks, true, 0.5f, caster.pManager);
                }
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
                LevelController.Instance.HandleTeleportCharacter(caster, tileTarget, abilityEffect.teleportVFX);
            }

            // Teleport Target
            else if (abilityEffect.effectType == AbilityEffectType.TeleportTargetToTile)
            {
                LevelController.Instance.HandleTeleportCharacter(target, tileTarget, abilityEffect.teleportVFX);
            }

            // Teleport Switch With Target
            else if (abilityEffect.effectType == AbilityEffectType.TeleportSwitchWithTarget)
            {
                LevelController.Instance.HandleTeleportSwitchTwoCharacters(caster, target, abilityEffect.teleportVFX);
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
                    {
                        validTiles.Add(h);
                    }
                }

                if (validTiles.Count == 0)
                {
                    return;
                }
                if (validTiles.Count == 1)
                {
                    destination = validTiles[0];
                }
                else
                {
                    destination = validTiles[RandomGenerator.NumberBetween(0, validTiles.Count - 1)];
                }

                LevelController.Instance.HandleTeleportCharacter(caster, destination, abilityEffect.teleportVFX);
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
                    if (nextHex != null && !Pathfinder.CanHexBeOccupied(nextHex))
                    {
                        // Collision end point found
                        //obstructionHex = nextHex;
                        break;
                    }
                    if (nextHex != null)
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
                            if (Pathfinder.CanHexBeOccupied(h))
                            {
                                previousTile = h;
                            }
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
                    {
                        break;
                    }
                }

                // found a hex that is valid to be knocked on to?
                if (startingTile != previousTile)
                {
                    LevelController.Instance.HandleKnockBackCharacter(target, previousTile);
                }

                // Characters that are knocked into on obstacle/player become stunned.
                if (shouldStun)
                {
                    // Knock back and forth towards obstruction
                    if (obstruction != null)
                    {
                        TaskTracker tracker = new TaskTracker();
                        VisualEventManager.CreateVisualEvent(() =>
                        {
                            Vector2 destination = Vector2.MoveTowards(target.currentTile.WorldPosition, caster.currentTile.WorldPosition, -0.5f);
                            HexCharacterController.Instance.TriggerKnockedBackIntoObstructionAnimation
                                (target.hexCharacterView, destination, tracker);
                        }).SetCoroutineData(tracker);
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
                            {
                                possibleTiles.Add(h);
                            }
                        }

                        if (possibleTiles.Count > 0)
                        {
                            possibleTiles.Shuffle();
                            spawnLocation = possibleTiles[0];
                        }
                    }

                    if (!spawnLocation)
                    {
                        break;
                    }

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
                    HexCharacterController.Instance.FadeOutCharacterModel(view.model, 0);
                    HexCharacterController.Instance.FadeOutCharacterShadow(view, 0);

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
                    List<HexCharacterModel> cachedOrder = TurnController.Instance.ActivationOrder.ToList();
                    VisualEventManager.CreateVisualEvent(() =>
                    {
                        TurnController.Instance.UpdateWindowPositions(cachedOrder);
                        TurnController.Instance.MoveActivationArrowTowardsEntityWindow(entityActivated);
                        HexCharacterController.Instance.FadeInCharacterWorldCanvas(view, null, abilityEffect.uiFadeInSpeed);
                        CharacterModeller.FadeInCharacterModel(view.model, abilityEffect.modelFadeInSpeed);
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
            // HexCharacterController.Instance.ModifyCurrentFatigue(character, GetAbilityFatigueCost(character, ability));

            // Check shed perk: remove if ability used was an aspect ability
            if (ability.abilityType.Contains(AbilityType.Aspect) && PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Shed))
            {
                PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Shed, -1);
            }

            // Increment skills used this turn
            character.abilitiesUsedThisCombat++;
            if (ability.abilityType.Contains(AbilityType.Skill))
            {
                character.skillAbilitiesUsedThisTurn++;
            }
            else if (ability.abilityType.Contains(AbilityType.RangedAttack))
            {
                character.rangedAttackAbilitiesUsedThisTurn++;
            }
            else if (ability.abilityType.Contains(AbilityType.MeleeAttack))
            {
                character.meleeAttackAbilitiesUsedThisTurn++;
            }
            else if (ability.abilityType.Contains(AbilityType.WeaponAttack))
            {
                character.weaponAbilitiesUsedThisTurn++;
            }
            else if (ability.abilityType.Contains(AbilityType.Spell))
            {
                character.spellAbilitiesUsedThisTurn++;
            }
        }
        private void OnAbilityUsedFinish(HexCharacterModel character, AbilityData ability, HexCharacterModel target = null)
        {
            SetAbilityOnCooldown(ability);
            if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Stealth) &&
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
                foreach (AbilityButton b in CombatUIController.Instance.AbilityButtons)
                {
                    if (b.MyAbilityData == ability)
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

        #region Input
        public void OnAbilityButtonClicked(AbilityButton b)
        {
            if (b.MyAbilityData == null)
            {
                return;
            }

            if(CurrentAbilityAwaiting != null && CurrentAbilityAwaiting == b.MyAbilityData)
            {
                HandleCancelCurrentAbilityOrder();
            }

            else if (IsAbilityUseable(b.MyAbilityData.myCharacter, b.MyAbilityData))
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

            if (CurrentAbilityAwaiting != null || ability.targetRequirement == TargetRequirement.NoTarget)
            {
                // player clicked a different ability, handle new selection
                TargetGuidanceController.Instance.Hide(0);
                LevelController.Instance.UnmarkAllTiles();
                LevelController.Instance.UnmarkAllSubTargetMarkers();
                AbilityButton selectedButton = CombatUIController.Instance.FindAbilityButton(CurrentAbilityAwaiting);
                if (selectedButton != null)
                {
                    selectedButton.SetSelectedGlow(false);
                }
            }

            // cache ability, get ready for second click, or for instant use
            CurrentAbilityAwaiting = b.MyAbilityData;
            CurrentSelectionPhase = AbilitySelectionPhase.None;

            // Set cursor
            CursorController.Instance.SetFallbackCursor(CursorType.TargetClick);
            CursorController.Instance.SetCursor(CursorType.TargetClick);

            // Highlight tiles in range of ability
            if (ability.targetRequirement != TargetRequirement.NoTarget)
            {
                bool neutral = true;
                if (ability.targetRequirement == TargetRequirement.Enemy)
                {
                    neutral = false;
                }
                CombatUIController.Instance.FindAbilityButton(CurrentAbilityAwaiting).SetSelectedGlow(true);
                LevelController.Instance.MarkTilesInRange(GetTargettableTilesOfAbility(ability, caster), neutral);
                TargetGuidanceController.Instance.BuildAndShow(ability.guidanceInstruction, 0.5f);
            }
            else if (ability.targetRequirement == TargetRequirement.NoTarget)
            {
                UseAbility(caster, CurrentAbilityAwaiting);
                HandleCancelCurrentAbilityOrder();
            }

            // Handle keyboard pressed to trigger ability click, and hit chance
            // modal is open while moused over the previous target
            if (AwaitingAbilityOrder() && CurrentAbilityAwaiting.targetRequirement == TargetRequirement.Enemy &&
                LevelController.HexMousedOver != null && LevelController.HexMousedOver.myCharacter != null)
            {
                ShowHitChancePopup(TurnController.Instance.EntityActivated, LevelController.HexMousedOver.myCharacter,
                    CurrentAbilityAwaiting, TurnController.Instance.EntityActivated.itemSet.mainHandItem);
            }
            else
            {
                HideHitChancePopup();
            }
        }
        public void HandleTargetSelectionMade(HexCharacterModel target)
        {
            HexCharacterModel caster = CurrentAbilityAwaiting.myCharacter;
            if (IsTargetOfAbilityValid(caster, target, CurrentAbilityAwaiting))
            {
                // multiple target selection abilities (e.g. telekinesis, phase shift, etc)
                if (CurrentAbilityAwaiting.secondaryTargetRequirement != SecondaryTargetRequirement.None &&
                    CurrentSelectionPhase == AbilitySelectionPhase.None)
                {
                    // Player has made their first selection, and the selection is valid, prepare for the 2nd selection
                    if (CurrentAbilityAwaiting.secondaryTargetRequirement == SecondaryTargetRequirement.UnoccupiedHexWithinRangeOfTarget)
                    {
                        List<LevelNode> validHexs = new List<LevelNode>();
                        foreach (LevelNode h in LevelController.Instance.GetAllHexsWithinRange(target.currentTile, CurrentAbilityAwaiting.rangeFromTarget))
                        {
                            if (Pathfinder.CanHexBeOccupied(h))
                            {
                                validHexs.Add(h);
                            }
                        }

                        // If player selected a target with no valid adjacent tiles, cancel the ability selection process
                        if (validHexs.Count == 0)
                        {
                            return;
                        }
                        // Get ready for second selection
                        CurrentSelectionPhase = AbilitySelectionPhase.First;
                        LevelController.Instance.UnmarkAllTiles();
                        LevelController.Instance.UnmarkAllSubTargetMarkers();
                        TargetGuidanceController.Instance.Hide(0);
                        bool neutral = true;
                        if (CurrentAbilityAwaiting.targetRequirement == TargetRequirement.Enemy)
                        {
                            neutral = false;
                        }
                        LevelController.Instance.MarkTilesInRange(validHexs, neutral);
                        TargetGuidanceController.Instance.BuildAndShow(CurrentAbilityAwaiting.guidanceInstructionTwo);
                        firstSelectionCharacter = target;
                    }

                }

                // single target selection abilities
                else
                {
                    UseAbility(caster, CurrentAbilityAwaiting, target);
                    HandleCancelCurrentAbilityOrder();
                }

            }
        }
        public void HandleTargetSelectionMade(LevelNode target)
        {
            // overload function for abilities that target a hex, not a character (e.g. teleport to location, throw grenade at location, etc)
            HexCharacterModel caster = CurrentAbilityAwaiting.myCharacter;
            if (CurrentSelectionPhase == AbilitySelectionPhase.None && IsTargetOfAbilityValid(caster, target, CurrentAbilityAwaiting))
            {
                UseAbility(caster, CurrentAbilityAwaiting, null, target);
                HandleCancelCurrentAbilityOrder();
            }
            else if (CurrentSelectionPhase == AbilitySelectionPhase.First)
            {
                // Secondary target that must be an unoccupied hex with range of the first character selection
                if (CurrentAbilityAwaiting.secondaryTargetRequirement == SecondaryTargetRequirement.UnoccupiedHexWithinRangeOfTarget)
                {
                    // is the tile selected valid?
                    if (Pathfinder.CanHexBeOccupied(target) && LevelController.Instance.GetAllHexsWithinRange(firstSelectionCharacter.currentTile, CurrentAbilityAwaiting.rangeFromTarget).Contains(target))
                    {
                        // it is, use the ability

                        // make sure that the target is teleportable if the effect is a teleport
                        if (CurrentAbilityAwaiting.abilityEffects.Count > 0 &&
                            CurrentAbilityAwaiting.abilityEffects[0].effectType == AbilityEffectType.TeleportTargetToTile &&
                            !HexCharacterController.Instance.IsCharacterTeleportable(firstSelectionCharacter))
                        {
                            return;
                        }

                        UseAbility(caster, CurrentAbilityAwaiting, firstSelectionCharacter, target);
                        HandleCancelCurrentAbilityOrder();
                    }
                }
            }
        }
        public void HandleCancelCurrentAbilityOrder()
        {
            AbilityButton selectedButton = CombatUIController.Instance.FindAbilityButton(CurrentAbilityAwaiting);
            if (selectedButton != null)
            {
                selectedButton.SetSelectedGlow(false);
            }
            CurrentAbilityAwaiting = null;
            firstSelectionCharacter = null;
            CurrentSelectionPhase = AbilitySelectionPhase.None;
            LevelController.Instance.UnmarkAllSubTargetMarkers();
            LevelController.Instance.UnmarkAllTiles();
            TargetGuidanceController.Instance.Hide();
            CursorController.Instance.SetFallbackCursor(CursorType.NormalPointer);
            CursorController.Instance.SetCursor(CursorType.NormalPointer);
            HideHitChancePopup();
        }
        private void Update()
        {
            if (GameController.Instance.GameState != GameState.CombatActive)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                HandleCancelCurrentAbilityOrder();
            }

            if (hitChanceRootCanvas.isActiveAndEnabled &&
                LevelController.HexMousedOver != null &&
                hitChancePositionParent.transform.position != LevelController.HexMousedOver.WorldPosition)
            {
                hitChancePositionParent.transform.position = LevelController.HexMousedOver.WorldPosition;
            }
        }

        #endregion

        #region Get Ability Calculations
        public int GetAbilityActionPointCost(HexCharacterModel character, AbilityData ability)
        {
            int apCost = ability.energyCost;

            // Check shed perk: aspect ability costs 0
            if (ability.abilityType.Contains(AbilityType.Aspect) && PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Shed))
            {
                return 0;
            }

            // Gifted Perk
            if (character != null &&
                character.abilitiesUsedThisCombat == 0 &&
                PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Gifted))
            {
                apCost -= 2;
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

            // prevent cost going negative
            if (apCost < 0)
            {
                apCost = 0;
            }

            return apCost;
        }
        public List<LevelNode> GetTargettableTilesOfAbility(AbilityData ability, HexCharacterModel caster)
        {
            List<LevelNode> targettableTiles = new List<LevelNode>();
            bool includeSelfTile = false;
            if (ability.targetRequirement == TargetRequirement.AllyOrSelf || ability.targetRequirement == TargetRequirement.AllCharacters)
            {
                includeSelfTile = true;
            }

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
                {
                    rangeReturned += 1;
                }

                if (ability.abilityType.Contains(AbilityType.Spell) && PerkController.Instance.DoesCharacterHavePerk(caster.pManager, Perk.SpellSight))
                {
                    rangeReturned += 1;
                }
            }

            if (caster.itemSet.mainHandItem != null && ability.abilityType.Contains(AbilityType.WeaponAttack))
            {
                rangeReturned += ItemController.Instance.GetInnateModifierFromWeapon(InnateItemEffectType.BonusMeleeRange, caster.itemSet.mainHandItem);
            }

            if (rangeReturned < 1)
            {
                rangeReturned = 1;
            }

            Debug.Log("Final calculated range of '" + ability.abilityName + "' is " + rangeReturned);
            return rangeReturned;
        }

        #endregion

        #region Ability Useability + Validation Logic
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
                        Debug.Log("AbilityController.DoesAbilityEffectMeetAllRequirements() failed 'Target Has Perk' requirement check (required" + req.perk + ").");
                        bRet = false;
                        break;
                    }
                }

                else if (req.requirementType == AbilityEffectRequirementType.CasterHasPerk)
                {
                    if (!PerkController.Instance.DoesCharacterHavePerk(character.pManager, req.perk))
                    {
                        Debug.Log("AbilityController.DoesAbilityEffectMeetAllRequirements() failed 'Caster Has Perk' requirement check (required" + req.perk + ").");
                        bRet = false;
                        break;
                    }
                }

                else if (req.requirementType == AbilityEffectRequirementType.TargetDoesNotHavePerk)
                {
                    if (PerkController.Instance.DoesCharacterHavePerk(target.pManager, req.perk))
                    {
                        Debug.Log("AbilityController.DoesAbilityEffectMeetAllRequirements() failed 'Target Does Not Have Perk' requirement check (required" + req.perk + ").");
                        bRet = false;
                        break;
                    }
                }
                else if (req.requirementType == AbilityEffectRequirementType.CasterDoesNotHavePerk)
                {
                    if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, req.perk))
                    {
                        Debug.Log("AbilityController.DoesAbilityEffectMeetAllRequirements() failed 'Caster Does Not Have Perk' requirement check (required" + req.perk + ").");
                        bRet = false;
                        break;
                    }
                }
            }

            return bRet;
        }
        public bool IsTargetOfAbilityValid(HexCharacterModel caster, HexCharacterModel target, AbilityData ability)
        {
            if (ability.targetRequirement == TargetRequirement.NoTarget)
            {
                return true;
            }

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

            Debug.Log("AbilityController.IsTargetOfAbilityValid() returning " + bRet);
            return bRet;
        }
        private bool IsTargetOfAbilityValid(HexCharacterModel caster, LevelNode target, AbilityData ability)
        {
            if (ability.targetRequirement == TargetRequirement.NoTarget)
            {
                return true;
            }

            Debug.Log("AbilityController.IsTargetOfAbilityValid() called, validating '" + ability.abilityName + "' usage by character " + caster.myName +
                " on hex tile " + target.GridPosition.x + ", " + target.GridPosition.y);

            // Function used AFTER "IsAbilityUseable" to check if the selected target of the ability is valid.
            bool bRet = false;

            // check target is in range
            if (IsTargetOfAbilityInRange(caster, target, ability) &&
                DoesAbilityMeetSubRequirements(caster, ability, null, target))
            {
                bRet = true;
            }
            else
            {
                ActionErrorGuidanceController.Instance.ShowErrorMessage(caster, "Target is out of range.");
            }

            Debug.Log("AbilityController.IsTargetOfAbilityValid() returning " + bRet);
            return bRet;
        }
        public bool IsAbilityUseable(HexCharacterModel character, AbilityData ability, bool showErrors = true)
        {
            // This called when the player first clicks an ability button
            Debug.Log("AbilityController.IsAbilityUseable() called, validating '" + ability.abilityName + "' usage by character " + character.myName);

            bool bRet = true;

            // check its actually characters turn
            if (TurnController.Instance.EntityActivated != character ||
                TurnController.Instance.EntityActivated == character && character.activationPhase != ActivationPhase.ActivationPhase)
            {
                Debug.Log("IsAbilityUseable() returning false: cannot use abilities when it is not your turn");
                if (showErrors)
                {
                    ActionErrorGuidanceController.Instance.ShowErrorMessage(character, "It is not this character's turn.");
                }
                bRet = false;
            }

            // Check movement impairment
            foreach (AbilityEffect ef in ability.abilityEffects)
            {
                if ((ef.effectType == AbilityEffectType.MoveInLine ||
                        ef.effectType == AbilityEffectType.MoveToTile) &&
                    !HexCharacterController.Instance.IsCharacterAbleToMove(character))
                {
                    if (showErrors)
                    {
                        ActionErrorGuidanceController.Instance.ShowErrorMessage(character, "Character is unable to move right now.");
                    }
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
                if (showErrors)
                {
                    ActionErrorGuidanceController.Instance.ShowErrorMessage(character, "Not enough Action Points.");
                }
                bRet = false;
            }

            // Check sub reqs
            if (!DoesAbilityMeetSubRequirements(character, ability))
            {
                Debug.Log("IsAbilityUseable() returning false: did not meet sub requirements");
                bRet = false;
            }

            // check cooldown
            if (ability.currentCooldown != 0)
            {
                Debug.Log("IsAbilityUseable() returning false: ability is on cooldown");
                if (showErrors)
                {
                    ActionErrorGuidanceController.Instance.ShowErrorMessage(character, "Ability is on cooldown.");
                }
                bRet = false;
            }

            // check weapon requirement
            if (!DoesCharacterMeetAbilityWeaponRequirement(character.itemSet, ability.weaponRequirement))
            {
                Debug.Log("IsAbilityUseable() returning false: character does not meet the ability's weapon requirement");
                bRet = false;
            }

            // Check smashed shield
            if (ability.weaponRequirement == WeaponRequirement.Shield &&
                PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.SmashedShield))
            {
                Debug.Log("IsAbilityUseable() returning false: character has 'Smashed Shield' perk");
                bRet = false;
            }

            // Check unloaded crossbow
            if (ability.abilityType.Contains(AbilityType.WeaponAttack) &&
                ability.abilityType.Contains(AbilityType.RangedAttack) &&
                PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Reload))
            {
                Debug.Log("IsAbilityUseable() returning false: character trying to use a ranged weapon attack with 'Reload' status...");
                if (showErrors)
                {
                    ActionErrorGuidanceController.Instance.ShowErrorMessage(character, "Character needs to reload first.");
                }
                bRet = false;
            }

            return bRet;
        }
        public bool DoesCharacterMeetAbilityWeaponRequirement(ItemSet itemSet, WeaponRequirement weaponReq)
        {
            bool bRet = false;

            if (weaponReq == WeaponRequirement.None)
            {
                bRet = true;
            }

            else if (weaponReq == WeaponRequirement.MeleeWeapon &&
                     itemSet.mainHandItem != null &&
                     itemSet.mainHandItem.IsMeleeWeapon)
            {
                bRet = true;
            }

            else if (weaponReq == WeaponRequirement.RangedWeapon &&
                     itemSet.mainHandItem != null &&
                     itemSet.mainHandItem.IsRangedWeapon)
            {
                bRet = true;
            }

            else if (weaponReq == WeaponRequirement.Shield &&
                     itemSet.offHandItem != null &&
                     itemSet.offHandItem.weaponClass == WeaponClass.Shield)
            {
                bRet = true;
            }

            else if (weaponReq == WeaponRequirement.Bow &&
                     itemSet.mainHandItem != null &&
                     itemSet.mainHandItem.weaponClass == WeaponClass.Bow)
            {
                bRet = true;
            }

            else if (weaponReq == WeaponRequirement.Crossbow &&
                     itemSet.mainHandItem != null &&
                     itemSet.mainHandItem.weaponClass == WeaponClass.Crossbow)
            {
                bRet = true;
            }

            else if (weaponReq == WeaponRequirement.BowOrCrossbow &&
                     itemSet.mainHandItem != null &&
                     (itemSet.mainHandItem.weaponClass == WeaponClass.Crossbow || itemSet.mainHandItem.weaponClass == WeaponClass.Bow))
            {
                bRet = true;
            }

            else if (weaponReq == WeaponRequirement.ThrowingNet &&
                     itemSet.offHandItem != null &&
                     itemSet.offHandItem.weaponClass == WeaponClass.ThrowingNet)
            {
                bRet = true;
            }

            else if (weaponReq == WeaponRequirement.EmptyOffhand &&
                     itemSet.offHandItem == null)
            {
                bRet = true;
            }

            return bRet;
        }
        private bool DoesCharacterHaveEnoughActionPoints(HexCharacterModel caster, AbilityData ability)
        {
            return caster.currentActionPoints >= GetAbilityActionPointCost(caster, ability);
        }
        private bool IsTargetOfAbilityInRange(HexCharacterModel caster, HexCharacterModel target, AbilityData ability)
        {
            bool bRet = false;
            bRet = GetTargettableTilesOfAbility(ability, caster).Contains(target.currentTile);
            bool showErrorPanel = false;
            if (TurnController.Instance.EntityActivated != null &&
                TurnController.Instance.EntityActivated.controller == Controller.Player)
            {
                showErrorPanel = true;
            }

            if (!bRet)
            {
                if (showErrorPanel)
                {
                    ActionErrorGuidanceController.Instance.ShowErrorMessage(caster, "Target is out of range.");
                }
                return false;
            }

            int stealthDistance = StatCalculator.GetTotalVision(caster) + 1;
            bool ignoreStealth = PerkController.Instance.DoesCharacterHavePerk(caster.pManager, Perk.TrueSight) ||
                PerkController.Instance.DoesCharacterHavePerk(caster.pManager, Perk.EyelessSight);

            if (stealthDistance < 1)
            {
                stealthDistance = 1;
            }

            // Check stealth + true eye / sniper (ignores stealth)
            if (caster.currentTile.Distance(target.currentTile) > stealthDistance &&
                HexCharacterController.Instance.IsTargetFriendly(caster, target) == false &&
                PerkController.Instance.DoesCharacterHavePerk(target.pManager, Perk.Stealth) &&
                !ignoreStealth)
            {
                if (showErrorPanel)
                {
                    ActionErrorGuidanceController.Instance.ShowErrorMessage(caster, "Target has Stealth.");
                }
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

            if (ability.targetRequirement == TargetRequirement.Ally &&
                allies.Contains(target))
            {
                bRet = true;
            }
            else if (ability.targetRequirement == TargetRequirement.AllyOrSelf &&
                     (allies.Contains(target) || caster == target))
            {
                bRet = true;
            }
            else if (ability.targetRequirement == TargetRequirement.Enemy &&
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
                ActionErrorGuidanceController.Instance.ShowErrorMessage(caster, "Invalid target for this ability.");
            }

            Debug.Log("AbilityController.DoesTargetOfAbilityMeetAllegianceRequirement() returning " + bRet);
            return bRet;

        }
        private bool DoesAbilityMeetSubRequirements(HexCharacterModel caster, AbilityData ability, HexCharacterModel target = null, LevelNode targetTile = null)
        {
            bool bRet = true;

            // check its actually characters turn
            if (TurnController.Instance.EntityActivated != caster ||
                TurnController.Instance.EntityActivated == caster && caster.activationPhase != ActivationPhase.ActivationPhase)
            {
                Debug.Log("IsAbilityUseable() returning false: cannot use abilities when it is not your turn");
                ActionErrorGuidanceController.Instance.ShowErrorMessage(caster, "It is not this character's turn.");
                bRet = false;
            }

            foreach (AbilityRequirement ar in ability.abilitySubRequirements)
            {
                if (ar.type == AbilityRequirementType.TargetHasUnoccupiedBackTile)
                {
                    if (target == null)
                    {
                        continue;
                    }

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
                    if (shouldContinue)
                    {
                        continue;
                    }
                    ActionErrorGuidanceController.Instance.ShowErrorMessage(caster, "Target's back tiles are obstructed.");
                }

                else if (ar.type == AbilityRequirementType.TargetHasRace)
                {
                    if (target == null || target != null && target.race == ar.race)
                    {
                        continue;
                    }
                }

                else if (ar.type == AbilityRequirementType.TargetHasPerk)
                {
                    if (target == null || target != null && PerkController.Instance.DoesCharacterHavePerk(target.pManager, ar.perk))
                    {
                        continue;
                    }
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
                    if (target == null || target != null && !PerkController.Instance.DoesCharacterHavePerk(target.pManager, ar.perk))
                    {
                        continue;
                    }
                }

                else if (ar.type == AbilityRequirementType.TargetHasShield)
                {
                    if (target == null || target != null && target.itemSet.offHandItem != null && target.itemSet.offHandItem.weaponClass == WeaponClass.Shield)
                    {
                        continue;
                    }
                    ActionErrorGuidanceController.Instance.ShowErrorMessage(caster, "Target does not have a shield.");
                }

                else if (ar.type == AbilityRequirementType.TargetIsTeleportable)
                {
                    if (target == null || target != null && HexCharacterController.Instance.IsCharacterTeleportable(target))
                    {
                        continue;
                    }
                    ActionErrorGuidanceController.Instance.ShowErrorMessage(caster, "Target cannot be teleported.");
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

                else if (targetTile == null ||
                         (ar.type == AbilityRequirementType.TargetTileCanBeOccupied &&
                         Pathfinder.CanHexBeOccupied(targetTile)))
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
            return CurrentAbilityAwaiting != null;
        }
        #endregion

        #region Hit Chance Popup Logic
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

                    if (effect != null)
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

                else if (ability.abilityType.Contains(AbilityType.Spell) || ability.abilityType.Contains(AbilityType.Skill))
                {
                    AbilityEffect effect = FindDebuffEffect(ability);
                    if (effect == null)
                    {
                        return;
                    }

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
            if (Application.isMobilePlatform && Input.touchCount == 0) return;
            hitChanceBoxesParent.SetActive(true);
            hitChanceHeaderParent.SetActive(true);

            // Header text
            hitChanceText.text = TextLogic.ReturnColoredText(data.FinalHitChance + "%", TextLogic.neutralYellow) + " chance to hit";

            // Reset tabs
            for (int i = 0; i < hitChanceBoxes.Length; i++)
            {
                hitChanceBoxes[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < data.details.Count; i++)
            {
                string extra = data.details[i].accuracyMod > 0 ? "+" : "";
                if (i == hitChanceBoxes.Length - 1)
                {
                    break;
                }
                hitChanceBoxes[i].Build(data.details[i].reason + ": " +
                    TextLogic.ReturnColoredText(extra + data.details[i].accuracyMod,
                        TextLogic.neutralYellow), data.details[i].accuracyMod > 0 ? DotStyle.Green : DotStyle.Red);
            }
        }
        private void BuildApplyPassiveChancePopup(HitChanceDataSet data, Sprite iconSprite)
        {
            applyPassiveBoxesParent.SetActive(true);
            applyPassiveHeaderParent.SetActive(true);

            // Header text
            applyPassiveText.text = TextLogic.ReturnColoredText(data.FinalHitChance + "%", TextLogic.neutralYellow) + " chance";
            applyPassiveIcon.PerkImage.sprite = iconSprite;

            // Reset tabs
            for (int i = 0; i < applyPassiveBoxes.Length; i++)
            {
                applyPassiveBoxes[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < data.details.Count; i++)
            {
                string extra = data.details[i].accuracyMod > 0 ? "+" : "";
                if (i == applyPassiveBoxes.Length - 1)
                {
                    break;
                }

                string accuracyModText = ": " + TextLogic.ReturnColoredText(extra + data.details[i].accuracyMod, TextLogic.neutralYellow);
                if (data.details[i].hideAccuracyMod)
                {
                    accuracyModText = "";
                }
                applyPassiveBoxes[i].Build(data.details[i].reason + accuracyModText, data.details[i].accuracyMod > 0 ? DotStyle.Green : DotStyle.Red);
            }
        }
        public void HideHitChancePopup()
        {
            hitChanceCg.alpha = 1;
            hitChanceCg.DOKill();
            hitChanceCg.DOFade(0f, 0.15f).OnComplete(() => hitChanceRootCanvas.enabled = false);
        }
        private AbilityEffect FindDebuffEffect(AbilityData ability)
        {
            AbilityEffect ret = null;

            foreach (AbilityEffect e in ability.abilityEffects)
            {
                if (e.effectType == AbilityEffectType.ApplyPassiveTarget)
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
    }
}
