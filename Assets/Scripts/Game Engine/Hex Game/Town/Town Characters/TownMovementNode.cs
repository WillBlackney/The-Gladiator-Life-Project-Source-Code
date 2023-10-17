using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeAreGladiators.TownFeatures
{
    public class TownMovementNode : MonoBehaviour
    {
        /* Sorting Orders
           ground  = 0
           arena = 2
           hospital  = 2
           library  = 4
           tavern  = 5
           armory = 8
           middle ground = 6
           foreground  = 9
        */

        [SerializeField] private bool adjustCharacterSortOrderOnArrival = false;
        [ShowIf("ShowBaseSortOrder")]
        [Range(0, 15)]
        [SerializeField] private int baseSortOrder;

        public TownCharacterView myPausedCharacter;

        public bool AdjustCharacterSortOrderOnArrival => adjustCharacterSortOrderOnArrival;
        public int BaseSortOrder => baseSortOrder;

        public bool ShowBaseSortOrder()
        {
            return adjustCharacterSortOrderOnArrival;
        }

        public void OnCharacterArrived(TownCharacterView townCharacterView)
        {
            if(adjustCharacterSortOrderOnArrival)
            {
                townCharacterView.Ucm.RootSortingGroup.sortingOrder = baseSortOrder + 1;
                townCharacterView.Ucm.RootSortingGroup.sortingOrder = baseSortOrder + 1;
            }
        }
    }
}

