using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button), typeof(TextMeshProUGUI))]
public class UiCatalogButton : MonoBehaviour
{
    Color buttonColorEnable = new Color(255, 255, 255);
    Color buttonColorDisable = new Color(74, 172, 247);

    private TextMeshProUGUI currentText;
    private GameObject buttonFocus;

    public void Awake()
    {
        currentText = GetComponent<TextMeshProUGUI>();
        buttonFocus = transform.GetChild(0).gameObject;
    }

    public void SetFocus(bool state)
    {
        buttonFocus.SetActive(state);
        if (state) currentText.color = buttonColorEnable;
        else currentText.color = buttonColorDisable;
    }
}
