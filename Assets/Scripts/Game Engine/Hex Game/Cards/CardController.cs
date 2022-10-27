using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexGameEngine.Utilities;
using HexGameEngine.RewardSystems;
using DG.Tweening;
using System;
using HexGameEngine.Abilities;
using HexGameEngine.Libraries;
using UnityEngine.UI;
using HexGameEngine.Perks;
using HexGameEngine.Characters;
using HexGameEngine.Items;

namespace HexGameEngine.Cards
{
    public class CardController : Singleton<CardController>
    {

        // Build Cards + View Models
        #region
        public void BuildCardViewModelFromRewardData(SingleRewardContainer reward, CardViewModel cardVM)
        {
            Debug.Log("CardController.BuildCardViewModelFromRewardData() called...");

            if (reward.rewardType == RewardType.Ability)
                BuildCardViewModelFromAbilityData(reward.abilityOffered, cardVM);
            else if (reward.rewardType == RewardType.Perk)
                BuildCardViewModelFromPerkTag(reward.perkOffered, cardVM);
            else if (reward.rewardType == RewardType.Talent)
                BuildCardViewModelFromTalentData(reward.talentOffered, cardVM);
        }
        private void BuildCardViewModelFromAbilityData(AbilityData ability, CardViewModel cardVM)
        {
            // disable all components first

            // build
            SetCardViewModelNameText(cardVM, ability.abilityName);
            AutoUpdateCardViewModelDescriptionTextFromAbilityData(cardVM, ability);
            SetCardViewModelGraphicImage(cardVM, ability.AbilitySprite);

            // type
            SetAbilityTypeIconVisbility(cardVM, true);
           // SetCardViewModelAbilityTypeIconImage(cardVM, SpriteLibrary.Instance.GetAbilityTypeSprite(ability.abilityType));

            // energy cost
            SetEnergyIconVisbility(cardVM, true);
            SetCardViewModelEnergyIconText(cardVM, ability.energyCost.ToString());

            // talent req
            SetTalentIconVisbility(cardVM, true);
            SetCardViewModelTalentIconImage(cardVM, CharacterDataController.Instance.GetTalentSprite(ability.talentRequirementData.talentSchool));
            ApplyCardViewModelTalentColoring(cardVM, ColorLibrary.Instance.GetTalentColor(ability.talentRequirementData.talentSchool));
            ApplyCardViewModelRarityColoring(cardVM, Color.white);

        }
        public void BuildCardViewModelFromPerkTag(Perk perk, CardViewModel cardVM)
        {
            PerkIconData perkData = PerkController.Instance.GetPerkIconDataByTag(perk);

            // Reset
            SetAbilityTypeIconVisbility(cardVM, false);
            SetEnergyIconVisbility(cardVM, false);
            SetTalentIconVisbility(cardVM, false);
            ApplyCardViewModelTalentColoring(cardVM, ColorLibrary.Instance.GetTalentColor(TalentSchool.None));
            ApplyCardViewModelRarityColoring(cardVM, Color.white);

            // build
            SetCardViewModelNameText(cardVM, perkData.passiveName);
            AutoUpdateCardViewModelDescriptionTextFromPerkData(cardVM, perkData);
            SetCardViewModelGraphicImage(cardVM, perkData.passiveSprite);
        }
        public void BuildCardViewModelFromTalentData(TalentSchool talent, CardViewModel cardVM)
        {
            TalentDataSO talentData = CharacterDataController.Instance.GetTalentDataFromTalentEnum(talent);

            // Reset
            SetAbilityTypeIconVisbility(cardVM, false);
            SetEnergyIconVisbility(cardVM, false);
            SetTalentIconVisbility(cardVM, false);
            ApplyCardViewModelTalentColoring(cardVM, ColorLibrary.Instance.GetTalentColor(talent));
            ApplyCardViewModelRarityColoring(cardVM, Color.white);

            // build
            SetCardViewModelNameText(cardVM, talent.ToString());
            AutoUpdateCardViewModelDescriptionTextFromTalentData(cardVM, talentData);
            SetCardViewModelGraphicImage(cardVM, talentData.talentSprite);
            //ApplyCardViewModelTalentColoring(cardVM, ColorLibrary.Instance.GetTalentColor(talent));
        }
        #endregion

        // Build Card View Model Specific Components
        #region
        private void SetCardViewModelNameText(CardViewModel cvm, string name)
        {
            cvm.nameText.text = name;
            cvm.nameText.color = Color.white;
            if (cvm.myPreviewCard != null)
            {
                Debug.Log("SETTING CARD VIEW MODEL PREVIEW NAME!!");
                SetCardViewModelNameText(cvm.myPreviewCard, name);
            }
        }
        private void AutoUpdateCardViewModelDescriptionTextFromAbilityData(CardViewModel cvm, AbilityData data)
        {
            string description = AbilityController.Instance.GetDynamicDescriptionFromAbility(data);
            cvm.descriptionText.text = description;

            // Apply change to 
            if (cvm.myPreviewCard != null)
                cvm.myPreviewCard.descriptionText.text = description;
        }
        private void AutoUpdateCardViewModelDescriptionTextFromPerkData(CardViewModel cvm, PerkIconData data)
        {
            string description = TextLogic.ConvertCustomStringListToString(data.passiveDescription);
            cvm.descriptionText.text = description;

            // Apply change to 
            if (cvm.myPreviewCard != null)
                cvm.myPreviewCard.descriptionText.text = description;
        }
        private void AutoUpdateCardViewModelDescriptionTextFromTalentData(CardViewModel cvm, TalentDataSO data)
        {
            string description = TextLogic.ConvertCustomStringListToString(data.talentDescription);
            cvm.descriptionText.text = description;

            // Apply change to 
            if (cvm.myPreviewCard != null)
                cvm.myPreviewCard.descriptionText.text = description;
        }
        private void SetCardViewModelGraphicImage(CardViewModel cvm, Sprite sprite)
        {
            cvm.graphicImage.sprite = sprite;
            if (cvm.myPreviewCard != null)
            {
                SetCardViewModelGraphicImage(cvm.myPreviewCard, sprite);
            }
        }
        private void SetCardViewModelEnergyIconText(CardViewModel cvm, string value)
        {
            cvm.energyText.text = value;
            if (cvm.myPreviewCard != null) SetCardViewModelEnergyIconText(cvm.myPreviewCard, value);
        }
        private void SetCardViewModelCooldownIconText(CardViewModel cvm, string value)
        {
            cvm.energyText.text = value;
            if (cvm.myPreviewCard != null) SetCardViewModelCooldownIconText(cvm.myPreviewCard, value);
        }
        private void SetCardViewModelTalentIconImage(CardViewModel cvm, Sprite sprite)
        {
            cvm.talentIconImage.sprite = sprite;
            if (cvm.myPreviewCard != null) SetCardViewModelTalentIconImage(cvm.myPreviewCard, sprite);
        }
        private void SetCardViewModelAbilityTypeIconImage(CardViewModel cvm, Sprite sprite)
        {
            cvm.abilityTypeImage.sprite = sprite;
            if (cvm.myPreviewCard != null) SetCardViewModelAbilityTypeIconImage(cvm.myPreviewCard, sprite);
        }
        #endregion

        // Show + Hide Card Icon Components
        #region
        private void SetEnergyIconVisbility(CardViewModel cvm, bool onOrOff)
        {
            cvm.energyIconVisualParent.SetActive(onOrOff);
            if (cvm.myPreviewCard != null) SetEnergyIconVisbility(cvm.myPreviewCard, onOrOff);
        }
        private void SetCooldownIconVisbility(CardViewModel cvm, bool onOrOff)
        {
            cvm.cooldownIconVisualParent.SetActive(onOrOff);
            if (cvm.myPreviewCard != null) SetCooldownIconVisbility(cvm.myPreviewCard, onOrOff);
        }
        private void SetTalentIconVisbility(CardViewModel cvm, bool onOrOff)
        {
            cvm.talentIconVisualParent.SetActive(onOrOff);
            if (cvm.myPreviewCard != null) SetTalentIconVisbility(cvm.myPreviewCard, onOrOff);
        }
        private void SetAbilityTypeIconVisbility(CardViewModel cvm, bool onOrOff)
        {
            cvm.abilityTypeIconVisualParent.SetActive(onOrOff);
            if (cvm.myPreviewCard != null) SetAbilityTypeIconVisbility(cvm.myPreviewCard, onOrOff);
        }
        #endregion

        // Card View Model Colouring
        #region
        private void ApplyCardViewModelTalentColoring(CardViewModel cvm, Color color)
        {
            foreach (Image sr in cvm.talentRenderers)
            {
                sr.color = color;
                /*

                if (cvm.card != null && cvm.card.racialCard)
                {
                    sr.color = ColorLibrary.Instance.racialColor;
                }
                else if (cvm.card != null && cvm.card.affliction)
                {
                    sr.color = ColorLibrary.Instance.afflictionColor;
                }
                else
                {
                    sr.color = color;
                }*/

            }
            if (cvm.myPreviewCard != null)
            {
                ApplyCardViewModelTalentColoring(cvm.myPreviewCard, color);
            }
        }
        private void ApplyCardViewModelRarityColoring(CardViewModel cvm, Color color)
        {
            foreach (Image sr in cvm.rarityRenderers)
            {
                sr.color = color;
            }
            if (cvm.myPreviewCard != null)
            {
                ApplyCardViewModelRarityColoring(cvm.myPreviewCard, color);
            }
        }
        #endregion

        // Card Movement
        #region
        public void MoveCardVMToPlayPreviewSpot(CardViewModel cvm, HandVisual hv)
        {
            MoveTransformToLocation(cvm.movementParent, hv.PlayPreviewSpot.position, 0.25f);
        }
        public void MoveTransformToLocation(Transform t, Vector3 location, float speed, bool localMove = false, Action onCompleteCallBack = null)
        {
            Sequence cardSequence = DOTween.Sequence();

            if (localMove)
            {
                cardSequence.Append(t.DOLocalMove(location, speed));
            }
            else
            {
                cardSequence.Append(t.DOMove(location, speed));
            }

            cardSequence.OnComplete(() =>
            {
                if (onCompleteCallBack != null)
                {
                    onCompleteCallBack.Invoke();
                }
            });
        }
        #endregion
    }
}