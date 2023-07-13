using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using WeAreGladiators.Characters;

namespace WeAreGladiators.UI
{
    public class UITalentRow : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI talentNameText;
        [SerializeField] UITalentIcon talentIcon;

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