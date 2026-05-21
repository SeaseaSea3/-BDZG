using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickToState : MonoBehaviour
{
    [Header("设置")]
    public int targetStateIndex; // 这个物体点击后要跳转到的状态索引
    public Animator mainAnimator; // 引用主状态机

    void OnMouseDown()
    {
        if (mainAnimator != null)
        {
            // 设置参数，驱动状态机跳转
            mainAnimator.SetInteger("StateIndex", targetStateIndex);
            Debug.Log($"点击了 {gameObject.name}，切换到状态索引: {targetStateIndex}");
        }
    }
}
