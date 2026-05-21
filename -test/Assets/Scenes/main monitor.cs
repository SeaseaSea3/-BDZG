using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainmonitor : MonoBehaviour
{
    // 这里可以直接拖拽你要控制的物体
    public GameObject targetObject;

    // 初始状态：false = 隐藏，true = 显示
    public bool initialState = false;

    void Start()
    {
        // 游戏开始时设置为初始状态
        if (targetObject != null)
            targetObject.SetActive(initialState);
    }

    void Update()
    {
        // 检测空格键的**单次按下**事件
        // GetKeyDown 只会在按下的那一帧触发一次，防止按住重复触发
        if (Input.GetKeyDown(KeyCode.Space) && targetObject != null)
        {
            // 核心逻辑：取反（! 符号表示逻辑非）
            // 如果现在是激活(true)，点击后变为关闭(false)；反之亦然
            bool currentState = targetObject.activeSelf;
            targetObject.SetActive(!currentState);
        }
    }
}

