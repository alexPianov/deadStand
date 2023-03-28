using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel.UI
{
    public class UiShop : MonoBehaviour
    {
        public enum Product
        {
            BattlePass, LuckyBox, GoldSmall, GoldMedium, GoldLarge
        }

        public void OpenProduct(Product product)
        {
            Purchase(product.ToString());
        }

        public void Purchase(string productName)
        {
            Debug.Log("Purchase " + productName);
            
            GetComponent<UIElementLoadByType>().Load();
        }
    }
}
