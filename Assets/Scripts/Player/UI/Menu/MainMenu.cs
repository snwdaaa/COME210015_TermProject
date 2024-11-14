using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("AI"); // AI 테스트용 임시 scene 로드
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
