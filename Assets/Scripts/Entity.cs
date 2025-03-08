using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{

    #region Components
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public SpriteRenderer sr { get; private set; }
    public CharacterStats stats { get; private set; }
    public CapsuleCollider2D cd { get; private set; }

    #endregion

    [Header("Knockback info")]
    [SerializeField] public Vector2 knockbackPower = new Vector2(1, 2);
    [SerializeField] protected Vector2 knockbackOffset = new Vector2(.1f, 1);
    [SerializeField] protected float knockbackDuration = .02f;
    public bool isKnocked;

    [Header("Collision info")]
    public Transform attackCheck;
    public float attackCheckRadius = 1.2f;
    [SerializeField] public Transform groundCheckFore;
    [SerializeField] public float groundCheckForeDistance = 1;
    [SerializeField] public Transform groundCheckBack;
    [SerializeField] public float groundCheckBackDistance = 1;
    [SerializeField] public Transform throughGroundCheck;    
    [SerializeField] public float throughGroundCheckDistance = 1;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance = .8f;
    [SerializeField] public LayerMask whatIsGround;
    [SerializeField] public LayerMask whatIsThroughGround;

    public int knockbackDir { get; private set; }
    public int facingDir { get; private set; } = 1;
    protected bool facingRight = true;

    public System.Action onFlipped;

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<CharacterStats>();
        cd = GetComponent<CapsuleCollider2D>();
    }

    protected virtual void Update()
    {

    }

    public virtual void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {

    }

    protected virtual void ReturnDefaultSpeed()
    {
        anim.speed = 1;
    }

    public virtual void SetupKnockbackDir(Transform _damageDirection)
    {
        if (_damageDirection.position.x > transform.position.x)
            knockbackDir = -1;
        else if (_damageDirection.position.x < transform.position.x)
            knockbackDir = 1;
    }

    public void SetupKnockbackPower(Vector2 _knockbackpower) => knockbackPower = _knockbackpower;

    public virtual void DamageImpact() => StartCoroutine("HitKnockback");

    protected virtual IEnumerator HitKnockback()
    {
        isKnocked = true;

        float xOffset = Random.Range(knockbackOffset.x, knockbackOffset.y);

        //if(knockbackPower.x > 0 || knockbackPower.y > 0) //This line makes player immune to freeze effect when he takes hit
        rb.velocity = new Vector2((knockbackPower.x + xOffset) * knockbackDir, knockbackPower.y);

        yield return new WaitForSeconds(knockbackDuration);
        isKnocked = false;

        //ノックバックをゼロにする処理
        SetupZeroKnockbackPower();
    }

    protected virtual void SetupZeroKnockbackPower()
    {

    }

    #region Velocity
    public void SetZeroVelocity()
    {
        if (isKnocked)
            return;

        rb.velocity = new Vector2(0, 0);

    }

    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        if (isKnocked)
            return;

        rb.velocity = new Vector2(_xVelocity, _yVelocity);
        FlipController(_xVelocity);
    }
    #endregion

    #region Collision

    //すり抜け床は判定しない
    public virtual bool IsGroundDetectedFore() => Physics2D.Raycast(groundCheckFore.position, Vector2.down, groundCheckForeDistance, whatIsGround);
    public virtual bool IsGroundDetectedBack() => Physics2D.Raycast(groundCheckBack.position, Vector2.down, groundCheckBackDistance, whatIsGround);
    public virtual bool IsThroughGroundDetected() => Physics2D.Raycast(throughGroundCheck.position, Vector2.down, throughGroundCheckDistance, whatIsThroughGround);

    public bool wallCheckEnabled { get; set; } = true;  // WallCheckの有効/無効を制御するフラグ

    public virtual bool IsWallDetected()
    {
        if (!wallCheckEnabled)
        {
            return false;  // 無効化されている場合、常にfalseを返す
        }

        return Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheckFore.position, new Vector3(groundCheckFore.position.x, groundCheckFore.position.y - groundCheckForeDistance));
        Gizmos.DrawLine(groundCheckBack.position, new Vector3(groundCheckBack.position.x, groundCheckBack.position.y - groundCheckBackDistance));
        Gizmos.DrawLine(throughGroundCheck.position, new Vector3(throughGroundCheck.position.x, throughGroundCheck.position.y - throughGroundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance * facingDir, wallCheck.position.y));
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }
    #endregion

    #region Flip
    public virtual void Flip()
    {
        facingDir = facingDir * -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);

        if (onFlipped != null)
        {
            onFlipped();
        }
    }

    public virtual void FlipController(float _x)
    {
        if (_x > 0 && !facingRight)
            Flip();
        else if (_x < 0 && facingRight)
            Flip();
    }

    public virtual void SetupDefaultFasingDir(int _direction)
    {
        facingDir = _direction;

        if (facingDir == -1)
            facingRight = false;
    }

    #endregion



    public virtual void Die()
    {

    }
}
