using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using TbsFramework.Cells;
using TbsFramework.Grid.GridStates;
using TbsFramework.Grid.UnitGenerators;
using TbsFramework.Players;
using TbsFramework.Units;

namespace TbsFramework.Grid
{
    /// <summary>
    /// CellGrid class keeps track of the game, stores cells, units and players objects. It starts the game and makes turn transitions. 
    /// It reacts to user interacting with units or cells, and raises events related to game progress. 
    /// </summary>
    public class CellGrid : MonoBehaviour
    {
        /// <summary>
        /// LevelLoading event is invoked before Initialize method is run.
        /// </summary>
        public event EventHandler LevelLoading;
        /// <summary>
        /// LevelLoadingDone event is invoked after Initialize method has finished running.
        /// </summary>
        public event EventHandler LevelLoadingDone;
        /// <summary>
        /// GameStarted event is invoked at the beggining of StartGame method.
        /// </summary>
        public event EventHandler GameStarted;
        /// <summary>
        /// GameEnded event is invoked when there is a single player left in the game.
        /// </summary>
        public event EventHandler GameEnded;
        /// <summary>
        /// Turn ended event is invoked at the end of each turn.
        /// </summary>
        public event EventHandler TurnEnded;

        /// <summary>
        /// UnitAdded event is invoked each time AddUnit method is called.
        /// </summary>
        public event EventHandler<UnitCreatedEventArgs> UnitAdded;

        private CellGridState _cellGridState; //The grid delegates some of its behaviours to cellGridState object.
        public CellGridState CellGridState
        {
            get
            {
                return _cellGridState;
            }
            set
            {
                if (_cellGridState != null)
                    _cellGridState.OnStateExit();
                _cellGridState = value;
                _cellGridState.OnStateEnter();
            }
        }

        public int NumberOfPlayers { get; private set; }

        public Player CurrentPlayer
        {
            get { return Players.Find(p => p.PlayerNumber.Equals(CurrentPlayerNumber)); }
        }
        public int CurrentPlayerNumber { get; private set; }

        /// <summary>
        /// GameObject that holds player objects.
        /// </summary>
        public Transform PlayersParent;

        public bool GameFinished { get; private set; }
        public List<Player> Players { get; private set; }
        public List<Cell> Cells { get; private set; }
        public List<Unit> Units { get; private set; }

        private void Start()
        {
            if (LevelLoading != null)
                LevelLoading.Invoke(this, new EventArgs());

            Initialize();

            if (LevelLoadingDone != null)
                LevelLoadingDone.Invoke(this, new EventArgs());

            StartGame();
        }

        private void Initialize()
        {
            GameFinished = false;
            Players = new List<Player>();
            for (int i = 0; i < PlayersParent.childCount; i++)
            {
                var player = PlayersParent.GetChild(i).GetComponent<Player>();
                if (player != null)
                    Players.Add(player);
                else
                    Debug.LogError("Invalid object in Players Parent game object");
            }
            NumberOfPlayers = Players.Count;
            CurrentPlayerNumber = Players.Min(p => p.PlayerNumber);

            Cells = new List<Cell>();
            for (int i = 0; i < transform.childCount; i++)
            {
                var cell = transform.GetChild(i).gameObject.GetComponent<Cell>();
                if (cell != null)
                    Cells.Add(cell);
                else
                    Debug.LogError("Invalid object in cells paretn game object");
            }

            foreach (var cell in Cells)
            {
                cell.CellClicked += OnCellClicked;
                cell.CellHighlighted += OnCellHighlighted;
                cell.CellDehighlighted += OnCellDehighlighted;
                cell.GetComponent<Cell>().GetNeighbours(Cells);
            }

            Units = new List<Unit>();
            var unitGenerator = GetComponent<IUnitGenerator>();
            if (unitGenerator != null)
            {
                var units = unitGenerator.SpawnUnits(Cells);
                foreach (var unit in units)
                {
                    AddUnit(unit.GetComponent<Transform>());
                }
            }
            else
                Debug.LogError("No IUnitGenerator script attached to cell grid");
        }

        private void OnCellDehighlighted(object sender, EventArgs e)
        {
            CellGridState.OnCellDeselected(sender as Cell);
        }
        private void OnCellHighlighted(object sender, EventArgs e)
        {
            CellGridState.OnCellSelected(sender as Cell);
        }
        private void OnCellClicked(object sender, EventArgs e)
        {
            CellGridState.OnCellClicked(sender as Cell);
        }

        private void OnUnitClicked(object sender, EventArgs e)
        {
            CellGridState.OnUnitClicked(sender as Unit);
        }
        private void OnUnitDestroyed(object sender, AttackEventArgs e)
        {
            Units.Remove(sender as Unit);
            var totalPlayersAlive = Units.Select(u => u.PlayerNumber).Distinct().ToList(); //Checking if the game is over
            if (totalPlayersAlive.Count == 1)
            {
                CellGridState = new CellGridStateBlockInput(this);
                GameFinished = true;
                if (GameEnded != null)
                    GameEnded.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// Adds unit to the game
        /// </summary>
        /// <param name="unit">Unit to add</param>
        public void AddUnit(Transform unit)
        {
            Units.Add(unit.GetComponent<Unit>());
            unit.GetComponent<Unit>().UnitClicked += OnUnitClicked;
            unit.GetComponent<Unit>().UnitDestroyed += OnUnitDestroyed;

            if (UnitAdded != null)
                UnitAdded.Invoke(this, new UnitCreatedEventArgs(unit));
        }

        /// <summary>
        /// Method is called once, at the beggining of the game.
        /// </summary>
        public void StartGame()
        {
            if (GameStarted != null)
                GameStarted.Invoke(this, new EventArgs());

            Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).ForEach(u => { u.OnTurnStart(); });
            Players.Find(p => p.PlayerNumber.Equals(CurrentPlayerNumber)).Play(this);
            Debug.Log("Game started");
        }
        /// <summary>
        /// Method makes turn transitions. It is called by player at the end of his turn.
        /// </summary>
        public void EndTurn()
        {
            CellGridState = new CellGridStateBlockInput(this);
            if (Units.Select(u => u.PlayerNumber).Distinct().Count() == 1)
            {
                return;
            }

            Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).ForEach(u => { u.OnTurnEnd(); });

            CurrentPlayerNumber = (CurrentPlayerNumber + 1) % NumberOfPlayers;
            while (Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).Count == 0)
            {
                CurrentPlayerNumber = (CurrentPlayerNumber + 1) % NumberOfPlayers;
            }//Skipping players that are defeated.

            if (TurnEnded != null)
                TurnEnded.Invoke(this, new EventArgs());

            Debug.Log(string.Format("Player {0} turn", CurrentPlayerNumber));
            Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).ForEach(u => { u.OnTurnStart(); });
            Players.Find(p => p.PlayerNumber.Equals(CurrentPlayerNumber)).Play(this);
        }
    }
}

