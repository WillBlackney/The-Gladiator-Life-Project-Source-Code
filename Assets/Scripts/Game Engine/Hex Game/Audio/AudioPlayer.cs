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
        [SerializeField] AudioDataBox myCurrentData;

        public AudioSource Source => source;
        public AudioDataBox MyCurrentData => myCurrentData;

        public void BuildFromData(AudioDataBox data)
        {
            myCurrentData = data;
            source.loop = data.loop;
            source.outputAudioMixerGroup = AudioManager.Instance.GetBus(data.bus);

            // Randomize clip
            if (data.randomizeClip) Source.clip = data.audioClips[RandomGenerator.NumberBetween(0, data.audioClips.Length - 1)];
            else Source.clip = data.audioClip;

            // Randomize pitch if marked to do so
            if (data.randomizePitch) Source.pitch = RandomGenerator.NumberBetween(data.randomPitchLowerLimit, data.randomPitchUpperLimit);
            else Source.pitch = data.pitch;

            // Randomize volume if marked to do so
            if (data.randomizeVolume) Source.volume = RandomGenerator.NumberBetween(data.randomVolumeLowerLimit, data.randomVolumeUpperLimit);
            else Source.volume = data.volume;
        }
    }
}