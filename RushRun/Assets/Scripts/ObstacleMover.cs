using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
    [Header("Ayarlar")]
    public float speed = 10f; 

    void Update()
    {
        if (PlayerController.isGameOver) return;
        
        transform.Translate(Vector2.left * speed * Time.deltaTime);
    }
}