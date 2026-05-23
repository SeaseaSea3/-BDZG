using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerGridMove : MonoBehaviour
{
    [Header("墙壁 Tilemap")]
    public Tilemap wallTilemap;

    [Header("终点")]
    public Transform goal;

    [Header("移动速度")]
    public float moveTime = 0.15f;

    private bool isMoving = false;
    private bool canMove = true;

    private void Start()
    {
        SnapToCellCenter();
    }

    private void Update()
    {
        if (!canMove || isMoving) return;

        Vector3Int dir = Vector3Int.zero;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            dir = Vector3Int.up;
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            dir = Vector3Int.down;
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            dir = Vector3Int.left;
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            dir = Vector3Int.right;

        if (dir != Vector3Int.zero)
        {
            TryMove(dir);
        }
    }

    private void TryMove(Vector3Int dir)
    {
        if (wallTilemap == null)
        {
            Debug.LogError("PlayerGridMove 没有拖入 WallTilemap");
            return;
        }

        Vector3Int currentCell = wallTilemap.WorldToCell(transform.position);
        Vector3Int targetCell = currentCell + dir;

        // 前方是墙：直接重来
        if (wallTilemap.HasTile(targetCell))
        {
            Debug.Log("撞到墙，重新开始");

            canMove = false;

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

            return;
        }

        Vector3 targetWorld = wallTilemap.GetCellCenterWorld(targetCell);
        StartCoroutine(MoveTo(targetWorld));
    }

    private IEnumerator MoveTo(Vector3 targetWorld)
    {
        isMoving = true;

        Vector3 startPos = transform.position;
        float timer = 0f;

        while (timer < moveTime)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, targetWorld, timer / moveTime);
            yield return null;
        }

        transform.position = targetWorld;

        CheckGoal();

        isMoving = false;
    }

    private void CheckGoal()
    {
        if (goal == null)
        {
            Debug.LogError("PlayerGridMove 没有拖入 Goal");
            return;
        }

        Vector3Int playerCell = wallTilemap.WorldToCell(transform.position);
        Vector3Int goalCell = wallTilemap.WorldToCell(goal.position);

        if (playerCell == goalCell)
        {
            canMove = false;

            Debug.Log("迷宫小游戏完成，返回主界面");

            if (MazeGameManager.Instance != null)
            {
                MazeGameManager.Instance.GameWin();
            }
        }
    }

    private void SnapToCellCenter()
    {
        if (wallTilemap == null)
        {
            Debug.LogError("PlayerGridMove 没有拖入 WallTilemap");
            return;
        }

        Vector3Int cell = wallTilemap.WorldToCell(transform.position);
        transform.position = wallTilemap.GetCellCenterWorld(cell);
    }
}