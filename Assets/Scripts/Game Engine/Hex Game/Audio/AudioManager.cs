using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using HexGameEngine.Utilities;
using DG.Tweening;

namespace HexGameEngine.Audio
{
    public class AudioManager : Singleton<AudioManager>
    {
        [Header("Components + Pooling")]
        [SerializeField] private GameObject audioPlayerPrefab;
        [SerializeField] private Transform audioPlayerPoolParent;
        [SerializeField] private List<AudioPlayer> audioPlayerPool;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Properties")]
        private AudioModel[] allAudioModels;
        private AudioModel previousCombatTrack = null;

        [Header("Audio Profiles")]
        [SerializeField] private AudioProfileData[] allProfiles;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Music")]
        [SerializeField] private AudioModel[] allMusic;

        [Header("Card SFX")]
        [SerializeField] private AudioModel[] allCardSFX;

        [Header("Ability SFX")]
        [SerializeField] private AudioModel[] allAbilitySFX;

        [Header("Weapon SFX")]
        [SerializeField] private AudioModel[] allWeaponSFX;

        [Header("Character SFX")]
        [SerializeField] private AudioModel[] allCharacterSFX;

        [Header("Projectile SFX")]
        [SerializeField] private AudioModel[] allProjectileSFX;

        [Header("Explosion SFX")]
        [SerializeField] private AudioModel[] allExplosionSFX;

        [Header("Crowd SFX")]
        [SerializeField] private AudioModel[] allCrowdSFX;

        [Header("Passive SFX")]
        [SerializeField] private AudioModel[] allPassiveSFX;

        [Header("GUI SFX")]
        [SerializeField] private AudioModel[] allGuiSFX;

        [Header("Item SFX")]
        [SerializeField] private AudioModel[] allItemSFX;

        [Header("Events SFX")]
        [SerializeField] private AudioModel[] allEventsSFX;

        [Header("Environments SFX")]
        [SerializeField] private AudioModel[] allEnvironmentsSFX;

        [Header("Ambience SFX")]
        [SerializeField] private AudioModel[] allAmbienceSFX;

       

        // Initialization 
        #region
        protected override void Awake()
        {
            base.Awake();

            // Add all audio effects to persistency
            List<AudioModel> allAudioModelsList = new List<AudioModel>();

            // Add individual arrays
            allAudioModelsList.AddRange(allMusic);
            allAudioModelsList.AddRange(allCardSFX);
            allAudioModelsList.AddRange(allAbilitySFX);
            allAudioModelsList.AddRange(allWeaponSFX);
            allAudioModelsList.AddRange(allCharacterSFX);
            allAudioModelsList.AddRange(allProjectileSFX);
            allAudioModelsList.AddRange(allExplosionSFX);
            allAudioModelsList.AddRange(allPassiveSFX);
            allAudioModelsList.AddRange(allGuiSFX);
            allAudioModelsList.AddRange(allEventsSFX);
            allAudioModelsList.AddRange(allEnvironmentsSFX);
            allAudioModelsList.AddRange(allAmbienceSFX);
            allAudioModelsList.AddRange(allCrowdSFX);
            allAudioModelsList.AddRange(allItemSFX);

            // Convert list to array
            allAudioModels = allAudioModelsList.ToArray();

            // Create an audio source for each audio model
            foreach (AudioModel a in allAudioModels)
            {
                a.source = gameObject.AddComponent<AudioSource>();
                a.source.playOnAwake = false;
                a.source.clip = a.audioClip;
                a.source.volume = a.volume;
                a.source.pitch = a.pitch;
                a.source.loop = a.loop;
            }
        }
        #endregion

        // Trigger Sounds
        #region
        public AudioModel GetAudioModel(Sound s)
        {
            AudioModel a = Array.Find(allAudioModels, sound => sound.soundType == s);
            return a;
        }
        public bool IsSoundPlaying(Sound s)
        {
            bool bReturned = false;

            AudioModel a = Array.Find(allAudioModels, sound => sound.soundType == s);

            if (a == null)
            {
                Debug.Log("AudioManager.IsSoundPlaying() couln't find the sound: " + s.ToString());
            }

            if (a != null && a.source.isPlaying)
            {
                bReturned = true;
            }

            return bReturned;
        }
        public void PlaySoundPooled(Sound s)
        {
            if (s == Sound.None) return;

            AudioModel a = Array.Find(allAudioModels, sound => sound.soundType == s);
            if (a != null)
            {
                // Set up audio player
                AudioPlayer player = GetNextAvailableAudioPlayer();
                BuildAudioPlayerFromAudioModelData(a, player);

                // Play the sound!
                player.source.Play();
            }
        }
        public void PlaySound(Sound s)
        {
            if (s == Sound.None)
            {
                return;
            }

            AudioModel a = Array.Find(allAudioModels, sound => sound.soundType == s);
            if (a != null)
            {
                a.fadingIn = false;
                a.fadingOut = false;

                // Randomize clip
                if (a.randomizeClip) a.source.clip = a.audioClips[RandomGenerator.NumberBetween(0, a.audioClips.Length - 1)];
                else a.source.clip = a.audioClip;

                // Randomize pitch if marked to do so
                if (a.randomizePitch) a.source.pitch = RandomGenerator.NumberBetween(a.randomPitchLowerLimit, a.randomPitchUpperLimit);
                else a.source.pitch = a.pitch;

                // Randomize volume if marked to do so
                if (a.randomizeVolume) a.source.volume = RandomGenerator.NumberBetween(a.randomVolumeLowerLimit, a.randomVolumeUpperLimit);
                else a.source.volume = a.volume;

                a.source.Play();
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
            player.source.Play();
        }
        private void BuildAudioPlayerFromAudioModelData(AudioModel data, AudioPlayer player)
        {
            // Randomize clip
            if (data.randomizeClip) player.source.clip = data.audioClips[RandomGenerator.NumberBetween(0, data.audioClips.Length - 1)];
            else player.source.clip = data.audioClip;

            // Randomize pitch if marked to do so
            if (data.randomizePitch) player.source.pitch = RandomGenerator.NumberBetween(data.randomPitchLowerLimit, data.randomPitchUpperLimit);
            else player.source.pitch = data.pitch;

            // Randomize volume if marked to do so
            if (data.randomizeVolume) player.source.volume = RandomGenerator.NumberBetween(data.randomVolumeLowerLimit, data.randomVolumeUpperLimit);
            else player.source.volume = data.volume;

        }
        public void StopSound(Sound s)
        {
            AudioModel a = Array.Find(allAudioModels, sound => sound.soundType == s);
            if (a != null)
            {
                a.fadingIn = false;
                a.fadingOut = false;
                a.source.Stop();
            }
            else
            {
                Debug.LogWarning("AudioManager.StopSound() did not find an audio model with the name " + name);
            }
        }
        public void FadeOutSound(Sound s, float duration)
        {
            AudioModel a = Array.Find(allAudioModels, sound => sound.soundType == s);
            if (a != null)
            {
                a.source.FadeOut(duration, a);
            }
            else
            {
                Debug.LogWarning("AudioManager.FadeOutSound() did not find an audio model with the name " + name);
            }
        }
        public void FadeInSound(Sound s, float duration)
        {
            AudioModel a = Array.Find(allAudioModels, sound => sound.soundType == s);
            if (a != null)
            {
                a.source.FadeIn(duration, a);
            }
            else
            {
                Debug.LogWarning("AudioManager.FadeInSound() did not find an audio model with the name " + name);
            }
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
            if (IsSoundPlaying(Sound.Music_Basic_Combat_1))
            {
                FadeOutSound(Sound.Music_Basic_Combat_1, fadeDuration);
            }
            if (IsSoundPlaying(Sound.Music_Basic_Combat_2))
            {
                FadeOutSound(Sound.Music_Basic_Combat_2, fadeDuration);
            }
            if (IsSoundPlaying(Sound.Music_Basic_Combat_3))
            {
                FadeOutSound(Sound.Music_Basic_Combat_3, fadeDuration);
            }
            if (IsSoundPlaying(Sound.Music_Elite_Combat_1))
            {
                FadeOutSound(Sound.Music_Elite_Combat_1, fadeDuration);
            }
            if (IsSoundPlaying(Sound.Music_Elite_Combat_2))
            {
                FadeOutSound(Sound.Music_Elite_Combat_2, fadeDuration);
            }
            if (IsSoundPlaying(Sound.Music_Boss_Combat_1))
            {
                FadeOutSound(Sound.Music_Boss_Combat_1, fadeDuration);
            }
            if (IsSoundPlaying(Sound.Music_Boss_Combat_2))
            {
                FadeOutSound(Sound.Music_Boss_Combat_2, fadeDuration);
            }
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
            AudioModel[] basicCombatMusic = Array.FindAll(allAudioModels, sound => sound.combatCategory == CombatMusicCategory.Basic && sound != previousCombatTrack);

            // Choose one track randomly
            AudioModel musicSelected = basicCombatMusic[RandomGenerator.NumberBetween(0, basicCombatMusic.Length - 1)];

            // Start music fade in
            musicSelected.source.FadeIn(fadeDuration, musicSelected);

            // Cache track so it cant be played twice in a row
            previousCombatTrack = musicSelected;

        }
        public void AutoPlayEliteCombatMusic(float fadeDuration)
        {
            /*
            // Find all basic combat music
            AudioModel[] eliteCombatMusic = Array.FindAll(allAudioModels, sound => sound.combatCategory == CombatMusicCategory.Elite && sound != previousCombatTrack);

            // Choose one track randomly
            AudioModel musicSelected = eliteCombatMusic[RandomGenerator.NumberBetween(0, eliteCombatMusic.Length - 1)];

            // Start music fade in
            musicSelected.source.FadeIn(fadeDuration, musicSelected);

            // Cache track so it cant be played twice in a row
            previousCombatTrack = musicSelected;
            */
        }
        public void AutoPlayBossCombatMusic(float fadeDuration)
        {
            /*
            // Find all basic combat music
            AudioModel[] bossCombatMusic = Array.FindAll(allAudioModels, sound => sound.combatCategory == CombatMusicCategory.Boss && sound != previousCombatTrack);

            // Choose one track randomly
            AudioModel musicSelected = bossCombatMusic[RandomGenerator.NumberBetween(0, bossCombatMusic.Length - 1)];

            // Start music fade in
            musicSelected.source.FadeIn(fadeDuration, musicSelected);

            // Cache track so it cant be played twice in a row
            previousCombatTrack = musicSelected;
            */
        }
        #endregion

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

        // Audio Player + Pooling Logic
        #region
        private AudioPlayer GetNextAvailableAudioPlayer()
        {
            AudioPlayer availablePlayer = null;

            // Find an available player
            foreach (AudioPlayer ap in audioPlayerPool)
            {
                if (ap.source.isPlaying == false)
                {
                    availablePlayer = ap;
                    break;
                }
            }

            // If there arent any available, create new one, add it to pool, then use it
            if (availablePlayer == null)
            {
                Debug.LogWarning("GetNextAvailableAudioPlayer() couldn't find an available player, creating a new one");
                availablePlayer = CreateAndAddAudioPlayerToPool();
            }

            return availablePlayer;
        }

        private AudioPlayer CreateAndAddAudioPlayerToPool()
        {
            AudioPlayer newAP = Instantiate(audioPlayerPrefab, audioPlayerPoolParent).GetComponent<AudioPlayer>();
            audioPlayerPool.Add(newAP);
            // Debug.LogWarning("AudioManager total player pool count = " + audioPlayerPool.Count.ToString());
            return newAP;
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
}

// AUDIO SOURCE EXTENSIONS!!
namespace UnityEngine
{
    public static class AudioSourceExtensions
    {
        public static void FadeOut(this AudioSource a, float duration, HexGameEngine.Audio.AudioModel data)
        {
            a.DOKill();
            data.fadingIn = false;
            data.fadingOut = true;
            a.DOFade(0f, duration).OnComplete(() =>
            {
                a.Stop();
                a.volume = data.volume; 
                data.fadingOut = false;
            });
            //a.GetComponent<MonoBehaviour>().StartCoroutine(FadeOutCore(a, duration, data));
        }

        private static IEnumerator FadeOutCore(AudioSource a, float duration, HexGameEngine.Audio.AudioModel data)
        {
            data.fadingIn = false;
            data.fadingOut = true;
            float startVolume = data.volume;

            while (a.volume > 0 && data.fadingOut)
            {
                a.volume -= startVolume * Time.deltaTime / duration;
                if (a.volume <= 0)
                {
                    a.Stop();
                    a.volume = startVolume;
                    data.fadingOut = false;
                }
                yield return new WaitForEndOfFrame();
            }


        }
        public static void FadeIn(this AudioSource a, float duration, HexGameEngine.Audio.AudioModel data)
        {
            a.DOKill();
            data.fadingOut = false;
            data.fadingIn = true;
            a.volume = 0f;
            a.Play();
            a.DOFade(data.volume, duration).OnComplete(() =>
            {
                data.fadingIn = false;
            });
        }
        private static IEnumerator FadeInCore(AudioSource a, float duration, HexGameEngine.Audio.AudioModel data)
        {
            data.fadingOut = false;
            data.fadingIn = true;

            float endVolume = data.volume;
            a.volume = 0f;
            a.Play();

            while (a.volume < endVolume && data.fadingIn)
            {
                a.volume += endVolume * Time.deltaTime / duration;

                if (a.volume >= endVolume)
                {
                    a.volume = endVolume;
                    data.fadingIn = false;
                }

                yield return new WaitForEndOfFrame();
            }
        }
    }
}
