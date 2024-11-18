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
