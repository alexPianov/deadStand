using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Playstel
{
    public class UiFriendsCatalogButton : UiCatalogButton
    {
        public UiFriends UiFriends;
        public UiFriends.Status currentStatus;
        public bool useAtStart;

        public void Start()
        {
            GetComponent<Button>().onClick.AddListener(OpenFriendList);
            if(useAtStart) OpenFriendList();
        }

        public void OpenFriendList()
        {
            UiFriends.ShowUsers(currentStatus);
        }
    }
}
