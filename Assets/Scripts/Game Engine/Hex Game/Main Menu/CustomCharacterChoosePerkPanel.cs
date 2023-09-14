using TMPro;
using UnityEngine;
using WeAreGladiators.MainMenu;
using WeAreGladiators.Perks;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.UI
{
    public class CustomCharacterChoosePerkPanel : MonoBehaviour
    {
        [SerializeField] private UIPerkIcon perkIcon;
        [SerializeField] private TextMeshProUGUI perkNameText;
        [SerializeField] private GameObject[] selectedIndicators;

        public UIPerkIcon PerkIcon => perkIcon;
        public void Reset()
        {
            gameObject.SetActive(false);
            SetSelectedViewState(false);
        }
        public void Build(ActivePerk perkData)
        {
            gameObject.SetActive(true);
            perkNameText.text = TextLogic.SplitByCapitals(perkData.perkTag.ToString());
            perkIcon.BuildFromActivePerk(perkData);
        }
        public void SetSelectedViewState(bool onOrOff)
        {
            for (int i = 0; i < selectedIndicators.Length; i++)
            {
                selectedIndicators[i].gameObject.SetActive(onOrOff);
            }
        }

        public void MouseEnter()
        {
            if (perkIcon.ActivePerk != null)
            {
                perkIcon.OnPointerEnter(null);
            }
        }
        public void MouseExit()
        {
            if (perkIcon.ActivePerk != null)
            {
                perkIcon.OnPointerExit(null);
            }
        }
        public void MouseClick()
        {
            MainMenuController.Instance.HandleChoosePerkPanelClicked(this);
        }
    }
}
