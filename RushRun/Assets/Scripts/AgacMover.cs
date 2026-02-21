using UnityEngine;

public class AgacMover : MonoBehaviour
{
    [Header("Paralaks Ayarý")]
    public float speed = 5f;

    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);
    }
}