using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardGameEngine
{
    public class InventoryCardSlot : MonoBehaviour
    {
        public CardInfoPanel cardInfoPanel;
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}