using UnityEngine;

public class EnvironmentSpawner : MonoBehaviour
{
    [Header("Aðaç Prefablarý")]
    public GameObject[] treePrefabs; 

    [Header("Zamanlama")]
    public float minSpawnTime = 2f;
    public float maxSpawnTime = 5f;
    public float destroyTime = 15f;

    [Header("Konum")]
    public Transform spawnPoint; 

    private float timer;
    private float nextSpawnTime;

    void Start() => SetNextTime();

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= nextSpawnTime)
        {
            SpawnTree();
            timer = 0;
            SetNextTime();
        }
    }

    void SetNextTime() => nextSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);

    void SpawnTree()
    {
        if (treePrefabs.Length == 0) return;

        int index = Random.Range(0, treePrefabs.Length);
        
        GameObject tree = Instantiate(treePrefabs[index], spawnPoint.position, Quaternion.identity);

        
        Destroy(tree, destroyTime);
    }
}