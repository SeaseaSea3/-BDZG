using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [Header("五个拼图块")]
    public HexPuzzlePiece[] pieces;

    [Header("开局是否随机旋转角度")]
    public bool randomRotationOnStart = true;

    private int selectedIndex = 0;
    private bool gameOver = false;

    void Start()
    {
        if (randomRotationOnStart)
        {
            RandomRotateAllPieces();
        }

        SelectPiece(0);
    }

    void Update()
    {
        if (gameOver)
        {
            return;
        }

        HandleSelectInput();
        HandleRotateInput();
    }

    private void RandomRotateAllPieces()
    {
        foreach (HexPuzzlePiece piece in pieces)
        {
            piece.RandomRotation();
        }
    }

    private void HandleSelectInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            MoveSelection(Vector2.up);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            MoveSelection(Vector2.down);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            MoveSelection(Vector2.left);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            MoveSelection(Vector2.right);
        }
    }

    private void HandleRotateInput()
    {
        if (pieces == null || pieces.Length == 0)
        {
            return;
        }

        HexPuzzlePiece currentPiece = pieces[selectedIndex];

        if (currentPiece.IsRotating())
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentPiece.RotateLeft(CheckFinish);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            currentPiece.RotateRight(CheckFinish);
        }
    }

    private void MoveSelection(Vector2 direction)
    {
        int nextIndex = selectedIndex;

        /*
            Piece0        Piece3

                   Piece2

            Piece1        Piece4
        */

        if (selectedIndex == 0)
        {
            if (direction == Vector2.down)
            {
                nextIndex = 1;
            }
            else if (direction == Vector2.right)
            {
                nextIndex = 2;
            }
        }
        else if (selectedIndex == 1)
        {
            if (direction == Vector2.up)
            {
                nextIndex = 0;
            }
            else if (direction == Vector2.right)
            {
                nextIndex = 2;
            }
        }
        else if (selectedIndex == 2)
        {
            if (direction == Vector2.left)
            {
                nextIndex = 0;
            }
            else if (direction == Vector2.right)
            {
                nextIndex = 3;
            }
            else if (direction == Vector2.down)
            {
                nextIndex = 1;
            }
            else if (direction == Vector2.up)
            {
                nextIndex = 0;
            }
        }
        else if (selectedIndex == 3)
        {
            if (direction == Vector2.down)
            {
                nextIndex = 4;
            }
            else if (direction == Vector2.left)
            {
                nextIndex = 2;
            }
        }
        else if (selectedIndex == 4)
        {
            if (direction == Vector2.up)
            {
                nextIndex = 3;
            }
            else if (direction == Vector2.left)
            {
                nextIndex = 2;
            }
        }

        SelectPiece(nextIndex);
    }

    private void SelectPiece(int index)
    {
        if (pieces == null || pieces.Length == 0)
        {
            return;
        }

        for (int i = 0; i < pieces.Length; i++)
        {
            pieces[i].SetSelected(false);
        }

        selectedIndex = Mathf.Clamp(index, 0, pieces.Length - 1);

        pieces[selectedIndex].SetSelected(true);
    }

    private void CheckFinish()
    {
        if (AllCorrect())
        {
            gameOver = true;

            Debug.Log("游戏结束：所有拼图都旋转回原来的角度！");

            for (int i = 0; i < pieces.Length; i++)
            {
                pieces[i].SetSelected(false);
            }
        }
    }

    private bool AllCorrect()
    {
        foreach (HexPuzzlePiece piece in pieces)
        {
            if (!piece.IsCorrect())
            {
                return false;
            }
        }

        return true;
    }
}