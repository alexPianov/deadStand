using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel
{
    public class ResultFinish : MonoBehaviour
    {
        [SerializeField] private Button buttonFinish;
        [SerializeField] private Button buttonNext;

        [SerializeField] private ResultSave resultSave;
        [Inject] private CacheAudio _cacheAudio;

        private void Start()
        {
            buttonFinish.onClick.AddListener(FinishSession);
            buttonNext.onClick.AddListener(NextRound);
            
            _cacheAudio.Play(CacheAudio.MenuSound.OnGetItem);
        }

        private void FinishSession()
        {
            Debug.Log("Finish session");
            resultSave.LeaveRoom();
        }

        private void NextRound()
        {
            Debug.Log("Next round");
            resultSave.LeaveRoom();
        }
    }
}