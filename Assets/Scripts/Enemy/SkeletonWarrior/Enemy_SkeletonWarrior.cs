public class Enemy_SkeletonWarrior : Enemy
{

    #region States

    public SkeletonWarriorIdleState idleState { get; private set; }
    public SkeletonWarriorMoveState moveState { get; private set; }
    public SkeletonWarriorBattleState battleState { get; private set; }
    public SkeletonWarriorAttackState attackState { get; private set; }

    public SkeletonWarriorStunnedState stunnedState { get; private set; }
    public SkeletonWarriorDeadState deadState { get; private set; }


    #endregion

    protected override void Awake()
    {
        base.Awake();

        idleState = new SkeletonWarriorIdleState(this, stateMachine, "Idle", this);
        moveState = new SkeletonWarriorMoveState(this, stateMachine, "Move", this);
        battleState = new SkeletonWarriorBattleState(this, stateMachine, "Battle", this);
        attackState = new SkeletonWarriorAttackState(this, stateMachine, "Attack", this);
        stunnedState = new SkeletonWarriorStunnedState(this, stateMachine, "Stunned", this);
        deadState = new SkeletonWarriorDeadState(this, stateMachine, "Die", this);

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

    protected override void Update()
    {
        base.Update();
    }

    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deadState);
    }
}
