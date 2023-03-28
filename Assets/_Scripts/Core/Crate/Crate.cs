using System;
using Photon.Pun;
using UnityEngine;

namespace Playstel
{
    public class Crate : MonoBehaviourPun
    {
        [HideInInspector] public CrateItems Items; 
        [HideInInspector] public CrateStat Stat;
        [HideInInspector] public CrateBoot Boot;
        [HideInInspector] public CrateHandler Handler;
        
        [Header("Instance")]
        [HideInInspector] public GameObject Instance;

        private void Awake()
        {
            Items = GetComponent<CrateItems>();
            Stat = GetComponent<CrateStat>();
            Boot = GetComponent<CrateBoot>();
            Handler = GetComponent<CrateHandler>();
        }

        public void SetInstance(GameObject instance)
        {
            Instance = instance;
        }

        public void BindInstanceToTrigger()
        {
            Instance.GetComponentInChildren<UICrateButtonLevel>().SetCrate(this);
        }
    }
}