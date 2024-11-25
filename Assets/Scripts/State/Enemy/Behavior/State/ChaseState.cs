using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : IState
{
    private Enemy enemy;

    // 생성자
    public ChaseState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        // 이동 속도 설정
        enemy.navAgent.speed = enemy.chaseSpeed;
        enemy.navAgent.stoppingDistance = 1f;
    }

    public void Update()
    {
        // 목표가 없는 경우 다시 Patrol 상태로 이동
        if (enemy.chaseTarget == null)
        {
            enemy.esm.TransferState(enemy.esm.patrolState);
        }

        // Attack 상태 전이
        if (enemy.isAttacking)
        {
            enemy.esm.TransferState(enemy.esm.attackState);
        }

        // 애니메이터 설정
        enemy.animator.SetFloat("MoveSpeed", enemy.navAgent.speed);

        enemy.UpdateChaseStatus();
        enemy.UpdateAttackStatus();
    }

    public void Exit()
    {
        enemy.navAgent.stoppingDistance = 0f;
    }
}
