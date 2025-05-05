using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameExit : MonoBehaviour
{
    [SerializeField] private GameObject confirmationPanel; // 引用确认弹窗的UI面板
    [SerializeField] private Button yesButton;           // 确认按钮
    [SerializeField] private Button noButton;            // 取消按钮

    void Start()
    {
        // 确保开始时弹窗是隐藏的
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(false);
        }

        // 为按钮添加监听器
        if (yesButton != null)
        {
            yesButton.onClick.AddListener(ConfirmExit);
        }

        if (noButton != null)
        {
            noButton.onClick.AddListener(CancelExit);
        }
    }

    public void OnButtonClick()
    {
        // 显示确认弹窗
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(true);
        }
    }

    private void ConfirmExit()
    {
        // 实际退出游戏
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    private void CancelExit()
    {
        // 隐藏确认弹窗
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(false);
        }
    }
}
