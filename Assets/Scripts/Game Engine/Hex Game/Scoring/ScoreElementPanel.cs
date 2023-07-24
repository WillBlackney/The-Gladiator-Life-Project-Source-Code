using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace WeAreGladiators.Scoring
{
    public class ScoreElementPanel : MonoBehaviour
    {
        // Properties + Components
        #region
        [SerializeField] TextMeshProUGUI nameText, valueText;
        [SerializeField] CanvasGroup myCg;
        #endregion

        #region Getters + Accessors
        public bool IsActive { get; set;} = false;
        public TextMeshProUGUI NameText => nameText;
        public TextMeshProUGUI ValueText => valueText;
        public CanvasGroup MyCg => myCg;
        #endregion
    }
}
