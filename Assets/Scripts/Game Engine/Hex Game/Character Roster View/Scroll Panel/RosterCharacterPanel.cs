using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using CardGameEngine.UCM;
using HexGameEngine.Characters;
using HexGameEngine.UCM;
using HexGameEngine.TownFeatures;
using HexGameEngine.Libraries;
using UnityEngine.EventSystems;
namespace HexGameEngine.UI
{
    public class RosterCharacterPanel : MonoBehaviour, IPointerClickHandler
    {
        // Components
        #region
        [Header("Core Components")]
        [SerializeField] TextMeshProUGUI nameText;
        [SerializeField] UniversalCharacterModel portaitModel;

        [Header("Health Bar Components")]
        [SerializeField] Slider healthBar;
        [SerializeField] TextMeshProUGUI  healthText;

        [Header("Stress Bar Components")]
        [SerializeField] Slider stressBar;
        //[SerializeField] TextMeshProUGUI stressText;

        [Header("Perk + Injury Components")]
        [SerializeField] UIPerkIcon[] perkIcons;

        [Header("Activity Indicator Components")]
        [SerializeField] private GameObject[] activityIndicatorParents;
        [SerializeField] private Image activityIndicatorImage;

        [Header("Level Components")]
        [SerializeField] private LevelUpButton levelUpIndicator;
        [SerializeField] private TextMeshProUGUI currentLevelText;

        // Non-inspector properties
        private HexCharacterData myCharacterData;
        #endregion

        // Getters + Accessors
        #region
        public HexCharacterData MyCharacterData
        {
            get { return myCharacterData; }
        }
        #endregion

        // Logic
        #region
        public void BuildFromCharacterData(HexCharacterData data)
        {
            myCharacterData = data;

            UpdateActivityIndicator();

            // Texts
            nameText.text = "<color=#BC8252>" + data.myName + "<color=#DDC6AB>    The " + data.myClassName;
            healthText.text = data.currentHealth.ToString();
            //stressText.text = data.currentStress.ToString();

            // Build model
            CharacterModeller.BuildModelFromStringReferencesAsMugshot(portaitModel, data.modelParts);
            CharacterModeller.ApplyItemSetToCharacterModelView(data.itemSet, portaitModel);
            portaitModel.SetBaseAnim();

            // Build bars
            healthBar.value = (float)((float) data.currentHealth / (float) StatCalculator.GetTotalMaxHealth(data));
            stressBar.value = (float)((float) data.currentStress / 20f);

            // Reset perk icons
            foreach (UIPerkIcon b in perkIcons)
                b.HideAndReset();

            // Injury perk icons
            for (int i = 0; i < data.passiveManager.perks.Count && i < perkIcons.Length; i++)
            {
                if(data.passiveManager.perks[i].Data.isInjury ||
                    data.passiveManager.perks[i].Data.isPermanentInjury)
                    perkIcons[i].BuildFromActivePerk(data.passiveManager.perks[i]);
            }

            // Level stuff
            UpdateLevelUpIndicator();
            currentLevelText.text = data.currentLevel.ToString();
        }
        public void UpdateActivityIndicator()
        {
            if (myCharacterData.currentTownActivity == TownActivity.None)
            {
                SetIndicatorParentViewStates(false);
            }
            else
            {
                SetIndicatorParentViewStates(true);
                activityIndicatorImage.sprite = SpriteLibrary.Instance.GetTownActivitySprite(myCharacterData.currentTownActivity);
            }
        }
        private void UpdateLevelUpIndicator()
        {
            if (myCharacterData == null) return;
            levelUpIndicator.Hide();
            if (myCharacterData.attributeRolls.Count > 0 ||
                myCharacterData.talentRolls.Count > 0 ||
                myCharacterData.perkPoints > 0)
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
            myCharacterData = null;
            gameObject.SetActive(false);
        }
        public void OnClickAndDragStart()
        {
            PortraitDragController.Instance.OnRosterCharacterPanelDragStart(this);
        }
        public void OnLevelButtonClicked()
        {
            CharacterRosterViewController.Instance.BuildAndShowFromCharacterData(myCharacterData);
        }
        private void SetIndicatorParentViewStates(bool onOrOff)
        {
            foreach (GameObject g in activityIndicatorParents)
                g.SetActive(onOrOff);
        }

        public void OnPointerClick(PointerEventData eventData)
        {

            if(eventData.button == PointerEventData.InputButton.Right)
            {
                Debug.Log("RIGHT CLICK!!");
                CharacterRosterViewController.Instance.BuildAndShowFromCharacterData(myCharacterData);
            }
        }
        #endregion
    }
}