using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 录制 HUD 画布控制脚本
/// </summary>
public class RecordingHUD : MonoBehaviour
{
    [Header("UI 元素 (TextMeshPro)")]
    [SerializeField] private TMP_Text statusText;     // 状态文字，例如“录制中...”
    [SerializeField] private Image progressBar;       // 进度条（Image 仍用 UnityEngine.UI）
    [SerializeField] private TMP_Text timerText;      // 倒计时文字，如“3.2s”

    /// <summary>
    /// 更新主状态文字。
    /// </summary>
    public void SetStatus(string msg)
    {
        if (statusText) statusText.text = msg;
    }

    /// <summary>
    /// 设置进度条填充量，值 0~1。
    /// </summary>
    public void SetProgress(float value)
    {
        if (progressBar) progressBar.fillAmount = Mathf.Clamp01(value);
    }

    /// <summary>
    /// 显示剩余录制时间（秒）。
    /// </summary>
    public void SetTimer(float seconds)
    {
        if (timerText)
            timerText.text = seconds > 0 ? seconds.ToString("F1") + "s" : "";
    }
}