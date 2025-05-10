using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class YellowBird_Collection : MonoBehaviour
{
    private int yellowBirds = 0;
    private int redBirds = 0;


    [SerializeField] private TextMeshProUGUI BirdCounter;

private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("YellowBird"))
        {
            Debug.Log("YellowBird collected");
            collision.gameObject.GetComponent<CollectibleItem>()?.PlayerCollected();
            yellowBirds++;
            BirdCounter.text = "YellowBirds: " + yellowBirds;
        }
        if (collision.gameObject.CompareTag("RedBird"))
        {
            Debug.Log("RedBird collected");
            collision.gameObject.GetComponent<CollectibleItem>()?.PlayerCollected();
            redBirds++;
            BirdCounter.text = "RedBirds: " + redBirds;
        }

    }
}