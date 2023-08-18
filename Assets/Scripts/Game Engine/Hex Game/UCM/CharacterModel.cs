using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Spriter2UnityDX;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using WeAreGladiators.Items;
using WeAreGladiators;
using UnityEngine.Rendering;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.UCM
{
    public class CharacterModel : MonoBehaviour
    {
        [Header("Core Components")]
        public Animator myAnimator;
        public EntityRenderer myEntityRenderer;
        [Tooltip("Leave this unticked if you need this UCM to be masked by masks that are not childed to this game object (e.g. scroll character roster).")]
        [SerializeField] bool allowAutoSorting = true;

        [ShowIf("ShowRootSorting")]
        [SerializeField] SortingGroup rootSortingGroup;
        [SerializeField] float combatScale = 1f;

        public SortingGroup RootSortingGroup => rootSortingGroup;
        public bool AllowAutoSorting => allowAutoSorting;
        public float CombatScale => combatScale;
        public bool ShowRootSorting()
        {
            return allowAutoSorting;
        }

    }
}