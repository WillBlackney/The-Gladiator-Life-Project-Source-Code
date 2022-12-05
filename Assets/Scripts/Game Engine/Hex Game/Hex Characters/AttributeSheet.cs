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
        public Attribute might = new Attribute(0, 0);
        [BoxGroup("Core Attributes")]
        [LabelWidth(100)]
        [GUIColor("Green")]
        public Attribute constitution = new Attribute(75, 0);
        [BoxGroup("Core Attributes")]
        [LabelWidth(100)]
        [GUIColor("Green")]
        public Attribute dodge = new Attribute(0, 0);
        [BoxGroup("Core Attributes")]
        [LabelWidth(100)]
        [GUIColor("Green")]
        public Attribute accuracy = new Attribute(60, 0);
        [BoxGroup("Core Attributes")]
        [LabelWidth(100)]
        [GUIColor("Green")]
        public Attribute resolve = new Attribute(5, 0);
        [BoxGroup("Core Attributes")]
        [LabelWidth(100)]
        [GUIColor("Green")]
        public Attribute fitness = new Attribute(60, 0);
        [BoxGroup("Core Attributes")]
        [LabelWidth(100)]
        [GUIColor("Green")]
        public Attribute wits = new Attribute(5, 0);



        [BoxGroup("Secondary Attributes", centerLabel: true)]
        [LabelWidth(100)]
        [GUIColor("Blue")]
        public int maxHealth = 0;
        [BoxGroup("Secondary Attributes")]
        [LabelWidth(100)]
        [GUIColor("Blue")]
        public int apRecovery = 8;
        [BoxGroup("Secondary Attributes")]
        [LabelWidth(100)]
        [GUIColor("Blue")]
        public int maxAp = 8;
        [BoxGroup("Secondary Attributes")]
        [LabelWidth(100)]
        [GUIColor("Blue")]
        public int initiative = 5;
        [BoxGroup("Secondary Attributes")]
        [LabelWidth(100)]
        [GUIColor("Blue")]
        public int fatigueRecovery = 8;

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
        public int physicalDamageBonus = 0;
        [BoxGroup("Misc Attributes")]
        [LabelWidth(100)]
        [GUIColor("Blue")]
        public int magicDamageBonus = 0;
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
            if (other == null)
            {
                Debug.LogWarning("AttributeSheet.CopyValuesIntoOther() other is null, creating a new attribute sheet...");
                other = new AttributeSheet();
            }

            other.might.value = might.value;
            other.might.stars = might.stars;
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
            other.fitness = fitness;
            other.fitness.stars = fitness.stars;

            other.fatigueRecovery = fatigueRecovery;          
            other.maxHealth = maxHealth;
            other.apRecovery = apRecovery;
            other.maxAp = maxAp;
            other.initiative = initiative;
            other.criticalChance = criticalChance;
            other.criticalModifier = criticalModifier;
            other.auraSize = auraSize;
            other.vision = vision;
            other.physicalDamageBonus = physicalDamageBonus;
            other.magicDamageBonus = magicDamageBonus;

            other.physicalResistance = physicalResistance;
            other.magicResistance = magicResistance;
            other.stressResistance = stressResistance;
            other.debuffResistance = debuffResistance;
            other.injuryResistance = injuryResistance;
            other.deathResistance = deathResistance;
        }

        public void LogCoreStats()
        {
            Debug.Log("might: " + might.value +", " +
                "constitution: " + constitution.value +", " +
                "accuracy: " + accuracy.value +", " +
                "dodge: " + dodge.value +", " +
                "wits: " + wits.value +", " +
                "fatigue: " + fitness.value + ", " +
                "resolve: " + resolve.value);
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
        [Range(0,3)]
        public int stars;

        public Attribute(int _value, int _stars)
        {
            value = _value;
            stars = _stars;
        }
    }
}