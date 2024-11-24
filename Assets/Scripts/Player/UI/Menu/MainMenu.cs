using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Update()
    {
        if (!Cursor.visible)
        {
            CursorManager.ShowCursor();
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single); // AI 테스트용 임시 scene 로드
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
