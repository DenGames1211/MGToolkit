using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoProgressUI : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Slider progressSlider;
    private bool isDragging = false;

    void Start()
    {
        if (progressSlider != null)
        {
            progressSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }
    }

    void Update()
    {
        if (videoPlayer != null && progressSlider != null && videoPlayer.isPrepared && !isDragging)
        {
            double progress = videoPlayer.time / videoPlayer.length;
            progressSlider.value = (float)progress;
        }
    }

    void OnSliderValueChanged(float value)
    {
        if (isDragging)
        {
            double newTime = value * videoPlayer.length;
            videoPlayer.time = newTime;
        }
    }

    // Optional: Call these from Event Triggers (on slider)
    public void OnBeginDrag()
    {
        isDragging = true;
    }

    public void OnEndDrag()
    {
        isDragging = false;
        // Seek to the new time after releasing drag
        videoPlayer.time = progressSlider.value * videoPlayer.length;
    }
}
