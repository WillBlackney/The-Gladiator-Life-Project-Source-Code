using System.Collections.Generic;
using UnityEngine;
using WeAreGladiators.Characters;
using WeAreGladiators.UI;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.Abilities
{
    public class AbilityData
    {

        public List<AbilityEffect> abilityEffects;
        public string abilityName;

        private Sprite abilitySprite;
        public List<AbilityRequirement> abilitySubRequirements;

        public AbilityType[] abilityType;
        public bool accuracyPenaltyFromMelee = false;
        public string baseAbilityDescription;
        public int baseCooldown;

        public int baseRange = 0;
        public List<AbilityEffect> chainedEffects;

        public int chainLoops;
        public int currentCooldown = 0;
        public bool derivedFromItemLoadout = false;
        public bool derivedFromWeapon = false;
        public string displayedName;
        public bool doesNotBreakStealth;
        public List<CustomString> dynamicDescription;

        public int energyCost;
        public bool gainRangeBonusFromVision = false;
        public GuidanceInstruction guidanceInstruction;
        public GuidanceInstruction guidanceInstructionTwo;
        public int hitChanceModifier;
        public int hitChanceModifierAgainstAdjacent;
        public List<KeyWordModel> keyWords;
        public HexCharacterModel myCharacter;
        public List<AbilityEffect> onCollisionEffects;
        public List<AbilityEffect> onCritEffects;
        public List<AbilityEffect> onHitEffects;
        public List<AbilityEffect> onPerkAppliedSuccessEffects;
        public int rangeFromTarget;
        public SecondaryTargetRequirement secondaryTargetRequirement;
        public int talentLevelRequirement = 0;
        public TalentPairing talentRequirementData;
        public TargetRequirement targetRequirement;
        public WeaponAbilityType weaponAbilityType;
        public WeaponClass weaponClass;
        public WeaponRequirement weaponRequirement;
        public Sprite AbilitySprite
        {
            get
            {
                if (abilitySprite == null)
                {
                    abilitySprite = GetMySprite();
                    return abilitySprite;
                }
                return abilitySprite;
            }
        }
        private Sprite GetMySprite()
        {
            Sprite s = null;

            foreach (AbilityDataSO i in AbilityController.Instance.AllAbilityDataSOs)
            {
                if (i.abilityName == abilityName)
                {
                    s = i.abilitySprite;
                    break;
                }
            }

            if (s == null)
            {
                Debug.LogWarning("ItemData.GetMySprite() could not sprite for item " + abilityName + ", returning null...");
            }

            return s;
        }
    }

    public enum WeaponAbilityType
    {
        None = 0,
        Basic = 1,
        Special = 2
    }
}
