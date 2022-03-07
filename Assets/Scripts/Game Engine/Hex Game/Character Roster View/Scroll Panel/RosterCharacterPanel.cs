using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using CardGameEngine.UCM;
namespace HexGameEngine.UI
{
    public class RosterCharacterPanel : MonoBehaviour
    {
        // Components
        #region
        [Header("Core Components")]
        [SerializeField] TextMeshProUGUI nameText;
        [SerializeField] UniversalCharacterModel portaitModel;

        [Header("Health Bar Components")]
        [SerializeField] Slider healthBar;
        [SerializeField] TextMeshProUGUI  healthText;

        [Header("Stress Bar Components")]
        [SerializeField] Slider stressBar;
        [SerializeField] TextMeshProUGUI stressText;
        #endregion
    }
}