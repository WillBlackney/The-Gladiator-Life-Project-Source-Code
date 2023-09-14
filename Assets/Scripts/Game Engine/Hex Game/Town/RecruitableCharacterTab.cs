using TMPro;
using UnityEngine;
using WeAreGladiators.Characters;
using WeAreGladiators.Player;
using WeAreGladiators.UCM;
using WeAreGladiators.UI;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.TownFeatures
{
    public class RecruitableCharacterTab : MonoBehaviour
    {

        // Input
        #region

        public void OnClick()
        {
            TownController.Instance.OnCharacterRecruitTabClicked(this);
        }

        #endregion
        // Components
        #region

        [Header("Core Components")]
        [SerializeField]
        private TextMeshProUGUI nameText;
        [SerializeField] private UniversalCharacterModel portaitModel;
        [SerializeField] private GameObject selectedParent;

        [Space(10)]
        [Header("Race Section Components")]
        [SerializeField]
        private TextMeshProUGUI racialText;
        [SerializeField] private UIRaceIcon racialIcon;

        [Space(10)]
        [Header("Background Section Components")]
        [SerializeField]
        private TextMeshProUGUI backgroundText;
        [SerializeField] private UIBackgroundIcon backgroundIcon;

        [Space(10)]
        [Header("Cost Section Components")]
        [SerializeField]
        private TextMeshProUGUI recruitCostText;
        [SerializeField] private TextMeshProUGUI upkeepCostText;

        [Space(10)]
        [Header("Misc Components")]
        [SerializeField]
        private TextMeshProUGUI levelText;

        // Non-inspector properties

        #endregion

        // Getters + Accesors
        #region

        public HexCharacterData MyCharacterData { get; private set; }
        public GameObject SelectedParent => selectedParent;

        #endregion

        // Logic
        #region

        public void BuildFromCharacterData(HexCharacterData data)
        {
            Show();
            MyCharacterData = data;

            // Texts
            levelText.text = data.currentLevel.ToString();
            nameText.text = "<color=#BC8252>" + data.myName + "<color=#DDC6AB>    " + data.mySubName;
            if (racialText != null)
            {
                racialText.text = data.race.ToString();
            }
            racialIcon.BuildFromRacialData(CharacterDataController.Instance.GetRaceData(data.race));
            if (backgroundText != null)
            {
                backgroundText.text = TextLogic.SplitByCapitals(data.background.backgroundType.ToString());
            }
            backgroundIcon.BuildFromBackgroundData(data.background);

            // Build model
            CharacterModeller.BuildModelFromStringReferencesAsMugshot(portaitModel, data.modelParts);
            CharacterModeller.ApplyItemSetToCharacterModelView(data.itemSet, portaitModel);
            portaitModel.SetBaseAnim();

            // Wage and recruit costs
            int cost = CharacterDataController.Instance.GetCharacterInitialHiringCost(data);
            string col = "<color=#DDC6AB>";
            if (PlayerDataController.Instance.CurrentGold < cost)
            {
                col = TextLogic.lightRed;
            }
            upkeepCostText.text = data.dailyWage.ToString();
            recruitCostText.text = TextLogic.ReturnColoredText(cost.ToString(), col);
        }
        private void Show()
        {
            gameObject.SetActive(true);
        }
        public void ResetAndHide()
        {
            MyCharacterData = null;
            gameObject.SetActive(false);
            selectedParent.SetActive(false);
        }

        #endregion
    }
}
