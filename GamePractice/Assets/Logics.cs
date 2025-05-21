using UnityEngine;
using UnityEngine.UI;

public class Logics : MonoBehaviour
{
    public int score;
    public bool win;

    public Text scoreText;
    public GameObject completeScreen;

    public void AddScore(int s)
    {
        score += s;
        scoreText.text = score.ToString();
    }

    public void Win()
    {
        win = true;
        completeScreen.gameObject.SetActive(true);
    }
}