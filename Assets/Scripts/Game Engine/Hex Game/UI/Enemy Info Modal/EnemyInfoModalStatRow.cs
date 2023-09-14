using TMPro;
using UnityEngine;
using WeAreGladiators.Perks;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.UI
{
    public class EnemyInfoModalStatRow : MonoBehaviour
    {
        [SerializeField] private UIPerkIcon icon;
        [SerializeField] private TextMeshProUGUI perkNameText;

        public void BuildAndShow(ActivePerk p)
        {
            gameObject.SetActive(true);
            icon.BuildFromActivePerk(p);
            perkNameText.text = TextLogic.SplitByCapitals(p.perkTag.ToString());
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
