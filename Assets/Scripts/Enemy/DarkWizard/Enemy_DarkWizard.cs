using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_DarkWizard : Enemy
{

    [Header("DarkWizard specific info")]
    [SerializeField] private GameObject darkBallPrefab;
    [SerializeField] private float darkBallSpeed;
    [SerializeField] private float darkBallDamage;

    public Vector2 jumpVelocity;
    public float jumpCooldown;
    public float safeDistance; // how close player should be to trigger jump on battle state
    [HideInInspector] public float lastTimeJumped;

    [Header("Additional collision check")]
    [SerializeField] private Transform groundBehindCheck;
    [SerializeField] private Vector2 groundBehindCheckSize;

    #region States

    public DarkWizardIdleState idleState { get; private set; }
    public DarkWizardMoveState moveState { get; private set; }
    public DarkWizardBattleState battleState { get; private set; }
    public DarkWizardAttackState attackState { get; private set; }

    public DarkWizardStunnedState stunnedState { get; private set; }
    public DarkWizardDeadState deadState { get; private set; }

    public DarkWizardJumpState jumpState { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        idleState = new DarkWizardIdleState(this, stateMachine, "Idle", this);
        moveState = new DarkWizardMoveState(this, stateMachine, "Move", this);
        battleState = new DarkWizardBattleState(this, stateMachine, "Idle", this);
        attackState = new DarkWizardAttackState(this, stateMachine, "Attack", this);
        stunnedState = new DarkWizardStunnedState(this, stateMachine, "Stunned", this);
        deadState = new DarkWizardDeadState(this, stateMachine, "Die", this);
        jumpState = new DarkWizardJumpState(this, stateMachine, "Jump", this);
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialized(idleState);

    }

    public override bool CanBeStunned()
    {
        if (base.CanBeStunned())
        {
            stateMachine.ChangeState(stunnedState);
            return true;
        }

        return false;
    }

    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deadState);
    }

    public override void AnimationSpecialAttackTrigger()
    {
        GameObject newDarkBall = Instantiate(darkBallPrefab, attackCheck.position, Quaternion.identity);

        newDarkBall.GetComponent<DarkBallSpell_Controller>().SetupDarkBall(darkBallSpeed * facingDir, stats);
    }

    public bool GroundBehind() => Physics2D.BoxCast(groundBehindCheck.position, groundBehindCheckSize, 0, Vector2.zero, 0, whatIsGround);
    public bool WallBehind() => Physics2D.Raycast(wallCheck.position, Vector2.right * -facingDir, wallCheckDistance + 2, whatIsGround);

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawWireCube(groundBehindCheck.position, groundBehindCheckSize);
    }
}
