using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DetectWall : MonoBehaviour
{
    [SerializeField]public GameObject square;
    [SerializeField] private KeyCode key;
    [SerializeField] public TextMeshPro textMesh;

    // Start is called before the first frame update
    void Start()
    {
        if (square == null)
        {
            square = transform.Find("Square").gameObject;
        }
        if(key == KeyCode.None)
        {
            Debug.Log("No key assigned! Set to KeyCode.N");
            key = KeyCode.N;
        }
        else if(key < KeyCode.A || key > KeyCode.Z)
        {
            Debug.Log("Invalid key! Set to KeyCode.N");
            key = KeyCode.N;
        }
        if (textMesh != null)
        {
            textMesh.text = key.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            BoxCollider2D squareCollider = square.GetComponent<BoxCollider2D>();
            if (squareCollider != null)
            {
                Debug.Log("Wall disabled!");
                squareCollider.enabled = false;
            }
        }else if (Input.GetKeyUp(key))
        {
            BoxCollider2D squareCollider = square.GetComponent<BoxCollider2D>();
            if (squareCollider != null)
            {
                Debug.Log("Wall enabled!");
                squareCollider.enabled = true;
            }
        }
    }
}
