using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;

public class InputMg : MonoBehaviour
{
    public OVRHand ovrHand;
    public StarterAssets.StarterAssetsInputs inputs;
    private Vector2 charMove = Vector2.zero;

    void Update()
    {
        HandleMovementGestures();
    }

    void HandleMovementGestures()
    {
        OVRHand.MicrogestureType gesture = ovrHand.GetMicrogestureType();

        switch (gesture)
        {
            case OVRHand.MicrogestureType.NoGesture:
                break;
            case OVRHand.MicrogestureType.SwipeLeft:
                charMove = Vector2.left;
                break;
            case OVRHand.MicrogestureType.SwipeRight:
                charMove = Vector2.right;
                break;
            case OVRHand.MicrogestureType.SwipeForward:
                charMove = Vector2.up;
                break;
            case OVRHand.MicrogestureType.SwipeBackward:
                charMove = Vector2.down;
                break;
            case OVRHand.MicrogestureType.ThumbTap:
                charMove = Vector2.zero;
                break;
            case OVRHand.MicrogestureType.Invalid:
                break;
            default:
                break;
        }

        inputs.MoveInput(charMove);
    }

}