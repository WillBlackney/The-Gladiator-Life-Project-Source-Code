using UnityEngine;
using Sirenix.OdinInspector;
using HexGameEngine.Utilities;

#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector.Editor;
#endif

namespace HexGameEngine.Libraries
{
    public class SpriteLibrary : Singleton<SpriteLibrary>
    {
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
        private Sprite corruptionBadge;
        [PreviewField(75)]
        [SerializeField]
        private Sprite naturalismBadge;
        [PreviewField(75)]
        [SerializeField]
        private Sprite manipulationBadge;
        #endregion

        // Attribute Images Images
        #region
        [Header("Core Attribute Images")]
        [PreviewField(75)]
        [SerializeField]
        private Sprite strength;
        [PreviewField(75)]
        [SerializeField]
        private Sprite intelligence;
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
        private Sprite staffIcon;
        [PreviewField(75)]
        [SerializeField]
        private Sprite bowIcon;
        [PreviewField(75)]
        [SerializeField]
        private Sprite shieldIcon;
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
        #endregion

        // Racial Images
        #region
        [Header("Racial Images")]
        [PreviewField(75)]
        [SerializeField]
        private Sprite human;
        [PreviewField(75)]
        [SerializeField]
        private Sprite orc;
        [PreviewField(75)]
        [SerializeField]
        private Sprite elf;
        [PreviewField(75)]
        [SerializeField]
        private Sprite goblin;
        [PreviewField(75)]
        [SerializeField]
        private Sprite undead;
        [PreviewField(75)]
        [SerializeField]
        private Sprite gnoll;
        [PreviewField(75)]
        [SerializeField]
        private Sprite satyr;
        #endregion

        // Stress State Images
        #region
        [Header("Stress State Images")]
        [PreviewField(75)]
        [SerializeField]
        private Sprite confident;
        [PreviewField(75)]
        [SerializeField]
        private Sprite nervous;
        [PreviewField(75)]
        [SerializeField]
        private Sprite wavering;
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

        // Misc Images
        #region
        [Header("Misc Images")]
        [PreviewField(75)]
        public Sprite weaponAbilityIcon;
        #endregion

        // Get Sprites Logic 
        #region
        public Sprite GetTownActivitySprite(TownActivity activity)
        {
            if (activity == TownActivity.BedRest) return bedRest;
            else if (activity == TownActivity.Surgery) return surgery;
            else if (activity == TownActivity.Therapy) return therapy;
            else return null;
        }
        public Sprite GetAttributeSprite(CoreAttribute attribute)
        {
            if (attribute == CoreAttribute.Strength) return strength;
            else if (attribute == CoreAttribute.Intelligence) return intelligence;
            else if (attribute == CoreAttribute.Accuracy) return accuracy;
            else if (attribute == CoreAttribute.Constituition) return constitution;
            else if (attribute == CoreAttribute.Dodge) return dodge;
            else if (attribute == CoreAttribute.Resolve) return resolve;
            else if (attribute == CoreAttribute.Wits) return wits;
            else return null;
        }
        public Sprite GetRacialSprite(CharacterRace race)
        {
            Sprite spriteReturned = null;

            if (race == CharacterRace.Human)
            {
                spriteReturned = human;
            }
            else if (race == CharacterRace.Elf)
            {
                spriteReturned = elf;
            }
            else if (race == CharacterRace.Gnoll)
            {
                spriteReturned = gnoll;
            }
            else if (race == CharacterRace.Goblin)
            {
                spriteReturned = goblin;
            }
            else if (race == CharacterRace.Orc)
            {
                spriteReturned = orc;
            }
            else if (race == CharacterRace.Satyr)
            {
                spriteReturned = satyr;
            }
            else if (race == CharacterRace.Undead)
            {
                spriteReturned = undead;
            }


            return spriteReturned;
        }
        public Sprite GetStressStateSprite(StressState state)
        {
            Sprite spriteReturned = null;

            if (state == StressState.Confident)
            {
                spriteReturned = confident;
            }
            else if (state == StressState.Nervous)
            {
                spriteReturned = nervous;
            }
            else if (state == StressState.Wavering)
            {
                spriteReturned = wavering;
            }
            else if (state == StressState.Panicking)
            {
                spriteReturned = panicking;
            }
            else if (state == StressState.Shattered)
            {
                spriteReturned = shattered;
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
            else if (ts == TalentSchool.Corruption)
            {
                spriteReturned = corruptionBadge;
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
            else if (weaponClass == WeaponClass.Bow)
            {
                spriteReturned = bowIcon;
            }
            else if (weaponClass == WeaponClass.Shield)
            {
                spriteReturned = shieldIcon;
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
                spriteReturned = naturalismBook ;
            }
            else if (ts == TalentSchool.Pyromania)
            {
                spriteReturned = pyromaniaBook;
            }
            else if (ts == TalentSchool.Shadowcraft)
            {
                spriteReturned = shadowcraftBook;
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
            var otherAttr = (ColorFoldoutGroupAttribute)other;

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
            this.isExpanded = this.GetPersistentValue<bool>("ColorFoldoutGroupAttributeDrawer.isExpaned",
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