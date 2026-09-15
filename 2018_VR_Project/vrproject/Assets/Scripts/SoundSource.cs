using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SocialVR
{
    public class SoundSource : MonoBehaviour
    {
        AudioSource _AudioSource = null;
        bool IsLoop = false;

        void Awake()
        {
            _AudioSource = GetComponent<AudioSource>();
            _AudioSource.Stop();
            _AudioSource.enabled = false;
        }

        private void Update()
        {
            if (_AudioSource.enabled && _AudioSource.isPlaying == false && IsLoop == false)
            {
                ResourceManager.Instance.ReturnObject(this.gameObject);
            }
        }

        public void Play(AudioMixerGroup group)
        {
            IsLoop = true;

            if (_AudioSource != null)
            {
                _AudioSource.enabled = true;
                _AudioSource.outputAudioMixerGroup = group;
                _AudioSource.Play();
            }
        }

        public void PlayOneShot(AudioMixerGroup group)
        {
            if (_AudioSource != null)
            {
                _AudioSource.enabled = true;
                _AudioSource.outputAudioMixerGroup = group;
                _AudioSource.PlayOneShot(_AudioSource.clip);
            }
        }

        public void Stop()
        {
            if (_AudioSource != null)
            {
                _AudioSource.Stop();
                _AudioSource.enabled = false;
            }
        }

        public bool IsPlaying()
        {
            if (_AudioSource == null)
                return false;

            return _AudioSource.isPlaying;
        }
    }
}
