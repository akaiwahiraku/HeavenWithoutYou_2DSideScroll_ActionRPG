using UnityEngine;

public class Enemy_NightBorne : Enemy
{

    #region States

    public NightBorneIdleState idleState { get; private set; }
    public NightBorneBattleState battleState { get; private set; }
    public NightBorneAttackState attackState { get; private set; }
    public NightBorneDeadState deadState { get; private set; }
    public NightBorneSpellCastState spellCastState { get; private set; }
    public NightBorneJumpState jumpState { get; private set; }

    #endregion

    public bool bossFightBegun;

    [Header("Arcane specific info")]
    [SerializeField] private GameObject arcanePrefab;
    [SerializeField] private float arcaneSpeed;
    [SerializeField] private float arcaneDamage;

    [Header("Jump info")]
    public Vector2 jumpVelocity = new Vector2(25f, 10f);
    public float jumpCooldown;
    public float safeDistance; // how close player should be to trigger jump on battle state
    [HideInInspector] public float lastTimeJumped;

    protected override void Awake()
    {
        base.Awake();

        //SetupDefaultFasingDir(-1);

        idleState = new NightBorneIdleState(this, stateMachine, "Idle", this);

        battleState = new NightBorneBattleState(this, stateMachine, "Move", this);
        attackState = new NightBorneAttackState(this, stateMachine, "Attack", this);

        deadState = new NightBorneDeadState(this, stateMachine, "Die", this);

        spellCastState = new NightBorneSpellCastState(this, stateMachine, "SpellCast", this);

        jumpState = new NightBorneJumpState(this, stateMachine, "Jump", this);
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialized(idleState);

    }

    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deadState);
    }


    public override void AnimationSpecialAttackTrigger()
    {
        GameObject newArcane = Instantiate(arcanePrefab, attackCheck.position, Quaternion.identity);

        newArcane.GetComponent<NightBorneSpell_Controller>().SetupArcane(arcaneSpeed * facingDir, stats);
    }



    private RaycastHit2D GroundBelow() => Physics2D.Raycast(transform.position, Vector2.down, 100, whatIsGround);
    //private bool SomethingIsAround() => Physics2D.BoxCast(transform.position, surroundingCheckSize, 0, Vector2.zero, 0, whatIsGround);

    //protected override void OnDrawGizmos()
    //{
    //    base.OnDrawGizmos();

    //    Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - GroundBelow().distance));
    //    Gizmos.DrawWireCube(transform.position, surroundingCheckSize);
    //}

}
