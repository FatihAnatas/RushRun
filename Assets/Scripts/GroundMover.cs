using UnityEngine;

public class GroundMover : MonoBehaviour
{
    [Header("Ayarlar")]
    public float speed = 5f;

    private float groundWidth;

    void Start()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            groundWidth = collider.size.x * transform.localScale.x;
        }
    }

    void FixedUpdate()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        if (transform.position.x < -groundWidth)
        {
            RepositionBackground();
        }
    }

    void RepositionBackground()
    {
        
        Vector2 vector = new Vector2(groundWidth * 2f, 0);
        transform.position = (Vector2)transform.position + vector;
    }
}