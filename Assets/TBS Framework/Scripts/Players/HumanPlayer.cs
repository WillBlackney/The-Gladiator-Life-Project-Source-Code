using TbsFramework.Grid;
using TbsFramework.Grid.GridStates;

namespace TbsFramework.Players
{
    /// <summary>
    /// Class representing a human player.
    /// </summary>
    public class HumanPlayer : Player
    {
        public override void Play(CellGrid cellGrid)
        {
            cellGrid.CellGridState = new CellGridStateWaitingForInput(cellGrid);
        }
    }
}
