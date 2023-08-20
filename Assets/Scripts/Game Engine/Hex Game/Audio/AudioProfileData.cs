using UnityEngine;
using Sirenix.OdinInspector;

namespace WeAreGladiators.Audio
{
    [CreateAssetMenu(fileName = "New AudioProfileData", menuName = "Audio Profile Data")]
    public class AudioProfileData : ScriptableObject
    {
        [Header("General Properties")]
        public AudioProfileType audioProfileType;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("SFX Buckets")]
        public AudioModel[] meleeAttackSounds;
        public AudioModel[] dieSounds;
        public AudioModel[] hurtSounds;       
        public AudioModel[] turnStartSounds;
    }

    public enum AudioProfileType
    {
        None = 0,
        Human_1 = 1,
        Elf_1 = 2,
        Orc_1 = 3,
        Undead_1 = 4,
        Goblin_1 = 5,
        Gnoll_1 = 6,
        Satyr_1 = 7,
        Lunker_Youngling = 8,
    }

    public enum AudioSet
    {
        None = 0,
        Attack = 1,
        Hurt = 2,
        Die = 3,
        Buff = 4,
        TurnStart = 5,
    }
}