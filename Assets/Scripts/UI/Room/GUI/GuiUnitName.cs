using System;
using EventBusSystem;
using TMPro;
using UnityEngine;

namespace Playstel.UI
{
    public class GuiUnitName : MonoBehaviour
    {
        public TextMeshProUGUI unitNickname;

        public void HandleNicknameUpdate(string nickname)
        {
            unitNickname.text = nickname;
        }
        
        public void Active(bool state)
        {
            unitNickname.gameObject.SetActive(state);
        }
    }
}
