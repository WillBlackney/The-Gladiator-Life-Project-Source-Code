using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WeAreGladiators.Utilities;
using WeAreGladiators.Perks;
using WeAreGladiators.Libraries;

namespace WeAreGladiators.UI
{
    public class KeyWordLayoutController : Singleton<KeyWordLayoutController>
    {
        // Properties + Component References
        #region
        [Header("Data")]
        [SerializeField] private KeyWordDataBox[] allKeyWordData;
        [Space(10)]

        [Header("Transform + Parent Component")]
        [SerializeField] private Canvas rootCanvas;
        [SerializeField] private RectTransform panelFitterParent;
        [SerializeField] private RectTransform normalPos;
        [SerializeField] private RectTransform combatPos;
        [Space(10)]
        [Header("Canvas Components")]
        [SerializeField] private CanvasGroup mainCg;
        [Space(10)]
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
                panel.framedImageParent.SetActive(false);
                panel.unframedImageParent.SetActive(false);

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
                TransformUtils.RebuildLayout(panel.rootLayoutRect);
            }

            // Rebuild entire GUI
            TransformUtils.RebuildLayout(panelFitterParent);
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
            TransformUtils.RebuildLayout(panel.rootLayoutRect);
            TransformUtils.RebuildLayout(panelFitterParent);
        }
        private void BuildKeywordPanelFromModel(KeyWordPanel panel, KeyWordModel model)
        {
            // Find data
            KeyWordDataBox data = GetMatchingKeyWordData(model);

            // Set text values
            panel.nameText.text = GetKeyWordNameString(data);
            panel.descriptionText.text = GetKeyWordDescriptionString(data);
            if (data.useSprite && data.sprite != null)
            {
                panel.framedImageParent.SetActive(false);
                panel.unframedImageParent.SetActive(true);
                panel.unframedImage.sprite = data.sprite;
            }

        }
        private void BuildKeywordPanelFromPerkData(KeyWordPanel panel, PerkIconData data)
        {
            // Set texts
            panel.nameText.text = data.passiveName;
            panel.descriptionText.text = TextLogic.ConvertCustomStringListToString(data.passiveDescription);

            // Enable image component if it has sprite data
            panel.framedImageParent.SetActive(true);
            panel.unframedImageParent.SetActive(false);
            panel.framedImage.sprite = data.passiveSprite;
        }
        #endregion

        // Get data
        #region
        private KeyWordDataBox GetMatchingKeyWordData(KeyWordModel model)
        {
            KeyWordDataBox dataReturned = null;

            foreach (KeyWordDataBox data in allKeyWordData)
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
        private string GetKeyWordDescriptionString(KeyWordDataBox data)
        {
            string stringReturned = "empty";

            if (data.kewWordType != KeyWordType.Perk)
            {
                stringReturned = TextLogic.ConvertCustomStringListToString(data.keyWordDescription);
            }

            return stringReturned;
        }
        private string GetKeyWordNameString(KeyWordDataBox data)
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
        private void EnableMainView()
        {
            rootCanvas.enabled = true;
            if (GameController.Instance.GameState == GameState.CombatActive ||
                GameController.Instance.GameState == GameState.CombatRewardPhase ||
                GameController.Instance.GameState == GameState.MainMenu)
                panelFitterParent.position = combatPos.position;
            else panelFitterParent.position = normalPos.position;
        }
        private void DisableMainView()
        {
            rootCanvas.enabled = false;
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