using System;
using System.Collections.Generic;
using UnityEngine;

namespace Playstel
{
    public class NpcCharacterIdleAnim : MonoBehaviour
    {
        private ItemInfo _itemInfo;
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void SetInfo(ItemInfo info)
        {
            _animator.Play(info.itemName);
        }
    }
}
