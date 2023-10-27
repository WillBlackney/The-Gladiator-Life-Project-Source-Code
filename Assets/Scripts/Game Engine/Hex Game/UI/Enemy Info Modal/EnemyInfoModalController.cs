using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeAreGladiators.Abilities;
using WeAreGladiators.Characters;
using WeAreGladiators.Combat;
using WeAreGladiators.HexTiles;
using WeAreGladiators.Libraries;
using WeAreGladiators.MainMenu;
using WeAreGladiators.Perks;
using WeAreGladiators.TurnLogic;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.UI
{
    public class EnemyInfoModalController : Singleton<EnemyInfoModalController>
    {

        public QueuedShow queuedShow;
        #region Components

        [Header("Core")]
        [SerializeField] private Canvas rootCanvas;
        [SerializeField] private CanvasGroup mainCg;
        [SerializeField] private Transform positionParent;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI turnOrderText;
        [SerializeField] private RectTransform[] layouts;
        [Space(20)]
        [Header("Stress State")]
        [SerializeField] private GameObject moraleStateParent;
        [SerializeField] private Image moraleStateImage;
        [SerializeField] private TextMeshProUGUI moraleStateText;
        [Space(20)]
        [Header("Sliders / Stats")]
        [SerializeField] private Slider healthBar;
        [SerializeField] private TextMeshProUGUI healthText;
        [Space(10)]
        [SerializeField] private Slider armourBar;
        [SerializeField] private TextMeshProUGUI armourText;
        [Space(20)]
        [Header("Perks")]
        [SerializeField] private GameObject perksParent;
        [SerializeField] private EnemyInfoModalStatRow perkRowPrefab;
        [SerializeField] private List<EnemyInfoModalStatRow> perkRows;

        #endregion

        #region Logic

        public void BuildAndShowModal(HexCharacterModel character)
        {
            StartCoroutine(BuildAndShowModalCoroutine(character));
        }
        private IEnumerator BuildAndShowModalCoroutine(HexCharacterModel character)
        {
            // Reveal delay
            LevelNode cachedTile = character.currentTile;
            QueuedShow thisQueuedShow = new QueuedShow();
            queuedShow = thisQueuedShow;
            yield return new WaitForSeconds(0.25f);
            if (queuedShow == null ||
                queuedShow != thisQueuedShow ||
                CombatController.Instance.CurrentCombatState == CombatGameState.CombatInactive ||
                AbilityController.Instance.AwaitingAbilityOrder() ||
                CharacterRosterViewController.Instance.MainVisualParent.activeSelf ||
                EnemyInfoPanel.Instance.PanelIsActive ||
                MainMenuController.Instance.InGameMenuScreenParent.activeSelf ||
                (Application.isMobilePlatform && Input.touchCount == 0))
            {
                yield break;
            }

            // Reset
            mainCg.DOKill();
            mainCg.alpha = 0f;

            // Show
            rootCanvas.enabled = true;
            mainCg.DOFade(1f, 0.25f);

            // Build view elements
            nameText.text = character.myName;
            if (character.characterData != null)
            {
                nameText.text += " " + character.characterData.mySubName;
            }

            BuildStatSection(character);
            BuildTurnSection(character);
            BuildStressStateSection(character);
            BuildPerksSection(character);

            // Position and resize
            TransformUtils.RebuildLayouts(layouts);
            if (character.currentTile != null)
            {
                cachedTile = character.currentTile;
            }
            positionParent.position = cachedTile.WorldPosition;
        }
        public void HideModal()
        {
            queuedShow = null;
            rootCanvas.enabled = false;
            mainCg.DOKill();
            mainCg.alpha = 0f;
        }

        private void BuildStatSection(HexCharacterModel character)
        {
            // Health bar
            float currentHealthFloat = character.currentHealth;
            float currentMaxHealthFloat = StatCalculator.GetTotalMaxHealth(character);
            float healthBarFloat = currentHealthFloat / currentMaxHealthFloat;
            healthBar.value = healthBarFloat;
            healthText.text = character.currentHealth + " / " + currentMaxHealthFloat;

            // Armour bar
            float currentArmourFloat = character.currentArmour;
            float currentMaxArmourFloat = character.startingArmour;
            float armourBarFloat = 0;
            if (character.currentArmour <= character.startingArmour &&
                character.currentArmour > 0 &&
                character.startingArmour > 0)
            {
                armourBarFloat = currentArmourFloat / currentMaxArmourFloat;
            }
            armourBar.value = armourBarFloat;
            armourText.text = character.currentArmour + " / " + character.startingArmour;

            // Stress bar
           // float currentStressFloat = character.currentMoraleState;
           // float currentMaxStressFloat = 20f;
           // float stressBarFloat = currentStressFloat / currentMaxStressFloat;
           // stressBar.value = stressBarFloat;
            //stressText.text = character.currentMoraleState + " / 20";
        }

        private void BuildTurnSection(HexCharacterModel character)
        {
            int turnsUntilMyTurn = TurnController.Instance.GetCharacterTurnsUntilTheirTurn(character);
            if (turnsUntilMyTurn == 0)
            {
                turnOrderText.text = "Acting now.";
            }
            else if (turnsUntilMyTurn < 0)
            {
                turnOrderText.text = "Turn done.";
            }
            else
            {
                turnOrderText.text = "Acts in " + turnsUntilMyTurn + " turns.";
            }
        }
        private void BuildStressStateSection(HexCharacterModel character)
        {
            if (character.characterData != null && (character.characterData.ignoreStress || PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Fearless)))
            {
                moraleStateParent.SetActive(false);
                return;
            }

            moraleStateParent.SetActive(true);
            MoraleState stressState = character.currentMoraleState;
            Sprite stressSprite = SpriteLibrary.Instance.GetMoraleStateSprite(stressState);
            moraleStateImage.sprite = stressSprite;
            moraleStateText.text = TextLogic.SplitByCapitals(stressState.ToString());
        }
        private void BuildPerksSection(HexCharacterModel character)
        {
            List<ActivePerk> perks = character.pManager.perks.FindAll(x => x.Data.hiddenOnPassivePanel == false);
            if (perks.Count == 0)
            {
                perksParent.SetActive(false);
                return;
            }

            perksParent.SetActive(true);
            perkRows.ForEach(x => x.Hide());
            for (int i = 0; i < perks.Count; i++)
            {
                if (i >= perkRows.Count)
                {
                    EnemyInfoModalStatRow newRow = Instantiate(perkRowPrefab, perkRowPrefab.transform.parent);
                    perkRows.Add(newRow);
                }

                perkRows[i].BuildAndShow(perks[i]);
            }
        }

        #endregion
    }

    public class QueuedShow
    {
    }
}
