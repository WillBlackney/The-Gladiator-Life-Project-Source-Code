using UnityEngine;
using System;
using System.Collections.Generic;
using TbsFramework.Pathfinding.DataStructs;
using TbsFramework.Units;

namespace TbsFramework.Cells
{
    /// <summary>
    /// Class representing a single field (cell) on the grid.
    /// </summary>
    public abstract class Cell : MonoBehaviour, IGraphNode, IEquatable<Cell>
    {
        [HideInInspector]
        [SerializeField]
        protected Vector2 _offsetCoord;
        /// <summary>
        /// Position of the cell on the grid.
        /// </summary>
        public Vector2 OffsetCoord { get { return _offsetCoord; } set { _offsetCoord = value; } }

        /// <summary>
        /// Indicates if something is occupying the cell.
        /// </summary>
        public bool IsTaken;
        /// <summary>
        /// Cost of moving through the cell.
        /// </summary>
        public float MovementCost = 1;

        public Unit CurrentUnit { get; set; }

        /// <summary>
        /// CellClicked event is invoked when user clicks on the cell. 
        /// It requires a collider on the cell game object to work.
        /// </summary>
        public event EventHandler CellClicked;
        /// <summary>
        /// CellHighlighed event is invoked when cursor enters the cell's collider. 
        /// It requires a collider on the cell game object to work.
        /// </summary>
        public event EventHandler CellHighlighted;
        /// <summary>
        /// CellDehighlighted event is invoked when cursor exits the cell's collider. 
        /// It requires a collider on the cell game object to work.
        /// </summary>
        public event EventHandler CellDehighlighted;

        protected virtual void OnMouseEnter()
        {
            if (CellHighlighted != null)
                CellHighlighted.Invoke(this, new EventArgs());
        }
        protected virtual void OnMouseExit()
        {
            if (CellDehighlighted != null)
                CellDehighlighted.Invoke(this, new EventArgs());
        }
        protected virtual void OnMouseDown()
        {
            if (CellClicked != null)
                CellClicked.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Method returns distance to a cell that is given as parameter. 
        /// </summary>
        public abstract int GetDistance(Cell other);

        /// <summary>
        /// Method returns cells adjacent to current cell, from list of cells given as parameter.
        /// </summary>
        public abstract List<Cell> GetNeighbours(List<Cell> cells);
        /// <summary>
        /// Method returns cell's physical dimensions It is used in grid generators.
        /// </summary>
        public abstract Vector3 GetCellDimensions();

        /// <summary>
        ///  Method marks the cell to give user an indication that selected unit can reach it.
        /// </summary>
        public abstract void MarkAsReachable();
        /// <summary>
        /// Method marks the cell as a part of a path.
        /// </summary>
        public abstract void MarkAsPath();
        /// <summary>
        /// Method marks the cell as highlighted. It gets called when the mouse is over the cell.
        /// </summary>
        public abstract void MarkAsHighlighted();
        /// <summary>
        /// Method returns the cell to its base appearance.
        /// </summary>
        public abstract void UnMark();

        public int GetDistance(IGraphNode other)
        {
            return GetDistance(other as Cell);
        }

        public virtual bool Equals(Cell other)
        {
            return (OffsetCoord.x == other.OffsetCoord.x && OffsetCoord.y == other.OffsetCoord.y);
        }

        public override bool Equals(object other)
        {
            if (!(other is Cell))
                return false;

            return Equals(other as Cell);
        }

        public override int GetHashCode()
        {
            int hash = 23;

            hash = (hash * 37) + (int)OffsetCoord.x;
            hash = (hash * 37) + (int)OffsetCoord.y;
            return hash;
        }

        /// <summary>
        /// Method for cloning field values into a new cell. Used in Tile Painter in Grid Helper
        /// </summary>
        /// <param name="newCell">Cell to copy field values to</param>
        public abstract void CopyFields(Cell newCell);
    }
}