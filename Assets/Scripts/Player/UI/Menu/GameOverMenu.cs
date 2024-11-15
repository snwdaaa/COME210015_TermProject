using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public GameObject gameOverMenu;

    private void Update()
    {
        if (gameOverMenu.activeSelf)
        {
            CursorManager.ShowCursor();
        }
    }

    /// <summary>
    /// 게임 다시 시작
    /// </summary>
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single); // AI 테스트용 임시 scene 로드
    }

    /// <summary>
    /// 게임 종료 버튼
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }
}
