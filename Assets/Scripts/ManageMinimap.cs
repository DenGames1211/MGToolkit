using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using StarterAssets;  // make sure this namespace matches your StarterAssets setup

public class ManageMinimap : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Your mini‐map quad or canvas")]
    public GameObject minimapQuad;

    [Tooltip("OVRHand component (left or right)")]
    public OVRHand ovrHand;

    [Tooltip("Root of your player rig (where the CharacterController lives)")]
    public Transform playerRoot;

    [Tooltip("Starter Assets input script")]
    public StarterAssetsInputs inputs;

    [Header("Teleport Settings")]
    [Tooltip("World units per swipe when minimap is active")]
    public float teleportDistance = 10f;

    // Internal
    private OVRHand.MicrogestureType _previousGesture = OVRHand.MicrogestureType.Invalid;
    private CharacterController _charController;

    void Awake()
    {
        // Try to find a CharacterController on the same object as playerRoot
        if (playerRoot != null)
            _charController = playerRoot.GetComponent<CharacterController>();
        if (_charController == null)
            Debug.LogWarning("ManageMinimap: No CharacterController found on playerRoot.", this);
    }

    void Update()
    {
        HandleGestures();
    }

    private void HandleGestures()
    {
        var gesture = ovrHand.GetMicrogestureType();

        // 1) Thumb‑tap toggles minimap (once per tap)
        if (gesture == OVRHand.MicrogestureType.ThumbTap &&
            _previousGesture != OVRHand.MicrogestureType.ThumbTap)
        {
            minimapQuad?.SetActive(!minimapQuad.activeSelf);
        }

        // 2) If minimap is up, swipes *teleport* the player instantly
        if (minimapQuad != null && minimapQuad.activeSelf)
        {
            if (gesture != _previousGesture)
            {
                Vector3 dir = Vector3.zero;
                switch (gesture)
                {
                    case OVRHand.MicrogestureType.SwipeLeft:  dir = Vector3.left;  break;
                    case OVRHand.MicrogestureType.SwipeRight: dir = Vector3.right; break;
                    case OVRHand.MicrogestureType.SwipeForward: dir = Vector3.forward; break;
                    case OVRHand.MicrogestureType.SwipeBackward:dir = Vector3.back;  break;
                }

                if (dir != Vector3.zero)
                    Teleport(dir.normalized * teleportDistance);
            }

            _previousGesture = gesture;
            return;  // skip normal movement while minimap is open
        }

        // 3) Normal swipe‐movement via StarterAssetsInputs
        Vector2 moveInput = Vector2.zero;
        switch (gesture)
        {
            case OVRHand.MicrogestureType.SwipeLeft:   moveInput = Vector2.left;  break;
            case OVRHand.MicrogestureType.SwipeRight:  moveInput = Vector2.right; break;
            case OVRHand.MicrogestureType.SwipeForward:moveInput = Vector2.up;    break;
            case OVRHand.MicrogestureType.SwipeBackward:moveInput = Vector2.down;  break;
        }

        inputs?.MoveInput(moveInput);

        _previousGesture = gesture;
    }

    private void Teleport(Vector3 offset)
    {
        if (playerRoot == null) return;

        if (_charController != null)
        {
            // Disable controller so it doesn’t fight the transform change
            _charController.enabled = false;
            playerRoot.position += offset;
            _charController.enabled = true;
        }
        else
        {
            // No controller—just move the transform
            playerRoot.position += offset;
        }
    }
}
