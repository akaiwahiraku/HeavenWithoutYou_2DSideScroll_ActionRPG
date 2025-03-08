using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DarkCircle_Skill_Controller : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private CircleCollider2D cd;
    private Player player;

    private float maxSize;
    private float growSpeed;
    private float shrinkSpeed;
    private float darkCircleTimer;

    private string originalSortingLayerName; // 元のレイヤー名を保存
    private TilemapRenderer foreGroundTilemapRenderer;

    public Transform myEnemy;
    public List<Transform> targets = new List<Transform>();

    private bool canGrow = true;
    private bool canShrink;
    private bool canRotate = true;
    private float freezeTimeDuration;

    private bool waitingForStateExit; // PlayerShadowBringerOverDrive2ndStateの終了待機フラグ

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CircleCollider2D>();
    }

    public void SetupDarkCircle(float _maxSize, float _growSpeed, float _shrinkSpeed, Transform _myEnemy, Vector2 _dir, float _gravityScale, Player _player, float _freezeTimeDuration, float _maxTravelDistance, float _hitCooldown, float _darkCircleDuration)
    {
        maxSize = _maxSize;
        growSpeed = _growSpeed;
        shrinkSpeed = _shrinkSpeed;
        myEnemy = _myEnemy;

        rb.velocity = _dir;
        rb.gravityScale = _gravityScale;

        player = _player;
        freezeTimeDuration = _freezeTimeDuration;
        darkCircleTimer = _darkCircleDuration;

        Enter(); // DarkCircleが生成された時にレイヤー変更処理を呼び出し
    }

    private void Enter()
    {
        // ForegroundのTilemapRendererを取得して、レイヤーを変更
        GameObject foreGround = GameObject.Find("Level/Grid/Foreground");
        if (foreGround != null)
        {
            foreGroundTilemapRenderer = foreGround.GetComponent<TilemapRenderer>();
            if (foreGroundTilemapRenderer != null)
            {
                originalSortingLayerName = foreGroundTilemapRenderer.sortingLayerName;

                // PlayerがShadowBringerOverDrive2ndStateにいるかどうかを確認
                if (player.GetComponent<CharacterStats>().isInShadowBringerOverDrive2ndState)
                {
                    waitingForStateExit = true; // ステート終了を待機
                }
                else
                {
                    foreGroundTilemapRenderer.sortingLayerName = "Ground";
                }
            }
        }
    }

    private void Update()
    {
        darkCircleTimer -= Time.deltaTime;

        // PlayerShadowBringerOverDrive2ndStateが終了したか確認して、レイヤーを元に戻す
        if (waitingForStateExit && !player.GetComponent<CharacterStats>().isInShadowBringerOverDrive2ndState)
        {
            waitingForStateExit = false;
            Exit(); // レイヤーを元に戻す処理を呼び出す
        }

        if (canRotate)
            transform.right = rb.velocity;

        if (canGrow && !canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);
        }

        if (darkCircleTimer < -3)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-10, -10), shrinkSpeed * Time.deltaTime);

            if (transform.localScale.x < 0)
                DestroyMe();
        }
    }

    private void Exit()
    {
        // PlayerShadowBringerOverDrive2ndStateがアクティブでない場合にのみレイヤーを元に戻す
        if (!player.GetComponent<CharacterStats>().isInShadowBringerOverDrive2ndState && foreGroundTilemapRenderer != null)
        {
            foreGroundTilemapRenderer.sortingLayerName = originalSortingLayerName;
        }
    }

    private void DestroyMe()
    {
        Exit(); // 破棄前にExitメソッドを呼び出してレイヤーを元に戻す
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            targets.Add(collision.transform);
            Enemy enemy = collision.GetComponent<Enemy>();
            DarkCircleSkillDamage(enemy);
        }
    }

    private void DarkCircleSkillDamage(Enemy enemy)
    {
        EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
        player.stats.DoMagicalDamage(enemyStats);
        enemy.FreezeTimeFor(freezeTimeDuration);
        enemyStats.MakeVulnerableFor(freezeTimeDuration);

        ItemData_Equipment equippedAmulet = Inventory.instance.GetEquipment(EquipmentType.Amulet);
        if (equippedAmulet != null)
            equippedAmulet.Effect(enemy.transform);
    }
}


