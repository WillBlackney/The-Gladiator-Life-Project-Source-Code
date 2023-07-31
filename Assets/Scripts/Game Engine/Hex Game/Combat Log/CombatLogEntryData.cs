using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeAreGladiators.CombatLog
{
    public class CombatLogEntryData
    {
        public Sprite Icon {  get; private set; }
        public string Description { get; private set; }

        public CombatLogEntryData(Sprite icon, string description)
        {
            Icon = icon;
            Description = description;
        }
    }
}