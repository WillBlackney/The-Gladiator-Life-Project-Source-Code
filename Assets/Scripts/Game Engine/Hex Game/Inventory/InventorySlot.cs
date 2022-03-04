using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.Items
{
    public class InventorySlot : MonoBehaviour
    {
        public void Reset()
        {
            gameObject.SetActive(false);
        }
    }
}