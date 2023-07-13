using WeAreGladiators.Items;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

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