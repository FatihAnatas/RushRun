using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct ObstacleData
    {
        public GameObject prefab;
        public float yOffset; // Havada kalıyorsa eksi (-), gömülüyorsa artı (+) değer ver

        [Header("Uzun Engeller İçin")]
        public float extraWaitTime; // YENİ: Bu engel çok uzunsa Spawner'ın ekstra bekleyeceği saniye (Örn: 1.5)
    }

    [Header("Ayarlar")]
    public ObstacleData[] obstacles;
    public float minTime = 1f;
    public float maxTime = 3f;
    public float destroyTime = 10f;
    public Transform spawnPoint;
    public LayerMask groundLayer;

    private float timer;
    private float timeBetweenSpawns;

    void Start() => SetRandomTime();

    void Update()
    {
        if (PlayerController.isGameOver) return;

        timer += Time.deltaTime;

        if (timer >= timeBetweenSpawns)
        {
            timer = 0;
            SetRandomTime(); // Önce normal rastgele süreyi (Örn: 1-3 sn arası) belirle

            // Engeli doğur ve eğer bu engelin bir 'extraWaitTime' değeri varsa onu al
            float additionalWait = SpawnObstacle();

            // Seçilen engel uzun bir engelse, o süreyi normal bekleme süremizin üstüne ekle!
            timeBetweenSpawns += additionalWait;
        }
    }

    void SetRandomTime() => timeBetweenSpawns = Random.Range(minTime, maxTime);

    // YENİ: void yerine float döndüren bir metoda çevirdik ki ekstra süreyi Update'e fırlatabilsin
    float SpawnObstacle()
    {
        if (obstacles.Length == 0) return 0f;

        int randomIndex = Random.Range(0, obstacles.Length);
        ObstacleData selected = obstacles[randomIndex];

        GameObject obstacle = Instantiate(selected.prefab, spawnPoint.position, Quaternion.identity);

        // Menzili artırıyoruz: 10 birim yukarıdan başla, 30 birim aşağı bak
        Vector2 rayOrigin = (Vector2)spawnPoint.position + Vector2.up * 10f;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 30f, groundLayer);

        if (hit.collider != null)
        {
            Collider2D col = obstacle.GetComponentInChildren<Collider2D>();
            float bottomOffset = 0;
            if (col != null) bottomOffset = obstacle.transform.position.y - col.bounds.min.y;

            float finalY = hit.point.y + bottomOffset + selected.yOffset;
            obstacle.transform.position = new Vector3(hit.point.x, finalY, 0);
        }
        else
        {
            // KRİTİK KORUMA: Eğer zemin bulunamadıysa, uçan engel oluşmasın diye hemen sil
            Destroy(obstacle);
            Debug.LogWarning("Zemin bulunamadı, engel silindi!");
            return 0f; // Engel silindiyse ekstra süre beklemeye gerek yok
        }

        Destroy(obstacle, destroyTime);

        // YENİ: Bu engelin panelde girdiğin ekstra süresini Update'e gönderiyoruz
        return selected.extraWaitTime;
    }
}