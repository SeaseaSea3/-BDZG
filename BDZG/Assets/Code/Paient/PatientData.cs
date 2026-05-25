using UnityEngine;

[CreateAssetMenu(fileName = "NewPatientData", menuName = "Game/Patient Data")]
public class PatientData : ScriptableObject
{
    [Header("基础信息")]
    public string patientID;
    public string patientName;

    [TextArea]
    public string patientDescription;

    [Header("病人类型")]
    public PatientType patientType;

    [Header("显示资源")]
    public Sprite prepareSprite;                 // 手术室界面显示的病人图
    public Sprite operateSprite;                 // 手术台界面显示的病人图
    public RuntimeAnimatorController animator;   // 病人动画控制器，可不填

    [Header("血量")]
    public int maxHP = 100;
    public int startHP = 100;

    [Header("该病人需要做的三个手术")]
    public SurgeryType surgery1;
    public SurgeryType surgery2;
    public SurgeryType surgery3;

    [Header("小游戏失败扣血")]
    public int failDamage = 30;

    [Header("结局相关")]
    public bool isImportantPatient;
    public string endingFlag;

    public SurgeryType[] GetSurgeries()
    {
        return new SurgeryType[] { surgery1, surgery2, surgery3 };
    }
}