using UnityEngine;

public class GroundMover : MonoBehaviour
{
    [Header("Ayarlar")]
    public float speed = 5f;

    // Zemin geniþliðini kod otomatik bulacak
    private float groundWidth;

    void Start()
    {
        // Üzerindeki BoxCollider2D'den geniþliðini hesapla
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            // Unity'de genelde varsayýlan kare 1 birimdir, scale ile çarpýlýr.
            // En güvenli yöntem collider'ýn "size" deðerini almaktýr.
            groundWidth = collider.size.x * transform.localScale.x;
        }
    }

    void Update()
    {
        // Sola kaydýr
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        // Eðer zemin ekranýn solundan tamamen çýktýysa (örneðin -15 noktasýna geldiyse)
        // Buradaki mantýk: X konumu < -(geniþlik) ise baþa sar
        if (transform.position.x < -groundWidth)
        {
            RepositionBackground();
        }
    }

    void RepositionBackground()
    {
        // Mevcut konumdan "2 x Geniþlik" kadar ileriye (saða) ýþýnla.
        // Neden 2 ile çarpýyoruz? Çünkü sahnede 2 tane zemin var.
        // Biri en soldayken, diðeri en saðda. En arkaya geçmesi için 2 boy gitmeli.
        Vector2 vector = new Vector2(groundWidth * 2f, 0);
        transform.position = (Vector2)transform.position + vector;
    }
}