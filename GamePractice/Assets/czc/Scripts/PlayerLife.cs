using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLife : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private PlayerController playerController;

    [SerializeField] private AudioSource deathSound;

    private bool isDead = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();

        // 清除旧的存档
        PlayerPrefs.DeleteKey("CheckpointX");
        PlayerPrefs.DeleteKey("CheckpointY");
        PlayerPrefs.DeleteKey("CheckpointScene");
        PlayerPrefs.Save();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;
        if (collision.gameObject.CompareTag("Trap"))
        {
            Debug.Log("oncolliding trap2\n");
            isDead = true;
            //deathSound.Play();
            Die();

            // 清空玩家背包鸟类数量
            playerController.ClearBirdInventory();
        }
    }

    private void Die()
    {
        Debug.Log("oncolliding trap\n");
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
        anim.SetTrigger("death");
    }

    // 动画事件调用（在死亡动画最后一帧调用此函数）
    public void OnDeathAnimationComplete()
    {
        RestartLevel();
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
        isDead = false;
    }
}
