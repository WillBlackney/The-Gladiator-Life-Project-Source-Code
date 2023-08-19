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
        [SerializeField] bool baseFacingIsRight = true;
        [Tooltip("Leave this unticked if you need this UCM to be masked by masks that are not childed to this game object (e.g. scroll character roster).")]
        [SerializeField] bool allowAutoSorting = true;

        [ShowIf("ShowRootSorting")]
        [SerializeField] SortingGroup rootSortingGroup;
        [SerializeField] float combatScale = 1f;
        
        [HideInInspector] public UniversalCharacterModelElement[] allModelElements;

        public SortingGroup RootSortingGroup => rootSortingGroup;
        public bool AllowAutoSorting => allowAutoSorting;
        public bool BaseFacingIsRight => baseFacingIsRight;
        public float CombatScale => combatScale;
        public UniversalCharacterModelElement[] AllModelElements
        {
            get
            {
                if (allModelElements == null || allModelElements.Length == 0) RunSetup(true);
                return allModelElements;
            }
            protected set { allModelElements = value; }
        }
        public bool ShowRootSorting()
        {
            return allowAutoSorting;
        }

        #region Lifecycle + Init

        protected bool hasRunSetup = false;
        protected virtual void Awake()
        {
            RunSetup();
        }
        protected virtual void Start()
        {
            RunSetup();
        }
        protected virtual void OnEnable()
        {
            RunSetup();
        }
        protected virtual void RunSetup(bool allowRerun = false)
        {
            if ((!hasRunSetup || (hasRunSetup && allowRerun)) && Application.isPlaying)
            {
                Debug.Log("UCM.RunSetup() called and executing setup...");
                if (hasRunSetup && allowRerun) Debug.Log("UCM.RunSetup() already had previous setup, now rerunning...");

                // Get all elements
                AllModelElements = GetComponentsInChildren<UniversalCharacterModelElement>(true);

                CharacterModeller.AutoSetSortingOrderValues(this);

                hasRunSetup = true;
            }
        }
        #endregion

    }
}