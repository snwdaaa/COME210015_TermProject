using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitArea : MonoBehaviour
{
    private BoxCollider exitAreaCollider;
    private GameManager gameManager;

    private void Start()
    {
        exitAreaCollider = GetComponent<BoxCollider>();
        gameManager = GameObject.Find("Managers").GetComponent<GameManager>();
    }

    public void EnableExitArea()
    {
        exitAreaCollider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (GameManager.gameMode == GameManager.GameMode.Doom)
            {
                gameManager.CompleteGame();
            }
            else if (GameManager.gameMode == GameManager.GameMode.Normal)
            {
                SceneManager.LoadScene(2, LoadSceneMode.Single); // 미로맵 Load
                GameManager.gameMode = GameManager.GameMode.Doom; // 모드 변경
            }
        }
    }
}
