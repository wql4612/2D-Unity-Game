using UnityEngine;

public class ObjectSpawn : MonoBehaviour
{
    public GameObject landPrefab;
    public GameObject cherryPrefab;
    public GameObject completePrefab;
    public Transform landSpawnPoint;
    public float landSpawnRate;
    private float _landSpawnTimer;
    public float completeTime;
    private float _flagSpawnTimer;
    private bool _flagSpawn;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Starting LandSpawn");
        for (int i = 0; i < 20; i++)
        {
            Instantiate(landPrefab, landSpawnPoint.position + Vector3.left * 50f * i, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_landSpawnTimer >= landSpawnRate)
        {
            Instantiate(landPrefab, landSpawnPoint.position, Quaternion.identity);
            if (!_flagSpawn && Random.Range(0, 100) <= 10)
            {
                float low = transform.position.y - 100f;
                float high = transform.position.y + 100f;
                Instantiate(cherryPrefab,
                    new Vector3(transform.position.x, Random.Range(low, high), 0),
                    Quaternion.identity);
            }

            _landSpawnTimer = 0;
        }

        if (_flagSpawnTimer >= completeTime && !_flagSpawn)
        {
            Instantiate(completePrefab, landSpawnPoint.position, Quaternion.identity);
            _flagSpawn = true;
        }

        _landSpawnTimer += Time.deltaTime;
        _flagSpawnTimer += Time.deltaTime;
    }
}