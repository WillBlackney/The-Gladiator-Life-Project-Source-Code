using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using HexGameEngine.Characters;
using UnityEngine.UI;
using HexGameEngine.Audio;

namespace HexGameEngine.UI
{
    public class AttributeLevelUpPage : MonoBehaviour
    {
        // Properties + Components
        #region
        [Header("Core Components")]
        [SerializeField] private GameObject mainVisualParent;
        [SerializeField] private AttributeLevelUpWidget[] attributeRows;
        [SerializeField] private TextMeshProUGUI totalSelectedAttributesText;

        [Header("Confirm Button Components")]
        [SerializeField] private Image confirmButtonImage;
        [SerializeField] private Sprite readyImage;
        [SerializeField] private Sprite notReadyImage;

        // Non inspector fields
        private HexCharacterData currentCharacter;
        #endregion

        // Input
        #region
        public void OnConfirmButtonClicked()
        {
            List<AttributeLevelUpWidget> selectedAttributes = GetSelectedAttributes();

            // Validate selections
            if (selectedAttributes.Count != 3) return;
            HandleSelectionsConfirmed(selectedAttributes);            
        }
        public void OnCancelButtonClicked()
        {
            currentCharacter = null;
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
                w.ResetViews();
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
            totalSelectedAttributesText.text = selected.ToString() + "/3";
            if (selected >= 3) confirmButtonImage.sprite = readyImage;
            else confirmButtonImage.sprite = notReadyImage;
        }
        private void HandleSelectionsConfirmed(List<AttributeLevelUpWidget> selections)
        {
            // Apply stat boosts
            foreach(AttributeLevelUpWidget w in selections)
            {
                if(w.MyAttribute == CoreAttribute.Might)                
                    currentCharacter.attributeSheet.might.value += currentCharacter.attributeRolls[0].mightRoll;
                else if (w.MyAttribute == CoreAttribute.Accuracy)
                    currentCharacter.attributeSheet.accuracy.value += currentCharacter.attributeRolls[0].accuracyRoll;
                else if (w.MyAttribute == CoreAttribute.Dodge)
                    currentCharacter.attributeSheet.dodge.value += currentCharacter.attributeRolls[0].dodgeRoll;
                else if (w.MyAttribute == CoreAttribute.Resolve)
                    currentCharacter.attributeSheet.resolve.value += currentCharacter.attributeRolls[0].resolveRoll;                
                else if (w.MyAttribute == CoreAttribute.Wits)
                    currentCharacter.attributeSheet.wits.value += currentCharacter.attributeRolls[0].witsRoll;
                else if (w.MyAttribute == CoreAttribute.Fitness)
                    currentCharacter.attributeSheet.fitness.value += currentCharacter.attributeRolls[0].fitnessRoll;
                else if (w.MyAttribute == CoreAttribute.Constitution)
                {
                    currentCharacter.attributeSheet.constitution.value += currentCharacter.attributeRolls[0].constitutionRoll;
                    CharacterDataController.Instance.SetCharacterHealth(currentCharacter, currentCharacter.currentHealth + currentCharacter.attributeRolls[0].constitutionRoll);
                }

            }

            // Pop stat roll from character
            currentCharacter.attributeRolls.RemoveAt(0);

            // Close attribute level up page views
            mainVisualParent.SetActive(false);

            // Rebuild character roster views
            AudioManager.Instance.PlaySound(Sound.Effects_Confirm_Level_Up);
            CharacterRosterViewController.Instance.HandleRedrawRosterOnCharacterUpdated();
            CharacterScrollPanelController.Instance.RebuildViews();
        }
        #endregion
    }
}