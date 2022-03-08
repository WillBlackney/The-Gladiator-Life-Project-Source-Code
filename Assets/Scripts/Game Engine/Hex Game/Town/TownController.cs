using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexGameEngine.Utilities;
using HexGameEngine.Persistency;
using HexGameEngine.Characters;

namespace HexGameEngine.TownFeatures
{

    public class TownController : Singleton<TownController>
    {
        // Properties + Components
        #region
        [Header("Core Components")]
        [SerializeField] GameObject mainVisualParent;

        [Header("Recruit Page Components")]
        [SerializeField] GameObject recruitPageVisualParent;


        // Non-inspector properties
        private List<HexCharacterData> currentRecruits = new List<HexCharacterData>();
        #endregion

        // Save + Load Logic
        #region
        public void BuildMyDataFromSaveFile(SaveGameData saveFile)
        {
            currentRecruits.Clear();
            foreach (HexCharacterData c in saveFile.townRecruits)
                currentRecruits.Add(c);
        }
        public void SaveMyDataToSaveFile(SaveGameData saveFile)
        {
            saveFile.townRecruits.Clear();
            foreach (HexCharacterData c in currentRecruits)
                saveFile.townRecruits.Add(c);
        }
        #endregion

        // Show + Hide Main View Logic
        #region
        public void ShowTownView()
        {
            mainVisualParent.SetActive(true);
        }
        public void HideTownView()
        {
            mainVisualParent.SetActive(false);
        }
        #endregion

        // Recruit Characters Logic
        #region
        public void GenerateDailyRecruits(int amount)
        {
            for (int i = 0; i < amount; i++)
                HandleAddNewRecruitFromCharacterDeck();
        }
        private void HandleAddNewRecruitFromCharacterDeck()
        {
            if (CharacterDataController.Instance.CharacterDeck.Count == 0)
                CharacterDataController.Instance.AutoGenerateAndCacheNewCharacterDeck();
            currentRecruits.Add(CharacterDataController.Instance.CharacterDeck[0]);
            CharacterDataController.Instance.CharacterDeck.RemoveAt(0);

        }
        public void BuildAndShowRecruitPage()
        {
            recruitPageVisualParent.SetActive(true);
        }
        public void HideRecruitPage()
        {
            recruitPageVisualParent.SetActive(false);
        }
        #endregion

    }
}