﻿using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WeAreGladiators.Characters;
using WeAreGladiators.GameIntroEvent;
using WeAreGladiators.Items;
using WeAreGladiators.Libraries;
using WeAreGladiators.StoryEvents;
using WeAreGladiators.TownFeatures;
using WeAreGladiators.UCM;

namespace WeAreGladiators.UI
{
    public class RosterCharacterPanel : MonoBehaviour, IPointerClickHandler
    {
        #region Components

        [Header("Core Components")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private UniversalCharacterModel portaitModel;
        [Space(10)]

        [Header("Health Bar Components")]
        [SerializeField] private Slider healthBar;
        [SerializeField] private TextMeshProUGUI healthText;
        [Space(10)]

        [Header("Stress Bar Components")]
        [SerializeField] private Image moralStateImage;
        [Space(10)]

        [Header("Perk + Injury Components")]
        [SerializeField] private UIPerkIcon[] perkIcons;
        [Space(10)]

        [Header("Level Components")]
        [SerializeField] private LevelUpButton levelUpIndicator;
        [SerializeField] private TextMeshProUGUI currentLevelText;

        #endregion

        #region Getters + Accessors

        public HexCharacterData MyCharacterData { get; private set; }

        #endregion

        #region Logic
        public void OnMoraleIconMouseEnter()
        {
            if (AllowInput())
            {
                MainModalController.Instance.BuildAndShowModal(MyCharacterData.currentMoraleState);
            }           
        }
        public void OnMoraleIconMouseExit()
        {
            if (AllowInput())
            {
                MainModalController.Instance.HideModal();
            }
        }
        public void BuildFromCharacterData(HexCharacterData data)
        {
            MyCharacterData = data;

            // Texts
            nameText.text = "<color=#BC8252>" + data.myName + "<color=#DDC6AB>    " + data.mySubName;
            healthText.text = data.currentHealth.ToString();
            moralStateImage.sprite = SpriteLibrary.Instance.GetMoraleStateSprite(data.currentMoraleState);

            // Build model
            CharacterModeller.BuildModelFromStringReferencesAsMugshot(portaitModel, data.modelParts);
            CharacterModeller.ApplyItemSetToCharacterModelView(data.itemSet, portaitModel);
            portaitModel.SetBaseAnim();

            // Build bars
            healthBar.value = data.currentHealth / (float)StatCalculator.GetTotalMaxHealth(data);

            // Reset perk icons
            foreach (UIPerkIcon b in perkIcons)
            {
                b.HideAndReset();
            }

            // Injury perk icons
            for (int i = 0; i < data.passiveManager.perks.Count && i < perkIcons.Length; i++)
            {
                if (data.passiveManager.perks[i].Data.isInjury ||
                    data.passiveManager.perks[i].Data.isPermanentInjury)
                {
                    perkIcons[i].BuildFromActivePerk(data.passiveManager.perks[i]);
                }
            }

            // Level stuff
            UpdateLevelUpIndicator();
            currentLevelText.text = data.currentLevel.ToString();
        }
        private void UpdateLevelUpIndicator()
        {
            if (MyCharacterData == null)
            {
                return;
            }
            levelUpIndicator.Hide();
            if (MyCharacterData.attributeRolls.Count > 0 ||
                MyCharacterData.talentPoints > 0 ||
                MyCharacterData.perkPoints > 0)
            {
                Debug.Log("Showing indicator!");
                levelUpIndicator.ShowAndAnimate();
            }
        }
        public void Show()
        {
            gameObject.SetActive(true);
        }
        public void ResetAndHide()
        {
            MyCharacterData = null;
            gameObject.SetActive(false);
        }
        public void OnClickAndDragStart()
        {
            if (TownController.Instance.DeploymentViewIsActive &&
                AllowInput())
            {
                PortraitDragController.Instance.OnRosterCharacterPanelDragStart(this);
            }

        }
        public void OnLevelButtonClicked()
        {
            if (!InventoryController.Instance.VisualParent.activeSelf &&
                AllowInput())
            {
                CharacterRosterViewController.Instance.HandleBuildAndShowCharacterRoster(MyCharacterData);
            }
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right &&
                AllowInput())
            {
                CharacterRosterViewController.Instance.HandleBuildAndShowCharacterRoster(MyCharacterData);
            }
        }
        private bool AllowInput()
        {
            bool ret = true;

            if(StoryEventController.Instance.ViewIsActive ||
                GameIntroController.Instance.ViewIsActive)
            {
                ret = false;
            }

            return ret;

        }
        #endregion
    }
}
