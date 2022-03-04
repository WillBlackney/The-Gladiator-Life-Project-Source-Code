namespace TbsFramework.Pathfinding.DataStructs
{
    public interface IGraphNode
    {
        /// <summary>
        /// Method returns distance to a IGraphNode that is given as parameter. 
        /// </summary>
        int GetDistance(IGraphNode other);
    }
}

