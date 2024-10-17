using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance
    { get; private set; }
    public string UserName
    { get; set; }
    public float HighScore
    { get; private set; }
    public string HighestScorerUserName
    { get; set; }
    public string[] TopTenScores
    { get; set; } = new string[10];

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        LoadHighScoreData();
        LoadTopTenScoresData();
    }

    public void SetHighScore(float newScore)
    {
        if (newScore > HighScore)
        {
            HighScore = newScore;
            HighestScorerUserName = UserName;
            // Save the highscore data here once a new highscore has been set;
            SaveHighScoreData();
        }
        Debug.Log("Score: " + HighScore + ", Player: " + UserName);
    }

    [Serializable]
    public class HighScoreData
    {
        public string userName;
        public float highScore;
    }

    void SaveHighScoreData()
    {
        HighScoreData saveData = new HighScoreData();
        saveData.userName = HighestScorerUserName;
        saveData.highScore = HighScore;

        string jsonSaveData = JsonUtility.ToJson(saveData);

        File.WriteAllText(Application.persistentDataPath + "/highScoreData", jsonSaveData);
    }

    void LoadHighScoreData()
    {
        string filePath = Application.persistentDataPath + "/highScoreData";

        if (File.Exists(filePath))
        {
            string jsonSaveData = File.ReadAllText(Application.persistentDataPath + "/highScoreData");

            HighScoreData saveData = JsonUtility.FromJson<HighScoreData>(jsonSaveData);

            HighestScorerUserName = saveData.userName;
            HighScore = saveData.highScore;
        }
    }

    public void UpdateTopTenScores(int newScore)
    {
        for (int i = 0; i < TopTenScores.Length; i++)
        {
            string textToSearch = "Score: ";
            int currentIndexScore = int.Parse(TopTenScores[i].Substring(TopTenScores[i].IndexOf(textToSearch) + textToSearch.Length));

            if (newScore > currentIndexScore)
            {
                // Take the value from the current index and shift them down the array - thereby removing the 10th entry, and creating a space in the current index.
                for (int j = TopTenScores.Length - 1; j >= i; j--)
                {
                    if (j > i)
                    {
                        // Updates the pre-fixed number representing position in the leaderboard. Maintains correct ordering and position of leaderboard.
                        TopTenScores[j] = TopTenScores[j - 1];
                        TopTenScores[j] = j + 1 + TopTenScores[j].Substring(TopTenScores[j].IndexOf('.'));
                    }
                    else if (j == i)
                    {
                        // Add the current new score into the now empty entry.
                        TopTenScores[i] = $"{i + 1}. {UserName} - Score: {newScore}";
                        SaveTopTenScoresData();
                        return;
                    }
                }
            }
        }
    }

    [Serializable]
    public class TopTenScoresData
    {
        public string scores;
    }

    // For each string entry within the top ten scores array, add the string to a new string, with each entry that isn't the last having a newline character appended.
    public string ConvertTopTenScoresToString()
    {
        string scoresAsString = string.Empty;

        for (int i = 0; i < TopTenScores.Length; i++)
        {
            scoresAsString += TopTenScores[i];

            if (i < TopTenScores.Length - 1)
            {
                scoresAsString += Environment.NewLine;
            }
        }

        return scoresAsString;
    }

    // Converts the string into an array, splitting whenever a newline character is found.
    public string[] ConvertTopTenScoresToArray(string stringToConvert)
    {
        string[] scoresAsArray = stringToConvert.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

        return scoresAsArray;
    }

    void SaveTopTenScoresData()
    {
        TopTenScoresData saveData = new TopTenScoresData();
        saveData.scores = ConvertTopTenScoresToString();

        string jsonSaveData = JsonUtility.ToJson(saveData);
        string filePath = Application.persistentDataPath + "/topTenScoresData";

        File.WriteAllText(filePath, jsonSaveData);
    }

    void LoadTopTenScoresData()
    {
        string filePath = Application.persistentDataPath + "/topTenScoresData";

        if (File.Exists(filePath))
        {
            string jsonSaveData = File.ReadAllText(filePath);
            TopTenScoresData saveData = JsonUtility.FromJson<TopTenScoresData>(jsonSaveData);

            TopTenScores = ConvertTopTenScoresToArray(saveData.scores);
        }
        else
        {
            // Provides a default top ten scores if no prior list exisits.
            TopTenScores = new string[]
            {
                "1. Name - Score: 0",
                "2. Name - Score: 0",
                "3. Name - Score: 0",
                "4. Name - Score: 0",
                "5. Name - Score: 0",
                "6. Name - Score: 0",
                "7. Name - Score: 0",
                "8. Name - Score: 0",
                "9. Name - Score: 0",
                "10. Name - Score: 0",
            };
        }
    }
}
