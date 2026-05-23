using UnityEngine;

public class BackgroundSwitcher : MonoBehaviour
{
    [Header("背景对象设置")]
    public GameObject mainBackground;
    public GameObject frontDeskBackground;

    // 初始状态设置：游戏一开始就执行
    private void Start()
    {
        // 强制初始状态：main背景显示，前台背景隐藏
        if (mainBackground != null)
            mainBackground.SetActive(true);

        if (frontDeskBackground != null)
            frontDeskBackground.SetActive(false);
    }

    // 鼠标点击到这个Sprite时触发
    private void OnMouseDown()
    {
        // 检查赋值是否完成
        if (mainBackground == null || frontDeskBackground == null)
        {
            Debug.LogError("请在Inspector中给两个背景对象赋值！");
            return;
        }

        // 双向切换：点击一次，main隐藏，前台显示；再点一次，main显示，前台隐藏
        bool isMainActive = mainBackground.activeSelf;
        mainBackground.SetActive(!isMainActive);
        frontDeskBackground.SetActive(isMainActive);

        Debug.Log(isMainActive ? "✅ 已切换到前台背景" : "✅ 已切换回main background");
    }

 
}