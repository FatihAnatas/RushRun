using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;  // Takip edilecek karakter
    public float smoothSpeed = 0.125f; // Takip yumuþaklýðý (Düþük = Daha yumuþak)
    public Vector3 offset;    // Kamera karakterden ne kadar uzakta dursun?

    [Header("Kamera Sýnýrý")]
    public float minX = 0f;
    void Start()
    {
        if (target != null)
        {
            // Ýstenen baþlangýç noktasý
            float startX = target.position.x + offset.x;

            // Eðer bu nokta minX'ten (örn: 0) küçükse, kamerayý minX'te tut
            startX = Mathf.Max(startX, minX);

            transform.position = new Vector3(startX, transform.position.y, transform.position.z);
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Ýstenen X noktasý
        float desiredX = target.position.x + offset.x;

        // Sýnýrýn altýna inmesini engelle (Mario mantýðý)
        desiredX = Mathf.Max(desiredX, minX);

        Vector3 desiredPosition = new Vector3(desiredX, transform.position.y, transform.position.z);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothedPosition;
    }
}