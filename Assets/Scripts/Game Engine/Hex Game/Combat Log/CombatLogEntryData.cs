using UnityEngine;

namespace WeAreGladiators.CombatLog
{
    public class CombatLogEntryData
    {

        public CombatLogEntryData(Sprite icon, string description)
        {
            Icon = icon;
            Description = description;
        }
        public Sprite Icon { get; private set; }
        public string Description { get; private set; }
    }
}
