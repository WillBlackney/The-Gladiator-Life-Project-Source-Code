using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using WeAreGladiators.Utilities;
using DG.Tweening;
using WeAreGladiators.Abilities;
using UnityEditor;
using Sirenix.Utilities;
using System.Linq;
using UnityEngine.Audio;

namespace WeAreGladiators.Audio
{
    public class AudioManager : Singleton<AudioManager>
    {
        #region Data + Scene Components
        [Header("Components + Pooling")]
        [SerializeField] private GameObject audioPlayerPrefab;
        [SerializeField] private Transform audioPlayerPoolParent;
        [SerializeField] private List<AudioPlayer> audioPlayerPool;

        [Space(10)]

        [Header("Data")]
        [SerializeField] private AudioDataBox[] allAudioModels;
        [SerializeField] private AudioProfileData[] allProfiles;

        [Space(10)]

        [Header("Audio Mixer")]
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private AudioMixerGroup musicBus;
        [SerializeField] private AudioMixerGroup effectsBus;       

        private AudioDataBox previousCombatTrack = null;

        public AudioMixer AudioMixer => audioMixer;
        public AudioMixerGroup MusicBus => musicBus;
        public AudioMixerGroup EffectsBus => effectsBus;
        #endregion

        #region Initialization + Misc
        public void Convert()
        {
            /*
            foreach(AudioModel am in allAudioModels)
            {
                AudioDataBox data = ScriptableObject.CreateInstance<AudioDataBox>();
                data.name = am.soundType.ToString();

                data.audioClip = am.audioClip;
                data.randomizeClip = am.randomizeClip;
                data.audioClips = am.audioClips.ToList().ToArray();
                data.soundType = am.soundType;
                data.combatCategory = am.combatCategory;
                data.randomizeVolume = am.randomizeVolume;
                data.randomVolumeLowerLimit = am.randomVolumeLowerLimit;
                data.randomVolumeUpperLimit = am.randomVolumeUpperLimit;
                data.volume = am.volume;
                data.randomizePitch = am.randomizePitch;
                data.randomPitchLowerLimit = am.randomPitchLowerLimit;
                data.randomPitchUpperLimit = am.randomPitchUpperLimit;
                data.pitch = am.pitch;
                data.loop = am.loop;


                AssetDatabase.CreateAsset(data, "Assets/SO Assets/Audio/Audio Data Boxes/" + data.name + ".asset");
            }
            AssetDatabase.SaveAssets();*/

        }
        
        #endregion

        #region Audio Player + Pooling Logic
        private AudioPlayer GetNextAvailableAudioPlayer()
        {
            // Find an available player
            AudioPlayer availablePlayer = audioPlayerPool.Find(i => !i.Source.isPlaying);

            // If there arent any available, create new one, add it to pool, then use it
            if (availablePlayer == null) availablePlayer = CreateAndAddAudioPlayerToPool();

            return availablePlayer;
        }
        private AudioPlayer CreateAndAddAudioPlayerToPool()
        {
            AudioPlayer newAP = Instantiate(audioPlayerPrefab, audioPlayerPoolParent).GetComponent<AudioPlayer>();
            audioPlayerPool.Add(newAP);
            return newAP;
        }

        #endregion

        #region Checks and Bools   
        public bool IsSoundPlaying(Sound s)
        {
            return GetActivePlayerForSound(s) != null;
        }
        private AudioPlayer GetActivePlayerForSound(Sound s)
        {
            return audioPlayerPool.Find(player => player.MyCurrentData != null && player.MyCurrentData.soundType == s && player.Source.isPlaying);
        }
        #endregion

        #region Play, Fade and Stop Sounds Logic    
        public void PlaySound(Sound s)
        {
            if (s == Sound.None) return;

            AudioDataBox a = Array.Find(allAudioModels, sound => sound.soundType == s);
            if (a != null)
            {
                // Prevent duplicate sound playing, if marked to do so
                if (!a.allowDuplicates)
                {
                    if (IsSoundPlaying(s)) return;
                }

                // Set up audio player
                AudioPlayer player = GetNextAvailableAudioPlayer();
                player.BuildFromData(a);

                // Play the sound!
                player.Source.Play();
            }
        }
        public void PlaySound(AudioProfileType type, AudioSet set)
        {
            if (GlobalSettings.Instance.PreventAudioProfiles) return;

            // Find the matching profile
            AudioProfileData apd = Array.Find(allProfiles, a => a.audioProfileType == type);

            if (apd == null)
            {
                Debug.LogWarning("PlaySound() could not find an AudioProfileData that matches the type '" + type.ToString() + "'.");
                return;
            }

            // Get the correct sounds
            List<AudioModel> validSounds = new List<AudioModel>();
            AudioModel soundPlayed = null;
            if (set == AudioSet.Die) soundPlayed = apd.dieSounds[RandomGenerator.NumberBetween(0, apd.dieSounds.Length - 1)];
            else if (set == AudioSet.Hurt) soundPlayed = apd.hurtSounds[RandomGenerator.NumberBetween(0, apd.hurtSounds.Length - 1)];
            else if (set == AudioSet.Attack) soundPlayed = apd.meleeAttackSounds[RandomGenerator.NumberBetween(0, apd.meleeAttackSounds.Length - 1)];
            else if (set == AudioSet.TurnStart) soundPlayed = apd.turnStartSounds[RandomGenerator.NumberBetween(0, apd.turnStartSounds.Length - 1)];

            if (soundPlayed == null)
            {
                Debug.LogWarning("PlaySound() did not find any valid sounds within the set '" + set.ToString() + "', returning...");
                return;
            }

            // Set up audio player
            AudioPlayer player = GetNextAvailableAudioPlayer();
            BuildAudioPlayerFromAudioModelData(soundPlayed, player);

            // Play the sound!
            player.Source.Play();
        }
        public void StopSound(Sound s)
        {
            List<AudioPlayer> a = audioPlayerPool.FindAll(player => player.MyCurrentData != null && player.MyCurrentData.soundType == s);
            a.ForEach(i =>
            {
                i.Source.volume = 0f;
                i.Source.Stop();
            });
        }
        public void FadeOutSound(Sound s, float duration)
        {
            AudioPlayer a = GetActivePlayerForSound(s);
            if (a != null)
            {
                a.Source.DOKill();
                a.Source.DOFade(0f, duration).OnComplete(() =>
                {
                    a.Source.volume = 0f;
                    a.Source.Stop();
                });
            }
        }
        public void FadeInSound(Sound s, float duration)
        {
            AudioDataBox a = Array.Find(allAudioModels, sound => sound.soundType == s);
            if (a != null)
            {
                // Prevent duplicate sound playing, if marked to do so
                if (!a.allowDuplicates)
                {
                    if (IsSoundPlaying(s)) return;
                }
                AudioPlayer player = GetNextAvailableAudioPlayer();
                player.BuildFromData(a);
                player.Source.DOKill();
                player.Source.volume = 0f;
                float targetVolume = a.volume;
                player.Source.Play();
                player.Source.DOFade(targetVolume, duration);
            }
        }
        #endregion
        
        #region Misc Logic              
        private void BuildAudioPlayerFromAudioModelData(AudioModel data, AudioPlayer player)
        {
            // Randomize clip
            if (data.randomizeClip) player.Source.clip = data.audioClips[RandomGenerator.NumberBetween(0, data.audioClips.Length - 1)];
            else player.Source.clip = data.audioClip;

            // Randomize pitch if marked to do so
            if (data.randomizePitch) player.Source.pitch = RandomGenerator.NumberBetween(data.randomPitchLowerLimit, data.randomPitchUpperLimit);
            else player.Source.pitch = data.pitch;

            // Randomize volume if marked to do so
            if (data.randomizeVolume) player.Source.volume = RandomGenerator.NumberBetween(data.randomVolumeLowerLimit, data.randomVolumeUpperLimit);
            else player.Source.volume = data.volume;

        }      
        public void FadeOutAllAmbience(float fadeDuration)
        {
            if (IsSoundPlaying(Sound.Ambience_Outdoor_Spooky)) FadeOutSound(Sound.Ambience_Outdoor_Spooky, fadeDuration);
            if (IsSoundPlaying(Sound.Ambience_Town_1)) FadeOutSound(Sound.Ambience_Town_1, fadeDuration);
            if (IsSoundPlaying(Sound.Ambience_Crypt)) FadeOutSound(Sound.Ambience_Crypt, fadeDuration);
            if (IsSoundPlaying(Sound.Environment_Camp_Fire)) FadeOutSound(Sound.Environment_Camp_Fire, fadeDuration);
            if (IsSoundPlaying(Sound.Ambience_Crowd_1)) FadeOutSound(Sound.Ambience_Crowd_1, fadeDuration);
        }
        public void FadeOutAllCombatMusic(float fadeDuration)
        {
            Debug.LogWarning("FadeOutAllCombatMusic()");
            if (IsSoundPlaying(Sound.Music_Basic_Combat_1)) FadeOutSound(Sound.Music_Basic_Combat_1, fadeDuration);            
            if (IsSoundPlaying(Sound.Music_Basic_Combat_2)) FadeOutSound(Sound.Music_Basic_Combat_2, fadeDuration);            
            if (IsSoundPlaying(Sound.Music_Basic_Combat_3)) FadeOutSound(Sound.Music_Basic_Combat_3, fadeDuration);            
            if (IsSoundPlaying(Sound.Music_Elite_Combat_1)) FadeOutSound(Sound.Music_Elite_Combat_1, fadeDuration);            
            if (IsSoundPlaying(Sound.Music_Elite_Combat_2)) FadeOutSound(Sound.Music_Elite_Combat_2, fadeDuration);            
            if (IsSoundPlaying(Sound.Music_Boss_Combat_1)) FadeOutSound(Sound.Music_Boss_Combat_1, fadeDuration);            
            if (IsSoundPlaying(Sound.Music_Boss_Combat_2)) FadeOutSound(Sound.Music_Boss_Combat_2, fadeDuration);            
        }
        public void ForceStopAllCombatMusic()
        {
            Debug.LogWarning("ForceStopAllCombatMusic()");
            StopSound(Sound.Music_Basic_Combat_1);
            StopSound(Sound.Music_Basic_Combat_2);
            StopSound(Sound.Music_Basic_Combat_3);
            StopSound(Sound.Music_Elite_Combat_1);
            StopSound(Sound.Music_Basic_Combat_2);
            StopSound(Sound.Music_Basic_Combat_3);
        }
        public void AutoPlayBasicCombatMusic(float fadeDuration)
        {
            // Find all basic combat music
            AudioDataBox[] basicCombatMusic = Array.FindAll(allAudioModels, sound => sound.combatCategory == CombatMusicCategory.Basic && sound != previousCombatTrack);

            // Choose one track randomly
            AudioDataBox musicSelected = basicCombatMusic[RandomGenerator.NumberBetween(0, basicCombatMusic.Length - 1)];

            // Start music fade in
            FadeInSound(musicSelected.soundType, fadeDuration);

            // Cache track so it cant be played twice in a row
            previousCombatTrack = musicSelected;

        }
        #endregion

        #region Main Menu Music Logic
        private MusicSession currentMusicSession;
        public void PlayMainMenuMusic()
        {
            if (currentMusicSession != null) return;
            MusicSession thisSession = new MusicSession();
            currentMusicSession = thisSession;

            PlaySound(Sound.Music_Main_Menu_Theme_Unlooped_1);
            DelayUtils.DelayedCall(58, () =>
            {
                if (currentMusicSession == thisSession)
                    PlaySound(Sound.Music_Main_Menu_Theme_Looped_1);
            });
        }
        public void StopMainMenuMusic(float fadeSpeed = 1f)
        {
            currentMusicSession = null;
            if (IsSoundPlaying(Sound.Music_Main_Menu_Theme_Unlooped_1)) FadeOutSound(Sound.Music_Main_Menu_Theme_Unlooped_1, fadeSpeed);
            if (IsSoundPlaying(Sound.Music_Main_Menu_Theme_Looped_1)) FadeOutSound(Sound.Music_Main_Menu_Theme_Looped_1, fadeSpeed);
        }
        #endregion

        #region Audio Mixer

        public void SetMasterVolume(float volume)
        {
            audioMixer.SetFloat("master_volume", volume);
        }
        public void SetMusicVolume(float volume)
        {
            audioMixer.SetFloat("music_volume", volume);
        }
        public void SetEffectsVolume(float volume)
        {
            audioMixer.SetFloat("effects_volume", volume);
        }
        public AudioMixerGroup GetBus(MixerBus bus)
        {
            if (bus == MixerBus.Music) return musicBus;
            else return effectsBus;
        }
        #endregion

    }

    public class MusicSession{}

    public enum Sound
    {
        None = 8,

        Ability_Gain_Block = 19,
        Ability_Damaged_Health_Lost = 12,
        Ability_Damaged_Block_Lost = 13,
        Ability_Bloody_Stab = 14,
        Ability_Oomph_Impact = 15,
        Ability_Sword_Ching = 16,
        Ability_Fire_Buff = 17,
        Ability_Holy_Buff = 18,
        Ability_Shadow_Buff = 20,
        Ability_Metallic_Ting = 37,
        Ability_Heal_Twinkle = 54,
        Ability_Dispell_Twang = 55,
        Ability_Enrage_Short = 56,
        Ability_Enrage_Long = 57,
        Ability_Charge_Movement = 95,
        Ability_Charge_Impact = 96,

        Ability_Big_Lightning_Explosion = 134,
        Ability_Dark_Buff = 135,
        Ability_Dark_Debuff = 136,
        Ability_Eagle_Call= 137,
        Ability_Enrage = 138,
        Ability_Go_Invisble = 139,
        Ability_Heroic_Buff = 140,
        Ability_Hymn = 141,
        Ability_Kick_Dirt = 142,
        Ability_Lightning_Buff = 143,
        Ability_Lightning_Explosion = 144,
        Ability_Poison_Debuff = 145,
        Ability_Rocky_Buff = 146,
        Ability_Shoot_Electricity = 152,
        Ability_Sinister_Laugh = 147,
        Ability_Swish = 148,
        Ability_Teleport = 149, 
        Ability_Water_Buff = 150,
        Ability_Twangy_Buff = 151,
        Ability_Cheeky_Laugh = 153,
        Ability_Feast = 154,

        Ambience_Outdoor_Spooky = 39,
        Ambience_Town_1 = 99,
        Ambience_Crypt = 40,
        Ambience_Crowd_1 = 88,       

        Card_Draw = 0,
        Card_Moused_Over = 1,
        Card_Dragging = 2,
        Card_Breathe = 3,
        Card_Discarded = 4,

        Character_Footsteps = 9,
        Character_Draw_Bow = 10,
        Character_Undead_Growl = 58,

        Effects_Story_Event_Start = 131,
        Effects_End_Deployment = 132,
        Effects_Confirm_Level_Up = 133,

        Environment_Gate_Open = 38,
        Environment_Camp_Fire = 49,
        Environment_Arena_Sting_1 = 100,
        Environment_Tavern_Sting_1 = 101,
        Environment_Armoury_Sting_1 = 102,
        Environment_Hospital_Sting_1 = 103,
        Environment_Library_Sting_1 = 104,

        Events_New_Game_Started = 35,
        Events_New_Turn_Notification = 41,

        Explosion_Fire_1 = 21,
        Explosion_Shadow_1 = 22,
        Explosion_Lightning_1 = 22,
        Explosion_Poison_1 = 23,

        Gold_Cha_Ching = 53,
        Gold_Gain = 51,
        Gold_Dropping = 52,

        UI_Chime_1 = 31,
        UI_Rolling_Bells = 32,

        UI_Button_Click = 116,
        UI_Button_Enter = 117,
        UI_Heavy_Click = 115,
        UI_Non_Button_Enter = 130,
        UI_Buy_Item = 107,
        UI_Sell_Item = 119,
        UI_Confirm_Click = 110,
        UI_Cancel_Click = 108,
        UI_Increase_Click = 111,
        UI_Decrease_Click = 112,
        UI_Open_Inventory = 109,
        UI_Close_Inventory = 118,    
        UI_Door_Open = 113,
        UI_Dragging_Constant = 114,  
        UI_Drag_Drop_End = 129,

        
        Item_Wooden_Weapon_Equip = 120,
        Item_Metal_Weapon_Equip = 121,
        Item_Bow_Equip = 122,
        Item_Shield_Equip = 123,
        Item_Trinket_Equip = 124,
        Item_Cloth_Armour_Equip = 125,
        Item_Mail_Armour_Equip = 126,
        Item_Plate_Armour_Equip = 127,
        Item_General_Unequip = 128,      
              
            
        Passive_General_Buff = 5,
        Passive_General_Debuff = 6,
        Passive_Overload_Gained = 24,
        Passive_Burning_Gained = 25,

        Projectile_Arrow_Fired = 26,
        Projectile_Fireball_Fired = 27,
        Projectile_Lightning_Fired = 28,
        Projectile_Poison_Fired = 29,
        Projectile_Shadowball_Fired = 30,

        Music_Basic_Combat_1 = 7,
        Music_Basic_Combat_2 = 42,
        Music_Basic_Combat_3 = 43,
        Music_Elite_Combat_1 = 44,
        Music_Elite_Combat_2 = 45,
        Music_Boss_Combat_1 = 47,
        Music_Boss_Combat_2 = 48,
        Music_Victory_Fanfare = 50,
        Music_Defeat_Fanfare = 59,
        Music_Main_Menu_Theme_Looped_1 = 36,
        Music_Main_Menu_Theme_Unlooped_1 = 106,
        Music_Town_Theme_1 = 98,

        Weapon_Aoe_Swing = 60,
        Weapon_Axe_1H_Swing = 61,
        Weapon_Axe_1H_Hit = 62,
        Weapon_Axe_2H_Swing = 63,
        Weapon_Axe_2H_Hit = 64,
        Weapon_Bow_Shoot = 92,
        Weapon_Bow_Hit = 93,
        Weapon_Crossbow_Shoot = 65,
        Weapon_Crossbow_Hit = 66,
        Weapon_Crossbow_Reload = 94,
        Weapon_Dagger_Swing = 67,
        Weapon_Dagger_Hit = 68,
        Weapon_Hammer_2H_Swing = 69,
        Weapon_Hammer_2H_Hit = 70,
        Weapon_Hammer_1H_Swing = 71,
        Weapon_Hammer_1H_Hit = 72,
        Weapon_Polearm_Swipe_Swing = 73,
        Weapon_Polearm_Swipe_Hit = 74,
        Weapon_Polearm_Thrust_Swing = 75,
        Weapon_Polearm_Thrust_Hit = 76,
        Weapon_Shield_Slam = 77,
        Weapon_Shield_Wall = 97,
        Weapon_Spear_Swing = 78,
        Weapon_Spear_Hit = 79,
        Weapon_Staff_Swing = 80,
        Weapon_Staff_Hit = 81,
        Weapon_Sword_2H_Swing = 82,
        Weapon_Sword_2H_Hit = 83,
        Weapon_Sword_1H_Swing = 84,
        Weapon_Sword_1H_Hit = 85,
        Weapon_Throw_Net = 86,
        Weapon_Split_Shield = 87,

        Crowd_Cheer_1 = 89,
        Crowd_Ooh_1 = 90,
        Crowd_Applause_1 = 91,
        Crowd_Big_Cheer_1 = 105,
    }
    public enum MixerBus
    {
        Effects = 0,
        Music = 1,        
    }
}

