using JetBrains.Annotations;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Entity
{
    [Header("Attack details")]
    public Vector2[] attackMovement;
    public float counterAttackDuration = .2f;
    public bool isBusy { get; private set; }

    [Header("Move info")]
    public float moveSpeed = 12f;
    public float jumpForce;
    private float defaultMoveSpeed;
    private float defaultJumpForce;

    [Header("Dash info")]
    public float dashSpeed;
    public float dashDuration;
    private float defaultDashSpeed;
    public float dashDir { get; set; }
    public bool canAirDash = true;

    [Header("ShadowBringerOverDrive1st info")]
    public float rushSpeed;
    public float rushDuration;
    private float defaultRushSpeed;
    public float rushDir { get; set; }
    public bool canAirRush = true;

    public SkillManager skill { get; private set; }
    public GameObject sword { get; private set; }
    public GameObject darkCircle { get; private set; }
    public GameObject shadowFlare { get; private set; }
    public GameObject force { get; private set; }
    public GameObject shatteredSun { get; private set; }


    public PlayerFX fx { get; private set; }

    public PhysicsMaterial2D SlippyMaterial; // Playerクラスのインスペクターで設定


    #region States
    public PlayerStateMachine stateMachine { get; private set; }

    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerWallSlideState wallSlide { get; private set; }
    public PlayerWallJumpState wallJump { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerPrimaryAttackState primaryAttack { get; private set; }
    public PlayerGuardState guard { get; private set; }
    public PlayerCounterAttackState counterAttack { get; private set; }
    public PlayerJumpAttackState jumpAttack { get; private set; }

    public PlayerAimSwordState aimSword { get; private set; }
    public PlayerAimDarkCircleState aimDarkCircle { get; private set; }
    public PlayerAimShadowFlareState aimShadowFlare { get; private set; }
    public PlayerAimForceState aimForce { get; private set; }
    public PlayerAimShatteredSunState aimShatteredSun { get; private set; }

    public PlayerHealState heal { get; private set; }

    public PlayerShadowBringerOverDrive2ndState shadowBringerOverDrive2nd { get; private set; }
    public PlayerShadowBringerOverDrive1stState shadowBringerOverDrive1st { get; private set; }
    public PlayerDeadState deadState { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        stateMachine = new PlayerStateMachine();
        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState = new PlayerAirState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlide = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJump = new PlayerWallJumpState(this, stateMachine, "Jump");
        primaryAttack = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        jumpAttack = new PlayerJumpAttackState(this, stateMachine, "JumpAttack");
        guard = new PlayerGuardState(this, stateMachine, "Guard");
        counterAttack = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");
        aimSword = new PlayerAimSwordState(this, stateMachine, "AimSword");
        aimDarkCircle = new PlayerAimDarkCircleState(this, stateMachine, "AimDarkCircle");
        aimShadowFlare = new PlayerAimShadowFlareState(this, stateMachine, "AimShadowFlare");
        aimForce = new PlayerAimForceState(this, stateMachine, "AimForce");
        aimShatteredSun = new PlayerAimShatteredSunState(this, stateMachine, "AimShatteredSun");
        heal = new PlayerHealState(this, stateMachine, "Heal");
        shadowBringerOverDrive1st = new PlayerShadowBringerOverDrive1stState(this, stateMachine, "ShadowBringerOverDrive1st");
        shadowBringerOverDrive2nd = new PlayerShadowBringerOverDrive2ndState(this, stateMachine, "Blackhole");
        deadState = new PlayerDeadState(this, stateMachine, "Die");
    }

    protected override void Start()
    {
        base.Start();
        fx = GetComponent<PlayerFX>();
        skill = SkillManager.instance;
        stateMachine.Initialize(idleState);

        defaultMoveSpeed = moveSpeed;
        defaultJumpForce = jumpForce;
        defaultDashSpeed = dashSpeed;
        defaultRushSpeed = rushSpeed;

    }

    protected override void Update()
    {
        if (Time.timeScale == 0)
            return;

        base.Update();
        stateMachine.currentState.Update();
    }

    public override void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {
        moveSpeed *= (1 - _slowPercentage);
        jumpForce *= (1 - _slowPercentage);
        dashSpeed *= (1 - _slowPercentage);
        anim.speed *= (1 - _slowPercentage);
        rushSpeed *= (1 - _slowPercentage);

        Invoke("ReturnDefaultSpeed", _slowDuration);
    }

    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();

        moveSpeed = defaultMoveSpeed;
        jumpForce = defaultJumpForce;
        dashSpeed = defaultDashSpeed;
        rushSpeed = defaultRushSpeed;
    }

    public void AssignNewSword(GameObject _newSword)
    {
        sword = _newSword;
    }

    public void AssignNewDarkCircle(GameObject _newDarkCircle)
    {
        darkCircle = _newDarkCircle;
    }
    
    public void AssignNewShadowFlare(GameObject _newShadowFlare)
    {
        shadowFlare = _newShadowFlare;
    }

    public void AssignNewForce(GameObject _newForce)
    {
        force = _newForce;
    }

    public void AssignNewShatteredSun(GameObject _newShatteredSun)
    {
        shatteredSun = _newShatteredSun;
    }

    //public void AssignNewPyre(GameObject _newPyre)
    //{
    //    pyre = _newPyre;
    //}

    public IEnumerator BusyFor(float _seconds)
    {
        isBusy = true;
        yield return new WaitForSeconds(_seconds);
        isBusy = false;
    }

    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();

    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deadState);
    }

    protected override void SetupZeroKnockbackPower()
    {
        knockbackPower = Vector2.zero;
    }
}
