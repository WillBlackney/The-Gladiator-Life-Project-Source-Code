using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WeAreGladiators.Characters;
using WeAreGladiators.Combat;
using WeAreGladiators.Perks;
using WeAreGladiators.VisualEvents;

namespace WeAreGladiators.Abilities
{
    [Serializable]
    public class AbilityEffect
    {

        // Effect Range Properties
        [BoxGroup("Effect Range Settings", true, true)]
        [LabelWidth(200)]
        [ShowIf("ShowAoeType")]
        public AoeType aoeType;

        [BoxGroup("Effect Range Settings")]
        [LabelWidth(200)]
        [ShowIf("ShowIncludeCentreTile")]
        public bool includeCentreTile = true;

        [BoxGroup("Effect Range Settings")]
        [LabelWidth(200)]
        [ShowIf("ShowAoeSize")]
        [Range(1, 3)]
        public int aoeSize = 1;

        [BoxGroup("Effect Range Settings")]
        [LabelWidth(200)]
        [ShowIf("ShowEffectsAlliesOrEnemies")]
        public bool effectsAllies;

        [BoxGroup("Effect Range Settings")]
        [LabelWidth(200)]
        [ShowIf("ShowEffectsAlliesOrEnemies")]
        public bool effectsEnemies;

        // Knock Back Settings Properties
        #region

        [BoxGroup("Knock Back Settings", true, true)]
        [ShowIf("ShowKnockBackDistance")]
        [LabelWidth(200)]
        public int knockBackDistance;

        #endregion

        // Perks
        #region

        public bool ShowPerkPairing()
        {
            if (effectType == AbilityEffectType.ApplyPassiveSelf ||
                effectType == AbilityEffectType.ApplyPassiveTarget ||
                effectType == AbilityEffectType.ApplyPassiveAoe ||
                effectType == AbilityEffectType.ApplyPassiveInLine ||
                effectType == AbilityEffectType.RemovePassiveTarget)
            {
                return true;
            }
            return false;
        }

        #endregion

        // Summon
        #region

        public bool ShowSummonProperties()
        {
            return effectType == AbilityEffectType.SummonCharacter;
        }

        #endregion

        // Misc
        #region

        public bool ShowStressEventData()
        {
            return effectType == AbilityEffectType.StressCheck || effectType == AbilityEffectType.StressCheckAoe;
        }

        #endregion
        // General Properties
        #region

        [BoxGroup("General Settings", true, true)]
        [LabelWidth(200)]
        public AbilityEffectType effectType;

        // Stress Attack Fields
        [BoxGroup("General Settings")]
        [LabelWidth(200)]
        [ShowIf("ShowStressEventData")]
        public StressEventData stressEventData;

        // Energy Fields
        [BoxGroup("General Settings")]
        [LabelWidth(200)]
        [ShowIf("ShowEnergyGained")]
        public int energyGained;

        // Misc stuff
        [BoxGroup("General Settings")]
        [LabelWidth(200)]
        [ShowIf("ShowNormalTeleportVFX")]
        public bool normalTeleportVFX = true;

        // Movement Effect Properties
        [BoxGroup("General Settings")]
        [LabelWidth(200)]
        [ShowIf("ShowTilesMoved")]
        public int tilesMoved;

        [BoxGroup("General Settings")]
        [LabelWidth(200)]
        [ShowIf("ShowTileLineLength")]
        public int lineLength;

        // Healing Properties
        [BoxGroup("General Settings")]
        [LabelWidth(200)]
        [ShowIf("ShowHealthGained")]
        public int healthGained;

        // Self Damage Effect Properties
        [BoxGroup("General Settings")]
        [LabelWidth(200)]
        [ShowIf("ShowHealthLost")]
        public int healthLost;

        [BoxGroup("General Settings")]
        [LabelWidth(200)]
        public List<AbilityEffectRequirement> requirements = new List<AbilityEffectRequirement>();

        [BoxGroup("General Settings")]
        [LabelWidth(200)]
        [ShowIf("ShowChainedEffect")]
        public bool chainedEffect;

        [BoxGroup("General Settings")]
        [LabelWidth(200)]
        [ShowIf("ShowTriggersChainEffectSequence")]
        public bool triggersChainEffectSequence;

        #endregion

        // Visual Event Properties
        #region

        [BoxGroup("Visual Events", true, true)]
        [LabelWidth(200)]
        [Header("On Start")]
        public List<AnimationEventData> visualEventsOnStart;
        [BoxGroup("Visual Events")]
        [LabelWidth(200)]
        [Header("On Hit")]
        public List<AnimationEventData> visualEventsOnHit;
        [BoxGroup("Visual Events")]
        [LabelWidth(200)]
        [Header("On Finish")]
        public List<AnimationEventData> visualEventsOnEffectFinish;

        #endregion

        // Damage Properties
        #region

        [BoxGroup("Damage Settings", true, true)]
        [ShowIf("ShowDamageType")]
        [LabelWidth(200)]
        public DamageType damageType;

        [BoxGroup("Damage Settings")]
        [LabelWidth(200)]
        [ShowIf("ShowBaseDamage")]
        public int minBaseDamage;

        [BoxGroup("Damage Settings")]
        [LabelWidth(200)]
        [ShowIf("ShowBaseDamage")]
        public int maxBaseDamage;

        [BoxGroup("Damage Settings")]
        [LabelWidth(200)]
        [ShowIf("ShowGuaranteedHit")]
        public bool guaranteedHit;

        [BoxGroup("Damage Settings")]
        [LabelWidth(200)]
        [ShowIf("ShowBonusCritDamage")]
        [Range(0, 100)]
        public int bonusCritDamage;

        [BoxGroup("Damage Settings")]
        [LabelWidth(200)]
        [ShowIf("ShowIgnoreBlock")]
        public bool ignoresGuard;

        [BoxGroup("Damage Settings")]
        [LabelWidth(200)]
        [ShowIf("ShowIgnoreBlock")]
        public bool ignoresArmour;

        [BoxGroup("Damage Settings")]
        [LabelWidth(200)]
        [ShowIf("ShowIgnoreBlock")]
        public int bonusArmourDamage;

        [BoxGroup("Damage Settings")]
        [LabelWidth(200)]
        [ShowIf("ShowWeaponUsed")]
        public WeaponSlot weaponUsed = WeaponSlot.None;

        [BoxGroup("Damage Settings")]
        [LabelWidth(200)]
        [ShowIf("ShowGuaranteedHit")]
        public InjuryType[] injuryTypesCaused;

        [BoxGroup("Damage Settings")]
        [LabelWidth(200)]
        [ShowIf("ShowDamageEffectModifiers")]
        public List<DamageEffectModifier> damageEffectModifiers;

        #endregion

        // Summon Properties
        #region

        [BoxGroup("Summon Settings", true, true)]
        [ShowIf("ShowSummonProperties")]
        [LabelWidth(200)]
        [OdinSerialize]
        [OnValueChanged("OnCharacterSummonedChanged")]
        public EnemyTemplateSO characterSummoned;

        [Header("Summoning Visual Event Properties")]
        [BoxGroup("Summon Settings")]
        [ShowIf("ShowSummonProperties")]
        [LabelWidth(150)]
        public string characterSummonedName = "ENTER NAME!!!";

        [Header("Summoning Visual Event Properties")]
        [BoxGroup("Summon Settings")]
        [ShowIf("ShowSummonProperties")]
        [LabelWidth(150)]
        public float amountSummoned = 1;

        [Header("Summoning Visual Event Properties")]
        [BoxGroup("Summon Settings")]
        [ShowIf("ShowSummonProperties")]
        [LabelWidth(150)]
        public float modelFadeInSpeed;

        [BoxGroup("Summon Settings")]
        [ShowIf("ShowSummonProperties")]
        [LabelWidth(150)]
        public float uiFadeInSpeed;

        [BoxGroup("Summon Settings")]
        [ShowIf("ShowSummonProperties")]
        [LabelWidth(150)]
        public AnimationEventData[] summonedCreatureVisualEvents;

        #endregion

        // Getters + Accessors
        #region

        public EnemyTemplateSO CharacterSummoned
        {
            get
            {
                if (characterSummoned == null)
                {
                    characterSummoned = CharacterDataController.Instance.FindEnemyTemplateByName(characterSummonedName);
                }
                return characterSummoned;
            }
        }
        [ExecuteInEditMode]
        public void OnCharacterSummonedChanged()
        {
            Debug.Log("OnCharacterSummonedChanged");
            if (characterSummoned != null)
            {
                characterSummonedName = characterSummoned.myName;
            }
            else
            {
                characterSummonedName = "";
            }
        }

        #endregion

        // Pasive Properties
        #region

        [BoxGroup("Perk Settings", true, true)]
        [ShowIf("ShowPerkPairing")]
        [LabelWidth(200)]
        public PerkPairingData perkPairing;

        [BoxGroup("Perk Settings")]
        [ShowIf("ShowPerkPairing")]
        [LabelWidth(200)]
        [Range(0, 100)]
        public int perkApplicationChance = 100;

        #endregion

        // SHOW IFS

        // Damage  
        #region

        public bool ShowHealthLost()
        {
            return effectType == AbilityEffectType.LoseHealthSelf;
        }
        public bool ShowHealthGained()
        {
            return effectType == AbilityEffectType.HealSelf;
        }
        public bool ShowBaseDamage()
        {
            if (effectType == AbilityEffectType.DamageTarget ||
                effectType == AbilityEffectType.DamageAoe)
            {
                return true;
            }

            return false;
        }
        public bool ShowWeaponUsed()
        {
            return effectType == AbilityEffectType.DamageAoe || effectType == AbilityEffectType.DamageTarget;
        }
        public bool ShowDamageType()
        {
            return effectType == AbilityEffectType.DamageAoe || effectType == AbilityEffectType.DamageTarget;
        }
        public bool ShowGuaranteedHit()
        {
            return effectType == AbilityEffectType.DamageAoe || effectType == AbilityEffectType.DamageTarget;
        }
        public bool ShowBonusCritDamage()
        {
            return effectType == AbilityEffectType.DamageAoe || effectType == AbilityEffectType.DamageTarget;
        }
        public bool ShowIgnoreBlock()
        {
            return effectType == AbilityEffectType.DamageAoe || effectType == AbilityEffectType.DamageTarget;
        }
        public bool ShowDamageEffectModifiers()
        {
            return effectType == AbilityEffectType.DamageAoe || effectType == AbilityEffectType.DamageTarget;
        }

        #endregion

        // Aoe
        #region

        public bool ShowIncludeCentreTile()
        {
            if ((effectType == AbilityEffectType.ApplyPassiveAoe ||
                    effectType == AbilityEffectType.DamageAoe) &&
                aoeType != AoeType.ZoneOfControl)
            {
                return true;
            }
            return false;
        }
        public bool ShowAoeSize()
        {
            if ((effectType == AbilityEffectType.ApplyPassiveAoe ||
                    effectType == AbilityEffectType.DamageAoe ||
                    effectType == AbilityEffectType.StressCheckAoe) &&
                (aoeType == AoeType.AtTarget || aoeType == AoeType.Line))
            {
                return true;
            }
            return false;
        }
        public bool ShowAoeType()
        {
            if (effectType == AbilityEffectType.ApplyPassiveAoe ||
                effectType == AbilityEffectType.DamageAoe ||
                effectType == AbilityEffectType.StressCheckAoe)
            {
                return true;
            }
            return false;
        }
        public bool ShowEffectsAlliesOrEnemies()
        {
            if (effectType == AbilityEffectType.ApplyPassiveAoe ||
                effectType == AbilityEffectType.DamageAoe)
            {
                return true;
            }
            return false;
        }
        public bool ShowKnockBackDistance()
        {
            if (effectType == AbilityEffectType.KnockBack)
            {
                return true;
            }
            return false;
        }

        #endregion

        // Chain related
        #region

        public bool ShowTriggersChainEffectSequence()
        {
            if (chainedEffect == false)
            {
                return true;
            }

            return false;
        }
        public bool ShowChainedEffect()
        {
            if (triggersChainEffectSequence == false)
            {
                return true;
            }

            return false;
        }

        #endregion

        // Movement related
        #region

        public bool ShowTilesMoved()
        {
            if (effectType == AbilityEffectType.MoveInLine)
            {
                return true;
            }

            return false;
        }
        public bool ShowTileLineLength()
        {
            if (effectType == AbilityEffectType.ApplyPassiveInLine)
            {
                return true;
            }

            return false;
        }
        public bool ShowNormalTeleportVFX()
        {
            if (effectType == AbilityEffectType.TeleportSwitchWithTarget)
            {
                return true;
            }

            return false;
        }
        public bool ShowEnergyGained()
        {
            if (effectType == AbilityEffectType.GainActionPoints ||
                effectType == AbilityEffectType.GainActionPointsTarget)
            {
                return true;
            }

            return false;
        }

        #endregion
    }

    [Serializable]
    public class DamageEffectModifier
    {
        public DamageEffectModifierType type;

        [ShowIf("ShowPerk")]
        public Perk perk;

        [ShowIf("ShowTargetRace")]
        public CharacterRace targetRace;

        [Range(0, 1)]
        [ShowIf("ShowBonusDamageModifier")]
        public float bonusDamageModifier;

        [ShowIf("ShowBonusCriticalModifier")]
        [Range(0, 100)]
        public int bonusCriticalChance;

        [ShowIf("ShowBonusCriticalDamage")]
        [Range(0f, 1f)]
        public float extraCriticalDamage;

        // Odin Show Ifs
        #region

        public bool ShowBonusDamageModifier()
        {
            if (type == DamageEffectModifierType.ExtraDamageAgainstRace ||
                type == DamageEffectModifierType.ExtraDamageIfTargetHasSpecificPerk ||
                type == DamageEffectModifierType.ExtraDamageIfCasterHasSpecificPerk ||
                type == DamageEffectModifierType.AddHealthMissingOnTargetToDamage ||
                type == DamageEffectModifierType.AddHealthMissingOnSelfToDamage)
            {
                return true;
            }
            return false;
        }

        public bool ShowBonusCriticalModifier()
        {
            if (type == DamageEffectModifierType.ExtraCriticalChanceAgainstRace ||
                type == DamageEffectModifierType.ExtraCriticalChanceIfTargetHasSpecificPerk)
            {
                return true;
            }
            return false;
        }
        public bool ShowPerk()
        {
            if (type == DamageEffectModifierType.ExtraDamageIfTargetHasSpecificPerk ||
                type == DamageEffectModifierType.ExtraDamageIfCasterHasSpecificPerk ||
                type == DamageEffectModifierType.ExtraCriticalChanceIfTargetHasSpecificPerk)
            {
                return true;
            }
            return false;
        }
        public bool ShowTargetRace()
        {
            if (type == DamageEffectModifierType.ExtraCriticalChanceAgainstRace ||
                type == DamageEffectModifierType.ExtraDamageAgainstRace)
            {
                return true;
            }
            return false;
        }
        public bool ShowBonusCriticalDamage()
        {
            return type == DamageEffectModifierType.ExtraCriticalDamage;
        }

        #endregion
    }
    public enum DamageEffectModifierType
    {
        None = 0,
        AddPhysicalResistanceToDamage = 1,
        AddHealthMissingOnSelfToDamage = 2,
        AddHealthMissingOnTargetToDamage = 9,
        ExtraCriticalChanceAgainstRace = 4,
        ExtraCriticalChanceIfTargetHasSpecificPerk = 6,
        ExtraDamageIfCasterHasSpecificPerk = 8,
        ExtraDamageIfTargetHasSpecificPerk = 5,
        ExtraDamageAgainstRace = 3,
        ExtraCriticalDamage = 10
    }
}
