using UnityEngine;

public class LandMove : MonoBehaviour
{
    public float moveSpeed;
    public float destroyX;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.left * (moveSpeed * Time.deltaTime));
        if (transform.position.x < destroyX)
        {
            Destroy(gameObject);
        }
    }
}