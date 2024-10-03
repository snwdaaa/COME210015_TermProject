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

    }

    public void Exit()
    {

    }
}
