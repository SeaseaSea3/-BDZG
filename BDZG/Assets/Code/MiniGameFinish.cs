using UnityEngine;

public class MiniGameFinish : MonoBehaviour
{
    [Header("当前小游戏ID")]
    public int miniGameId;

    private bool hasFinished = false;

    public void FinishMiniGame()
    {
        if (hasFinished)
        {
            return;
        }

        hasFinished = true;

        if (TreatmentGameManager.Instance == null)
        {
            Debug.LogError("没有找到 TreatmentGameManager！");
            return;
        }

        TreatmentGameManager.Instance.CompleteMiniGame(miniGameId);
    }
}