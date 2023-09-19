using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WeAreGladiators.Characters;
using WeAreGladiators.GameIntroEvent;
using WeAreGladiators.Items;
using WeAreGladiators.Libraries;
using WeAreGladiators.TownFeatures;
using WeAreGladiators.UCM;

namespace WeAreGladiators.UI
{
    public class RosterCharacterPanel : MonoBehaviour, IPointerClickHandler
    {

        // Getters + Accessors
        #region

        public HexCharacterData MyCharacterData { get; private set; }

        #endregion
        // Components
        #region

        [Header("Core Components")]
        [SerializeField]
        private TextMeshProUGUI nameText;
        [SerializeField] private UniversalCharacterModel portaitModel;

        [Header("Health Bar Components")]
        [SerializeField]
        private Slider healthBar;
        [SerializeField] private TextMeshProUGUI healthText;

        [Header("Stress Bar Components")]
        [SerializeField]
        private Slider stressBar;
        //[SerializeField] TextMeshProUGUI stressText;

        [Header("Perk + Injury Components")]
        [SerializeField]
        private UIPerkIcon[] perkIcons;

        [Header("Activity Indicator Components")]
        [SerializeField] private GameObject[] activityIndicatorParents;
        [SerializeField] private Image activityIndicatorImage;

        [Header("Level Components")]
        [SerializeField] private LevelUpButton levelUpIndicator;
        [SerializeField] private TextMeshProUGUI currentLevelText;

        // Non-inspector properties

        #endregion

        // Logic
        #region

        public void BuildFromCharacterData(HexCharacterData data)
        {
            MyCharacterData = data;

            UpdateActivityIndicator();

            // Texts
            nameText.text = "<color=#BC8252>" + data.myName + "<color=#DDC6AB>    " + data.mySubName;
            healthText.text = data.currentHealth.ToString();
            //stressText.text = data.currentStress.ToString();

            // Build model
            CharacterModeller.BuildModelFromStringReferencesAsMugshot(portaitModel, data.modelParts);
            CharacterModeller.ApplyItemSetToCharacterModelView(data.itemSet, portaitModel);
            portaitModel.SetBaseAnim();

            // Build bars
            healthBar.value = data.currentHealth / (float) StatCalculator.GetTotalMaxHealth(data);
            //stressBar.value = data.currentMoraleState / 20f;

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
        public void UpdateActivityIndicator()
        {
            if (MyCharacterData.currentTownActivity == TownActivity.None)
            {
                SetIndicatorParentViewStates(false);
            }
            else
            {
                SetIndicatorParentViewStates(true);
                activityIndicatorImage.sprite = SpriteLibrary.Instance.GetTownActivitySprite(MyCharacterData.currentTownActivity);
            }
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
            if ((TownController.Instance.HospitalViewIsActive ||
                    TownController.Instance.LibraryViewIsActive ||
                    TownController.Instance.DeploymentViewIsActive) &&
                !InventoryController.Instance.VisualParent.activeSelf &&
                !GameIntroController.Instance.ViewIsActive)
            {
                PortraitDragController.Instance.OnRosterCharacterPanelDragStart(this);
            }

        }
        public void OnLevelButtonClicked()
        {
            if (!InventoryController.Instance.VisualParent.activeSelf &&
                !GameIntroController.Instance.ViewIsActive)
            {
                CharacterRosterViewController.Instance.BuildAndShowFromCharacterData(MyCharacterData);
            }
        }
        private void SetIndicatorParentViewStates(bool onOrOff)
        {
            //foreach (GameObject g in activityIndicatorParents)
            //    g.SetActive(onOrOff);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right &&
                !InventoryController.Instance.VisualParent.activeSelf &&
                !GameIntroController.Instance.ViewIsActive)
            {
                CharacterRosterViewController.Instance.BuildAndShowFromCharacterData(MyCharacterData);
            }
        }

        #endregion
    }
}
