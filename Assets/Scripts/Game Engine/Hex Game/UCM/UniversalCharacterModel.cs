using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Spriter2UnityDX;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using HexGameEngine.Items;
using HexGameEngine;
using UnityEngine.Rendering;
using HexGameEngine.Utilities;

namespace HexGameEngine.UCM
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
        [SerializeField] GameObject[] oneHandAnimationBones;
        [SerializeField] GameObject[] twoHandAnimationBones;
        [Tooltip("Leave this unticked if you need this UCM to be masked by masks that are not childed to this game object (e.g. scroll character roster).")]
        [SerializeField] bool allowAutoSorting = true;
        [ShowIf("ShowRootSorting")]
        [SerializeField] SortingGroup rootSortingGroup;
        [ShowIf("ShowRootSorting")]
        [SerializeField] SortingGroup headSortingGroup;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]       

        [Header("Active Particle References")]
        [HideInInspector] public UniversalCharacterModelElement activeChestParticles;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Active Lighting References")]
        [HideInInspector] public UniversalCharacterModelElement activeChestLighting;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [HideInInspector] public UniversalCharacterModelElement[] allModelElements;
        [HideInInspector] public SpriteMask[] allHeadWearSpriteMasks;
        [HideInInspector] public UniversalCharacterModelElement[] allMainHandWeapons;
        [HideInInspector] public UniversalCharacterModelElement[] allOffHandWeapons;
        [HideInInspector] public UniversalCharacterModelElement[] allChestArmour;
        [HideInInspector] public UniversalCharacterModelElement[] allHeadArmour;

        public bool AllowAutoSorting => allowAutoSorting;
        public SortingGroup RootSortingGroup => rootSortingGroup;
        public SortingGroup HeadSortingGroup => headSortingGroup;
        public UniversalCharacterModelElement[] AllModelElements 
        { 
            get
            {
                if(allModelElements == null || allModelElements.Length == 0) RunSetup(true);                
                return allModelElements;
            }
            private set { allModelElements = value; } 
        }
        public SpriteMask[] AllHeadWearSpriteMasks
        {
            get
            {
                if (allHeadWearSpriteMasks == null || allHeadWearSpriteMasks.Length == 0) RunSetup(true);
                return allHeadWearSpriteMasks;
            }
            private set { allHeadWearSpriteMasks = value; }
        }
        public UniversalCharacterModelElement[] AllMainHandWeapons
        {
            get
            {
                if (allMainHandWeapons == null || allMainHandWeapons.Length == 0) RunSetup(true);
                return allMainHandWeapons;
            }
            private set { allMainHandWeapons = value; }
        }
        public UniversalCharacterModelElement[] AllOffHandWeapons
        {
            get
            {
                if (allOffHandWeapons == null || allOffHandWeapons.Length == 0) RunSetup(true);
                return allOffHandWeapons;
            }
            private set { allOffHandWeapons = value; }
        }
        public UniversalCharacterModelElement[] AllChestArmour
        {
            get
            {
                if (allChestArmour == null || allChestArmour.Length == 0) RunSetup(true);
                return allChestArmour;
            }
            private set { allChestArmour = value; }
        }
        public UniversalCharacterModelElement[] AllHeadArmour
        {
            get
            {
                if (allHeadArmour == null || allHeadArmour.Length == 0) RunSetup(true);
                return allHeadArmour;
            }
            private set { allHeadArmour = value; }
        }

        [Header("Active Body Part References")]
        [HideInInspector] public UniversalCharacterModelElement activeHead;
        [HideInInspector] public UniversalCharacterModelElement activeFace;
        [HideInInspector] public UniversalCharacterModelElement activeLeftLeg;
        [HideInInspector] public UniversalCharacterModelElement activeRightLeg;
        [HideInInspector] public UniversalCharacterModelElement activeRightHand;
        [HideInInspector] public UniversalCharacterModelElement activeRightHand2H;
        [HideInInspector] public UniversalCharacterModelElement activeRightArm;
        [HideInInspector] public UniversalCharacterModelElement activeLeftHand;
        [HideInInspector] public UniversalCharacterModelElement activeLeftHand2H;
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
        [HideInInspector] public UniversalCharacterModelElement activeLeftHandWear2H;
        [HideInInspector] public UniversalCharacterModelElement activeRightHandWear;
        [HideInInspector] public UniversalCharacterModelElement activeRightHandWear2H;
        [HideInInspector] public UniversalCharacterModelElement activeMainHandWeapon;
        [HideInInspector] public UniversalCharacterModelElement activeOffHandWeapon;

        private bool hasRunSetup = false;

        private Sprite normalFaceSprite;
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
        private void OnEnable()
        {
            RunSetup();
        }
        public void RunSetup(bool allowRerun = false)
        {
            if ((!hasRunSetup || (hasRunSetup && allowRerun)) && Application.isPlaying)
            {
                Debug.Log("UCM.RunSetup() called and executing setup...");
                if (hasRunSetup && allowRerun) Debug.Log("UCM.RunSetup() already had previous setup, now rerunning...");
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

                HexGameEngine.UCM.CharacterModeller.AutoSetSortingOrderValues(this);
                SetMode(UcmMode.Standard);

                // Turn everything off to start
                AllModelElements.ForEach(x => x.gameObject.SetActive(false));

                hasRunSetup = true;
            }
        }
        
        #endregion

        // Animation Logic
        #region 
        public void ResetAnimationSpeed()
        {
            myAnimator.speed = 1;
        }
        public void RandomizeAnimationSpeed()
        {
            float newSpeed = RandomGenerator.NumberBetween(85, 115) * 0.01f;
            myAnimator.speed = newSpeed;
        }
        public void SetBaseAnim()
        {
            myAnimator.SetTrigger("Base");
        }
        public void SetIdleAnim()
        {
            myAnimator.SetTrigger("Idle");

        }
        public void ShowHurtFace()
        {
            Debug.LogWarning("ShowHurtFace");
            if(activeFace != null)
            {
                SpriteRenderer sr = activeFace.GetComponent<SpriteRenderer>();
                normalFaceSprite = sr.sprite;
                sr.sprite = activeFace.hurtFace;
            }
        }
        public void ShowNormalFace()
        {
            Debug.LogWarning("ShowNormalFace");
            if (activeFace != null && normalFaceSprite != null)
            {
                activeFace.GetComponent<SpriteRenderer>().sprite = normalFaceSprite;
            }
        }

        public void StopAnimController()
        {
            myAnimator.enabled = false;
        }

        #endregion

        // Mode Logic
        #region

        public UcmMode UcmMode
        {
            get; private set;
        }
        public void SetModeFromItemSet(ItemSet itemSet)
        {
            // Do weapons first
            if (itemSet.mainHandItem != null &&
            itemSet.mainHandItem.IsMeleeWeapon &&
                itemSet.mainHandItem.handRequirement == HandRequirement.TwoHanded)
                SetMode(UcmMode.TwoHandMelee);

            else SetMode(UcmMode.Standard);
        }
        public void SetMode(UcmMode mode)
        {
            UcmMode = mode;
            if(mode == UcmMode.Standard)
            {
                oneHandAnimationBones.ForEach(x => x.SetActive(true));
                twoHandAnimationBones.ForEach(x => x.SetActive(false));
            }
            else if (mode == UcmMode.TwoHandMelee)
            {
                oneHandAnimationBones.ForEach(x => x.SetActive(false));
                twoHandAnimationBones.ForEach(x => x.SetActive(true));
            }
        }

        
        #endregion

        public bool ShowRootSorting()
        {
            return allowAutoSorting;
        }

    }

    public enum UcmMode
    {
        Standard = 0,
        TwoHandMelee = 1,
    }
}