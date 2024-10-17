using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public Text BestScoreText;
    public Text CurrentNameText;
    public GameObject GameOverText;
    public Button ReturnToMenuButton;
    
    private bool m_Started = false;
    private int m_Points;
    
    private bool m_GameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }

        // Set the UI text for current users name and the previous high score and usernam UI.
        if (DataManager.Instance != null)
        {
            CurrentNameText.text = $"Name: {DataManager.Instance.UserName}";
            UpdateHighScoreText();
        }
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    //public void GameOver()
    //{
    //    GameAndUIHandler.Instance.SetBestScore(m_Points);
    //    m_GameOver = true;
    //    GameOverText.SetActive(true);
    //}

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
        ReturnToMenuButton.gameObject.SetActive(true);
        if (DataManager.Instance != null)
        {
            // Will only allow the new highscore to be set if the current points are greater.
            DataManager.Instance.SetHighScore(m_Points);
            UpdateHighScoreText();

            // Adds the highscore to the top ten leaderboards if it is within those scores.
            DataManager.Instance.UpdateTopTenScores(m_Points);
        }
    }

    void UpdateHighScoreText()
    {
        BestScoreText.text = $"Best Score : {DataManager.Instance.HighestScorerUserName} : {DataManager.Instance.HighScore}";
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Start Menu");
    }
}
