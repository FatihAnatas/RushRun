using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Ayarlar")]
    public GameObject[] obstaclePrefabs;
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
        // 1. Engeli oluştur
        int randomIndex = Random.Range(0, obstaclePrefabs.Length);
        GameObject obstacle = Instantiate(obstaclePrefabs[randomIndex], spawnPoint.position, Quaternion.identity);

        // 2. Işın at (Sadece Ground katmanını görür, Default olan engellere çarpmaz)
        Vector2 rayOrigin = (Vector2)spawnPoint.position + Vector2.up * 5f;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 20f, groundLayer);

        if (hit.collider != null)
        {
            Collider2D col = obstacle.GetComponentInChildren<Collider2D>();
            if (col != null)
            {
                // Mesafe farkını bul
                float bottomOffset = obstacle.transform.position.y - col.bounds.min.y;

                // Emniyet payı ekle: 0.02f (Gömülmeyi engellemek için minik bir boşluk)
                float safetyMargin = 0.02f;

                obstacle.transform.position = new Vector3(hit.point.x, hit.point.y + bottomOffset + safetyMargin, 0);
            }
        }

        Destroy(obstacle, destroyTime);
    }
}