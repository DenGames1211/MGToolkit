using UnityEngine;
using UnityEngine.Video;

public class VideoPlaylistPlayer : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public VideoClip[] playlist;
    private int currentIndex = 0;

    void Start()
    {
        if (playlist.Length == 0 || videoPlayer == null) return;

        videoPlayer.loopPointReached += OnVideoEnd;
        PlayCurrentVideo();
    }

    void PlayCurrentVideo()
    {
        videoPlayer.clip = playlist[currentIndex];
        videoPlayer.Play();
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        currentIndex = (currentIndex + 1) % playlist.Length;
        PlayCurrentVideo();
    }
}
