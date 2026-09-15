using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SocialVR
{
    public enum ESoundGroup
    {
        BGM,
        SFX,
        UI
    }

    public class SoundManager : Singleton<SoundManager>
    {
        public AudioMixer audioMixer = null;

        SoundSource _bgmSource = null;
        bool _isBGMPlaying = false;

        public void Play(string path, ESoundGroup group, bool loop)
        {
            SoundSource source = ResourceManager.Instance.Load<SoundSource>(path);
            Play(source, group, loop);
        }

        void Play(SoundSource source, ESoundGroup group, bool loop)
        {
            if (source == null)
                return;

            if (loop)
                source.Play(audioMixer.FindMatchingGroups(group.ToString())[0]);
            else
                source.PlayOneShot(audioMixer.FindMatchingGroups(group.ToString())[0]);

            source.transform.SetParent(this.transform);
        }

        public void PlayBGM(string path)
        {
            _isBGMPlaying = true;

            _bgmSource = ResourceManager.Instance.Load<SoundSource>(path);

            Play(_bgmSource, ESoundGroup.BGM, true);
        }

        public void ResumeBGM()
        {
            if (_isBGMPlaying == true)
                return;

            if (_bgmSource != null)
            {
                Play(_bgmSource, ESoundGroup.BGM, true);
            }
        }

        public void StopBGM()
        {
            if (_bgmSource != null)
            {
                _isBGMPlaying = false;

                _bgmSource.Stop();
            }
        }

        public void PlayUISound(SoundSource soundsource)
        {
            string path = soundsource.name;

            SoundSource source = ResourceManager.Instance.Load<SoundSource>(path);

            if (source != null)
            {
                source.PlayOneShot(audioMixer.FindMatchingGroups(ESoundGroup.SFX.ToString())[0]);

                source.transform.SetParent(this.transform);
            }
        }
    }
}
