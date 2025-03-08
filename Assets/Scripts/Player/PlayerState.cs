using UnityEngine;

public class PlayerState
{
    protected PlayerStateMachine stateMachine;
    protected Player player;
    protected Rigidbody2D rb;

    private string animBoolName;
    public bool canDoubleJump = true;
    protected float stateTimer;
    protected bool triggerCalled;

    public bool inBlackholeState = false;

    protected JoystickInputManager joystickInputManager;
    protected float horizontalInput;
    protected float verticalInput;

    public PlayerState(Player player, PlayerStateMachine stateMachine, string animBoolName)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
        joystickInputManager = JoystickInputManager.Instance;
    }

    public virtual void Enter()
    {
        player.anim.SetBool(animBoolName, true);
        rb = player.rb;
        triggerCalled = false;
    }

    public virtual void Exit()
    {
        player.anim.SetBool(animBoolName, false);
    }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;

        // joystickInputManagerから直接入力を取得
        if (joystickInputManager != null && joystickInputManager.isEnabled)
        {
            horizontalInput = joystickInputManager.Horizontal;
            verticalInput = joystickInputManager.Vertical;
        }

        player.anim.SetFloat("yVelocity", rb.velocity.y);

        // 現在のヘルスがクライシスモードの条件を満たしているかチェック
        bool isInCrisisMode = player.stats.currentHealth <= player.stats.GetMaxHealthValue() * player.stats.crisisPercent;

        // CrisisModeに入っている場合、アニメーションを変更
        player.anim.SetBool("isInCrisisMode", isInCrisisMode);
    }

    // 任意のステートからロックの設定を操作するためのメソッド
    public void SetLockStateTransition(bool lockTransition)
    {
        // 既に希望の状態なら再設定しない
        if (stateMachine.IsTransitionLocked() == lockTransition)
        {
            return;
        }

        stateMachine.LockStateTransition(lockTransition);
    }

    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true;
    }
}
