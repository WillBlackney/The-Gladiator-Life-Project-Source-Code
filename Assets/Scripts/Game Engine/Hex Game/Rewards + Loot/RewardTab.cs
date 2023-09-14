using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WeAreGladiators.RewardSystems
{
    public class RewardTab : MonoBehaviour
    {
        public TextMeshProUGUI descriptionText;
        public Image typeImage;

        public void OnRewardButtonClicked()
        {
            Debug.Log("RewardTab.OnRewardButtonClicked() called...");
            RewardController.Instance.OnRewardTabButtonClicked(this);
        }
    }
}
