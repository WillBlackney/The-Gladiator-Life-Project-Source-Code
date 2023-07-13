using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeAreGladiators.Utilities;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using WeAreGladiators.Abilities;
using WeAreGladiators.Characters;
using WeAreGladiators.Libraries;
using WeAreGladiators.Items;
using WeAreGladiators.TownFeatures;
using WeAreGladiators.RewardSystems;
using System.Linq;

namespace WeAreGladiators.UI
{
    public class AbilityPopupController : Singleton<AbilityPopupController>
    {
        // Components + Properties
        #region
        [Header("Core Panel Components")]
        [SerializeField] Canvas rootCanvas;
        [SerializeField] RectTransform mainPositioningRect;
        [SerializeField] RectTransform[] transformsRebuilt;
        [SerializeField] CanvasGroup mainCg;
        [SerializeField] TextMeshProUGUI nameText;
        [SerializeField] TextMeshProUGUI energyCostText;
        [SerializeField] TextMeshProUGUI cooldownText;
        [SerializeField] TextMeshProUGUI fatigueCostText;
        [SerializeField] TextMeshProUGUI descriptionText;

        [Header("Range Section Components")]      
        [SerializeField] TextMeshProUGUI rangeText;
        [SerializeField] TextMeshProUGUI plusText;
        [SerializeField] TextMeshProUGUI auraText;
        [SerializeField] GameObject visionIcon;

        [Header("Requirement Components")]
        [SerializeField] GameObject requirementsParent;
        [SerializeField] ModalDottedRow[] requirementRows;
        [SerializeField] GameObject talentReqRowParent;
        [SerializeField] Image[] talentReqImages;
        [SerializeField] TextMeshProUGUI talentReqRowText;

        [Header("Ability Type Components")]
        [SerializeField] TextMeshProUGUI abilityTypeText;
        #endregion

        // Input
        #region
        public void OnCombatAbilityLootIconMousedOver(CombatLootIcon b)
        {
            FadeInPanel();
            BuildPanelFromAbilityData(b.AbilityReward);
            PlacePanelAboveTransform(b.transform);
            TransformUtils.RebuildLayouts(transformsRebuilt);
            PlacePanelAboveTransform(b.transform);
        }
        public void OnCombatContractAbilityIconMousedOver(CombatContractCard b)
        {
            FadeInPanel();
            BuildPanelFromAbilityData(b.MyContractData.combatRewardData.abilityAwarded);
            PlacePanelAboveTransform(b.AbilityTomeImage.transform);
            TransformUtils.RebuildLayouts(transformsRebuilt);
            PlacePanelAboveTransform(b.AbilityTomeImage.transform);
        }
        public void OnAbilityButtonMousedOver(AbilityButton b)
        {
            FadeInPanel();
            BuildPanelFromAbilityData(b.MyAbilityData);
            PlacePanelOnAbilityBarButton(b);
            TransformUtils.RebuildLayouts(transformsRebuilt);
            PlacePanelOnAbilityBarButton(b);
        }
        public void OnRosterAbilityButtonMousedOver(UIAbilityIcon b, bool above = true)
        {
            Debug.Log("AbilityPopupController.OnRosterAbilityButtonMousedOver()");
            FadeInPanel();
            BuildPanelFromAbilityData(b.MyDataRef);
            TransformUtils.RebuildLayouts(transformsRebuilt);
            if (above)
                PlacePanelAboveAbilityIcon(b);
            else PlacePanelWestOfAbilityIcon(b);
            TransformUtils.RebuildLayouts(transformsRebuilt);
            if (above)
                PlacePanelAboveAbilityIcon(b);
            else PlacePanelWestOfAbilityIcon(b);
        }
       
        public void OnAbilityBookItemMousedOver(InventoryItemView item)
        {
            FadeInPanel();
            BuildPanelFromAbilityData(item.MyItemRef.abilityData);
            PlacePanelOnInventoryItemPosition(item);
            TransformUtils.RebuildLayouts(transformsRebuilt);
            PlacePanelOnInventoryItemPosition(item);
        }
        public void OnAbilityShopTomeMousedOver(AbilityTomeShopSlot slot)
        {
            FadeInPanel();
            BuildPanelFromAbilityData(slot.MyData.ability);
            PlacePanelOnAbilityTomeShopSlotPosition(slot);
            TransformUtils.RebuildLayouts(transformsRebuilt);
            PlacePanelOnAbilityTomeShopSlotPosition(slot);
        }
        public void OnLibraryAbilityDropSlotMousedOver(LibraryAbilityDropSlot slot)
        {
            FadeInPanel();
            BuildPanelFromAbilityData(slot.MyAbilityData);
            PlacePanelOnLibraryAbilityDropSlotPosition(slot);
            TransformUtils.RebuildLayouts(transformsRebuilt);
            PlacePanelOnLibraryAbilityDropSlotPosition(slot);
        }
        
        public void OnAbilityButtonMousedExit()
        {
            HidePanel();
        }
       
        #endregion

        // Show + Hide Logic
        #region
        private void FadeInPanel()
        {
            mainCg.DOKill();
            rootCanvas.enabled = true;
            mainCg.alpha = 0.0001f;
            mainCg.DOFade(1, 0.25f);

        }
        public void HidePanel()
        {
            mainCg.DOKill();
            mainCg.alpha = 0f;
            rootCanvas.enabled = false;
        }
        private void PlacePanelOnAbilityBarButton(AbilityButton b)
        {
            float yOffSet = 85f;

            mainPositioningRect.position = b.transform.position;
            mainPositioningRect.localPosition = new Vector3(mainPositioningRect.localPosition.x, mainPositioningRect.localPosition.y + yOffSet, 0);
        }
        private void PlacePanelAboveAbilityIcon(UIAbilityIcon b)
        {
            float yOffSet = 50f;

            mainPositioningRect.position = b.transform.position;
            mainPositioningRect.localPosition = new Vector3(mainPositioningRect.localPosition.x, mainPositioningRect.localPosition.y + yOffSet, 0);
        }
        private void PlacePanelWestOfAbilityIcon(UIAbilityIcon b)
        {
            mainPositioningRect.position = b.transform.position;
            float xOffset = mainPositioningRect.rect.width / 2 + 40;
            float yOffset = mainPositioningRect.rect.height - 100;
            mainPositioningRect.localPosition = new Vector3(mainPositioningRect.localPosition.x - xOffset, mainPositioningRect.localPosition.y - yOffset, 0);
        }
        private void PlacePanelAboveTransform(Transform b)
        {
            float yOffSet = 50f;

            mainPositioningRect.position = b.transform.position;
            mainPositioningRect.localPosition = new Vector3(mainPositioningRect.localPosition.x, mainPositioningRect.localPosition.y + yOffSet, 0);
        }       
        private void PlacePanelOnInventoryItemPosition(InventoryItemView item)
        {
            mainPositioningRect.position = item.transform.position;
            float xOffset = mainPositioningRect.rect.width / 2 + 80;
            float yOffset = mainPositioningRect.rect.height - 100;
            mainPositioningRect.localPosition = new Vector3(mainPositioningRect.localPosition.x - xOffset, mainPositioningRect.localPosition.y - yOffset, 0);
        }
        private void PlacePanelOnAbilityTomeShopSlotPosition(AbilityTomeShopSlot slot)
        {
            mainPositioningRect.position = slot.transform.position;
            float xOffset = mainPositioningRect.rect.width / 2 + 80;
            mainPositioningRect.localPosition = new Vector3(mainPositioningRect.localPosition.x - xOffset, mainPositioningRect.localPosition.y, 0);
        }
        private void PlacePanelOnLibraryAbilityDropSlotPosition(LibraryAbilityDropSlot slot)
        {
            mainPositioningRect.position = slot.transform.position;
            float xOffset = mainPositioningRect.rect.width / 2 + 80;
            mainPositioningRect.localPosition = new Vector3(mainPositioningRect.localPosition.x + xOffset, mainPositioningRect.localPosition.y + (slot.GetComponent<RectTransform>().rect.height / 2) - mainPositioningRect.rect.height, 0);
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

            nameText.text = data.displayedName;
            cooldownText.text = data.baseCooldown.ToString();
            fatigueCostText.text = data.fatigueCost.ToString();

            if (data.myCharacter != null)
            {
                string col = TextLogic.white;
                int baseCost = data.fatigueCost;
                int dynamicCost = AbilityController.Instance.GetAbilityFatigueCost(data.myCharacter, data);
                if (baseCost > dynamicCost) col = TextLogic.lightGreen;
                else if (baseCost < dynamicCost) col = TextLogic.lightRed;
                fatigueCostText.text = TextLogic.ReturnColoredText(dynamicCost.ToString(), col);
            }


        }
        private void BuildDescriptionText(AbilityData data)
        {
            descriptionText.text = AbilityController.Instance.GetDynamicDescriptionFromAbility(data);
        }
        private void BuildRequirementsText(AbilityData data)
        {
            for (int i = 0; i < requirementRows.Length; i++)
                requirementRows[i].gameObject.SetActive(false);
            requirementsParent.SetActive(false);
            int modalRowCount = 0;

            if (data.talentRequirementData.talentSchool != TalentSchool.None && data.talentRequirementData.talentSchool != TalentSchool.Neutral)
            {
                DotStyle dotStyle = DotStyle.Neutral;
                if (data.myCharacter != null && CharacterDataController.Instance.DoesCharacterHaveTalent(data.myCharacter.talentPairings, data.talentRequirementData.talentSchool, 1))
                    dotStyle = DotStyle.Green;
                else if (data.myCharacter != null && !CharacterDataController.Instance.DoesCharacterHaveTalent(data.myCharacter.talentPairings, data.talentRequirementData.talentSchool, 1))
                    dotStyle = DotStyle.Red;

                requirementsParent.SetActive(true);
                requirementRows[modalRowCount].gameObject.SetActive(true);
                requirementRows[modalRowCount].Build("Requires " + data.talentRequirementData.talentSchool.ToString() + " Talent", dotStyle);

                modalRowCount++;
            }

            if (data.weaponRequirement != WeaponRequirement.None)
            {
                DotStyle dotStyle = DotStyle.Neutral;
                if (data.myCharacter != null && AbilityController.Instance.DoesCharacterMeetAbilityWeaponRequirement(data.myCharacter.itemSet, data.weaponRequirement))
                    dotStyle = DotStyle.Green;
                if (data.myCharacter != null && !AbilityController.Instance.DoesCharacterMeetAbilityWeaponRequirement(data.myCharacter.itemSet, data.weaponRequirement))
                    dotStyle = DotStyle.Red;

                requirementsParent.SetActive(true);
                requirementRows[modalRowCount].gameObject.SetActive(true);
                requirementRows[modalRowCount].Build("Requires " + TextLogic.SplitByCapitals(data.weaponRequirement.ToString()), dotStyle);
            }

        }
        private void BuildEnergyCostSection(AbilityData data)
        {
            energyCostText.text = data.energyCost.ToString();
            if(data.myCharacter != null)
            {
                string col = TextLogic.white;
                int baseCost = data.energyCost;
                int dynamicCost = AbilityController.Instance.GetAbilityActionPointCost(data.myCharacter, data);
                if (baseCost > dynamicCost) col = TextLogic.lightGreen;
                else if (baseCost < dynamicCost) col = TextLogic.lightRed;
                energyCostText.text = TextLogic.ReturnColoredText(dynamicCost.ToString(), col);
            }
        }
        private void BuildAbilityTypeComponents(AbilityData data)
        {
            abilityTypeText.text = "";
            for (int i = 0; i < data.abilityType.Length; i++)
            {
                abilityTypeText.text += TextLogic.SplitByCapitals(data.abilityType[i].ToString());
                if (i != data.abilityType.Length - 1)
                {
                    abilityTypeText.text += ", ";
                }
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
                SetTalentRequirementImages(SpriteLibrary.Instance.GetTalentSchoolSprite(data.talentRequirementData.talentSchool));
                talentReqRowText.text = TextLogic.SplitByCapitals(data.talentRequirementData.talentSchool.ToString());
            }
            else if (data.derivedFromWeapon || data.derivedFromItemLoadout)
            {
                talentReqRowParent.SetActive(true);
                SetTalentRequirementImages(SpriteLibrary.Instance.GetWeaponSprite(data.weaponClass));
                talentReqRowText.text = TextLogic.SplitByCapitals(data.weaponClass.ToString());
            }

        }
        private void SetTalentRequirementImages(Sprite s)
        {
            foreach (Image i in talentReqImages)
                i.sprite = s;
        }
        #endregion
                 
    }

    public enum PopupPositon
    {
        CharacterRoster = 0,
        DraftCharacterSheet = 1,
    }
}