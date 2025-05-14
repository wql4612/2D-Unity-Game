using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static AllControl;

public class PlayerLife : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    [SerializeField] private AudioSource deathSound;
    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        PlayerPrefs.DeleteKey("CheckpointX"); // 删除存档点数据
        PlayerPrefs.DeleteKey("CheckpointY");
        PlayerPrefs.DeleteKey("CheckpointScene");
        PlayerPrefs.Save();
    }

    // Update is called once per frame
    void Update()
    {

    }
    //触碰到陷阱死亡
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Trap"))
        {
            deathSound.Play();
            Die();
            GameManager.Instance.score = 0;
        }
    }

    private void Die()
    {
        rb.bodyType = RigidbodyType2D.Static;
        anim.SetTrigger("death");
    }

    private void RestartLevelold()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void RestartLevel()
    {
        if (PlayerPrefs.HasKey("CheckpointX"))
        {

            float x = PlayerPrefs.GetFloat("CheckpointX");
            float y = PlayerPrefs.GetFloat("CheckpointY");
            string sceneName = PlayerPrefs.GetString("CheckpointScene");


            if (SceneManager.GetActiveScene().name != sceneName)
            {
                SceneManager.sceneLoaded += OnSceneLoaded;
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                RebornPlayer(new Vector3(x, y, 0));
            }
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 确保只执行一次
        SceneManager.sceneLoaded -= OnSceneLoaded;

        float x = PlayerPrefs.GetFloat("CheckpointX");
        float y = PlayerPrefs.GetFloat("CheckpointY");
        RebornPlayer(new Vector3(x, y, 0));
    }

    private void RebornPlayer(Vector3 position)
    {
        transform.position = position;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.velocity = Vector2.zero;

        anim.Play("PlayerIdle");
    }
}
