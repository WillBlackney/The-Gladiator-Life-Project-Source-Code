using TbsFramework.Units;

namespace TbsFramework.Example4
{
    public class AdvWrsWizard : AdvWrsUnit
    {
        public int RangedAttackFactor;

        protected override AttackAction DealDamage(Unit other)
        {
            int distance = Cell.GetDistance(other.Cell);
            if(distance == 1)
            {
                return new AttackAction(AttackFactor, 1);
            }
            else
            {
                return new AttackAction(RangedAttackFactor, 1);
            }
        }
    }
}

