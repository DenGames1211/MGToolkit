using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class VideoTimeDisplay : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public TMPro.TextMeshProUGUI timeText;

    void Update()
    {
        if (videoPlayer.isPlaying)
        {
            double time = videoPlayer.time;
            double duration = videoPlayer.length;

            string currentTime = FormatTime(time);
            string totalTime = FormatTime(duration);

            timeText.text = currentTime + " / " + totalTime;
        }
    }

    string FormatTime(double timeInSeconds)
    {
        int minutes = Mathf.FloorToInt((float)timeInSeconds / 60F);
        int seconds = Mathf.FloorToInt((float)timeInSeconds - minutes * 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
