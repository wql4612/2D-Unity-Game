using UnityEngine;

public class LevelComplete : MonoBehaviour
{
    public float moveSpeed;

    public Logics logics;

    // Start is called before the first frame update
    void Start()
    {
        logics = GameObject.FindGameObjectWithTag("Logics").GetComponent<Logics>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.left * (moveSpeed * Time.deltaTime));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Complete");
        logics.Win();
        moveSpeed = 0;
    }
}