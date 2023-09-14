using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeAreGladiators.UI;

namespace WeAreGladiators.Perks
{
    public class PerkIconView : MonoBehaviour
    {
        // Properties + Component References
        #region

        [Header("Component References")]
        [SerializeField] private TextMeshProUGUI statusStacksText;
        [SerializeField] private Image passiveImage;
        [SerializeField] private Image frameImage;
        [SerializeField] private Sprite normalFrame;
        [SerializeField] private Sprite injuryFrame;

        private bool showBothModals;

        public static PerkIconView MousedOver { get; private set; }

        #endregion

        // Getters + Accessors
        #region

        [field: Header("Properties")]
        public PerkIconData MyIconData { get; private set; }
        public string StatusName { get; private set; }
        public int StatusStacks { get; private set; }

        #endregion

        // Input 
        #region

        public void MouseEnter()
        {
            if (MyIconData != null &&
                GameController.Instance.GameState == GameState.CombatActive &&
                (MousedOver == null || MousedOver != this))
            {
                if (showBothModals)
                {
                    MainModalController.Instance.BuildAndShowModal(new ActivePerk(MyIconData.perkTag, 1));
                    KeyWordLayoutController.Instance.BuildAllViewsFromKeyWordModels(MyIconData.keywords.ToList());
                }
                else
                {
                    KeyWordLayoutController.Instance.BuildAllViewsFromPassiveTag(MyIconData.perkTag);
                }
            }

        }
        public void MouseExit()
        {
            if (showBothModals)
            {
                MainModalController.Instance.HideModal();
            }
            KeyWordLayoutController.Instance.FadeOutMainView();
        }

        #endregion

        // Logic
        #region

        public void Build(PerkIconData iconData, bool showBothModals = false)
        {
            MyIconData = iconData;
            this.showBothModals = showBothModals;
            passiveImage.sprite = PerkController.Instance.GetPassiveSpriteByName(iconData.passiveName);

            StatusName = iconData.passiveName;
            if (iconData.showStackCount && iconData.isInjury == false)
            {
                statusStacksText.gameObject.SetActive(true);
            }
            else
            {
                statusStacksText.gameObject.SetActive(false);
            }

            if (iconData.hiddenOnPassivePanel)
            {
                gameObject.SetActive(false);
            }

            statusStacksText.text = StatusStacks.ToString();

            // Build frame
            frameImage.sprite = normalFrame;
            if (iconData.isInjury)
            {
                frameImage.sprite = injuryFrame;
            }

        }
        public void ModifyIconViewStacks(int stacksGainedOrLost)
        {
            StatusStacks += stacksGainedOrLost;
            statusStacksText.text = StatusStacks.ToString();
            if (StatusStacks == 0)
            {
                statusStacksText.gameObject.SetActive(false);
            }
        }

        #endregion
    }
}
