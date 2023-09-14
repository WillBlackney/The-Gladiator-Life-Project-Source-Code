using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using WeAreGladiators.Items;

namespace WeAreGladiators.UCM
{

    public class UniversalCharacterModelElement : MonoBehaviour
    {
        [Header("Core Properties + Components")]
        public int sortingOrderBonus;
        public BodyPartType bodyPartType;
        public ItemDataSO[] hexItemsWithMyView;
        public List<UniversalCharacterModelElement> connectedElements;

        [ShowIf("ShowHurtFace")]
        public Sprite hurtFace;

        public bool ShowHurtFace()
        {
            return bodyPartType == BodyPartType.Face;
        }
    }
}
