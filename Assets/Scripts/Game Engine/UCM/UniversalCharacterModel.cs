using System.Collections;
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
        [SerializeField] GameObject headMasksParent;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]       

        [Header("Active Particle References")]
        [HideInInspector] public UniversalCharacterModelElement activeChestParticles;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Active Lighting References")]
        [HideInInspector] public UniversalCharacterModelElement activeChestLighting;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        public UniversalCharacterModelElement[] AllModelElements { get; private set; }
        public SpriteMask[] AllHeadWearSpriteMasks { get; private set; }
        public UniversalCharacterModelElement[] AllMainHandWeapons { get; private set; }
        public UniversalCharacterModelElement[] AllOffHandWeapons { get; private set; }
        public UniversalCharacterModelElement[] AllChestArmour { get; private set; }
        public UniversalCharacterModelElement[] AllHeadArmour { get; private set; }

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
        public void RunSetup()
        {
            if (!hasRunSetup)
            {
                // Get all elements
                AllModelElements = GetComponentsInChildren<UniversalCharacterModelElement>(true);

                // Get all head wear sprite masks
                AllHeadWearSpriteMasks = headMasksParent.GetComponentsInChildren<SpriteMask>(true);

                // setup main hand weapons
                List<UniversalCharacterModelElement> mhElements = new List<UniversalCharacterModelElement>();
                List<UniversalCharacterModelElement> ohElements = new List<UniversalCharacterModelElement>();
                List<UniversalCharacterModelElement> chestElements = new List<UniversalCharacterModelElement>();
                List<UniversalCharacterModelElement> headElements = new List<UniversalCharacterModelElement>();

                foreach (var element in AllModelElements)
                {
                    switch (element.bodyPartType)
                    {
                        case BodyPartType.ChestWear:
                            chestElements.Add(element);
                            break;
                        case BodyPartType.HeadWear:
                            headElements.Add(element);
                            break;
                        case BodyPartType.MainHandWeapon:
                            mhElements.Add(element);
                            break;
                        case BodyPartType.OffHandWeapon:
                            ohElements.Add(element);
                            break;
                    }
                }

                AllChestArmour = chestElements.ToArray();
                AllMainHandWeapons = mhElements.ToArray();
                AllOffHandWeapons = ohElements.ToArray();
                AllHeadArmour = headElements.ToArray();

                HexGameEngine.UCM.CharacterModeller.AutoSetHeadMaskOrderInLayer(this);

                hasRunSetup = true;
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