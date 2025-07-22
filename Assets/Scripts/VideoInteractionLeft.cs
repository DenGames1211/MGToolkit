using UnityEngine;
using UnityEngine.Video;

public class HandSwipeVideoPlaylist : MonoBehaviour
{
    public OVRHand hand;
    public VideoPlayer videoPlayer;
    public VideoClip[] videoClips;

    private int currentIndex = 0;
    private OVRHand.MicrogestureType lastGesture = OVRHand.MicrogestureType.NoGesture;

    void Start()
    {
        if (videoClips.Length == 0 || videoPlayer == null || hand == null)
        {
            Debug.LogError("Missing references or empty playlist.");
            enabled = false;
            return;
        }

        videoPlayer.clip = videoClips[currentIndex];
        videoPlayer.Play();
    }

    void Update()
    {
        var currentGesture = hand.GetMicrogestureType();

        if (currentGesture == OVRHand.MicrogestureType.NoGesture)
            return;

        if (currentGesture == OVRHand.MicrogestureType.SwipeLeft)
        {
            GoToNextVideo();
        }
        else if (currentGesture == OVRHand.MicrogestureType.SwipeRight)
        {
            GoToPreviousVideo();
        }

        lastGesture = currentGesture;
    }

    void GoToNextVideo()
    {
        currentIndex = (currentIndex + 1) % videoClips.Length;
        PlayCurrentVideo();
        Debug.Log("Swiped Left → Next video: " + videoClips[currentIndex].name);
    }

    void GoToPreviousVideo()
    {
        currentIndex = (currentIndex - 1 + videoClips.Length) % videoClips.Length;
        PlayCurrentVideo();
        Debug.Log("Swiped Right → Previous video: " + videoClips[currentIndex].name);
    }

    void PlayCurrentVideo()
    {
        videoPlayer.clip = videoClips[currentIndex];
        videoPlayer.Play();
    }
}
