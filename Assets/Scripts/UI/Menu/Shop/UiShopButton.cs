using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Playstel.UI
{
    [RequireComponent(typeof(Button))]
    public class UiShopButton : MonoBehaviour
    {
        public UiShop UiShop;
        public UiShop.Product currentProduct;
        public TextMeshProUGUI priceText;
        
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(PurchasePanel);
        }

        public void SetPrice(int price)
        {
            priceText.text = priceText + "$";
        }

        private void PurchasePanel()
        {
            UiShop.OpenProduct(currentProduct);
        }
    }
}
