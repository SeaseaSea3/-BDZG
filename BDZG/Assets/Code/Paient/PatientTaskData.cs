using UnityEngine;

[CreateAssetMenu(fileName = "NewPatientTaskData", menuName = "Surgery Game/Patient Task Data")]
public class PatientTaskData : ScriptableObject
{
    [Header("病人名称")]
    public string patientName;

    [Header("当前病人需要完成的3个小游戏ID")]
    public int[] requiredMiniGameIds = new int[3];

    [Header("每个小游戏完成后的回血量")]
    public float[] healAmounts = new float[3];
}