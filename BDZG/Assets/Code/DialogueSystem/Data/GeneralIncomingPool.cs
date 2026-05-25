using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GeneralIncomingPool", menuName = "BBPhone/General Incoming Pool")]
public class GeneralIncomingPool : ScriptableObject
{
    [Tooltip("与监控无关的随机来电起点，拖入多个 DialogueNode 根资源")]
    public List<DialogueNode> startNodes = new List<DialogueNode>();

    public DialogueNode PickRandom()
    {
        if (startNodes == null || startNodes.Count == 0)
            return null;
        return startNodes[Random.Range(0, startNodes.Count)];
    }
}
