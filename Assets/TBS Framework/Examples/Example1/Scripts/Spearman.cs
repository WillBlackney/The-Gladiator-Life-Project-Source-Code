using TbsFramework.Units;

namespace TbsFramework.Example1
{
    public class Spearman : MyUnit
    {
        protected override int Defend(Unit other, int damage)
        {
            var realDamage = damage;
            if (other is Archer)
                realDamage *= 2;//Archer deals double damage to spearman.

            return realDamage - DefenceFactor;
        }
    }
}
