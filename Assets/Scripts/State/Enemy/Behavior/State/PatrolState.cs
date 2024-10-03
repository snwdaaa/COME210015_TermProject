using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IState
{
    private Enemy enemy;

    // 생성자
    public PatrolState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        // 이동 속도 설정
        enemy.navAgent.speed = enemy.patrolSpeed;
    }

    public void Update()
    {
        if (enemy.chaseTarget != null)
        {
            enemy.esm.TransferState(enemy.esm.chaseState);
        }

        enemy.UpdatePatrolStatus();
    }

    public void Exit()
    {

    }
}
