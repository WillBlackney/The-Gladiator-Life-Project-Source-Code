using System.Collections.Generic;
using UnityEngine;

namespace CardGameEngine.UCM
{

    public class UniversalCharacterModelElement : MonoBehaviour
    {
        [Header("Core Properties + Components")]
        public int sortingOrderBonus;
        public BodyPartType bodyPartType;
        public List<ItemDataSO> itemsWithMyView;
        public HexGameEngine.Items.ItemDataSO[] hexItemsWithMyView;
        public List<UniversalCharacterModelElement> connectedElements;



    }
}