using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using HexGameEngine.Characters;
using HexGameEngine.Utilities;
using UnityEngine.UI;

namespace HexGameEngine.UI
{
    public class CharacterScrollPanelController : Singleton<CharacterScrollPanelController>
    {
        // Components + Properties
        #region
        [SerializeField] private GameObject mainVisualParent;
        [SerializeField] private TextMeshProUGUI totalCharactersText;
        [SerializeField] private RosterCharacterPanel[] allCharacterPanels;
        [SerializeField] private RectTransform[] dynamicContentFitters;
        #endregion

        // Getters + Accessors
        #region

        #endregion

        // Show + Hide Logic
        #region
        
        public void ShowMainView()
        {
            mainVisualParent.SetActive(true);
        }
        public void HideMainView()
        {
            mainVisualParent.SetActive(false);
        }
        #endregion

        // Build Logic
        #region
        public void BuildAndShowPanel()
        {
            ShowMainView();
            BuildViews(CharacterDataController.Instance.AllPlayerCharacters);
        }
        public void BuildViews(List<HexCharacterData> characters = null)
        {
            if (characters == null) characters = CharacterDataController.Instance.AllPlayerCharacters;

            // Set total characters text
            int maxCharacters = 10; // TO DO: get the actual max character limit
            totalCharactersText.text = characters.Count.ToString() + " / " + maxCharacters.ToString();

            // Reset all tabs
            foreach (RosterCharacterPanel tab in allCharacterPanels)
                tab.ResetAndHide();

            // Build a tab for each character
            for (int i = 0; i < characters.Count; i++)
            {
                allCharacterPanels[i].Show();
                allCharacterPanels[i].BuildFromCharacterData(characters[i]);
            }
               

            RebuildFitters();
        }
        private void RebuildFitters()
        {
            for(int j = 0; j < 2; j++)
            {
                for (int i = 0; i < dynamicContentFitters.Length; i++)
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(dynamicContentFitters[i]);
                }

            }
            
        }
        #endregion
    }
}