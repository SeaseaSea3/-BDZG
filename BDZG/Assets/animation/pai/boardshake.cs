using UnityEngine;

public class BoardSwing : MonoBehaviour
{
    [Header("摇晃角度幅度（建议2-5）")]
    public float swingAngle = 3f;
    [Header("摇晃快慢（建议1.5-3）")]
    public float swingSpeed = 2f;

    private Transform anchor;

    void Start()
    {
        // 锚点就是当前物体自身
        anchor = transform;
    }

    void Update()
    {
        // 用正弦曲线实现平滑的左右摆动
        float swing = Mathf.Sin(Time.time * swingSpeed) * swingAngle;
        anchor.localEulerAngles = new Vector3(0, 0, swing);
    }
}
