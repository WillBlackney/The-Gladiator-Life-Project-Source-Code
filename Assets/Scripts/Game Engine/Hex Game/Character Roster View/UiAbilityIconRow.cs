using HexGameEngine.Abilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace HexGameEngine.UI
{
    public class UiAbilityIconRow : MonoBehaviour
    {
        // Properties + Components
        #region
        [SerializeField] UIAbilityIcon abilityIcon;
        [SerializeField] TextMeshProUGUI abilityNameText;

        #endregion
    
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
    }
}