using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using WeAreGladiators.Abilities;
using WeAreGladiators.Items;
using WeAreGladiators.Perks;

namespace WeAreGladiators.Characters
{
    [CreateAssetMenu(fileName = "New Starting Character Template", menuName = "Starting Character Template", order = 52)]
    public class HexCharacterTemplateSO : ScriptableObject
    {
        // General Info
        [BoxGroup("General Info", centerLabel: true)]
        [LabelWidth(100)]
        [GUIColor("Yellow")]
        public string myName;
        [BoxGroup("General Info")]
        [LabelWidth(100)]
        [GUIColor("Yellow")]
        public string mySubName;
        [BoxGroup("General Info")]
        [LabelWidth(100)]
        [GUIColor("Yellow")]
        public CharacterRace race;
        [BoxGroup("General Info")]
        [LabelWidth(100)]
        [GUIColor("Yellow")]
        public CharacterBackground background;
        [BoxGroup("General Info")]
        [LabelWidth(100)]
        [GUIColor("Yellow")]
        public CharacterModelSize modelSize;

        public SerializedAttrbuteSheet attributeSheet;

        [BoxGroup("Abilities", centerLabel: true)]
        [LabelWidth(100)]
        [GUIColor("Yellow")]
        public SerializedAbilityBook abilityBook;

        // Passive Traits
        [BoxGroup("Passive Data", centerLabel: true)]
        [LabelWidth(100)]
        public SerializedPerkManagerModel serializedPassiveManager;

        [Header("Model Properties")]
        public List<string> modelParts;

        [Header("Item Properties")]
        public SerializedItemSet itemSet;

        [Header("Talent Properties")]
        public List<TalentPairing> talentPairings = new List<TalentPairing>();

        // GUI Colours for Odin
        #region

        private Color Blue() { return Color.cyan; }
        private Color Green() { return Color.green; }
        private Color Yellow() { return Color.yellow; }
        private Color Red() { return Color.red; }

        #endregion
    }
}
