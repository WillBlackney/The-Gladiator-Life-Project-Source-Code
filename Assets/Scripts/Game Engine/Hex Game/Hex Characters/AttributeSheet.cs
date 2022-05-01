using Sirenix.OdinInspector;
using UnityEngine;


namespace HexGameEngine.Characters
{
    public class AttributeSheet
    {
        // Core Attributes
        [BoxGroup("Core Attributes", centerLabel: true)]
        [LabelWidth(100)]
        [GUIColor("Green")]
        public Attribute strength = new Attribute(0, 0);
        [BoxGroup("Core Attributes")]
        [LabelWidth(100)]
        [GUIColor("Green")]
        public Attribute intelligence = new Attribute(0, 0);
        [BoxGroup("Core Attributes")]
        [LabelWidth(100)]
        [GUIColor("Green")]
        public Attribute constitution = new Attribute(80, 0);
        [BoxGroup("Core Attributes")]
        [LabelWidth(100)]
        [GUIColor("Green")]
        public Attribute dodge = new Attribute(0, 0);
        [BoxGroup("Core Attributes")]
        [LabelWidth(100)]
        [GUIColor("Green")]
        public Attribute accuracy = new Attribute(75, 0);
        [BoxGroup("Core Attributes")]
        [LabelWidth(100)]
        [GUIColor("Green")]
        public Attribute resolve = new Attribute(0, 0);
        [BoxGroup("Core Attributes")]
        [LabelWidth(100)]
        [GUIColor("Green")]
        public Attribute wits = new Attribute(0, 0);


        [BoxGroup("Secondary Attributes", centerLabel: true)]
        [LabelWidth(100)]
        [GUIColor("Blue")]
        public int maxHealth = 100;
        [BoxGroup("Secondary Attributes")]
        [LabelWidth(100)]
        [GUIColor("Blue")]
        public int energyRecovery = 8;
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
        [BoxGroup("Misc Attributes")]
        [LabelWidth(100)]
        [GUIColor("Blue")]
        public int vision = 0;

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
        [BoxGroup("Resistances")]
        [LabelWidth(150)]
        [Range(0, 100)]
        [GUIColor("Yellow")]
        public int deathResistance = 25;

        public void CopyValuesIntoOther(AttributeSheet other)
        {
            other.strength.value = strength.value;
            other.strength.stars = strength.stars;
            other.intelligence = intelligence;
            other.intelligence.stars = intelligence.stars;
            other.constitution = constitution;
            other.constitution.stars = constitution.stars;
            other.accuracy = accuracy;
            other.accuracy.stars = accuracy.stars;
            other.dodge = dodge;
            other.dodge.stars = dodge.stars;
            other.resolve = resolve;
            other.resolve.stars = resolve.stars;
            other.wits = wits;
            other.wits.stars = wits.stars;

            other.maxHealth = maxHealth;
            other.energyRecovery = energyRecovery;
            other.maxEnergy = maxEnergy;
            other.initiative = initiative;
            other.criticalChance = criticalChance;
            other.criticalModifier = criticalModifier;
            other.auraSize = auraSize;
            other.vision = vision;

            other.physicalResistance = physicalResistance;
            other.magicResistance = magicResistance;
            other.stressResistance = stressResistance;
            other.debuffResistance = debuffResistance;
            other.injuryResistance = injuryResistance;
            other.deathResistance = deathResistance;
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
    [System.Serializable]
    public class Attribute
    {
        public int value;
        [Range(0,2)]
        public int stars;

        public Attribute(int _value, int _stars)
        {
            value = _value;
            stars = _stars;
        }
    }
}