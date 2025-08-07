using System.Collections.Generic;
using UnityEngine;

public class CloudController : MonoBehaviour
{
    [Header("Clouds settings")]
    public GameObject[] cloudPrefabs;  // Массив префабов облаков
    public int cloudCount = 10;        // Количество облаков в начале

    [Header("Spawn Areas")]
    public Vector2 spawnAreaMin;       // Минимальная координата (X,Z)
    public Vector2 spawnAreaMax;       // Максимальная координата (X,Z)

    public Vector2 cloudAreaMin;       // Минимальная координата (X,Z)
    public Vector2 cloudAreaMax;       // Максимальная координата (X,Z)
    public float spawnHeight = 20f;    // Высота спавна облаков

    [Header("Clouds move")]
    public Vector3 moveDirection = Vector3.right; // Направление движения
    public float minSpeed = 1f;   // Мин. скорость
    public float maxSpeed = 3f;   // Макс. скорость
    public float updateTime = 0.25f;


    public float currentTime =0f;
    private List<Cloud> clouds = new List<Cloud>();

    void Start()
    {
        spawnAreaMin.x = spawnAreaMin.x + transform.position.x;
        spawnAreaMin.y = spawnAreaMin.y + transform.position.z;

        spawnAreaMax.x = spawnAreaMax.x + transform.position.x;
        spawnAreaMax.y = spawnAreaMax.y + transform.position.z;

        cloudAreaMin.x = cloudAreaMin.x + transform.position.x;
        cloudAreaMin.y = cloudAreaMin.y + transform.position.z;

        cloudAreaMax.x = cloudAreaMax.x + transform.position.x;
        cloudAreaMax.y = cloudAreaMax.y + transform.position.z;



        for (int i = 0; i < cloudCount; i++)
        {
            SpawnCloud(true);
        }
    }

    void FixedUpdate()
    {

        currentTime += Time.deltaTime;

        if (currentTime > updateTime)
        {
            // Двигаем облака
            for (int i = clouds.Count - 1; i >= 0; i--)
            {
                clouds[i].Move();

                // Проверка выхода за границы
                if (!IsInsideArea(clouds[i].gameObject.transform.position))
                {
                    Destroy(clouds[i].gameObject);
                    clouds.RemoveAt(i);
                    SpawnCloud(false);
                }
            }

            currentTime = 0;
        }
    }

    void SpawnCloud(bool randomPosition)
    {
        if (cloudPrefabs.Length == 0) return;

        GameObject cloudPrefab = cloudPrefabs[Random.Range(0, cloudPrefabs.Length)];
        GameObject newCloud = Instantiate(cloudPrefab, GetSpawnPosition(randomPosition), Quaternion.identity);

        float speed = Random.Range(minSpeed, maxSpeed);
        clouds.Add(new Cloud(newCloud, speed, moveDirection));
    }

    Vector3 GetSpawnPosition(bool random)
    {
        float x = random ? Random.Range(cloudAreaMin.x, cloudAreaMax.x) : Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float z = random ? Random.Range(cloudAreaMin.y, cloudAreaMax.y) : Random.Range(spawnAreaMin.y, spawnAreaMax.y);
        return new Vector3(x, spawnHeight, z);
    }

    bool IsInsideArea(Vector3 position)
    {
        return position.x >= cloudAreaMin.x && position.x <= cloudAreaMax.x &&
               position.z >= cloudAreaMin.y && position.z <= cloudAreaMax.y;
    }

    class Cloud
    {
        public GameObject gameObject;
        public float speed;
        private Vector3 direction;

        public Cloud(GameObject obj, float spd, Vector3 dir)
        {
            gameObject = obj;
            speed = spd;
            direction = dir;
        }

        public void Move()
        {
            gameObject.transform.position += direction * speed * Time.deltaTime;
        }
    }
}
