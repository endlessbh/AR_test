using UnityEngine;
using UnityEngine.UI;

public class ToggleAudioOnClick : MonoBehaviour
{
    public Button yourButton; // ��ť����
    public AudioSource audioSource; // ��ƵԴ����

    private bool isPlaying = false; // ���ڸ�����Ƶ����״̬

    void Start()
    {
        // Ϊ��ť��ӵ���¼�������
        yourButton.onClick.AddListener(ToggleAudio);
    }

    void ToggleAudio()
    {
        // �л���Ƶ����״̬
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
            isPlaying = !isPlaying; // ���²���״̬
        }
    }
}
