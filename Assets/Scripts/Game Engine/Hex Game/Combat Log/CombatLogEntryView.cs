using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WeAreGladiators.CombatLog
{
    public class CombatLogEntryView : MonoBehaviour
    {
        [SerializeField] Image icon;
        [SerializeField] TextMeshProUGUI descriptionText;

        public void Build(CombatLogEntryData data)
        {
            gameObject.SetActive(true);
            icon.sprite = data.Icon;
            descriptionText.text = data.Description;
        }
    }
}