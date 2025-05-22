using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class czcPlayerLife : MonoBehaviour
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

        // ����ɵĴ浵
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

            // �����ұ�����������
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

    // �����¼����ã��������������һ֡���ô˺�����
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
