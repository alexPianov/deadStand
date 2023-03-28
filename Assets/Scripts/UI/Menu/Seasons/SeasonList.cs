using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Playstel
{
    public class SeasonList : MonoBehaviour
    {
        public List<SeasonSlot> slots;

        public bool newLevelMode;
        public GameObject backButton;

        public void Start()
        {
            foreach (var slot in slots)
            {
                slot.InitialProgressValues();
                
                if (newLevelMode)
                {
                    slot.SetNewLevelMode();
                }
                else
                {
                    slot.SetCommonMode();
                }
            }

            if (newLevelMode)
            {
                ShowBackButton();
            }
        }

        private void ShowBackButton()
        {
            int maxLevelGoals = 0;

            foreach (var slot in slots)
            {
                if (slot.maxLevelReached)
                {
                    maxLevelGoals++;
                }
            }

            if (maxLevelGoals >= slots.Count)
            {
                backButton.SetActive(true);
            }
            else
            {
                backButton.SetActive(false);
            }
        }
    }
}
