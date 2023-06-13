using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using HexGameEngine.MainMenu;
using HexGameEngine.Perks;
using HexGameEngine.Utilities;

namespace HexGameEngine.UI
{
    public class CustomCharacterChoosePerkPanel : MonoBehaviour
    {
        [SerializeField] UIPerkIcon perkIcon;
        [SerializeField] TextMeshProUGUI perkNameText;
        [SerializeField] GameObject[] selectedIndicators;

        public UIPerkIcon PerkIcon
        {
            get { return perkIcon; }
        }
        public void Build(ActivePerk perkData)
        {
            gameObject.SetActive(true);
            perkNameText.text = TextLogic.SplitByCapitals(perkData.perkTag.ToString());
            perkIcon.BuildFromActivePerk(perkData);
        }
        public void Reset()
        {
            gameObject.SetActive(false);
            SetSelectedViewState(false);
        }
        public void SetSelectedViewState(bool onOrOff)
        {
            for(int i = 0; i < selectedIndicators.Length; i++)            
                selectedIndicators[i].gameObject.SetActive(onOrOff);            
        }

        public void MouseEnter()
        {
            if (perkIcon.ActivePerk != null) perkIcon.OnPointerEnter(null);            
        }
        public void MouseExit()
        {
            if (perkIcon.ActivePerk != null) perkIcon.OnPointerExit(null);
        }
        public void MouseClick()
        {
            MainMenuController.Instance.HandleChoosePerkPanelClicked(this);
        }
    }
}