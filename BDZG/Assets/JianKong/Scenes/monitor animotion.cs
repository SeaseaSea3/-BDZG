using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monitoranimotion : MonoBehaviour
{
   


      
    [Header("引用设置")]
    public Animator animator;
    public int stateCount = 6;

    [Header("运行时显示")]
    public int currentState = 0;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        UpdateAnimatorState();
    }

    void Update()
    {
        // 按 Q 上一个
        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentState--;
            if (currentState < 0)
                currentState = stateCount - 1;
            UpdateAnimatorState();
        }

        // 按 E 下一个
        if (Input.GetKeyDown(KeyCode.E))
        {
            currentState++;
            if (currentState >= stateCount)
                currentState = 0;
            UpdateAnimatorState();
        }
    }

    void UpdateAnimatorState()
    {
        animator.SetInteger("StateIndex", currentState);
        currentState = animator.GetInteger("StateIndex");
        Debug.Log($"当前状态: {currentState}");
    }
}
    

