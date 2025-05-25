using UnityEngine;

public class CherryCollect : MonoBehaviour
{
    public Logics logics;

    // Start is called before the first frame update
    void Start()
    {
        logics = GameObject.FindGameObjectWithTag("Logics").GetComponent<Logics>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.gameObject.name + " entered");
        logics.AddScore(1);
        Destroy(gameObject);
    }
}