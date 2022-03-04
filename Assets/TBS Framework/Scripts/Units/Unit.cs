using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using TbsFramework.Cells;
using TbsFramework.Pathfinding.Algorithms;
using TbsFramework.Units.UnitStates;

namespace TbsFramework.Units
{
    /// <summary>
    /// Base class for all units in the game.
    /// </summary>
    [ExecuteInEditMode]
    public abstract class Unit : MonoBehaviour
    {
        Dictionary<Cell, List<Cell>> cachedPaths = null;
        /// <summary>
        /// UnitClicked event is invoked when user clicks the unit. 
        /// It requires a collider on the unit game object to work.
        /// </summary>
        public event EventHandler UnitClicked;
        /// <summary>
        /// UnitSelected event is invoked when user clicks on unit that belongs to him. 
        /// It requires a collider on the unit game object to work.
        /// </summary>
        public event EventHandler UnitSelected;
        /// <summary>
        /// UnitDeselected event is invoked when user click outside of currently selected unit's collider.
        /// It requires a collider on the unit game object to work.
        /// </summary>
        public event EventHandler UnitDeselected;
        /// <summary>
        /// UnitHighlighted event is invoked when user moves cursor over the unit. 
        /// It requires a collider on the unit game object to work.
        /// </summary>
        public event EventHandler UnitHighlighted;
        /// <summary>
        /// UnitDehighlighted event is invoked when cursor exits unit's collider. 
        /// It requires a collider on the unit game object to work.
        /// </summary>
        public event EventHandler UnitDehighlighted;
        /// <summary>
        /// UnitAttacked event is invoked when the unit is attacked.
        /// </summary>
        public event EventHandler<AttackEventArgs> UnitAttacked;
        /// <summary>
        /// UnitDestroyed event is invoked when unit's hitpoints drop below 0.
        /// </summary>
        public event EventHandler<AttackEventArgs> UnitDestroyed;
        /// <summary>
        /// UnitMoved event is invoked when unit moves from one cell to another.
        /// </summary>
        public event EventHandler<MovementEventArgs> UnitMoved;

        public UnitState UnitState { get; set; }
        public void SetState(UnitState state)
        {
            UnitState.MakeTransition(state);
        }

        /// <summary>
        /// A list of buffs that are applied to the unit.
        /// </summary>
        public List<Buff> Buffs { get; private set; }

        public int TotalHitPoints { get; private set; }
        public float TotalMovementPoints { get; private set; }
        public float TotalActionPoints { get; private set; }

        /// <summary>
        /// Cell that the unit is currently occupying.
        /// </summary>
        [SerializeField]
        [HideInInspector]
        private Cell cell;
        public Cell Cell
        {
            get
            {
                return cell;
            }
            set
            {
                cell = value;
            }
        }

        public int HitPoints;
        public int AttackRange;
        public int AttackFactor;
        public int DefenceFactor;
        /// <summary>
        /// Determines how far on the grid the unit can move.
        /// </summary>
        [SerializeField]
        private float movementPoints;
        public virtual float MovementPoints
        {
            get
            {
                return movementPoints;
            }
            protected set
            {
                movementPoints = value;
            }
        }
        /// <summary>
        /// Determines speed of movement animation.
        /// </summary>
        public float MovementAnimationSpeed;
        /// <summary>
        /// Determines how many attacks unit can perform in one turn.
        /// </summary>
        [SerializeField]
        private float actionPoints = 1;
        public float ActionPoints
        {
            get
            {
                return actionPoints;
            }
            protected set
            {
                actionPoints = value;
            }
        }

        /// <summary>
        /// Indicates the player that the unit belongs to. 
        /// Should correspoond with PlayerNumber variable on Player script.
        /// </summary>
        public int PlayerNumber;

        /// <summary>
        /// Indicates if movement animation is playing.
        /// </summary>
        public bool IsMoving { get; set; }

        private static DijkstraPathfinding _pathfinder = new DijkstraPathfinding();
        private static IPathfinding _fallbackPathfinder = new AStarPathfinding();

        /// <summary>
        /// Method called after object instantiation to initialize fields etc. 
        /// </summary>
        public virtual void Initialize()
        {
            Buffs = new List<Buff>();

            UnitState = new UnitStateNormal(this);

            TotalHitPoints = HitPoints;
            TotalMovementPoints = MovementPoints;
            TotalActionPoints = ActionPoints;
        }

        protected virtual void OnMouseDown()
        {
            if (UnitClicked != null)
            {
                UnitClicked.Invoke(this, new EventArgs());
            }
        }
        protected virtual void OnMouseEnter()
        {
            if (UnitHighlighted != null)
            {
                UnitHighlighted.Invoke(this, new EventArgs());
            }
        }
        protected virtual void OnMouseExit()
        {
            if (UnitDehighlighted != null)
            {
                UnitDehighlighted.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// Method is called at the start of each turn.
        /// </summary>
        public virtual void OnTurnStart()
        {
            MovementPoints = TotalMovementPoints;
            ActionPoints = TotalActionPoints;

            SetState(new UnitStateMarkedAsFriendly(this));
        }
        /// <summary>
        /// Method is called at the end of each turn.
        /// </summary>
        public virtual void OnTurnEnd()
        {
            cachedPaths = null;
            Buffs.FindAll(b => b.Duration == 0).ForEach(b => { b.Undo(this); });
            Buffs.RemoveAll(b => b.Duration == 0);
            Buffs.ForEach(b => { b.Duration--; });

            SetState(new UnitStateNormal(this));
        }
        /// <summary>
        /// Method is called when units HP drops below 1.
        /// </summary>
        protected virtual void OnDestroyed()
        {
            Cell.IsTaken = false;
            MarkAsDestroyed();
            Destroy(gameObject);
        }

        /// <summary>
        /// Method is called when unit is selected.
        /// </summary>
        public virtual void OnUnitSelected()
        {
            SetState(new UnitStateMarkedAsSelected(this));
            if (UnitSelected != null)
            {
                UnitSelected.Invoke(this, new EventArgs());
            }
        }
        /// <summary>
        /// Method is called when unit is deselected.
        /// </summary>
        public virtual void OnUnitDeselected()
        {
            SetState(new UnitStateMarkedAsFriendly(this));
            if (UnitDeselected != null)
            {
                UnitDeselected.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// Method indicates if it is possible to attack a unit from given cell.
        /// </summary>
        /// <param name="other">Unit to attack</param>
        /// <param name="sourceCell">Cell to perform an attack from</param>
        /// <returns>Boolean value whether unit can be attacked or not</returns>
        public virtual bool IsUnitAttackable(Unit other, Cell sourceCell)
        {
            return sourceCell.GetDistance(other.Cell) <= AttackRange
                && other.PlayerNumber != PlayerNumber
                && ActionPoints >= 1;
        }

        /// <summary>
        /// Method performs an attack on given unit.
        /// </summary>
        public void AttackHandler(Unit unitToAttack)
        {
            if (!IsUnitAttackable(unitToAttack, Cell))
            {
                return;
            }

            AttackAction attackAction = DealDamage(unitToAttack);
            MarkAsAttacking(unitToAttack);
            unitToAttack.DefendHandler(this, attackAction.Damage);
            AttackActionPerformed(attackAction.ActionCost);
        }
        /// <summary>
        /// Method for calculating damage and action points cost of attacking given unit
        /// </summary>
        /// <returns></returns>
        protected virtual AttackAction DealDamage(Unit unitToAttack)
        {
            return new AttackAction(AttackFactor, 1f);
        }
        /// <summary>
        /// Method called after unit performed an attack.
        /// </summary>
        /// <param name="actionCost">Action point cost of performed attack</param>
        protected virtual void AttackActionPerformed(float actionCost)
        {
            ActionPoints -= actionCost;
            if (ActionPoints == 0)
            {
                MovementPoints = 0;
                SetState(new UnitStateMarkedAsFinished(this));
            }
        }

        /// <summary>
        /// Handler method for defending against an attack.
        /// </summary>
        /// <param name="aggressor">Unit that performed the attack</param>
        /// <param name="damage">Amount of damge that the attack caused</param>
        public void DefendHandler(Unit aggressor, int damage)
        {
            MarkAsDefending(aggressor);
            int damageTaken = Defend(aggressor, damage);
            HitPoints -= damageTaken;
            DefenceActionPerformed();

            if (UnitAttacked != null)
            {
                UnitAttacked.Invoke(this, new AttackEventArgs(aggressor, this, damage));
            }
            if (HitPoints <= 0)
            {
                if (UnitDestroyed != null)
                {
                    UnitDestroyed.Invoke(this, new AttackEventArgs(aggressor, this, damage));
                }
                OnDestroyed();
            }
        }
        /// <summary>
        /// Method for calculating actual damage taken by the unit.
        /// </summary>
        /// <param name="aggressor">Unit that performed the attack</param>
        /// <param name="damage">Amount of damge that the attack caused</param>
        /// <returns>Amount of damage that the unit has taken</returns>        
        protected virtual int Defend(Unit aggressor, int damage)
        {
            return Mathf.Clamp(damage - DefenceFactor, 1, damage);
        }
        /// <summary>
        /// Method callef after unit performed defence.
        /// </summary>
        protected virtual void DefenceActionPerformed() { }

        /// <summary>
        /// Handler method for moving the unit.
        /// </summary>
        /// <param name="destinationCell">Cell to move the unit to</param>
        /// <param name="path">A list of cells, path from source to destination cell</param>
        public virtual void Move(Cell destinationCell, List<Cell> path)
        {
            var totalMovementCost = path.Sum(h => h.MovementCost);
            MovementPoints -= totalMovementCost;

            Cell.IsTaken = false;
            Cell.CurrentUnit = null;
            Cell = destinationCell;
            destinationCell.IsTaken = true;
            destinationCell.CurrentUnit = this;

            if (MovementAnimationSpeed > 0)
            {
                StartCoroutine(MovementAnimation(path));
            }
            else
            {
                transform.position = Cell.transform.position;
            }

            if (UnitMoved != null)
            {
                UnitMoved.Invoke(this, new MovementEventArgs(Cell, destinationCell, path));
            }
        }
        protected virtual IEnumerator MovementAnimation(List<Cell> path)
        {
            IsMoving = true;
            path.Reverse();
            foreach (var cell in path)
            {
                Vector3 destination_pos = new Vector3(cell.transform.localPosition.x, cell.transform.localPosition.y, transform.localPosition.z);
                while (transform.localPosition != destination_pos)
                {
                    transform.localPosition = Vector3.MoveTowards(transform.localPosition, destination_pos, Time.deltaTime * MovementAnimationSpeed);
                    yield return 0;
                }
            }
            IsMoving = false;
            OnMoveFinished();
        }
        /// <summary>
        /// Method called after movement animation has finished.
        /// </summary>
        protected virtual void OnMoveFinished() { }

        ///<summary>
        /// Method indicates if unit is capable of moving to cell given as parameter.
        /// </summary>
        public virtual bool IsCellMovableTo(Cell cell)
        {
            return !cell.IsTaken;
        }
        /// <summary>
        /// Method indicates if unit is capable of moving through cell given as parameter.
        /// </summary>
        public virtual bool IsCellTraversable(Cell cell)
        {
            return !cell.IsTaken;
        }
        /// <summary>
        /// Method returns all cells that the unit is capable of moving to.
        /// </summary>
        public HashSet<Cell> GetAvailableDestinations(List<Cell> cells)
        {
            cachedPaths = new Dictionary<Cell, List<Cell>>();

            var paths = CachePaths(cells);
            foreach (var key in paths.Keys)
            {
                if (!IsCellMovableTo(key))
                {
                    continue;
                }
                var path = paths[key];

                var pathCost = path.Sum(c => c.MovementCost);
                if (pathCost <= MovementPoints)
                {
                    cachedPaths.Add(key, path);
                }
            }
            return new HashSet<Cell>(cachedPaths.Keys);
        }

        private Dictionary<Cell, List<Cell>> CachePaths(List<Cell> cells)
        {
            var edges = GetGraphEdges(cells);
            var paths = _pathfinder.findAllPaths(edges, Cell);
            return paths;
        }

        public List<Cell> FindPath(List<Cell> cells, Cell destination)
        {
            if (cachedPaths != null && cachedPaths.ContainsKey(destination))
            {
                return cachedPaths[destination];
            }
            else
            {
                return _fallbackPathfinder.FindPath(GetGraphEdges(cells), Cell, destination);
            }
        }
        /// <summary>
        /// Method returns graph representation of cell grid for pathfinding.
        /// </summary>
        protected virtual Dictionary<Cell, Dictionary<Cell, float>> GetGraphEdges(List<Cell> cells)
        {
            Dictionary<Cell, Dictionary<Cell, float>> ret = new Dictionary<Cell, Dictionary<Cell, float>>();
            foreach (var cell in cells)
            {
                if (IsCellTraversable(cell) || cell.Equals(Cell))
                {
                    ret[cell] = new Dictionary<Cell, float>();
                    foreach (var neighbour in cell.GetNeighbours(cells).FindAll(IsCellTraversable))
                    {
                        ret[cell][neighbour] = neighbour.MovementCost;
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// Gives visual indication that the unit is under attack.
        /// </summary>
        /// <param name="aggressor">
        /// Unit that is attacking.
        /// </param>
        public abstract void MarkAsDefending(Unit aggressor);
        /// <summary>
        /// Gives visual indication that the unit is attacking.
        /// </summary>
        /// <param name="target">
        /// Unit that is under attack.
        /// </param>
        public abstract void MarkAsAttacking(Unit target);
        /// <summary>
        /// Gives visual indication that the unit is destroyed. It gets called right before the unit game object is
        /// destroyed.
        /// </summary>
        public abstract void MarkAsDestroyed();

        /// <summary>
        /// Method marks unit as current players unit.
        /// </summary>
        public abstract void MarkAsFriendly();
        /// <summary>
        /// Method mark units to indicate user that the unit is in range and can be attacked.
        /// </summary>
        public abstract void MarkAsReachableEnemy();
        /// <summary>
        /// Method marks unit as currently selected, to distinguish it from other units.
        /// </summary>
        public abstract void MarkAsSelected();
        /// <summary>
        /// Method marks unit to indicate user that he can't do anything more with it this turn.
        /// </summary>
        public abstract void MarkAsFinished();
        /// <summary>
        /// Method returns the unit to its base appearance
        /// </summary>
        public abstract void UnMark();

        [ExecuteInEditMode]
        public void OnDestroy()
        {
            if (Cell != null)
            {
                Cell.IsTaken = false;
            }
        }
    }

    public class AttackAction
    {
        public readonly int Damage;
        public readonly float ActionCost;

        public AttackAction(int damage, float actionCost)
        {
            Damage = damage;
            ActionCost = actionCost;
        }
    }

    public class MovementEventArgs : EventArgs
    {
        public Cell OriginCell;
        public Cell DestinationCell;
        public List<Cell> Path;

        public MovementEventArgs(Cell sourceCell, Cell destinationCell, List<Cell> path)
        {
            OriginCell = sourceCell;
            DestinationCell = destinationCell;
            Path = path;
        }
    }
    public class AttackEventArgs : EventArgs
    {
        public Unit Attacker;
        public Unit Defender;

        public int Damage;

        public AttackEventArgs(Unit attacker, Unit defender, int damage)
        {
            Attacker = attacker;
            Defender = defender;

            Damage = damage;
        }
    }
    public class UnitCreatedEventArgs : EventArgs
    {
        public Transform unit;

        public UnitCreatedEventArgs(Transform unit)
        {
            this.unit = unit;
        }
    }
}
