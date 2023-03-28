using UnityEngine;

public class ScreenAttractableItemTrigger : MonoBehaviour
{
    public string itemName;

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag != "Player") return;

        // if (collider.GetComponent<Character>())
        // {
        //     
        // }
    }
}
