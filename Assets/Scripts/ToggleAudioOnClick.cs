using UnityEngine;
using UnityEngine.UI;

public class ToggleAudioOnClick : MonoBehaviour
{
    public Button yourButton; // 按钮引用
    public AudioSource audioSource; // 音频源引用

    private bool isPlaying = false; // 用于跟踪音频播放状态

    void Start()
    {
        // 为按钮添加点击事件监听器
        yourButton.onClick.AddListener(ToggleAudio);
    }

    void ToggleAudio()
    {
        // 切换音频播放状态
        if (audioSource != null)
        {
            if (isPlaying)
            {
                audioSource.Stop();
            }
            else
            {
                audioSource.Play();
            }
            isPlaying = !isPlaying; // 更新播放状态
        }
    }
}
