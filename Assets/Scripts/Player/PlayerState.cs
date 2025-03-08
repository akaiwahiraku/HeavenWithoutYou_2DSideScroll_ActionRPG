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

        // joystickInputManager���璼�ړ��͂��擾
        if (joystickInputManager != null && joystickInputManager.isEnabled)
        {
            horizontalInput = joystickInputManager.Horizontal;
            verticalInput = joystickInputManager.Vertical;
        }

        player.anim.SetFloat("yVelocity", rb.velocity.y);

        // ���݂̃w���X���N���C�V�X���[�h�̏����𖞂����Ă��邩�`�F�b�N
        bool isInCrisisMode = player.stats.currentHealth <= player.stats.GetMaxHealthValue() * player.stats.crisisPercent;

        // CrisisMode�ɓ����Ă���ꍇ�A�A�j���[�V������ύX
        player.anim.SetBool("isInCrisisMode", isInCrisisMode);
    }

    // �C�ӂ̃X�e�[�g���烍�b�N�̐ݒ�𑀍삷�邽�߂̃��\�b�h
    public void SetLockStateTransition(bool lockTransition)
    {
        // ���Ɋ�]�̏�ԂȂ�Đݒ肵�Ȃ�
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
