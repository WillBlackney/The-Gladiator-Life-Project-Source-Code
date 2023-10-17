using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.TownFeatures
{
    public class TownPath : MonoBehaviour
    {
        [Header("Scene Components")]
        [SerializeField] private TownMovementNode[] nodes;
        public TownMovementNode[] Nodes => nodes;          

    }
}