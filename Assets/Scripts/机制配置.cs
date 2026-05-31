using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "开发/机制配置", fileName = "机制配置")]
public class 机制配置 : SerializedScriptableObject
{

    public float 压力板失效延迟 = 1;

    [LabelText("藤蔓茎生长(上下)")]
    public 藤蔓.生长 藤蔓茎生长;
    
    [LabelText("藤蔓头生长(左右)")]
    public 藤蔓.生长 藤蔓头生长;
}
