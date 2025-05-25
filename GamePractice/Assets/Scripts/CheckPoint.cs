using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckPoint : MonoBehaviour
{
    private bool isActive = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player")&&!isActive)
        {
            isActive = true;
            SaveCheckPointData();
        }
    }

    private void SaveCheckPointData()
    {
        Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        PlayerPrefs.SetFloat("CheckpointX", playerPos.x);
        PlayerPrefs.SetFloat("CheckpointY", playerPos.y);
        PlayerPrefs.SetString("CheckpointScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
