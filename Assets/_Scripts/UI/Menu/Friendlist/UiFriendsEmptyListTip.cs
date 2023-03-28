using System.Collections.Generic;
using System.Linq;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;

namespace Playstel
{
    public class UiFriendsEmptyListTip : MonoBehaviour
    {
        public TextMeshProUGUI tipText;

        public void SetEmptyListTip(UiFriends.Status status)
        {
            switch (status)
            {
                case UiFriends.Status.Confirmed:
                    SetTipText("You don't have any friends yet.");
                    break;
                case UiFriends.Status.External:
                    SetTipText("User is not found. Check your input and try again.");
                    break;
                case UiFriends.Status.Pending:
                    SetTipText("You don't have any users waiting for friendship confirmation from you.");
                    break;
                case UiFriends.Status.Invited:
                    SetTipText("You don't have invited friends. Send an invite code to your friends. You will receive a gold pack when friend uses this code.");
                    break;
                case UiFriends.Status.Requested:
                    SetTipText("You have not sent friendship requests.");
                    break;
            }
        }

        public void SetTipText(string text)
        {
            tipText.text = text;
        }
    }
}
