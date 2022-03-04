using System.Collections.Generic;
using TbsFramework.Cells;
using TbsFramework.Units;

namespace TbsFramework.Grid.UnitGenerators
{
    public interface IUnitGenerator
    {
        List<Unit> SpawnUnits(List<Cell> cells);
    }
}
