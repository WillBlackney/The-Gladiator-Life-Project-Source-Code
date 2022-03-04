using System;
using TbsFramework.Cells;
using TbsFramework.Grid;
using TbsFramework.Players;
using TbsFramework.Units;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TbsFramework.Example4
{
    public class GuiController : MonoBehaviour
    {
        public CellGrid CellGrid;

        public Text UnitNameText;
        public Text HitPointsText;
        public Text DefenceText;
        public Text MeeleAttackText;
        public Text RangedAttackText;

        public Text TerrainNameText;
        public Text MovementCostText;
        public Text DefenceBoostText;

        public GameObject UnitPanel;
        public GameObject TerrainPanel;

        public GameObject GameOverPanel;
        public Text GameOverText;

        public Button NextTurnButton;

        void Awake()
        {
            UnitPanel.SetActive(false);
            TerrainPanel.SetActive(false);
            GameOverPanel.SetActive(false);

            CellGrid.GameStarted += OnGameStarted;
            CellGrid.TurnEnded += OnTurnEnded;
            CellGrid.GameEnded += OnGameEnded;
            CellGrid.UnitAdded += OnUnitAdded;
        }

        private void OnGameStarted(object sender, EventArgs e)
        {
            foreach (Transform cell in CellGrid.transform)
            {
                cell.GetComponent<Cell>().CellHighlighted += OnCellHighlighted;
                cell.GetComponent<Cell>().CellDehighlighted += OnCellDehighlighted;
            }

            OnTurnEnded(sender, e);
        }

        private void OnGameEnded(object sender, EventArgs e)
        {
            GameOverText.text = string.Format("Player {0} wins", (sender as CellGrid).CurrentPlayerNumber + 1);
            GameOverPanel.SetActive(true);
        }
        private void OnTurnEnded(object sender, EventArgs e)
        {
            NextTurnButton.interactable = ((sender as CellGrid).CurrentPlayer is HumanPlayer);
        }
        private void OnCellDehighlighted(object sender, EventArgs e)
        {
            TerrainPanel.SetActive(false);
        }

        private void OnCellHighlighted(object sender, EventArgs e)
        {
            var cell = sender as AdvWrsSquare;
            TerrainNameText.text = cell.TileType;
            MovementCostText.text = string.Format("Mov cost: {0}", cell.MovementCost);
            DefenceBoostText.text = string.Format("Def boost: {0}", cell.DefenceBoost);

            TerrainPanel.SetActive(true);
        }

        private void OnUnitAttacked(object sender, AttackEventArgs e)
        {
            if (!(CellGrid.CurrentPlayer is HumanPlayer)) return;
            OnUnitDehighlighted(sender, new EventArgs());

            if ((sender as Unit).HitPoints <= 0) return;

            OnUnitHighlighted(sender, e);
        }

        private void OnUnitDehighlighted(object sender, EventArgs e)
        {
            UnitPanel.SetActive(false);
        }

        private void OnUnitHighlighted(object sender, EventArgs e)
        {
            UnitPanel.SetActive(true);

            var unit = sender as AdvWrsUnit;
            UnitNameText.text = unit.UnitName;
            HitPointsText.text = string.Format("HP: {0}/{1}", unit.HitPoints, unit.TotalHitPoints);

            string defenceText = "";
            if((unit.Cell as AdvWrsSquare).DefenceBoost != 0)
            {
                defenceText = string.Format("Def: {0} + {1}", unit.DefenceFactor, (unit.Cell as AdvWrsSquare).DefenceBoost);
            }
            else
            {
                defenceText = string.Format("Def: {0}", unit.DefenceFactor);
            }
            DefenceText.text = defenceText;
            MeeleAttackText.text = string.Format("Meele: {0}", unit.AttackFactor);
            if(unit is AdvWrsWizard)
            {
                RangedAttackText.text = string.Format("Ranged: {0}", (unit as AdvWrsWizard).RangedAttackFactor);
            }
            else
            {
                RangedAttackText.text = "";
            }

            OnCellHighlighted(unit.Cell, null);
        }

        private void OnUnitAdded(object sender, UnitCreatedEventArgs e)
        {
            RegisterUnit(e.unit);
        }

        private void RegisterUnit(Transform unit)
        {
            unit.GetComponent<Unit>().UnitHighlighted += OnUnitHighlighted;
            unit.GetComponent<Unit>().UnitDehighlighted += OnUnitDehighlighted;
            unit.GetComponent<Unit>().UnitAttacked += OnUnitAttacked;
        }

        public void RestartLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}