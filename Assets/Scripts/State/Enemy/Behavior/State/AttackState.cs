using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : IState
{
    private Enemy enemy;

    // 생성자
    public AttackState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {

    }

    public void Update()
    {
        // 공격 후 다시 Chase 상태로 이동
        if (!enemy.isAttacking)
        {
            enemy.esm.TransferState(enemy.esm.chaseState);
        }
    }

    public void Exit()
    {

    }
}
