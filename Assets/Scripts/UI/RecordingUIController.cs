using UnityEngine;
using UniRx;
using Controller;
using Record;

/// <summary>
/// 录制 UI 控制器 - 连接录制系统与 RecordingHUD
/// </summary>
public class RecordingUIController : MonoBehaviour
{
    [Header("引用")]
    [SerializeField] private RecordingHUD recordingHUD;
    [SerializeField] private RecordableController targetController;

    private CompositeDisposable disposables = new CompositeDisposable();

    private void Awake()
    {
        if (targetController == null)
            targetController = FindObjectOfType<RecordableController>();

        if (recordingHUD == null)
            recordingHUD = GetComponent<RecordingHUD>();
    }

    private void Start()
    {
        if (targetController == null || recordingHUD == null)
        {
            Debug.LogWarning("[RecordingUIController] 缺少必要的引用");
            return;
        }

        var rc = RecordController.main;
        if (rc != null)
        {
            // 录制开始
            rc.IsRecording
                .Where(isRecording => isRecording)
                .Subscribe(_ => ShowRecordingUI())
                .AddTo(disposables);

            // 录制结束
            rc.IsRecording
                .Where(isRecording => !isRecording)
                .Subscribe(_ => HideRecordingUI())
                .AddTo(disposables);
        }

        // 每帧更新进度
        Observable.EveryUpdate()
            .Where(_ => targetController.IsCurrentlyRecording)
            .Subscribe(_ => UpdateProgress())
            .AddTo(disposables);
    }

    private void ShowRecordingUI()
    {
        recordingHUD.gameObject.SetActive(true);
        recordingHUD.SetStatus("录制中...");
        recordingHUD.SetProgress(0f);
        recordingHUD.SetTimer(玩家配置.main.maxRecordDuration);
    }

    private void HideRecordingUI()
    {
        recordingHUD.SetStatus("录制完成");
        recordingHUD.SetProgress(1f);
        recordingHUD.SetTimer(0f);
    }

    private void UpdateProgress()
    {
        float progress = targetController.RecordingProgress;
        float remaining = targetController.RemainingTime;

        recordingHUD.SetProgress(progress);
        recordingHUD.SetTimer(remaining);
    }

    private void OnDestroy()
    {
        disposables.Dispose();
    }
}