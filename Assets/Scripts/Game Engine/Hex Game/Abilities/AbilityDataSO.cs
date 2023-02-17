using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using HexGameEngine.Utilities;
using HexGameEngine.UI;
using HexGameEngine.Characters;
using System.Linq;

namespace HexGameEngine.Abilities
{
    [CreateAssetMenu(fileName = "New Ability Data", menuName = "Ability Data", order = 52)]
    public class AbilityDataSO : ScriptableObject
    {
        [BoxGroup("General Info", true, true)]
        [GUIColor("Blue")]
        [LabelWidth(100)]
        public string abilityName;

        [BoxGroup("General Info")]
        [GUIColor("Blue")]
        [LabelWidth(100)]
        public string displayedName;

        [GUIColor("Blue")]
        [BoxGroup("General Info")]
        [LabelWidth(100)]
        public bool includeInGame = true;
      

        [GUIColor("Blue")]
        [BoxGroup("General Info")]
        [LabelWidth(100)]
        [TextArea]
        public string baseAbilityDescription;

        [HorizontalGroup("Core Data", 75)]
        [HideLabel]
        [PreviewField(75)]
        [GUIColor("Blue")]
        public Sprite abilitySprite;

        [VerticalGroup("Core Data/Stats")]
        [LabelWidth(100)]
        [GUIColor("Blue")]
        [Header("Costs & Type")]
        public int energyCost;
        [VerticalGroup("Core Data/Stats")]
        [LabelWidth(100)]
        [GUIColor("Blue")]
        public int fatigueCost;
        [VerticalGroup("Core Data/Stats")]
        [LabelWidth(100)]
        [GUIColor("Blue")]
        public int baseCooldown;
        [VerticalGroup("Core Data/Stats")]
        [LabelWidth(100)]
        [GUIColor("Blue")]
        public AbilityType[] abilityType;
        [Header("Misc Attributes")]
        [VerticalGroup("Core Data/Stats")]
        [LabelWidth(150)]
        [GUIColor("Blue")]
        public bool doesNotBreakStealth = false;

        [VerticalGroup("Requirements")]
        [LabelWidth(150)]
        [GUIColor("Blue")]
        [Header("Weapon Properties")]
        public WeaponRequirement weaponRequirement;
        [VerticalGroup("Requirements")]
        [LabelWidth(150)]
        [GUIColor("Blue")]
        public bool derivedFromWeapon;
        [VerticalGroup("Requirements")]
        [LabelWidth(150)]
        [GUIColor("Blue")]
        public bool derivedFromItemLoadout;
        [VerticalGroup("Requirements")]
        [LabelWidth(150)]
        [GUIColor("Blue")]
        [ShowIf("ShowWeaponAbilityType")]
        public WeaponAbilityType weaponAbilityType;
        [VerticalGroup("Requirements")]
        [LabelWidth(150)]
        [GUIColor("Blue")]
        [ShowIf("ShowWeaponAbilityType")]
        public WeaponClass weaponClass;
        [VerticalGroup("Requirements")]
        [LabelWidth(150)]
        [GUIColor("Blue")]
        [Header("Requirements")]
        public TargetRequirement targetRequirement;
        [VerticalGroup("Requirements")]
        [LabelWidth(150)]
        [GUIColor("Blue")]
        public SecondaryTargetRequirement secondaryTargetRequirement;
        [VerticalGroup("Requirements")]
        [LabelWidth(150)]
        [GUIColor("Blue")]
        [ShowIf("ShowRangeFromTarget")]
        public int rangeFromTarget;
        [VerticalGroup("Requirements")]
        [LabelWidth(150)]
        [GUIColor("Blue")]
        public TalentPairing talentRequirementData;
        [VerticalGroup("Requirements")]
        [LabelWidth(100)]
        [GUIColor("Blue")]
        public List<AbilityRequirement> abilitySubRequirements;

        [BoxGroup("Range Settings", true, true)]
        [LabelWidth(200)]
        [GUIColor("Yellow")]
        [ShowIf("ShowBaseRangeBonus")]
        public int baseRange = 0;

        [BoxGroup("Range Settings")]
        [LabelWidth(200)]
        [GUIColor("Yellow")]
        [ShowIf("ShowGainRangeBonusFromVision")]
        public bool gainRangeBonusFromVision = false;

        [BoxGroup("Range Settings")]
        [LabelWidth(200)]
        [GUIColor("Yellow")]
        [ShowIf("ShowHitChanceModifier")]
        [Range(-50, 50)]
        public int hitChanceModifier;

        [BoxGroup("Range Settings")]
        [LabelWidth(200)]
        [GUIColor("Yellow")]
        [ShowIf("ShowHitChanceModifier")]
        [Range(-50, 50)]
        public int hitChanceModifierAgainstAdjacent;

        [BoxGroup("Range Settings")]
        [LabelWidth(200)]
        [GUIColor("Yellow")]
        [ShowIf("ShowAccuracyPenaltyFromMelee")]
        public bool accuracyPenaltyFromMelee = false;

        [VerticalGroup("Ability Effects")]
        [LabelWidth(200)]
        public List<AbilityEffect> abilityEffects;
        [VerticalGroup("Ability Effects")]
        [LabelWidth(200)]
        public List<AbilityEffect> onHitEffects;
        [VerticalGroup("Ability Effects")]
        [LabelWidth(200)]
        public List<AbilityEffect> onCritEffects;
        [VerticalGroup("Ability Effects")]
        [LabelWidth(200)]
        public List<AbilityEffect> onPerkAppliedSuccessEffects;
        [VerticalGroup("Ability Effects")]
        [LabelWidth(200)]
        public List<AbilityEffect> onCollisionEffects;

        [BoxGroup("Chain Effect Settings", true, true)]
        [LabelWidth(200)]
        public int chainLoops;
        [BoxGroup("Chain Effect Settings")]
        [LabelWidth(200)]
        public List<AbilityEffect> chainedEffects;

        [VerticalGroup("List Groups")]
        [LabelWidth(200)]
        public List<CustomString> dynamicDescription;
        [VerticalGroup("List Groups")]
        [LabelWidth(200)]
        public List<KeyWordModel> keyWords;


        // GUI Colours
        #region
        private Color Blue() { return Color.cyan; }
        private Color Green() { return Color.green; }
        private Color Yellow() { return Color.yellow; }
        #endregion

        // Odin Showifs
        #region
        public bool ShowSpecialWeaponAbility()
        {
            if (weaponRequirement != WeaponRequirement.None)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool ShowWeaponAbilityType()
        {
            if (derivedFromWeapon)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
       
        public bool ShowGainRangeBonusFromWeapon()
        {
            return targetRequirement != TargetRequirement.NoTarget;
        }
        public bool ShowBaseRangeBonus()
        {
            return targetRequirement != TargetRequirement.NoTarget;
        }
        public bool ShowGainRangeBonusFromVision()
        {
            return targetRequirement != TargetRequirement.NoTarget;
        }
        public bool ShowAccuracyPenaltyFromMelee()
        {
            return abilityType.Contains(AbilityType.RangedAttack);
        }
        public bool ShowRangeFromTarget()
        {
            return secondaryTargetRequirement == SecondaryTargetRequirement.UnoccupiedHexWithinRangeOfTarget;
        }
        public bool ShowHitChanceModifier()
        {
            return abilityType.Contains(AbilityType.MeleeAttack) || abilityType.Contains(AbilityType.RangedAttack) || abilityType.Contains(AbilityType.WeaponAttack);
        }

        #endregion

    }

    [System.Serializable]
    public class AbilityRequirement
    {
        public AbilityRequirementType type;

        [ShowIf("ShowHealthRequired")]
        public int healthRequired;

        [ShowIf("ShowRace")]
        public CharacterRace race;

        [ShowIf("ShowPerk")]
        public Perk perk;

        public bool ShowHealthRequired()
        {
            return type == AbilityRequirementType.CasterHasEnoughHealth;
        }
        public bool ShowRace()
        {
            return type == AbilityRequirementType.TargetHasRace;
        }
        public bool ShowPerk()
        {
            return type == AbilityRequirementType.TargetHasPerk || type == AbilityRequirementType.TargetDoesNotHavePerk || type == AbilityRequirementType.CasterHasPerk || type == AbilityRequirementType.CasterDoesNotHavePerk;
        }
    }
}