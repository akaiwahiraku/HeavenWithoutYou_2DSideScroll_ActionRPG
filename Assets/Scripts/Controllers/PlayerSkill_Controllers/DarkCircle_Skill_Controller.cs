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

    private string originalSortingLayerName; // ���̃��C���[����ۑ�
    private TilemapRenderer foreGroundTilemapRenderer;

    public Transform myEnemy;
    public List<Transform> targets = new List<Transform>();

    private bool canGrow = true;
    private bool canShrink;
    private bool canRotate = true;
    private float freezeTimeDuration;

    private bool waitingForStateExit; // PlayerShadowBringerOverDrive2ndState�̏I���ҋ@�t���O

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

        Enter(); // DarkCircle���������ꂽ���Ƀ��C���[�ύX�������Ăяo��
    }

    private void Enter()
    {
        // Foreground��TilemapRenderer���擾���āA���C���[��ύX
        GameObject foreGround = GameObject.Find("Level/Grid/Foreground");
        if (foreGround != null)
        {
            foreGroundTilemapRenderer = foreGround.GetComponent<TilemapRenderer>();
            if (foreGroundTilemapRenderer != null)
            {
                originalSortingLayerName = foreGroundTilemapRenderer.sortingLayerName;

                // Player��ShadowBringerOverDrive2ndState�ɂ��邩�ǂ������m�F
                if (player.GetComponent<CharacterStats>().isInShadowBringerOverDrive2ndState)
                {
                    waitingForStateExit = true; // �X�e�[�g�I����ҋ@
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

        // PlayerShadowBringerOverDrive2ndState���I���������m�F���āA���C���[�����ɖ߂�
        if (waitingForStateExit && !player.GetComponent<CharacterStats>().isInShadowBringerOverDrive2ndState)
        {
            waitingForStateExit = false;
            Exit(); // ���C���[�����ɖ߂��������Ăяo��
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
        // PlayerShadowBringerOverDrive2ndState���A�N�e�B�u�łȂ��ꍇ�ɂ̂݃��C���[�����ɖ߂�
        if (!player.GetComponent<CharacterStats>().isInShadowBringerOverDrive2ndState && foreGroundTilemapRenderer != null)
        {
            foreGroundTilemapRenderer.sortingLayerName = originalSortingLayerName;
        }
    }

    private void DestroyMe()
    {
        Exit(); // �j���O��Exit���\�b�h���Ăяo���ă��C���[�����ɖ߂�
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


