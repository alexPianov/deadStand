using System.Collections;
using Cysharp.Threading.Tasks;
using EventBusSystem;
using Photon.Pun;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class UnitAnimation : MonoBehaviourPun
    {
        public Animator rigController;
        public Animator animator;

        private Unit _unit;

        public void Awake()
        {
            _unit = GetComponent<Unit>();
        }

        public enum Actions
        {
            Pick, Shot, Throw, Recoil, Reload, Melee
        }

        int currentLayer;
        public void ItemAnimation(Actions action, ItemInfo item = null, bool instantAnimation = false, int layer = 0)
        {
            string animationAddress = action.ToString();

            if (action is Actions.Shot)
            {
                animator.SetTrigger(animationAddress);
                return;
            }

            if (!item)
            {
                item = _unit.HandleItems.currentItem.info;
            }

            if (action is Actions.Melee)
            {
                var num = Random.Range(0, 3);
                animationAddress += "_" + num;
                layer = 3;
            }

            if (action is Actions.Recoil)
            {
                animationAddress += "_" + item.ItemSubclass;
                layer = 4;
            }

            if (action is Actions.Pick)
            {
                if (item.itemClass is ItemInfo.Class.Firearm)
                {
                    animationAddress += "_" + item.itemName;
                }
                
                if (item.itemClass is ItemInfo.Class.Melee)
                {
                    animationAddress += "_" + item.itemClass;
                }
                
                if (item.itemClass is ItemInfo.Class.Grenade)
                {
                    animationAddress += "_" + item.itemName;
                }

                layer = 0;
            }

            if (action is Actions.Reload)
            {
                animationAddress += "_" + item.itemName;
                layer = 2;
            }

            if(action is Actions.Throw)
            {
                animationAddress += "_" + item.itemClass;
                layer = 2;
            }

            var normalizedTime = 0;
            if (instantAnimation) normalizedTime = 1;

            if(rigController) rigController.Play(animationAddress, layer, normalizedTime);
            currentLayer = layer;
        }
        
        public async UniTask AwaitAnimation(float waitingTimeSec = 0)
        {
            if (!_unit.Await) return; 
            
            _unit.Await.Await(true);
            
            await UniTask.Delay(100);
            
            if (waitingTimeSec == 0)
            {
                waitingTimeSec = GetCurrentAnimationTime() - 0.15f;
            }
            
            var waitingTimeMSec = Mathf.RoundToInt(waitingTimeSec * 1000);
            
            _unit.Await.SetMaxAwaitTime(waitingTimeSec);

            if (waitingTimeMSec < 0) waitingTimeMSec = 100;
            
            await UniTask.Delay(waitingTimeMSec);
            
            _unit.Await.Await(false);
        }

        public float GetCurrentAnimationTime()
        {
            return rigController.GetCurrentAnimatorStateInfo(currentLayer).length;
        }

        public void Hit()
        {
            if(rigController) rigController.Play("Hit", 1);
        }

        public void LeftFootStep()
        {

        }

        public void RightFootStep()
        {

        }
    }
}
