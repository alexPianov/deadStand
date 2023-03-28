using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Playstel
{
    public class RoomSoundtrack : MonoBehaviour
    {
        [Inject] private CacheSoundClips _cacheSoundClips;
        [Inject] private CacheAudio _cacheAudio;
        private AudioClip currentClip;
        
        public void PlayRoundSoundtrack()
        {
            var soundtracks = _cacheSoundClips.levelMusic;
            _cacheAudio.PlayMusic(soundtracks);
        }

        private void Update()
        {
            if (currentClip && !_cacheAudio.musicSource.isPlaying)
            {
                PlayRoundSoundtrack();
            }
        }
    }
}