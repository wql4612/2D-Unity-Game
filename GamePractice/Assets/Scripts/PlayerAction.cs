using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    public bool state;
    public Rigidbody2D rb2d;
    public Logics logics;

    // Start is called before the first frame update
    void Start()
    {
        state = false;
        rb2d = GetComponent<Rigidbody2D>();
        logics = GameObject.FindGameObjectWithTag("Logics").GetComponent<Logics>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!logics.win && Input.GetKeyDown(KeyCode.Space))
        {
            state = !state;
            transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
            rb2d.gravityScale = -rb2d.gravityScale;
        }
    }
}