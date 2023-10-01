using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeAreGladiators.Items
{
    public class ItemGridScrollView : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int minimumSlotsShown;

        [Space(10)]
        [Header("Components")]
        [SerializeField] private ItemGridSlot[] slots;


    }
}