using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Tyrant;
using UnityEngine;

[CreateAssetMenu(fileName = "玩家配置", menuName = "单例/玩家配置")]
public class 玩家配置 : SingletonSO<玩家配置>
{
    
    
    [LabelText("移动速度")]
    [HorizontalGroup("Basic")]
    [SerializeField] public float moveSpeed = 5f;

    [LabelText("跳跃")]
    [HorizontalGroup("Basic")]
    [SerializeField] public float jumpForce = 10f;
    
    [LabelText("录制时间")]
    [SerializeField] public float maxRecordDuration = 4f;


    [ShowInInspector, SerializeField]
    public 机制配置 机制;

}
