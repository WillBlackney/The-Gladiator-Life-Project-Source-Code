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
        [Header("Components")]
        [SerializeField] private GameObject mainVisualParent;
        [SerializeField] private TextMeshProUGUI totalCharactersText;
        [SerializeField] private List<RosterCharacterPanel> allCharacterPanels;
        [SerializeField] private RectTransform[] dynamicContentFitters;
        [SerializeField] private GameObject characterPanelPrefab;
        [SerializeField] private Transform characterPanelParent;
        #endregion

        // Getters + Accessors
        #region
        public RosterCharacterPanel GetCharacterPanel(HexCharacterData character)
        {
            RosterCharacterPanel ret = null;
            foreach(RosterCharacterPanel panel in allCharacterPanels)
            {
                if(panel.MyCharacterData == character)
                {
                    ret = panel;
                    break;
                }
            }
            return ret;

        }
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
            RebuildViews(CharacterDataController.Instance.AllPlayerCharacters);
        }
        public void RebuildViews(List<HexCharacterData> characters = null)
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
                if(i >= allCharacterPanels.Count)
                {
                    RosterCharacterPanel newTab = Instantiate(characterPanelPrefab, characterPanelParent).GetComponent<RosterCharacterPanel>();
                    allCharacterPanels.Add(newTab);
                }
                allCharacterPanels[i].Show();
                allCharacterPanels[i].BuildFromCharacterData(characters[i]);
            }

            TransformUtils.RebuildLayouts(dynamicContentFitters);
        }
        #endregion
    }
}