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

        public Image Icon => icon;
        public TextMeshProUGUI DescriptionText => descriptionText;
    }
}