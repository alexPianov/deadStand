using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Playstel
{
    public class ResultStars : MonoBehaviour
    {
        public List<UiTransparency> stars = new();
        public List<int> showedStarNumbers = new();

        public void ShowStar(int value)
        {
            showedStarNumbers.Add(value);
            stars[value].Transparency(false);
        }
    }
}