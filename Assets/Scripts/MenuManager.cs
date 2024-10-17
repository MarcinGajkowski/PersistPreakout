using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MenuManager : MonoBehaviour
{
    public TextMeshProUGUI bestScoreText;
    public TMP_InputField iField
    { get; private set; }

    private void Awake()
    {
        iField = GameObject.Find("InputField").GetComponent<TMP_InputField>();
    }

    // Start is called before the first frame update
    void Start()
    {
        iField.onEndEdit.AddListener(value => SetUserName(value));
        if (DataManager.Instance.HighestScorerUserName != "")
        {
            bestScoreText.text = $"Best Score: {DataManager.Instance.HighestScorerUserName} : {DataManager.Instance.HighScore}";
        }
    }

    public void StartMainGame()
    {
        if (iField.text != "")
        {
            SceneManager.LoadScene("main");
        }
        else
        {
            Debug.Log("PLEASE enter a name!");
        }
    }

    public void LoadHighScores()
    {
        SceneManager.LoadScene("High Scores");
    }

    public void ExitApplication()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#elif UNITY_WEBGL
        Application.OpenURL("about:blank");
#else
        Application.Quit();
#endif
    }

    public void SetUserName(string name)
    {
        if (DataManager.Instance != null)
        {
            DataManager.Instance.UserName = name;
        }
    }
}
