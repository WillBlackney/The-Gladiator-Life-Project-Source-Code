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
        [SerializeField] GameObject mainVisualParent;

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

        // Show + Hide Logic
        #region
        public void ShowTownDefaultView()
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
        #endregion

    }
}