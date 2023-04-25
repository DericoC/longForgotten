using UnityEngine;
using System.Collections;

namespace LF.LongForgotten
{
    public class AudioManagerService : MonoBehaviour, IAudioManagerService
    {
        private readonly struct OneShotCoroutine
        {
            public AudioClip Clip { get; }
            public AudioSettings Settings { get; }
            public float Delay { get; }

            public OneShotCoroutine(AudioClip clip, AudioSettings settings, float delay)
            {
                Clip = clip;
                Settings = settings;
                Delay = delay;
            }
        }

        private IEnumerator PlayOneShotAfterDelay(OneShotCoroutine value)
        {
            yield return new WaitForSeconds(value.Delay);
            PlayOneShot_Internal(value.Clip, value.Settings);
        }

        private void PlayOneShot_Internal(AudioClip clip, AudioSettings settings)
        {
            if (clip == null)
                return;

            var newSourceObject = new GameObject($"Audio Source -> {clip.name}");
            var newAudioSource = newSourceObject.AddComponent<AudioSource>();

            newAudioSource.volume = settings.Volume;
            newAudioSource.spatialBlend = settings.SpatialBlend;

            newAudioSource.PlayOneShot(clip);
        }

        #region Audio Manager Service Interface

        public void PlayOneShot(AudioClip clip, AudioSettings settings = default)
        {
            PlayOneShot_Internal(clip, settings);
        }

        public void PlayOneShotDelayed(AudioClip clip, AudioSettings settings = default, float delay = 1.0f)
        {
            StartCoroutine(nameof(PlayOneShotAfterDelay), new OneShotCoroutine(clip, settings, delay));
        }

        #endregion
    }
}