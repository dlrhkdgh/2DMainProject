using UnityEngine;
public enum PlayerAnimState
{
    None = 0,
    Idle,
    Walk,
    Shoot,
    ShootAndWalk,
    Damaged
}

public class PlayerAnimController : MonoBehaviour
{
    [SerializeField] private Animator _playerAnimator;
    private PlayerAnimState currentAnimState;
    private void Awake()
    {
        if (_playerAnimator == null)
        {
            _playerAnimator = GetComponent<Animator>();
        }
    }
    public void SetAnimState(PlayerAnimState newState)
    {
        if (newState == currentAnimState) return;

        currentAnimState = newState;

        switch (newState)
        {

            case PlayerAnimState.Idle: ResetAllAnimParameters(); break;
            case PlayerAnimState.Walk: _playerAnimator.SetBool("IsWalk", true); break;
            case PlayerAnimState.Shoot: _playerAnimator.SetBool("IsShoot", true); _playerAnimator.SetBool("IsWalk", false); break;
            case PlayerAnimState.ShootAndWalk: _playerAnimator.SetBool("IsWalk", true); break;
            case PlayerAnimState.Damaged: _playerAnimator.SetTrigger("IsDamaged"); break;
            default: ResetAllAnimParameters(); break;
        }
    }
    private void ResetAllAnimParameters()
    {
        _playerAnimator.SetBool("IsWalk", false);
        _playerAnimator.SetBool("IsShoot", false);
    }
    
}
