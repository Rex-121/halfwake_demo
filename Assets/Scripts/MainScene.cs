using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 主场景管理脚本
/// </summary>
public class MainScene : MonoBehaviour
{
    /// <summary>
    /// 切换到玩家场景
    /// 可由Button组件的OnClick事件调用
    /// </summary>
    public void LoadPlayerScene()
    {
        SceneManager.LoadScene("游玩");
    }
}