using System.Collections;
using UnityEngine;

public class Clone_Skill_Controller : MonoBehaviour
{
    private Player player;
    private SpriteRenderer sr;
    private Animator anim;
    [SerializeField] private float colorloosingSpeed;

    private float cloneTimer;
    private float attackMultiplier;
    [SerializeField] private Transform attackCheck;
    [SerializeField] private float attackCheckRadius = .8f;

    private int facingDir = 1;

    private bool canDuplicateClone;
    private float chanceToDuplicate;

    [Space]
    [SerializeField] private LayerMask whatIsEnemy;
    [SerializeField] private float closestEnemyCheckRadius = 25;
    [SerializeField] private Transform closestEnemy;

    private Rigidbody2D rb; // クローンの Rigidbody2D
    private bool followPlayer = false; // プレイヤー追従のフラグ
    private EntityFX entityFX;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2Dを取得

        // EntityFX コンポーネントを取得
        entityFX = GetComponent<EntityFX>();

        StartCoroutine(FaceClosestTarget(false));
    }

    private void Update()
    {
        cloneTimer -= Time.deltaTime;

        if (followPlayer && player != null)
        {
            // プレイヤーの動きを追従
            FollowPlayerMovement();
        }

        if (cloneTimer < 0)
        {
            sr.color = new Color(0, 255, 255, sr.color.a - (Time.deltaTime * colorloosingSpeed));

            if (sr.color.a <= 0)
                Destroy(gameObject);
        }
    }

    public void SetupClone(Transform _newTransform, float _cloneDuration, bool _canAttack, Vector3 _offset, bool _canDuplicate, float _chanceToDuplicate, Player _player, float _attackMultiplier, bool followPlayerFlag, bool faceClosestEnemy)
    {
        if (_newTransform == null)
        {
            Debug.LogWarning("Clone transform is null. Skipping SetupClone.");
            return;
        }

        if (_canAttack)
        {
            int numberRandom = Random.Range(1, 4);
            anim.SetInteger("AttackNumber", numberRandom);
        }

        attackMultiplier = _attackMultiplier;
        player = _player;
        transform.position = _newTransform.position + _offset;
        cloneTimer = _cloneDuration;

        canDuplicateClone = _canDuplicate;
        chanceToDuplicate = _chanceToDuplicate;

        // プレイヤー追従のフラグを設定
        followPlayer = followPlayerFlag;

        // faceClosestEnemy に基づいて敵に顔を向ける処理を行う
        StartCoroutine(FaceClosestTarget(faceClosestEnemy));
    }


    private void FollowPlayerMovement()
    {
        if (player != null)
        {
            // プレイヤーの動きを追従（速度と位置）
            rb.velocity = player.rb.velocity;

            // プレイヤーの向きに応じてクローンの向きを調整
            if (player.facingDir != facingDir)
            {
                facingDir = player.facingDir;
                transform.Rotate(0, 180, 0);
            }
        }
    }

    private void AnimationTrigger()
    {
        cloneTimer = -.1f;
    }

    protected void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);

        foreach (var hit in colliders)
        {
            Enemy enemy = hit.GetComponent<Enemy>();

            if (enemy != null && !enemy.stats.isDead)
            {
                hit.GetComponent<Entity>().SetupKnockbackDir(transform);

                PlayerStats playerStats = player.GetComponent<PlayerStats>();
                EnemyStats enemyStats = hit.GetComponent<EnemyStats>();

                playerStats.CloneDoDamage(enemyStats, attackMultiplier, facingDir);

                if (player.skill.clone.canApplyOnHitEffect)
                {
                    ItemData_Equipment weaponData = Inventory.instance.GetEquipment(EquipmentType.Weapon);

                    if (weaponData != null)
                        weaponData.Effect(hit.transform);
                }

                if (canDuplicateClone)
                {
                    if (Random.Range(0, 100) < chanceToDuplicate)
                    {
                        SkillManager.instance.clone.CreateClone(hit.transform, new Vector3(.5f * facingDir, 0), false, true);
                    }
                }

            }
        }
    }

    private IEnumerator FaceClosestTarget(bool faceClosestEnemy)
    {
        if (!faceClosestEnemy)
        {
            yield break;  // faceClosestEnemy が false なら処理をスキップ
        }

        yield return null;

        FindClosestEnemy();

        if (closestEnemy != null)
        {
            if (transform.position.x > closestEnemy.position.x)
            {
                facingDir = -1;
                transform.Rotate(0, 180, 0);
            }
        }
    }

    private void FindClosestEnemy()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, closestEnemyCheckRadius, whatIsEnemy);

        float closestDistance = Mathf.Infinity;

        foreach (var hit in colliders)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                closestEnemy = hit.transform;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, closestEnemyCheckRadius);
    }
}