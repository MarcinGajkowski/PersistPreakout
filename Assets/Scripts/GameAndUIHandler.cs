using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameAndUIHandler : MonoBehaviour
{
    public TextMeshProUGUI bestScoreText;
    public TMP_InputField iField;

    public string playerName = "";

    public string bestName;
    public int bestScore;

    public static GameAndUIHandler Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadGameInfo();
    }

    // Start is called before the first frame update
    void Start()
    {
        iField = GameObject.Find("InputField").GetComponent<TMP_InputField>();
        if (bestName != "")
        {
            bestScoreText.text = "Best Score: " + bestName + " : " + bestScore;
        }
    }

    public void SetBestScore(int score)
    {
        if (score > bestScore)
        {
            bestScore = score;
            bestName = playerName;
            SaveGameInfo();
            MainManager.Instance.BestScoreText.text = "Best Score: " + bestName + " : " + bestScore;
        }
        Debug.Log("Score: " + score + ", Player: " + playerName);
    }

    public void StartNew()
    {
        if (iField.text != "")
        {
            playerName = iField.text;
            SceneManager.LoadScene(1);
        }
        else
        {
            Debug.Log("PLEASE enter a name!");
        }
    }

    public void Exit()
    {
        // uuuuuuuhhhhhh
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    [System.Serializable]
    class SaveData
    {
        public string playerName;
        public int bestScore;
    }

    public void SaveGameInfo()
    {
        SaveData data = new SaveData();
        data.playerName = bestName;
        data.bestScore = bestScore;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadGameInfo()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            bestName = data.playerName;
            bestScore = data.bestScore;
        }
    }
}
