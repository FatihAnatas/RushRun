using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct ObstacleData
    {
        public GameObject prefab;
        public float yOffset; // Havada kalıyorsa eksi (-), gömülüyorsa artı (+) değer ver
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
        timer += Time.deltaTime;
        if (timer >= timeBetweenSpawns)
        {
            SpawnObstacle();
            timer = 0;
            SetRandomTime();
        }
    }

    void SetRandomTime() => timeBetweenSpawns = Random.Range(minTime, maxTime);

    void SpawnObstacle()
    {
        if (obstacles.Length == 0) return;
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
        }

        Destroy(obstacle, destroyTime);
    }
}