using System.Collections;
using TMPro;
using UnityEngine;

public class czcItemCollection : MonoBehaviour
{
    private int[] itemCounts = new int[4]; // 0: Cherry, 1: Banana, 2: Kiwi, 3: Orange

    [SerializeField] private TextMeshProUGUI itemText;
    [SerializeField] private Sprite placeholderSprite;  // ռλ��ͼ

    private PlayerController playerController;

    void Start()
    {
        itemCounts = GetComponent<PlayerController>().birdInventory;
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        UpdateItemText();
    }

    void Update()
    {
        // �������������������߼�
        UpdateItemText();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Item collected: {collision.gameObject.name}");
        if (collision.CompareTag("Cherry"))
        {
            CollectItem(collision.gameObject, 0, "Cherry");
        }
        else if (collision.CompareTag("Banana"))
        {
            CollectItem(collision.gameObject, 1, "Banana");
        }
        else if (collision.CompareTag("Kiwi"))
        {
            CollectItem(collision.gameObject, 2, "Kiwi");
        }
        else if (collision.CompareTag("Orange"))
        {
            CollectItem(collision.gameObject, 3, "Orange");
        }
    }

    private void CollectItem(GameObject item, int index, string itemName)
    {

        playerController.AddItem(itemName);
        UpdateItemText();

        StartCoroutine(HandleItemRespawn(item, 6f));
    }

    private IEnumerator HandleItemRespawn(GameObject item, float delay)
    {
        Collider2D col = item.GetComponent<Collider2D>();
        SpriteRenderer sr = item.GetComponent<SpriteRenderer>();
        Animator animator = item.GetComponent<Animator>();

        if (sr == null || placeholderSprite == null)
            yield break;

        // ����ԭʼ״̬
        Sprite originalSprite = sr.sprite;
        bool animatorWasEnabled = animator != null && animator.enabled;

        // ���ö���������ռλͼ���ر���ײ��
        if (animator != null) animator.enabled = false;
        sr.sprite = placeholderSprite;
        if (col != null) col.enabled = false;

        yield return new WaitForSeconds(delay);

        // �ָ���������ͼ����ײ��
        sr.sprite = originalSprite;
        if (animator != null) animator.enabled = animatorWasEnabled;
        if (col != null) col.enabled = true;
    }

    private void UpdateItemText()
    {
        itemText.text = $"Cherry: {itemCounts[0]} | Banana: {itemCounts[1]} | Kiwi: {itemCounts[2]} | Orange: {itemCounts[3]}";
    }


}