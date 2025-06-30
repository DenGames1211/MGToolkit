using UnityEngine;

public class InputMg : MonoBehaviour
{
    public OVRHand ovrHand;
    private Vector2 charMove = Vector2.zero;
    public StarterAssets.StarterAssetsInputs inputs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        OVRHand.MicrogestureType gesture = ovrHand.GetMicrogestureType();

        switch (gesture) {
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
