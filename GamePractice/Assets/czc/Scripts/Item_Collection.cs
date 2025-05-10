using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_Collection : MonoBehaviour
{
    private int cherries = 0;
    [SerializeField] private Text cherriesText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Cherry")||collision.gameObject.CompareTag("Banana"))
        {
            Debug.Log("Item collected");
            Destroy(collision.gameObject);
            cherries++;
            cherriesText.text = "Cherries:" + cherries;
        }
    }
}
