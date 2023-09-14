using TMPro;
using UnityEngine;
using WeAreGladiators.Characters;

namespace WeAreGladiators.UI
{
    public class UITalentRow : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI talentNameText;
        [SerializeField] private UITalentIcon talentIcon;

        public void HideAndReset()
        {
            talentIcon.HideAndReset();
            gameObject.SetActive(false);
        }

        public void BuildFromTalentPairing(TalentPairing tp)
        {
            gameObject.SetActive(true);
            talentNameText.text = tp.talentSchool.ToString();
            talentIcon.BuildFromTalentPairing(tp);
        }
    }
}
