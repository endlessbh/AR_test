using UnityEngine;
using Vuforia; // 确保引用了 Vuforia 命名空间
using UnityEngine.UI;

public class VirtualButtonHandler : MonoBehaviour
{
    public GameObject virtualButtonObject; // 虚拟按钮对象
    public GameObject infoPanel; // 简介的 UI Panel

    private bool isInfoPanelVisible = false;

    void Start()
    {
        // 获取所有虚拟按钮组件
        VirtualButtonBehaviour[] vbs = virtualButtonObject.GetComponentsInChildren<VirtualButtonBehaviour>();
        for (int i = 0; i < vbs.Length; i++)
        {
            vbs[i].RegisterOnButtonPressed(OnButtonPressed);
            vbs[i].RegisterOnButtonReleased(OnButtonReleased);
        }

        // 初始隐藏简介 Panel
        if (infoPanel)
        {
            infoPanel.SetActive(false);
        }
    }

    public void OnButtonPressed(VirtualButtonBehaviour vb)
    {
        // 切换简介 Panel 的显示状态
        isInfoPanelVisible = !isInfoPanelVisible;
        if (infoPanel)
        {
            infoPanel.SetActive(isInfoPanelVisible);
        }
    }

    public void OnButtonReleased(VirtualButtonBehaviour vb)
    {
        // 虚拟按钮释放时不需要做什么
    }
}
