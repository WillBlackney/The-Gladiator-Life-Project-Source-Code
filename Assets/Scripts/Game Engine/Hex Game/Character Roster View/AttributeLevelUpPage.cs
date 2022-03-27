using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using HexGameEngine.Characters;
using UnityEngine.UI;

namespace HexGameEngine.UI
{
    public class AttributeLevelUpPage : MonoBehaviour
    {
        // Properties + Components
        #region
        [Header("Text Components")]
        [SerializeField] private GameObject mainVisualParent;
        [SerializeField] private AttributeLevelUpWidget[] attributeRows;
        [SerializeField] private TextMeshProUGUI totalSelectedAttributesText;

        [Header("Button Components")]
        [SerializeField] private Image confirmButtonImage;
        [SerializeField] private Sprite readyImage;
        [SerializeField] private Sprite notReadyImage;

        // Non inspector fields
        private HexCharacterData currentCharacter;
        #endregion

        // Getters + Accessors
        #region
        #endregion

        // Input
        #region
        public void OnConfirmButtonClicked()
        {
            // validate selections
            // apply stat boosts
            // pop a stat roll from character
            // close window
            // rebuild character roster views
        }
        public void OnCancelButtonClicked()
        {
            mainVisualParent.SetActive(false);
        }
        #endregion

        // Logic
        #region
        public void ShowAndBuildPage(HexCharacterData character)
        {
            mainVisualParent.SetActive(true);
            currentCharacter = character;

            foreach (AttributeLevelUpWidget w in attributeRows)
            {
                w.Reset();
                w.BuildViews(character);
            }

            UpdateTotalSelectedAttributes();
        }       
        public List<AttributeLevelUpWidget> GetSelectedAttributes()
        {
            List<AttributeLevelUpWidget> ret = new List<AttributeLevelUpWidget>();
            foreach (AttributeLevelUpWidget a in attributeRows)            
                if (a.Selected)                
                    ret.Add(a);                
            
            return ret;
        }
        public void UpdateTotalSelectedAttributes()
        {
            int selected = GetSelectedAttributes().Count;
            totalSelectedAttributesText.text = selected.ToString() + " / 3";
            if (selected >= 3) confirmButtonImage.sprite = readyImage;
            else confirmButtonImage.sprite = notReadyImage;
        }
        #endregion
    }
}