using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
    [Header("Ayarlar")]
    public float speed = 10f; // GroundMover hýzýyla ayný olmalý!

    void Update()
    {
        // Engeli her karede SOLA doðru kaydýrýr
        transform.Translate(Vector2.left * speed * Time.deltaTime);
    }
}