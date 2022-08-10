using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HexGameEngine.Utilities;
using HexGameEngine.Perks;
using HexGameEngine.Libraries;

namespace HexGameEngine.UI
{
    public class KeyWordLayoutController : Singleton<KeyWordLayoutController>
    {
        // Properties + Component References
        #region
        [Header("Transform + Parent Component")]
        [SerializeField] private GameObject visualParent;
        [SerializeField] private RectTransform panelFitterParent;
        [SerializeField] private RectTransform normalPos;
        [SerializeField] private RectTransform combatPos;

        [Header("Canvas Components")]
        [SerializeField] private CanvasGroup mainCg;

        [Header("Keyword Panel Components")]
        [SerializeField] private KeyWordPanel[] allKeyWordPanels;       
        #endregion

        // Build Keyword Panels
        #region
        public void BuildAllViewsFromKeyWordModels(List<KeyWordModel> keyWords)
        {
            // Enable + reset main view
            ResetAllKeyWordPanels();
            EnableMainView();
            FadeInMainView();

            // build each panel
            for (int i = 0; i < keyWords.Count && i < allKeyWordPanels.Length; i++)
            {
                // Setup
                KeyWordPanel panel = allKeyWordPanels[i];
                KeyWordModel data = keyWords[i];

                // Disable panel image icon
                panel.panelImageParent.SetActive(false);

                // Enable panel parent
                panel.gameObject.SetActive(true);

                // Is this a perk effect?
                if (data.kewWordType == KeyWordType.Perk)
                {
                    BuildKeywordPanelFromPerkData
                        (panel, PerkController.Instance.GetPerkIconDataByTag(data.passiveType));
                }

                // Else build normally
                else
                {
                    BuildKeywordPanelFromModel(panel, data);
                }

                // Rebuild content on panel
                panel.RebuildLayout();
            }

            // Rebuild entire GUI
            RebuildEntireLayout();
        }
        public void BuildAllViewsFromPassiveTag(Perk tag)
        {
            // Enable + Reset main view
            ResetAllKeyWordPanels();
            EnableMainView();
            FadeInMainView();

            // Get passive data
            PerkIconData data = PerkController.Instance.GetPerkIconDataByTag(tag);

            // Get and enable first panel
            KeyWordPanel panel = allKeyWordPanels[0];
            panel.gameObject.SetActive(true);

            // Build panel views from data
            BuildKeywordPanelFromPerkData(panel, data);

            // Rebuild the panel, then rebuild the entire GUI
            panel.RebuildLayout();
            RebuildEntireLayout();
        }
        private void BuildKeywordPanelFromModel(KeyWordPanel panel, KeyWordModel model)
        {
            // Find data
            KeyWordData data = GetMatchingKeyWordData(model);

            // Set text values
            panel.nameText.text = GetKeyWordNameString(data);
            panel.descriptionText.text = GetKeyWordDescriptionString(data);
            if (data.useSprite && data.sprite != null)
            {
                panel.panelImageParent.SetActive(true);
                panel.panelImage.sprite = data.sprite;
            }

        }
        private void BuildKeywordPanelFromPerkData(KeyWordPanel panel, PerkIconData data)
        {
            // Set texts
            panel.nameText.text = data.passiveName;
            panel.descriptionText.text = TextLogic.ConvertCustomStringListToString(data.passiveDescription);

            // Enable image component if it has sprite data
            panel.panelImageParent.SetActive(true);
            panel.panelImage.sprite = data.passiveSprite;
        }
        #endregion

        // Get data
        #region
        private KeyWordData GetMatchingKeyWordData(KeyWordModel model)
        {
            KeyWordData dataReturned = null;

            foreach (KeyWordData data in KeywordLibrary.Instance.allKeyWordData)
            {
                if (model.kewWordType == data.kewWordType)
                {
                    dataReturned = data;
                    break;
                }
            }

            if (dataReturned == null)
            {
                Debug.LogWarning("GetMatchingKeyWordData() could not find a matching key word data object for '" +
                    model.ToString() + "' key word modelm, returning null!...");
            }

            return dataReturned;
        }
        private string GetKeyWordDescriptionString(KeyWordData data)
        {
            string stringReturned = "empty";

            if (data.kewWordType != KeyWordType.Perk)
            {
                stringReturned = TextLogic.ConvertCustomStringListToString(data.keyWordDescription);
            }

            return stringReturned;
        }
        private string GetKeyWordNameString(KeyWordData data)
        {
            string stringReturned = "empty";

            if (data.kewWordType == KeyWordType.Perk)
            {
                // do passive stuff
                stringReturned = data.passiveType.ToString();
            }
            else
            {
                stringReturned = TextLogic.SplitByCapitals(data.kewWordType.ToString());
            }

            return stringReturned;
        }
        #endregion

        // View Logic
        #region
        private void ResetAllKeyWordPanels()
        {
            foreach (KeyWordPanel panel in allKeyWordPanels)
            {
                panel.gameObject.SetActive(false);
            }
        }
        private void RebuildEntireLayout()
        {
            for(int i = 0; i < 2; i++)
                LayoutRebuilder.ForceRebuildLayoutImmediate(panelFitterParent);
        }
        private void EnableMainView()
        {
            visualParent.SetActive(true);
            if (GameController.Instance.GameState == GameState.CombatActive)
                panelFitterParent.position = combatPos.position;
            else panelFitterParent.position = normalPos.position;
        }
        private void DisableMainView()
        {
            visualParent.SetActive(false);
        }
        private void FadeInMainView()
        {
            mainCg.DOKill();
            Sequence s = DOTween.Sequence();
            s.Append(mainCg.DOFade(1f, 0.25f));
        }
        public void FadeOutMainView()
        {
            DisableMainView();
            mainCg.DOKill();
            mainCg.alpha = 0f;
        }
        #endregion


    }
}