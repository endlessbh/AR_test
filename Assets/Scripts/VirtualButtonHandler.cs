using UnityEngine;
using Vuforia; // ȷ�������� Vuforia �����ռ�
using UnityEngine.UI;

public class VirtualButtonHandler : MonoBehaviour
{
    public GameObject virtualButtonObject; // ���ⰴť����
    public GameObject infoPanel; // ���� UI Panel

    private bool isInfoPanelVisible = false;

    void Start()
    {
        // ��ȡ�������ⰴť���
        VirtualButtonBehaviour[] vbs = virtualButtonObject.GetComponentsInChildren<VirtualButtonBehaviour>();
        for (int i = 0; i < vbs.Length; i++)
        {
            vbs[i].RegisterOnButtonPressed(OnButtonPressed);
            vbs[i].RegisterOnButtonReleased(OnButtonReleased);
        }

        // ��ʼ���ؼ�� Panel
        if (infoPanel)
        {
            infoPanel.SetActive(false);
        }
    }

    public void OnButtonPressed(VirtualButtonBehaviour vb)
    {
        // �л���� Panel ����ʾ״̬
        isInfoPanelVisible = !isInfoPanelVisible;
        if (infoPanel)
        {
            infoPanel.SetActive(isInfoPanelVisible);
        }
    }

    public void OnButtonReleased(VirtualButtonBehaviour vb)
    {
        // ���ⰴť�ͷ�ʱ����Ҫ��ʲô
    }
}
