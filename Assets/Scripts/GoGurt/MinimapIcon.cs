using UnityEngine;
using UnityEngine.UI;

public class MinimapIcon : MonoBehaviour
{
    public Transform player; // Player's Transform (Drag in Inspector)
    public RectTransform minimapImage; // Player icon RectTransform
    public RectTransform minimapPanel; // Minimap panel RectTransform

    public Vector2 mapWorldSize = new Vector2(100f, 100f); // Adjust based on track size
    private Vector2 mapUISize;

    public Vector3 minimapOrigin = new Vector3(-50, 0, -50); // Bottom-left corner of world space

    void Start()
    {
        mapUISize = minimapPanel.sizeDelta; // Get the minimap UI size
    }

    void Update()
    {
        Vector3 playerPos = player.position;

        // Convert world position to minimap position
        float mappedX = ((playerPos.x - minimapOrigin.x) / mapWorldSize.x) * mapUISize.x;
        float mappedY = ((playerPos.z - minimapOrigin.z) / mapWorldSize.y) * mapUISize.y;

        // Update the icon position
        minimapImage.anchoredPosition = new Vector2(mappedX, mappedY);

        // Rotate the icon to match player rotation
        minimapImage.rotation = Quaternion.Euler(0, 0, -player.eulerAngles.y);
    }
}
