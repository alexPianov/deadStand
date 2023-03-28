using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Playstel
{
    public class GuiButtonInventory : MonoBehaviour
    {
        public GameObject inventoryPanel;
        
        private void Start()
        {
            BindButtonListener();
        }

        private void BindButtonListener()
        {
            GetComponent<Button>().onClick.AddListener(Active);
        }

        private void Active()
        {
            inventoryPanel.SetActive(true);
        }
    }
}
