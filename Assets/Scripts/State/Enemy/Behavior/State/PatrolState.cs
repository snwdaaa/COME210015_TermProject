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
        // Chase 상태 전이
        if (enemy.chaseTarget != null)
        {
            enemy.esm.TransferState(enemy.esm.chaseState);
        }

        // Attack 상태 전이
        if (enemy.isAttacking)
        {
            enemy.esm.TransferState(enemy.esm.attackState);
        }

        // 애니메이터 설정
        enemy.animator.SetFloat("MoveSpeed", enemy.navAgent.speed);

        enemy.UpdatePatrolStatus();
        enemy.UpdateAttackStatus();
    }

    public void Exit()
    {

    }
}
