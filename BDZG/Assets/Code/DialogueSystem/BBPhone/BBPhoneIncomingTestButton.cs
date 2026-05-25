using UnityEngine;

/// <summary>
/// 测试用：把 Button OnClick 绑到 TriggerIncomingCall() 即可模拟来电。
/// </summary>
public class BBPhoneIncomingTestButton : MonoBehaviour
{
    [SerializeField] private BBPhoneController controller;

    public void TriggerIncomingCall()
    {
        if (controller != null)
            controller.TriggerIncomingCall();
    }
}
