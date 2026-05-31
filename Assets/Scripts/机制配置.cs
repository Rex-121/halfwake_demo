using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "开发/机制配置", fileName = "机制配置")]
public class 机制配置 : SerializedScriptableObject
{

    public int 压力板失效延迟 = 1;
    
}
