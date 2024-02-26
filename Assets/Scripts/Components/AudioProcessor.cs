using UnityEngine;

namespace Components
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioProcessor : ResourcesProcessor<AudioClip>
    {
        [SerializeField] private AudioSource _audioSource;
        
        private void OnValidate()
        {
            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
                _audioSource.playOnAwake = false;
                _audioSource.loop = false;
            }
        }

        protected override void ManipulateResource(AudioClip resource)
        {
            _audioSource.PlayOneShot(resource);
        }
    }
}