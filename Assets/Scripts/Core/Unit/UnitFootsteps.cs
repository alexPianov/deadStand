using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Playstel
{
    public class UnitFootsteps : MonoBehaviour
    {
        public AudioSource AudioSource;

        public List<AudioClip> asphalt;
        public List<AudioClip> grass;
        public List<AudioClip> floor;
        public List<AudioClip> metal;
        public List<AudioClip> sand;

        private Transform _transform;
        private string _floorTag;
        
        private void Awake()
        {
            _transform = transform;
        }

        public void FootstepSound()
        {
            if (AudioSource.isPlaying) return;
            
            AudioSource.clip = GetStepClip();
            AudioSource.pitch = RandomValue(0.1f);
            AudioSource.volume = RandomValue(0.1f, 0.15f);
            
            AudioSource.Play();
        }
        
        private float RandomValue(float amplitude, float volume = 1)
        {
            return Random.Range(volume - amplitude, volume + amplitude);
        }
        
        private AudioClip GetStepClip()
        {
            var hit = new RaycastHit();
            
            if(Physics.Raycast(_transform.position, Vector3.down, out hit))
            {
                _floorTag = hit.collider.gameObject.tag;
                
                switch (_floorTag)
                {
                    case "Grass":
                        return grass[Random.Range(0, grass.Count)];
                    
                    case "Asphalt":
                        return asphalt[Random.Range(0, asphalt.Count)];
                    
                    case "Floor":
                        return floor[Random.Range(0, floor.Count)];
                    
                    case "Metal":
                        return metal[Random.Range(0, metal.Count)];
                    
                    case "Sand":
                        return sand[Random.Range(0, sand.Count)];
                    
                    case "Untagged":
                        return asphalt[Random.Range(0, asphalt.Count)];
                }
            }

            return null;
        }
    }
}