using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject optionMenu;
    public static bool isMenuOpened = false;

    private void Update()
    {
        // 메뉴가 열려있으면 
        if (isMenuOpened)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {   
                Resume(); // ESC 누르면 모든 메뉴 닫고 게임 재개
            }

            if (Time.timeScale != 0f) Time.timeScale = 0f; // 시간 멈춤
        }
        else
        {

            if (Input.GetKeyDown(KeyCode.Escape) && !pauseMenu.activeSelf)
            {
                pauseMenu.SetActive(true);
                isMenuOpened = true;
                CursorManager.ShowCursor(); // 커서 표시
            }

            if (Time.timeScale == 0f) Time.timeScale = 1f; // 시간 되돌림
        }
    }

    /// <summary>
    /// 게임 재개 버튼
    /// </summary>
    public void Resume()
    {
        CursorManager.HideCursor(); // 커서 숨김

        if (pauseMenu.activeSelf)
            pauseMenu.SetActive(false); // 얼시정지 메뉴 UI 비활성화
        if (optionMenu.activeSelf)
            optionMenu.SetActive(false); // 옵션 UI 비활성화

        isMenuOpened = false;
    }

    /// <summary>
    /// 옵션 메뉴 활성화
    /// </summary>
    public void OpenOptionMenu()
    {
        // 일시 정지 메뉴 숨김
        if (pauseMenu.activeSelf)
            pauseMenu.SetActive(false); // 얼시정지 메뉴 UI 비활성화

        // 옵션 메뉴 표시
        if (!optionMenu.activeSelf)
            optionMenu.SetActive(true); // 옵션 UI 비활성화
    }

    public void CloseOptionMenu()
    {
        // 옵션 메뉴 숨김
        if (optionMenu.activeSelf)
            optionMenu.SetActive(false); // 옵션 UI 비활성화

        // 일시 정지 메뉴 표시
        if (!pauseMenu.activeSelf)
            pauseMenu.SetActive(true); // 얼시정지 메뉴 UI 비활성화
    }

    /// <summary>
    /// 메인 메뉴 이동 버튼
    /// </summary>
    public void ExitToMainMenu()
    {
        if (Time.timeScale == 0f) Time.timeScale = 1f; // 시간 되돌림
        isMenuOpened = false;
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    /// <summary>
    /// 게임 종료 버튼
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }
}
