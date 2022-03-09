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
        public Sprite neutralBadge;
        [PreviewField(75)]
        public Sprite warfareBadge;
        [PreviewField(75)]
        public Sprite guardianBadge;
        [PreviewField(75)]
        public Sprite scoundrelBadge;
        [PreviewField(75)]
        public Sprite rangerBadge;
        [PreviewField(75)]
        public Sprite pyromaniaBadge;
        [PreviewField(75)]
        public Sprite divinityBadge;
        [PreviewField(75)]
        public Sprite shadowCraftBadge;
        [PreviewField(75)]
        public Sprite corruptionBadge;
        [PreviewField(75)]
        public Sprite naturalismBadge;
        [PreviewField(75)]
        public Sprite manipulationBadge;
        #endregion

        // Ability Type Images
        #region
        [Header("Ability Type Images")]
        [PreviewField(75)]
        public Sprite meleeAttack;
        [PreviewField(75)]
        public Sprite rangedAttack;
        [PreviewField(75)]
        public Sprite skill;
        #endregion

        // Weapon Images
        #region
        [Header("Weapon Images")]
        [PreviewField(75)]
        public Sprite axeIcon;
        [PreviewField(75)]
        public Sprite swordIcon;
        [PreviewField(75)]
        public Sprite hammerIcon;
        [PreviewField(75)]
        public Sprite daggerIcon;
        [PreviewField(75)]
        public Sprite staffIcon;
        [PreviewField(75)]
        public Sprite bowIcon;
        [PreviewField(75)]
        public Sprite shieldIcon;
        #endregion

        // Skill Book Images
        #region
        [Header("Skill Book Images")]
        [PreviewField(75)]
        public Sprite warfareBook;

        [PreviewField(75)]
        public Sprite guadianBook;

        [PreviewField(75)]
        public Sprite scoundrelBook;

        [PreviewField(75)]
        public Sprite rangerBook;

        [PreviewField(75)]
        public Sprite divinityBook;

        [PreviewField(75)]
        public Sprite naturalismBook;

        [PreviewField(75)]
        public Sprite shadowcraftBook;

        [PreviewField(75)]
        public Sprite manipulationBook;

        [PreviewField(75)]
        public Sprite pyromaniaBook;
        #endregion

        // Stress State Images
        #region
        [Header("Racial Images")]
        [PreviewField(75)]
        public Sprite human;
        [PreviewField(75)]
        public Sprite orc;
        [PreviewField(75)]
        public Sprite elf;
        [PreviewField(75)]
        public Sprite goblin;
        [PreviewField(75)]
        public Sprite undead;
        [PreviewField(75)]
        public Sprite gnoll;
        [PreviewField(75)]
        public Sprite satyr;
        #endregion

        // Stress State Images
        #region
        [Header("Stress State Images")]
        [PreviewField(75)]
        public Sprite confident;
        [PreviewField(75)]
        public Sprite nervous;
        [PreviewField(75)]
        public Sprite wavering;
        [PreviewField(75)]
        public Sprite panicking;
        [PreviewField(75)]
        public Sprite shattered;
        #endregion

        // Misc Images
        #region
        [Header("Misc Images")]
        [PreviewField(75)]
        public Sprite weaponAbilityIcon;
        #endregion

        // Logic 
        #region
        public Sprite GetRacialSpriteFromEnum(CharacterRace race)
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
        public Sprite GetStressStateSpriteFromEnum(StressState state)
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
        public Sprite GetTalentSchoolSpriteFromEnumData(TalentSchool data)
        {
            Sprite spriteReturned = null;

            if (data == TalentSchool.Scoundrel)
            {
                spriteReturned = scoundrelBadge;
            }
            else if (data == TalentSchool.Warfare)
            {
                spriteReturned = warfareBadge;
            }
            else if (data == TalentSchool.Ranger)
            {
                spriteReturned = rangerBadge;
            }
            else if (data == TalentSchool.Corruption)
            {
                spriteReturned = corruptionBadge;
            }
            else if (data == TalentSchool.Divinity)
            {
                spriteReturned = divinityBadge;
            }
            else if (data == TalentSchool.Guardian)
            {
                spriteReturned = guardianBadge;
            }
            else if (data == TalentSchool.Manipulation)
            {
                spriteReturned = manipulationBadge;
            }
            else if (data == TalentSchool.Naturalism)
            {
                spriteReturned = naturalismBadge;
            }
            else if (data == TalentSchool.Pyromania)
            {
                spriteReturned = pyromaniaBadge;
            }
            else if (data == TalentSchool.Shadowcraft)
            {
                spriteReturned = shadowCraftBadge;
            }
            else if (data == TalentSchool.Neutral)
            {
                spriteReturned = neutralBadge;
            }

            return spriteReturned;
        }
        public Sprite GetAbilityTypeImageFromTypeEnumData(AbilityType abilityType)
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
        public Sprite GetWeaponSpriteFromEnumData(WeaponClass weaponClass)
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
        public Sprite GetTalentSchoolBookFromEnumData(TalentSchool data)
        {
            Sprite spriteReturned = null;

            if (data == TalentSchool.Scoundrel)
            {
                spriteReturned = scoundrelBook;
            }
            else if (data == TalentSchool.Warfare)
            {
                spriteReturned = warfareBook;
            }
            else if (data == TalentSchool.Ranger)
            {
                spriteReturned = rangerBook;
            }
            else if (data == TalentSchool.Divinity)
            {
                spriteReturned = divinityBook;
            }
            else if (data == TalentSchool.Guardian)
            {
                spriteReturned = guadianBook;
            }
            else if (data == TalentSchool.Manipulation)
            {
                spriteReturned = manipulationBook;
            }
            else if (data == TalentSchool.Naturalism)
            {
                spriteReturned = naturalismBook ;
            }
            else if (data == TalentSchool.Pyromania)
            {
                spriteReturned = pyromaniaBook;
            }
            else if (data == TalentSchool.Shadowcraft)
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