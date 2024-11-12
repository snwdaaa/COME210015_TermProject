using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ExitArea : MonoBehaviour
{
    private BoxCollider exitAreaCollider;
    private GameManager gameManager;

    private void Start()
    {
        exitAreaCollider = GetComponent<BoxCollider>();
        gameManager = GameObject.Find("Managers").GetComponent<GameManager>();

        exitAreaCollider.enabled = false; // 기본적으로 시작할 때 자기 자신 비활성화
    }

    public void EnableExitArea()
    {
        exitAreaCollider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            gameManager.CompleteGame();
        }
    }
}
