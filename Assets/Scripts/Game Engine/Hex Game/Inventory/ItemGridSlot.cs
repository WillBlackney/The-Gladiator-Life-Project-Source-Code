using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WeAreGladiators.Items
{
    public class ItemGridSlot : MonoBehaviour
    {
        [SerializeField] private ItemGridView myItemView;
        public ItemGridView MyItemView => myItemView;

        public void Reset()
        {
            gameObject.SetActive(false);
        }
        public void Show()
        {
            gameObject.SetActive(true);
        }
    }
}