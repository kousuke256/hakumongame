using UnityEngine;
using UnityEngine.Video;

public class VideoLoopController : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    // 交互に再生する動画
    public VideoClip[] videos;

    private int currentIndex = 0;

    void Start()
    {
        // 動画が最後まで再生されたときに呼ばれる
        videoPlayer.loopPointReached += OnVideoEnded;

        // 最初の動画を再生
        PlayVideo(0);
    }

    void PlayVideo(int index)
    {
        if (videos.Length == 0) return;

        currentIndex = index;

        videoPlayer.isLooping = false;
        videoPlayer.clip = videos[currentIndex];
        videoPlayer.Play();
    }

    void OnVideoEnded(VideoPlayer vp)
    {
        // 次の動画に切り替える
        int nextIndex = (currentIndex + 1) % videos.Length;
        PlayVideo(nextIndex);
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoEnded;
        }
    }
}