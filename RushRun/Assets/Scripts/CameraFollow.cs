using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;  // Takip edilecek karakter
    public float smoothSpeed = 0.125f; // Takip yumuþaklýðý (Düþük = Daha yumuþak)
    public Vector3 offset;    // Kamera karakterden ne kadar uzakta dursun?

    [Header("Kamera Sýnýrlarý")]
    public float minX = 0f;
    public float minY = 0f; // YENÝ: Kameranýn inebileceði en alt nokta (Zeminin altýný görmemek için)

    void Start()
    {
        if (target != null)
        {
            // Ýstenen baþlangýç noktasý (Hem X hem Y için)
            float startX = target.position.x + offset.x;
            float startY = target.position.y + offset.y;

            // Eðer bu noktalar sýnýrlardan küçükse, kamerayý sýnýrda tut
            startX = Mathf.Max(startX, minX);
            startY = Mathf.Max(startY, minY);

            transform.position = new Vector3(startX, startY, transform.position.z);
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 1. Ýstenen X ve Y noktalarýný karakterin o anki konumuna göre belirle
        float desiredX = target.position.x + offset.x;
        float desiredY = target.position.y + offset.y; // YENÝ: Y eksenini de hesaba katýyoruz

        // 2. Sýnýrýn altýna inmesini engelle (Karakter çukura/suya düþtüðünde kamera peþinden yeraltýna girmesin)
        desiredX = Mathf.Max(desiredX, minX);
        desiredY = Mathf.Max(desiredY, minY);

        // 3. Hedef pozisyonu oluþtur (Z eksenini ellemiyoruz, genelde -10'da kalmalýdýr)
        Vector3 desiredPosition = new Vector3(desiredX, desiredY, transform.position.z);

        // 4. Kamerayý hedef pozisyona yumuþakça (Smooth) kaydýr
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothedPosition;
    }
}