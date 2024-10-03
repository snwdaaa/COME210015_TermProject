using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    // 컴포넌트
    private Enemy enemy;
    
    public IState CurrentState { get; private set; } // Enemy의 현재 상태

    // State
    public PatrolState patrolState;
    public ChaseState chaseState;
    public AttackState attackState;

    // Start is called before the first frame update
    void Awake()
    {
        enemy = GetComponent<Enemy>();

        // 모든 State 생성자 호출
        this.patrolState = new PatrolState(enemy);
        this.chaseState = new ChaseState(enemy);
        this.attackState = new AttackState(enemy);
    }

    // Update is called once per frame
    void Update()
    {
        // 현재 상태 계속 업데이트
        if (CurrentState != null)
        {
            CurrentState.Update();
        }
    }

    /// <summary>
    /// 시작 State 초기화
    /// </summary>
    /// <param name="startingState">시작 State</param>
    public void Initialize(IState startingState)
    {
        CurrentState = startingState;
        startingState.Enter();
    }

    /// <summary>
    /// State 전이
    /// </summary>
    /// <param name="nextState">전이할 State</param>
    public void TransferState(IState nextState)
    {
        CurrentState.Exit(); // 현재 상태 Exit
        CurrentState = nextState; // 상태 변수 Update
        nextState.Enter(); // 다음 상태 Enter
    }
}
