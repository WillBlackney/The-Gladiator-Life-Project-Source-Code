using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WeAreGladiators.Characters;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.UI
{
    public class CharacterScrollPanelController : Singleton<CharacterScrollPanelController>
    {
        #region Components + Properties

        [Header("Components")]
        [SerializeField] private GameObject mainVisualParent;
        [SerializeField] private TextMeshProUGUI totalCharactersText;
        [SerializeField] private List<RosterCharacterPanel> allCharacterPanels;
        [SerializeField] private RectTransform[] dynamicContentFitters;
        [SerializeField] private GameObject characterPanelPrefab;
        [SerializeField] private Transform characterPanelParent;

        #endregion

        #region Getters + Accessors
        public RosterCharacterPanel GetCharacterPanel(HexCharacterData character)
        {
            RosterCharacterPanel ret = null;
            foreach (RosterCharacterPanel panel in allCharacterPanels)
            {
                if (panel.MyCharacterData == character)
                {
                    ret = panel;
                    break;
                }
            }
            return ret;

        }

        #endregion

        #region Show + Hide Logic

        public void ShowMainView()
        {
            mainVisualParent.SetActive(true);
        }
        public void HideMainView()
        {
            mainVisualParent.SetActive(false);
        }

        #endregion

        #region Build Logic

        public void BuildAndShowPanel()
        {
            ShowMainView();
            RebuildViews(CharacterDataController.Instance.AllPlayerCharacters);
        }
        public void RebuildViews(List<HexCharacterData> characters = null)
        {
            if (characters == null)
            {
                characters = CharacterDataController.Instance.AllPlayerCharacters;
            }

            // Set total characters text
            int maxCharacters = CharacterDataController.Instance.MaxAllowedCharacters; 
            totalCharactersText.text = characters.Count + " / " + maxCharacters;

            // Reset all
            allCharacterPanels.ForEach(i => i.ResetAndHide());

            // Build a tab for each character
            for (int i = 0; i < characters.Count; i++)
            {
                if (i >= allCharacterPanels.Count)
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
