// MiniMapFollow.cs
using UnityEngine;

public class MiniMapFollow : MonoBehaviour
{
    [Tooltip("Drag your OVRCameraRig (or its root) here")]
    public Transform playerRoot;

    [Tooltip("Height above the player to position the mini‑map camera")]
    public float mapHeight = 50f;

    void LateUpdate()
    {
        if (playerRoot == null) return;

        Vector3 newPos = playerRoot.position;
        newPos.y = mapHeight;
        transform.position = newPos;

        // Ensure it's looking straight down
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
