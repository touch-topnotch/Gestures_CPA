using UnityEngine;

namespace Components
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioProcessor : ResourcesProcessor<AudioClip>
    {
        public AudioSource source;

        private void OnValidate()
        {
            // if (_audioSource == null)
            // {
            //     _audioSource = GetComponent<AudioSource>();
            //     _audioSource.playOnAwake = false;
            //     _audioSource.loop = false;
            // }
        }

        public void PlayOneSound(string key,float volume, bool loop)
        {
            ActivateResource(key, (e) =>
            {
                if (loop)
                {
                    source.clip = e;
                    source.loop = true;
                    source.volume = volume;
                    source.Play();
                }
                else
                {
                    source.PlayOneShot(e,volume);
                }


            });
        }

        public void PlaySequencedSound(string key, int id)
        {
            ActivateSequencedResource(key, id, (e) =>
            {
                source.PlayOneShot(e);
            });
        }
        protected override bool shouldAddMissingComponents { get; }

        protected override void ManipulateResource(AudioClip resource)
        {
            source.PlayOneShot(resource);
        }
    }
}