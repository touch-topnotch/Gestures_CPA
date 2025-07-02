using UnityEngine;

namespace Components
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioProcessor : ResourcesProcessor<AudioClip>
    {
        [SerializeField] private AudioSource _audioSource;

        private void OnValidate()
        {
            // if (_audioSource == null)
            // {
            //     _audioSource = GetComponent<AudioSource>();
            //     _audioSource.playOnAwake = false;
            //     _audioSource.loop = false;
            // }
        }

        public void PlaySequencedSound(string key, int id)
        {
            ActivateSequencedResource(key, id, (e) =>
            {
                _audioSource.clip = e;
                _audioSource.Play();
            });
        }
        protected override bool shouldAddMissingComponents { get; }

        protected override void ManipulateResource(AudioClip resource)
        {
            _audioSource.PlayOneShot(resource);
        }
    }
}