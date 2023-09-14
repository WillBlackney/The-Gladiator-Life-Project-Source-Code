using Sirenix.OdinInspector;
using Spriter2UnityDX;
using UnityEngine;
using UnityEngine.Rendering;

namespace WeAreGladiators.UCM
{
    public class CharacterModel : MonoBehaviour
    {
        [Header("Core Components")]
        public Animator myAnimator;
        public EntityRenderer myEntityRenderer;
        [ShowIf("ShowPortraitSprite")]
        [SerializeField]
        private Sprite portraitSprite;
        [SerializeField] private bool baseFacingIsRight = true;
        [Tooltip("Leave this unticked if you need this UCM to be masked by masks that are not childed to this game object (e.g. scroll character roster).")]
        [SerializeField]
        private bool allowAutoSorting = true;

        [ShowIf("ShowRootSorting")]
        [SerializeField]
        private SortingGroup rootSortingGroup;
        [SerializeField] private float combatScale = 1f;

        [Header("Animation Specifics")]
        [SerializeField]
        [Range(0, 5)]
        private int totalDeathAnims = 1;
        [SerializeField]
        [Range(0, 5)]
        private int totalDecapitationAnims = 1;

        [HideInInspector] public UniversalCharacterModelElement[] allModelElements;

        public SortingGroup RootSortingGroup => rootSortingGroup;
        public bool AllowAutoSorting => allowAutoSorting;
        public Sprite PortraitSprite => portraitSprite;
        public bool BaseFacingIsRight => baseFacingIsRight;
        public float CombatScale => combatScale;
        public int TotalDeathAnims => totalDeathAnims;
        public int TotalDecapitationAnims => totalDecapitationAnims;
        public UniversalCharacterModelElement[] AllModelElements
        {
            get
            {
                if (allModelElements == null || allModelElements.Length == 0)
                {
                    RunSetup(true);
                }
                return allModelElements;
            }
            protected set => allModelElements = value;
        }

        #region Lifecycle + Init

        protected bool hasRunSetup;
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
            if ((!hasRunSetup || hasRunSetup && allowRerun) && Application.isPlaying)
            {
                Debug.Log("UCM.RunSetup() called and executing setup...");
                if (hasRunSetup && allowRerun)
                {
                    Debug.Log("UCM.RunSetup() already had previous setup, now rerunning...");
                }

                // Get all elements
                AllModelElements = GetComponentsInChildren<UniversalCharacterModelElement>(true);

                CharacterModeller.AutoSetSortingOrderValues(this);

                hasRunSetup = true;
            }
        }

        #endregion

        #region Odin ShowIfs

        public bool ShowRootSorting()
        {
            return allowAutoSorting;
        }
        public bool ShowPortraitSprite()
        {
            return this is UniversalCharacterModel == false;
        }
        public virtual void StopAnimController()
        {
            // Must be called at some point during each death animation file as an anim event.
            myAnimator.enabled = false;
        }

        #endregion
    }
}
