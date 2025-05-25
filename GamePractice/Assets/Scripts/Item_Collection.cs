using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static AllControl;

public class Item_Collection : MonoBehaviour
{
    int cherries = GameManager.Instance.cherries;
    int strawberries = GameManager.Instance.strawberries;
    int apples = GameManager.Instance.apples;
    [SerializeField] private Text cherriesText;
    [SerializeField] private Text strawberriesText;
    [SerializeField] private Text applesText;

    [SerializeField] private AudioSource collectSound;
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
        if(collision.gameObject.CompareTag("Cherry"))
        {
            collectSound.Play();
            Destroy(collision.gameObject);
            cherries++;
            cherriesText.text = "Cherries:" + cherries;
            GameManager.Instance.cherries = cherries;
        }
        if(collision.gameObject.CompareTag("Strawberry"))
        {
            collectSound.Play();
            Destroy(collision.gameObject);
            strawberries++;
            strawberriesText.text = "Strawberries:" + strawberries;
            GameManager.Instance.strawberries = strawberries;
        }
        if (collision.gameObject.CompareTag("Apple"))
        {
            collectSound.Play();
            Destroy(collision.gameObject);
            apples++;
            applesText.text = "Apples:" + apples;
            GameManager.Instance.apples = apples;
        }
    }
}
