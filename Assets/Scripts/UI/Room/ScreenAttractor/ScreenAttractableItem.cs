using UnityEngine;
using UnityEngine.UI;

public class ScreenAttractableItem : MonoBehaviour
{
    [Header("Info")]
    public string itemName;

    [Header("Settings")]
    private float destroyDistance = 10f;
    private float step = 0.2f;

    [Header("Refs")]
    private Vector3 finalRect;
    private RectTransform attractionRect;
    private float distanceToTarget;

    public void StartAttraction(Vector3 guiPosition)
    {
        Debug.Log("StartAttraction: " + itemName);
        transform.position = guiPosition;

        Init();
    }

    private void Init()
    {
        finalRect = ScreenAttractor.attractor.GetAttractorPos(itemName);

        if (GetComponent<RectTransform>())
        {
            attractionRect = GetComponent<RectTransform>();
        }
    }

    public void Update()
    {
        if (attractionRect)
        {
            attractionRect.position = Lerp(attractionRect.position, finalRect);
            distanceToTarget = DistanceToTarget();

            if (distanceToTarget < destroyDistance)
            {
                ScreenAttractor.attractor.ReceiveItem(itemName);
                Destroy(gameObject);
            }
        }
    }

    private Vector3 Lerp(Vector3 itemPos, Vector3 attractorPos)
    {
        var x = Mathf.Lerp(itemPos.x, attractorPos.x, step);
        var y = Mathf.Lerp(itemPos.y, attractorPos.y, step);
        var z = Mathf.Lerp(itemPos.z, attractorPos.z, step);
        var vector = new Vector3(x, y, z);
        return vector;
    }

    private float DistanceToTarget()
    {
        return Vector3.Distance(attractionRect.position, finalRect);
    }
}
