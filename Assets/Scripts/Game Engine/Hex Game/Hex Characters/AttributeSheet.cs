using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HexGameEngine.Characters
{
    public class AttributeSheet
    {
        // Core Attributes
        [BoxGroup("Core Attributes", centerLabel: true)]
        [LabelWidth(100)]
        [GUIColor("Green")]
        public int strength = 0;
        [BoxGroup("Core Attributes")]
        [LabelWidth(100)]
        [GUIColor("Green")]
        public int intelligence = 0;
        [BoxGroup("Core Attributes")]
        [LabelWidth(100)]
        [GUIColor("Green")]
        public int constitution = 0;
        [BoxGroup("Core Attributes")]
        [LabelWidth(100)]
        [GUIColor("Green")]
        public int dodge = 0;
        [BoxGroup("Core Attributes")]
        [LabelWidth(100)]
        [GUIColor("Green")]
        public int accuracy = 75;
        [BoxGroup("Core Attributes")]
        [LabelWidth(100)]
        [GUIColor("Green")]
        public int resolve = 0;
        [BoxGroup("Core Attributes")]
        [LabelWidth(100)]
        [GUIColor("Green")]
        public int wits = 0;


        [BoxGroup("Secondary Attributes", centerLabel: true)]
        [LabelWidth(100)]
        [GUIColor("Blue")]
        public int maxHealth = 100;
        [BoxGroup("Secondary Attributes")]
        [LabelWidth(100)]
        [GUIColor("Blue")]
        public int stamina = 8;
        [BoxGroup("Secondary Attributes")]
        [LabelWidth(100)]
        [GUIColor("Blue")]
        public int maxEnergy = 8;
        [BoxGroup("Secondary Attributes")]
        [LabelWidth(100)]
        [GUIColor("Blue")]
        public int initiative = 5;

        [BoxGroup("Misc Attributes", centerLabel: true)]
        [LabelWidth(100)]
        [GUIColor("Blue")]
        public float criticalChance = 5;
        [BoxGroup("Misc Attributes")]
        [LabelWidth(100)]
        [GUIColor("Blue")]
        public int criticalModifier = 50;
        [BoxGroup("Misc Attributes")]
        [LabelWidth(100)]
        [GUIColor("Blue")]
        public int auraSize = 1;

        [BoxGroup("Resistances", centerLabel: true)]
        [LabelWidth(150)]
        [GUIColor("Yellow")]
        [Range(-100, 100)]
        public int physicalResistance = 0;
        [BoxGroup("Resistances")]
        [LabelWidth(150)]
        [Range(-100, 100)]
        [GUIColor("Yellow")]
        public int magicResistance = 0;
        [BoxGroup("Resistances")]
        [LabelWidth(150)]
        [Range(0, 100)]
        [GUIColor("Yellow")]
        public int debuffResistance = 0;
        [BoxGroup("Resistances")]
        [LabelWidth(150)]
        [Range(0, 100)]
        [GUIColor("Yellow")]
        public int stressResistance = 0;
        [BoxGroup("Resistances")]
        [LabelWidth(150)]
        [Range(0, 100)]
        [GUIColor("Yellow")]
        public int injuryResistance = 0;

        // copy function!!

        public void CopyValuesIntoOther(AttributeSheet other)
        {
            other.strength = strength;
            other.intelligence = intelligence;
            other.constitution = constitution;
            other.accuracy = accuracy;
            other.dodge = dodge;
            other.resolve = resolve;
            other.wits = wits;

            other.maxHealth = maxHealth;
            other.stamina = stamina;
            other.maxEnergy = maxEnergy;
            other.initiative = initiative;
            other.criticalChance = criticalChance;
            other.criticalModifier = criticalModifier;
            other.auraSize = auraSize;

            other.physicalResistance = physicalResistance;
            other.magicResistance = magicResistance;
            other.stressResistance = stressResistance;
            other.debuffResistance = debuffResistance;
            other.injuryResistance = injuryResistance;
        }

        // GUI Colours for Odin
        #region
        private Color Blue() { return Color.cyan; }
        private Color Green() { return Color.green; }
        private Color Yellow() { return Color.yellow; }
        private Color Red() { return Color.red; }
        #endregion
    }

    [System.Serializable]
    public class SerializedAttrbuteSheet : AttributeSheet
    {

    }
}