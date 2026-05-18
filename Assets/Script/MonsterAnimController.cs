using UnityEngine;
public enum MonsterAnimState
{
    None = 0,
    Idle,
    Walk,
    Damaged,
    Die,
    Attack
}
public class MonsterAnimController : MonoBehaviour
{
    [SerializeField] private Animator _monsterAnimator;
    private MonsterAnimState currentAnimState;
    public void SetAnimState(MonsterAnimState newState)
    {
        if (newState == currentAnimState) return;

        currentAnimState = newState;

        switch (newState)
        {

            case MonsterAnimState.Idle: ResetAllAnimParameters(); break;
            case MonsterAnimState.Walk: _monsterAnimator.SetBool("IsWalk", true); break;
            case MonsterAnimState.Damaged: _monsterAnimator.SetTrigger("IsDamaged"); break;
            case MonsterAnimState.Die: _monsterAnimator.SetBool("IsDead",true); break;

            default: ResetAllAnimParameters(); break;
        }
    }
    private void ResetAllAnimParameters()
    {
        _monsterAnimator.SetBool("IsWalk", false);
        _monsterAnimator.SetBool("IsDead", false);
    }
    public float GetCurrentAnimLength()//현재 재생중인 애니메이션의 길이를 반환
    {
        AnimatorStateInfo stateInfo = _monsterAnimator.GetCurrentAnimatorStateInfo(0); // 변수명이 animator라면 그걸로 매칭
 
        return stateInfo.length;
    }
}
