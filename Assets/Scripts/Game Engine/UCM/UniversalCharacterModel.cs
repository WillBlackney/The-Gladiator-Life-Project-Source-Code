﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Spriter2UnityDX;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace CardGameEngine.UCM
{
    public class UniversalCharacterModel : MonoBehaviour
    {
        // Properties + Component References
        #region
        [HideInInspector] public bool activelyFading = false;

        [Header("Core Components")]
        public Animator myAnimator;
        public EntityRenderer myEntityRenderer;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("All Model Element References")]
        public UniversalCharacterModelElement[] allModelElements;
        public SpriteMask[] allHeadWearSpriteMasks;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Active Particle References")]
        [HideInInspector] public UniversalCharacterModelElement activeChestParticles;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Active Lighting References")]
        [HideInInspector] public UniversalCharacterModelElement activeChestLighting;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Weapon References")]
        public List<UniversalCharacterModelElement> allMainHandWeapons;
        public List<UniversalCharacterModelElement> allOffHandWeapons;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Armour References")]
        public UniversalCharacterModelElement[] allChestArmour;
        public UniversalCharacterModelElement[] allHeadArmour;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Active Body Part References")]
        [HideInInspector] public UniversalCharacterModelElement activeHead;
        [HideInInspector] public UniversalCharacterModelElement activeFace;
        [HideInInspector] public UniversalCharacterModelElement activeLeftLeg;
        [HideInInspector] public UniversalCharacterModelElement activeRightLeg;
        [HideInInspector] public UniversalCharacterModelElement activeRightHand;
        [HideInInspector] public UniversalCharacterModelElement activeRightArm;
        [HideInInspector] public UniversalCharacterModelElement activeLeftHand;
        [HideInInspector] public UniversalCharacterModelElement activeLeftArm;
        [HideInInspector] public UniversalCharacterModelElement activeChest;

        [Header("Active Clothing Part References")]
        [HideInInspector] public UniversalCharacterModelElement activeHeadWear;
        [HideInInspector] public UniversalCharacterModelElement activeChestWear;
        [HideInInspector] public UniversalCharacterModelElement activeLeftLegWear;
        [HideInInspector] public UniversalCharacterModelElement activeRightLegWear;
        [HideInInspector] public UniversalCharacterModelElement activeLeftArmWear;
        [HideInInspector] public UniversalCharacterModelElement activeRightArmWear;
        [HideInInspector] public UniversalCharacterModelElement activeLeftHandWear;
        [HideInInspector] public UniversalCharacterModelElement activeRightHandWear;
        [HideInInspector] public UniversalCharacterModelElement activeMainHandWeapon;
        [HideInInspector] public UniversalCharacterModelElement activeOffHandWeapon;

        private bool hasRunSetup = false;
        #endregion

        // Initialization
        #region
        private void Awake()
        {
            RunSetup();
        }
        private void Start()
        {
            RunSetup();
        }
        void RunSetup()
        {
            if (!hasRunSetup)
            {
                UniversalCharacterModelElement[] elements = GetComponentsInChildren<UniversalCharacterModelElement>(true);
                Debug.LogWarning("Found " + elements.Length.ToString() + " element components");
                allModelElements = elements;
                hasRunSetup = true;
                HexGameEngine.UCM.CharacterModeller.AutoSetHeadMaskOrderInLayer(this);
            }
            
        }
        private void OnEnable()
        {
            RunSetup();
        }
        #endregion

        // Animation Logic
        #region 
        public void SetBaseAnim()
        {
            myAnimator.SetTrigger("Base");
        }
        public void SetIdleAnim()
        {
            myAnimator.SetTrigger("Idle");

        }

        #endregion

    }
}