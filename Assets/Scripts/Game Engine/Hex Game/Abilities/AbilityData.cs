using WeAreGladiators.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeAreGladiators.Utilities;
using WeAreGladiators.UI;

namespace WeAreGladiators.Abilities
{
    public class AbilityData 
    {
        public HexCharacterModel myCharacter;
        public string abilityName;
        public string displayedName;
        public string baseAbilityDescription;
        public WeaponAbilityType weaponAbilityType;

        public AbilityType[] abilityType;
        public bool doesNotBreakStealth;
        public GuidanceInstruction guidanceInstruction;
        public GuidanceInstruction guidanceInstructionTwo;
        public TargetRequirement targetRequirement;
        public WeaponRequirement weaponRequirement;
        public bool derivedFromWeapon = false;
        public bool derivedFromItemLoadout = false;
        public WeaponClass weaponClass;
        public TalentPairing talentRequirementData;
        public int talentLevelRequirement = 0;
        public List<AbilityRequirement> abilitySubRequirements;
        public SecondaryTargetRequirement secondaryTargetRequirement;
        public int rangeFromTarget;

        public int energyCost;
        public int baseCooldown;
        public int currentCooldown = 0;       

        public int baseRange = 0;
        public bool gainRangeBonusFromVision = false;
        public int hitChanceModifier;
        public bool accuracyPenaltyFromMelee = false;
        public int hitChanceModifierAgainstAdjacent;

        public List<AbilityEffect> abilityEffects;
        public List<AbilityEffect> onHitEffects;
        public List<AbilityEffect> onCritEffects;
        public List<AbilityEffect> onPerkAppliedSuccessEffects;
        public List<AbilityEffect> onCollisionEffects;
        public List<CustomString> dynamicDescription;
        public List<KeyWordModel> keyWords;

        public int chainLoops;
        public List<AbilityEffect> chainedEffects;

        private Sprite abilitySprite;
        public Sprite AbilitySprite
        {
            get
            {
                if (abilitySprite == null)
                {
                    abilitySprite = GetMySprite();
                    return abilitySprite;
                }
                else
                {
                    return abilitySprite;
                }
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
                Debug.LogWarning("ItemData.GetMySprite() could not sprite for item " + abilityName + ", returning null...");


            return s;
        }
    }

    public enum WeaponAbilityType
    {
        None = 0,
        Basic = 1,
        Special = 2,
    }
}