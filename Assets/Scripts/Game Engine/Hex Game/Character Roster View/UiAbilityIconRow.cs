using TMPro;
using UnityEngine;
using WeAreGladiators.Abilities;

namespace WeAreGladiators.UI
{
    public class UiAbilityIconRow : MonoBehaviour
    {

        public void Build(AbilityData data)
        {
            gameObject.SetActive(true);
            abilityIcon.BuildFromAbilityData(data);
            abilityNameText.text = data.abilityName;
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        // Properties + Components
        #region

        [SerializeField] private UIAbilityIcon abilityIcon;
        [SerializeField] private TextMeshProUGUI abilityNameText;

        #endregion
    }
}
