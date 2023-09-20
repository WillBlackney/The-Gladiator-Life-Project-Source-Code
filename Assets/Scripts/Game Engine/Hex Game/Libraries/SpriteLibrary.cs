using Sirenix.OdinInspector;
using UnityEngine;
using WeAreGladiators.Utilities;
                                #if UNITY_EDITOR
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector.Editor;
#endif

namespace WeAreGladiators.Libraries
{
    public class SpriteLibrary : Singleton<SpriteLibrary>
    {

        // Misc Images
        #region

        [Header("Misc Images")]
        [PreviewField(75)]
        public Sprite weaponAbilityIcon;

        #endregion
        // Talent School Badges
        #region

        [Header("Talent School Badges")]
        [PreviewField(75)]
        [SerializeField]
        private Sprite neutralBadge;
        [PreviewField(75)]
        [SerializeField]
        private Sprite warfareBadge;
        [PreviewField(75)]
        [SerializeField]
        private Sprite guardianBadge;
        [PreviewField(75)]
        [SerializeField]
        private Sprite scoundrelBadge;
        [PreviewField(75)]
        [SerializeField]
        private Sprite rangerBadge;
        [PreviewField(75)]
        [SerializeField]
        private Sprite pyromaniaBadge;
        [PreviewField(75)]
        [SerializeField]
        private Sprite divinityBadge;
        [PreviewField(75)]
        [SerializeField]
        private Sprite shadowCraftBadge;
        [PreviewField(75)]
        [SerializeField]
        private Sprite naturalismBadge;
        [PreviewField(75)]
        [SerializeField]
        private Sprite manipulationBadge;
        [PreviewField(75)]
        [SerializeField]
        private Sprite metamorphBadge;

        #endregion

        // Attribute Images Images
        #region

        [Header("Core Attribute Images")]
        [PreviewField(75)]
        [SerializeField]
        private Sprite might;
        [PreviewField(75)]
        [SerializeField]
        private Sprite constitution;
        [PreviewField(75)]
        [SerializeField]
        private Sprite accuracy;
        [PreviewField(75)]
        [SerializeField]
        private Sprite dodge;
        [PreviewField(75)]
        [SerializeField]
        private Sprite wits;
        [PreviewField(75)]
        [SerializeField]
        private Sprite resolve;
        [PreviewField(75)]
        [SerializeField]
        private Sprite fitness;

        #endregion

        // Ability Type Images
        #region

        [Header("Ability Type Images")]
        [PreviewField(75)]
        [SerializeField]
        private Sprite meleeAttack;
        [PreviewField(75)]
        [SerializeField]
        private Sprite rangedAttack;
        [PreviewField(75)]
        [SerializeField]
        private Sprite skill;

        #endregion

        // Weapon Images
        #region

        [Header("Weapon Images")]
        [PreviewField(75)]
        [SerializeField]
        private Sprite axeIcon;
        [PreviewField(75)]
        [SerializeField]
        private Sprite swordIcon;
        [PreviewField(75)]
        [SerializeField]
        private Sprite hammerIcon;
        [PreviewField(75)]
        [SerializeField]
        private Sprite daggerIcon;
        [PreviewField(75)]
        [SerializeField]
        private Sprite spearIcon;
        [PreviewField(75)]
        [SerializeField]
        private Sprite staffIcon;
        [PreviewField(75)]
        [SerializeField]
        private Sprite bowIcon;
        [SerializeField]
        [PreviewField(75)]
        private Sprite crossbowIcon;
        [PreviewField(75)]
        [SerializeField]
        private Sprite shieldIcon;
        [PreviewField(75)]
        [SerializeField]
        private Sprite throwingNetIcon;
        [PreviewField(75)]
        [SerializeField]
        private Sprite polearmIcon;
        [PreviewField(75)]
        [SerializeField]
        private Sprite holdableIcon;
        [PreviewField(75)]
        [SerializeField]
        private Sprite unarmedIcon;
        [PreviewField(75)]
        [SerializeField]
        private Sprite twoHandedIcon;
        [PreviewField(75)]
        [SerializeField]
        private Sprite dualWieldingIcon;

        #endregion

        // Skill Book Images
        #region

        [Header("Skill Book Images")]
        [PreviewField(75)]
        [SerializeField]
        private Sprite warfareBook;
        [PreviewField(75)]
        [SerializeField]
        private Sprite guadianBook;
        [PreviewField(75)]
        [SerializeField]
        private Sprite scoundrelBook;
        [PreviewField(75)]
        [SerializeField]
        private Sprite rangerBook;
        [PreviewField(75)]
        [SerializeField]
        private Sprite divinityBook;
        [PreviewField(75)]
        [SerializeField]
        private Sprite naturalismBook;
        [PreviewField(75)]
        [SerializeField]
        private Sprite shadowcraftBook;
        [PreviewField(75)]
        [SerializeField]
        private Sprite manipulationBook;
        [PreviewField(75)]
        [SerializeField]
        private Sprite pyromaniaBook;
        [PreviewField(75)]
        [SerializeField]
        private Sprite metamorphBook;

        #endregion

        // Stress State Images
        #region

        [Header("Stress State Images")]
        [PreviewField(75)]
        [SerializeField]
        private Sprite confident;
        [PreviewField(75)]
        [SerializeField]
        private Sprite steady;
        [PreviewField(75)]
        [SerializeField]
        private Sprite nervous;
        [PreviewField(75)]
        [SerializeField]
        private Sprite panicking;
        [PreviewField(75)]
        [SerializeField]
        private Sprite shattered;

        #endregion

        // Town Activity Images
        #region

        [Header("Town Activity Images")]
        [PreviewField(75)]
        [SerializeField]
        private Sprite bedRest;
        [PreviewField(75)]
        [SerializeField]
        private Sprite surgery;
        [PreviewField(75)]
        [SerializeField]
        private Sprite therapy;

        #endregion

        // Get Sprites Logic 
        #region

        public Sprite GetTownActivitySprite(TownActivity activity)
        {
            if (activity == TownActivity.BedRest)
            {
                return bedRest;
            }
            if (activity == TownActivity.Surgery)
            {
                return surgery;
            }
            if (activity == TownActivity.Therapy)
            {
                return therapy;
            }
            return null;
        }
        public Sprite GetAttributeSprite(CoreAttribute attribute)
        {
            if (attribute == CoreAttribute.Might)
            {
                return might;
            }
            if (attribute == CoreAttribute.Accuracy)
            {
                return accuracy;
            }
            if (attribute == CoreAttribute.Constitution)
            {
                return constitution;
            }
            if (attribute == CoreAttribute.Dodge)
            {
                return dodge;
            }
            if (attribute == CoreAttribute.Resolve)
            {
                return resolve;
            }
            if (attribute == CoreAttribute.Wits)
            {
                return wits;
            }
            if (attribute == CoreAttribute.Fitness)
            {
                return fitness;
            }
            return null;
        }
        public Sprite GetMoraleStateSprite(MoraleState state)
        {
            Sprite spriteReturned;

            if (state == MoraleState.Confident)
            {
                spriteReturned = confident;
            }
            else if (state == MoraleState.Steady)
            {
                spriteReturned = steady;
            }
            else if (state == MoraleState.Nervous)
            {
                spriteReturned = nervous;
            }
            else if (state == MoraleState.Panicking)
            {
                spriteReturned = panicking;
            }
            else if (state == MoraleState.Shattered)
            {
                spriteReturned = shattered;
            }
            else
            {
                spriteReturned = steady;
            }

            return spriteReturned;
        }
        public Sprite GetTalentSchoolSprite(TalentSchool ts)
        {
            Sprite spriteReturned = null;

            if (ts == TalentSchool.Scoundrel)
            {
                spriteReturned = scoundrelBadge;
            }
            else if (ts == TalentSchool.Warfare)
            {
                spriteReturned = warfareBadge;
            }
            else if (ts == TalentSchool.Ranger)
            {
                spriteReturned = rangerBadge;
            }

            else if (ts == TalentSchool.Divinity)
            {
                spriteReturned = divinityBadge;
            }
            else if (ts == TalentSchool.Guardian)
            {
                spriteReturned = guardianBadge;
            }
            else if (ts == TalentSchool.Manipulation)
            {
                spriteReturned = manipulationBadge;
            }
            else if (ts == TalentSchool.Naturalism)
            {
                spriteReturned = naturalismBadge;
            }
            else if (ts == TalentSchool.Pyromania)
            {
                spriteReturned = pyromaniaBadge;
            }
            else if (ts == TalentSchool.Shadowcraft)
            {
                spriteReturned = shadowCraftBadge;
            }
            else if (ts == TalentSchool.Metamorph)
            {
                spriteReturned = metamorphBadge;
            }
            else if (ts == TalentSchool.Neutral)
            {
                spriteReturned = neutralBadge;
            }

            return spriteReturned;
        }
        public Sprite GetAbilityTypeSprite(AbilityType abilityType)
        {
            Sprite spriteReturned = null;

            if (abilityType == AbilityType.MeleeAttack)
            {
                spriteReturned = meleeAttack;
            }
            else if (abilityType == AbilityType.RangedAttack)
            {
                spriteReturned = rangedAttack;
            }
            else if (abilityType == AbilityType.Skill)
            {
                spriteReturned = skill;
            }
            return spriteReturned;
        }
        public Sprite GetWeaponSprite(WeaponClass weaponClass)
        {
            Sprite spriteReturned = null;

            if (weaponClass == WeaponClass.Axe)
            {
                spriteReturned = axeIcon;
            }
            else if (weaponClass == WeaponClass.Sword)
            {
                spriteReturned = swordIcon;
            }
            else if (weaponClass == WeaponClass.Hammer)
            {
                spriteReturned = hammerIcon;
            }
            else if (weaponClass == WeaponClass.Staff)
            {
                spriteReturned = staffIcon;
            }
            else if (weaponClass == WeaponClass.Dagger)
            {
                spriteReturned = daggerIcon;
            }
            else if (weaponClass == WeaponClass.Spear)
            {
                spriteReturned = spearIcon;
            }
            else if (weaponClass == WeaponClass.Bow)
            {
                spriteReturned = bowIcon;
            }
            else if (weaponClass == WeaponClass.Crossbow)
            {
                spriteReturned = crossbowIcon;
            }
            else if (weaponClass == WeaponClass.Shield)
            {
                spriteReturned = shieldIcon;
            }
            else if (weaponClass == WeaponClass.ThrowingNet)
            {
                spriteReturned = throwingNetIcon;
            }
            else if (weaponClass == WeaponClass.Polearm)
            {
                spriteReturned = polearmIcon;
            }
            else if (weaponClass == WeaponClass.Holdable)
            {
                spriteReturned = holdableIcon;
            }
            else if (weaponClass == WeaponClass.Unarmed)
            {
                spriteReturned = unarmedIcon;
            }
            else if (weaponClass == WeaponClass.TwoHanded)
            {
                spriteReturned = twoHandedIcon;
            }
            else if (weaponClass == WeaponClass.DualWielding)
            {
                spriteReturned = dualWieldingIcon;
            }
            return spriteReturned;
        }
        public Sprite GetTalentSchoolBookSprite(TalentSchool ts)
        {
            Sprite spriteReturned = null;

            if (ts == TalentSchool.Scoundrel)
            {
                spriteReturned = scoundrelBook;
            }
            else if (ts == TalentSchool.Warfare)
            {
                spriteReturned = warfareBook;
            }
            else if (ts == TalentSchool.Ranger)
            {
                spriteReturned = rangerBook;
            }
            else if (ts == TalentSchool.Divinity)
            {
                spriteReturned = divinityBook;
            }
            else if (ts == TalentSchool.Guardian)
            {
                spriteReturned = guadianBook;
            }
            else if (ts == TalentSchool.Manipulation)
            {
                spriteReturned = manipulationBook;
            }
            else if (ts == TalentSchool.Naturalism)
            {
                spriteReturned = naturalismBook;
            }
            else if (ts == TalentSchool.Pyromania)
            {
                spriteReturned = pyromaniaBook;
            }
            else if (ts == TalentSchool.Shadowcraft)
            {
                spriteReturned = shadowcraftBook;
            }
            else if (ts == TalentSchool.Metamorph)
            {
                spriteReturned = metamorphBook;
            }

            return spriteReturned;
        }

        #endregion
    }

#if UNITY_EDITOR
    public class ColorFoldoutGroupAttribute : PropertyGroupAttribute
    {
        public float R, G, B, A;

        public ColorFoldoutGroupAttribute(string path) : base(path)
        {

        }

        public ColorFoldoutGroupAttribute(string path, float r, float g, float b, float a = 1f) : base(path)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        protected override void CombineValuesWith(PropertyGroupAttribute other)
        {
            ColorFoldoutGroupAttribute otherAttr = (ColorFoldoutGroupAttribute) other;

            R = Mathf.Max(otherAttr.R, R);
            G = Mathf.Max(otherAttr.G, G);
            B = Mathf.Max(otherAttr.B, B);
            A = Mathf.Max(otherAttr.A, A);
        }
    }

    public class ColorFoldoutGroupAttributeDrawer : OdinGroupDrawer<ColorFoldoutGroupAttribute>
    {
        private LocalPersistentContext<bool> isExpanded;

        protected override void Initialize()
        {
            isExpanded = this.GetPersistentValue("ColorFoldoutGroupAttributeDrawer.isExpaned",
                GeneralDrawerConfig.Instance.ExpandFoldoutByDefault);
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            GUIHelper.PushColor(new Color(Attribute.R, Attribute.G, Attribute.B, Attribute.A));
            SirenixEditorGUI.BeginBox();
            SirenixEditorGUI.BeginBoxHeader();
            GUIHelper.PopColor();

            isExpanded.Value = SirenixEditorGUI.Foldout(isExpanded.Value, label);
            SirenixEditorGUI.EndBoxHeader();

            if (SirenixEditorGUI.BeginFadeGroup(this, isExpanded.Value))
            {
                for (int i = 0; i < Property.Children.Count; i++)
                {
                    Property.Children[i].Draw();
                }
            }
            SirenixEditorGUI.EndFadeGroup();
            SirenixEditorGUI.EndBox();

        }
    }

#endif
}
