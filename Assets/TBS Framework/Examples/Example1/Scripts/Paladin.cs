using TbsFramework.Units;

namespace TbsFramework.Example1
{
    public class Paladin : MyUnit
    {
        protected override int Defend(Unit other, int damage)
        {
            var realDamage = damage;
            if (other is Spearman)
                realDamage *= 2;//Spearman deals double damage to paladin.

            return realDamage - DefenceFactor;
        }
    }
}
