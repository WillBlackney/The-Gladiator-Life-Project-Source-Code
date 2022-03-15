using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexGameEngine.Utilities;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using HexGameEngine.Abilities;
using HexGameEngine.Characters;
using HexGameEngine.Libraries;
using HexGameEngine.Items;
using HexGameEngine.TownFeatures;

namespace HexGameEngine.UI
{
    public class AbilityPopupController : Singleton<AbilityPopupController>
    {
        // Components + Properties
        #region
        [Header("Core Panel Components")]
        [SerializeField] GameObject visualParent;
        [SerializeField] RectTransform mainPositioningRect;
        [SerializeField] RectTransform characterRosterPosition;
        [SerializeField] RectTransform draftCharacterPosition;
        [SerializeField] RectTransform[] transformsRebuilt;
        [SerializeField] CanvasGroup mainCg;
        [SerializeField] TextMeshProUGUI nameText;
        [SerializeField] TextMeshProUGUI energyCostText;
        [SerializeField] TextMeshProUGUI cooldownText;
        [SerializeField] TextMeshProUGUI descriptionText;

        [Header("Range Section Components")]      
        [SerializeField] TextMeshProUGUI rangeText;
        [SerializeField] TextMeshProUGUI plusText;
        [SerializeField] TextMeshProUGUI auraText;
        [SerializeField] GameObject visionIcon;

        [Header("Requirement Components")]
        [SerializeField] GameObject requirementsParent;
        [SerializeField] TextMeshProUGUI requirementsText;
        [SerializeField] GameObject talentReqRowParent;
        [SerializeField] Image[] talentReqImages;
        [SerializeField] TextMeshProUGUI talentReqRowText;

        [Header("Ability Type Components")]
        [SerializeField] Image abilityTypeImage;
        [SerializeField] TextMeshProUGUI abilityTypeText;
        [SerializeField] Sprite meleeAttackIcon;
        [SerializeField] Sprite rangedAttackIcon;
        [SerializeField] Sprite skillIcon;
        #endregion

        // Input
        #region
        public void OnCombatContractAbilityIconMousedOver(CombatContractCard b)
        {
            FadeInPanel();
            BuildPanelFromAbilityData(b.MyContractData.combatRewardData.abilityAwarded);
            PlacePanelOnCombatContractAbilityIcon(b.AbilityTomeImage.transform);
            ForceRebuildLayouts();
        }
        public void OnAbilityButtonMousedOver(AbilityButton b)
        {
            FadeInPanel();
            BuildPanelFromAbilityData(b.myAbilityData);
            PlacePanelOnAbilityBarButton(b);
            ForceRebuildLayouts();
        }
        public void OnRosterAbilityButtonMousedOver(UIAbilityIcon b)
        {
            FadeInPanel();
            BuildPanelFromAbilityData(b.MyDataRef);
            if (b.PopupPosition == PopupPositon.CharacterRoster) PlacePanelOnRosterAbilityButtonPosition();
            else if (b.PopupPosition == PopupPositon.DraftCharacterSheet) PlacePanelOnDraftScreenAbilityButtonPosition();
            ForceRebuildLayouts();
        }
        public void OnAbilityBookItemMousedOver(InventoryItemView item)
        {
            FadeInPanel();
            BuildPanelFromAbilityData(item.MyItemRef.abilityData);
            PlacePanelOnInventoryItemPosition(item);
            ForceRebuildLayouts();
        }
        public void OnAbilityButtonMousedExit(AbilityButton b = null)
        {
            HidePanel();
        }
       
        #endregion

        // Show + Hide Logic
        #region
        private void FadeInPanel()
        {
            visualParent.transform.DOKill();
            mainCg.DOKill();
            visualParent.SetActive(true);
            mainCg.alpha = 0.0001f;
            mainCg.DOFade(1, 0.25f);

        }
        public void HidePanel()
        {
            visualParent.transform.DOKill();
            mainCg.DOKill();
            mainCg.alpha = 0f;
            visualParent.SetActive(false);
        }
        private void PlacePanelOnAbilityBarButton(AbilityButton b)
        {
            float yOffSet = 75f;

            mainPositioningRect.position = b.transform.position;
            mainPositioningRect.localPosition = new Vector3(mainPositioningRect.localPosition.x, mainPositioningRect.localPosition.y + yOffSet, 0);
        }
        private void PlacePanelOnCombatContractAbilityIcon(Transform b)
        {
            float yOffSet = 50f;

            mainPositioningRect.position = b.transform.position;
            mainPositioningRect.localPosition = new Vector3(mainPositioningRect.localPosition.x, mainPositioningRect.localPosition.y + yOffSet, 0);
        }
        private void PlacePanelOnRosterAbilityButtonPosition()
        {
            mainPositioningRect.localPosition = characterRosterPosition.transform.localPosition;
        }
        private void PlacePanelOnDraftScreenAbilityButtonPosition()
        {
            mainPositioningRect.localPosition = draftCharacterPosition.transform.localPosition;
        }
        private void PlacePanelOnInventoryItemPosition(InventoryItemView item)
        {
            mainPositioningRect.position = item.transform.position;
            float xOffset = mainPositioningRect.rect.width / 2 + 80;
            float yOffset = mainPositioningRect.rect.height - 100;
            mainPositioningRect.localPosition = new Vector3(mainPositioningRect.localPosition.x - xOffset, mainPositioningRect.localPosition.y - yOffset, 0);
        }
        #endregion

        // Setup + Build Individual Components
        #region
        private void BuildPanelFromAbilityData(AbilityData data)
        {
            BuildDescriptionText(data);
            BuildRequirementsText(data);
            BuildEnergyCostSection(data);
            BuildAbilityTypeComponents(data);
            BuildAbilityRangeComponents(data);
            BuildTalentRequirementSection(data);

            nameText.text = data.abilityName;
            cooldownText.text = data.baseCooldown.ToString();


        }
        private void BuildDescriptionText(AbilityData data)
        {
            descriptionText.text = AbilityController.Instance.GetDynamicDescriptionFromAbility(data);
        }
        private void BuildRequirementsText(AbilityData data)
        {
            requirementsParent.SetActive(false);
            string message = "";

            if (data.talentRequirementData.talentSchool != TalentSchool.None && data.talentRequirementData.talentSchool != TalentSchool.Neutral)
            {
                requirementsParent.SetActive(true);
                message += "- Requires " + data.talentRequirementData.talentSchool.ToString() + " " + data.talentRequirementData.level.ToString();
            }

            if (data.weaponRequirement != WeaponRequirement.None)
            {
                requirementsParent.SetActive(true);
                message += "\n" + "- Requires " + TextLogic.SplitByCapitals(data.weaponRequirement.ToString());
            }

            requirementsText.text = message;
        }
        private void BuildEnergyCostSection(AbilityData data)
        {
            energyCostText.text = data.energyCost.ToString();
        }
        private void BuildAbilityTypeComponents(AbilityData data)
        {
            if (data.abilityType == AbilityType.MeleeAttack)
            {
                abilityTypeImage.sprite = meleeAttackIcon;
                abilityTypeText.text = "Melee Attack";
            }

            else if (data.abilityType == AbilityType.RangedAttack)
            {
                abilityTypeImage.sprite = rangedAttackIcon;
                abilityTypeText.text = "Ranged Attack";
            }
            else if (data.abilityType == AbilityType.Skill)
            {
                abilityTypeImage.sprite = skillIcon;
                abilityTypeText.text = "Skill";
            }
        }
        private void BuildAbilityRangeComponents(AbilityData data)
        {
            // Start by turning everything off
            rangeText.gameObject.SetActive(false);
            plusText.gameObject.SetActive(false);
            auraText.gameObject.SetActive(false);
            visionIcon.SetActive(false);

          
            if (data.abilityEffects[0] != null && data.abilityEffects[0].aoeType == AoeType.Aura)
            {
                auraText.gameObject.SetActive(true);
                auraText.text = "Aura";
            }
            else if (data.targetRequirement == TargetRequirement.NoTarget)
            {
                auraText.gameObject.SetActive(true);
                auraText.text = "Self";
            }
            else if (data.baseRange > 0)
            {
                rangeText.gameObject.SetActive(true);
                rangeText.text = data.baseRange.ToString();
            }            
        }
        private void BuildTalentRequirementSection(AbilityData data)
        {
            talentReqRowParent.SetActive(false);
            if (data.talentRequirementData.talentSchool != TalentSchool.None && data.talentRequirementData.talentSchool != TalentSchool.Neutral)
            {
                talentReqRowParent.SetActive(true);
                SetTalentRequirementImages(SpriteLibrary.Instance.GetTalentSchoolSpriteFromEnumData(data.talentRequirementData.talentSchool));
                talentReqRowText.text = data.talentRequirementData.talentSchool.ToString();
            }
            else if (data.derivedFromWeapon)
            {
                talentReqRowParent.SetActive(true);
                SetTalentRequirementImages(SpriteLibrary.Instance.GetWeaponSpriteFromEnumData(data.weaponClass));
                talentReqRowText.text = data.weaponClass.ToString();
            }
        }
        private void SetTalentRequirementImages(Sprite s)
        {
            foreach (Image i in talentReqImages)
                i.sprite = s;
        }
        #endregion

        // Misc
        #region
        private void ForceRebuildLayouts()
        {
            for(int i = 0; i < 2; i++)
            foreach (RectTransform t in transformsRebuilt)
                LayoutRebuilder.ForceRebuildLayoutImmediate(t);
        }
        #endregion

         
    }

    public enum PopupPositon
    {
        CharacterRoster = 0,
        DraftCharacterSheet = 1,
    }
}