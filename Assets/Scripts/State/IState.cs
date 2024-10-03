using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// State 관리 인터페이스
/// </summary>
public interface IState
{
    /// <summary>
    /// 상태에 처음 진입할 때 실행되는 코드
    /// </summary>
    public void Enter();

    /// <summary>
    /// 프레임 당 로직. 새로운 상태로 전환하는 조건 포함.
    /// Update의 모든 기능은 상태 변경을 트리거하는 조건이 감지될 때까지 각 프레임 실행
    /// </summary>
    public void Update();

    /// <summary>
    /// 상태에서 벗어날 때 새로운 상태로 전환되기 전에 실행되는 코드
    /// </summary>
    public void Exit();

}
