using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Item_Collection : MonoBehaviour
{
    private int[] itemCounts = new int[4]; // 0: Cherry, 1: Banana, 2: Kiwi, 3: Orange

    [SerializeField] private TextMeshProUGUI itemText;

    private PlayerController playerController;

    void Start()
    {
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        UpdateItemText();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Cherry"))
        {
            itemCounts[0]++;
            playerController.AddItem("Cherry");
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Banana"))
        {
            itemCounts[1]++;
            playerController.AddItem("Banana");
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Kiwi"))
        {
            itemCounts[2]++;
            playerController.AddItem("Kiwi");
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Orange"))
        {
            itemCounts[3]++;
            playerController.AddItem("Orange");
            Destroy(collision.gameObject);
        }

        UpdateItemText();
    }

    private void UpdateItemText()
    {
        itemText.text = $"Cherry: {itemCounts[0]} | Banana: {itemCounts[1]} | Kiwi: {itemCounts[2]} | Orange: {itemCounts[3]}";
    }
}
