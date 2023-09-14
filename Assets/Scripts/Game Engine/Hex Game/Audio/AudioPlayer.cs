using UnityEngine;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.Audio
{
    public class AudioPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource source;

        public AudioSource Source => source;
        public AudioDataBox MyCurrentData { get; private set; }

        public void BuildFromData(AudioDataBox data)
        {
            MyCurrentData = data;
            source.loop = MyCurrentData.loop;
            source.outputAudioMixerGroup = AudioManager.Instance.GetBus(MyCurrentData.bus);

            // Randomize clip
            if (MyCurrentData.randomizeClip)
            {
                source.clip = MyCurrentData.audioClips[RandomGenerator.NumberBetween(0, MyCurrentData.audioClips.Length - 1)];
            }
            else
            {
                source.clip = MyCurrentData.audioClip;
            }

            // Randomize pitch if marked to do so
            if (MyCurrentData.randomizePitch)
            {
                source.pitch = RandomGenerator.NumberBetween(MyCurrentData.randomPitchLowerLimit, MyCurrentData.randomPitchUpperLimit);
            }
            else
            {
                source.pitch = MyCurrentData.pitch;
            }

            // Randomize volume if marked to do so
            if (MyCurrentData.randomizeVolume)
            {
                source.volume = RandomGenerator.NumberBetween(MyCurrentData.randomVolumeLowerLimit, MyCurrentData.randomVolumeUpperLimit);
            }
            else
            {
                source.volume = MyCurrentData.volume;
            }
        }
    }
}
