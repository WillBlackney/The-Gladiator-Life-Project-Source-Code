using WeAreGladiators.Utilities;
using System.Collections;
using System.Collections.Generic;
using TbsFramework.Players;
using UnityEngine;
using UnityEngine.Audio;

namespace WeAreGladiators.Audio
{
    public class AudioPlayer : MonoBehaviour
    {
        [SerializeField] AudioSource source;
        AudioDataBox myCurrentData;

        public AudioSource Source => source;
        public AudioDataBox MyCurrentData => myCurrentData;

        public void BuildFromData(AudioDataBox data)
        {
            myCurrentData = data;
            source.loop = myCurrentData.loop;
            source.outputAudioMixerGroup = AudioManager.Instance.GetBus(myCurrentData.bus);

            // Randomize clip
            if (myCurrentData.randomizeClip) source.clip = myCurrentData.audioClips[RandomGenerator.NumberBetween(0, myCurrentData.audioClips.Length - 1)];
            else source.clip = myCurrentData.audioClip;

            // Randomize pitch if marked to do so
            if (myCurrentData.randomizePitch) source.pitch = RandomGenerator.NumberBetween(myCurrentData.randomPitchLowerLimit, myCurrentData.randomPitchUpperLimit);
            else source.pitch = myCurrentData.pitch;

            // Randomize volume if marked to do so
            if (myCurrentData.randomizeVolume) source.volume = RandomGenerator.NumberBetween(myCurrentData.randomVolumeLowerLimit, myCurrentData.randomVolumeUpperLimit);
            else source.volume = myCurrentData.volume;
        }
    }
}