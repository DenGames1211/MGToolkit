using UnityEngine;
using UnityEngine.Video;

public class MicroGestureVideoControl : MonoBehaviour
{
    public OVRHand hand;
    public VideoPlayer videoPlayer;
    [Range(0f, 1f)] public float volumeStep = 0.1f;

    void Update()
    {
        if (hand == null || videoPlayer == null)
            return;

        var currentGesture = hand.GetMicrogestureType();

        if (currentGesture == OVRHand.MicrogestureType.NoGesture)
            return;

        switch (currentGesture)
        {
            case OVRHand.MicrogestureType.ThumbTap:
                Debug.Log("ThumbsUp: Play or Stop");
                if (videoPlayer.isPlaying) PauseVideo();
                else if (!videoPlayer.isPlaying) PlayVideo();
                break;

            case OVRHand.MicrogestureType.SwipeLeft:
                Debug.Log("SwipeLeft: Rewind");
                RewindVideo();
                break;

            case OVRHand.MicrogestureType.SwipeRight:
                Debug.Log("SwipeRight: Forward");
                ForwardVideo();
                break;

            case OVRHand.MicrogestureType.SwipeForward:
                Debug.Log("SwipeForward: Pause");
                IncreaseVolume();
                break;

            case OVRHand.MicrogestureType.SwipeBackward:
                Debug.Log("SwipeBackward: Play");
                DecreaseVolume();
                break;
        }
    }

    void PlayVideo()
    {
        if (!videoPlayer.isPlaying)
            videoPlayer.Play();
    }

    void PauseVideo()
    {
        if (videoPlayer.isPlaying)
            videoPlayer.Pause();
    }

    void StopVideo()
    {
        videoPlayer.Stop();
    }

    void RewindVideo()
    {
        videoPlayer.time = Mathf.Max(0, (float)videoPlayer.time - 10f);
    }

    void ForwardVideo()
    {
        videoPlayer.time = Mathf.Min((float)videoPlayer.length, (float)videoPlayer.time + 10f);
    }
    
    void IncreaseVolume()
    {
        float newVolume = Mathf.Clamp(videoPlayer.GetDirectAudioVolume(0) + volumeStep, 0f, 1f);
        videoPlayer.SetDirectAudioVolume(0, newVolume);
        Debug.Log($"Volume increased to {newVolume:F2}");
    }

    void DecreaseVolume()
    {
        float newVolume = Mathf.Clamp(videoPlayer.GetDirectAudioVolume(0) - volumeStep, 0f, 1f);
        videoPlayer.SetDirectAudioVolume(0, newVolume);
        Debug.Log($"Volume decreased to {newVolume:F2}");
    }
}
