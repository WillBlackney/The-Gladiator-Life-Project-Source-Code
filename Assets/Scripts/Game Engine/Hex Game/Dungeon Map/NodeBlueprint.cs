using UnityEngine;

namespace WeAreGladiators.DungeonMap
{
    [CreateAssetMenu]
    public class NodeBlueprint : ScriptableObject
    {
        [Header("Core Properties")]
        public EncounterType nodeType;

        [Header("Image Files")]
        public Sprite sprite;
        public Sprite greyScaleSprite;
        public Sprite outlineSprite;
    }
}
