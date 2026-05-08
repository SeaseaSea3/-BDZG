using UnityEngine;

public class HookHeadTrigger : MonoBehaviour
{
    public HookStretchController controller;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Åöµ½ÁË£º" + other.name);

        if (other.CompareTag("Cell"))
        {
            controller.FailGame();
        }
        else if (other.CompareTag("Bullet"))
        {
            controller.ClearGame();
        }
    }
}