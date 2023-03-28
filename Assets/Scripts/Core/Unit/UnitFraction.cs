using System;
using CodeStage.AntiCheat.ObscuredTypes;
using EventBusSystem;
using Photon.Pun;
using Playstel.UI;
using UnityEngine;

namespace Playstel
{
    public class UnitFraction : MonoBehaviourPun
    {
        public Fraction currentFraction = Fraction.Null;
        public GuiUnitHealth UnitHealth;

        public Color colorBlue;
        public Color colorRed;

        public enum Fraction
        { 
            Null = 0, Red = 1, Blue = 2
        }

        public string UnitColor()
        {
            if (currentFraction == Fraction.Red) return "F24D43";
            if (currentFraction == Fraction.Blue) return "0678C9";
            return "FFFFFF";
        }

        public void SetFractionNumber()
        {
            var fraction = PhotonHandler.GetPlayerHash<int>
                (PhotonHandler.Hash.Fraction, photonView.Owner);

            SetFractionNumber((Fraction)fraction);
        }

        public void SetFractionNumber(Fraction fraction)
        {
            currentFraction = fraction;
            
            UnitHealth.HandleHealthSprite(currentFraction.ToString());
        }
    }
}
