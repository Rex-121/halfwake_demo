using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace 藤蔓
{
        [Serializable, InlineProperty, Title("生长")]
        public struct 生长
        {
            [SerializeField, HorizontalGroup, LabelWidth(30)]
            public float 速度;
            [SerializeField, HorizontalGroup, LabelWidth(30)]
            public float 距离;
        }
}