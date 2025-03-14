using System.Collections;
using UnityEngine;

public class Pyre_Skill_Controller : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private CapsuleCollider2D cd;
    private Player player;
    private float projectileDuration;
    private float freezeTimeDuration;
    private float damageMultiplier;

    // �񌂖ڂ��ǂ����̃t���O�itrue�Ȃ�񌂖ڗp�j
    private bool isSecondAttack = false;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CapsuleCollider2D>();
    }

    /// <summary>
    /// �����A�j���[�V�����ɑJ�ڂ��Ă���A���莞�Ԍ�ɃI�u�W�F�N�g��j������
    /// </summary>
    private void ExplosionAndDestroy()
    {
        if (anim != null)
        {
            anim.SetTrigger("Explode");
        }
        // �����A�j���[�V�����������邽�߁A0.2�b��ɔj���i�K�v�ɉ����Ē����j
        Destroy(gameObject, 0.2f);
    }

    /// <summary>
    /// �P���ڗp�Z�b�g�A�b�v
    /// </summary>
    public void SetupPyre(Vector2 _dir, Player _player, float _projectileDuration, float _freezeTimeDuration, float _damageMultiplier)
    {
        rb.velocity = _dir;
        player = _player;
        projectileDuration = _projectileDuration;
        freezeTimeDuration = _freezeTimeDuration;
        damageMultiplier = _damageMultiplier;
        isSecondAttack = false;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && _dir.x < 0)
        {
            sr.flipX = true;
        }

        Destroy(gameObject, projectileDuration);
    }

    /// <summary>
    /// �Q���ڗp�Z�b�g�A�b�v
    /// Collider �� Animator �̐ݒ��Prefab���Őݒ�ς݂̂��߁A�ˏo���x�͊O������v�Z�ς݂̒l���g�p
    /// </summary>
    public void SetupPyre(Vector2 _dir, Player _player, float _projectileDuration, float _freezeTimeDuration, float _damageMultiplier, Vector2 _launchForce)
    {
        player = _player;
        projectileDuration = _projectileDuration;
        freezeTimeDuration = _freezeTimeDuration;
        damageMultiplier = _damageMultiplier;
        isSecondAttack = true;

        // �v���C���[�̌����𔽉f���� _launchForce�i�v���C���[�̌����𔽉f�ς݁j�����̂܂ܐݒ�
        rb.velocity = _launchForce;

        // Collider �� Animator �̐ݒ��Prefab���ɔC����

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && _dir.x < 0)
        {
            if (sr.flipX == true)
                sr.flipX = false;
            else
                sr.flipX = true;
        }

        // �Q���ڂ́A�ݒ肵���������Ԍ�ɔ����A�j���[�V�����ɑJ�ڂ��Ĕj������
        Invoke("ExplosionAndDestroy", projectileDuration);
    }

    private void Update()
    {
        // �K�v�ɉ������X�V�����i��F�ړ��␳�Ȃǁj
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isSecondAttack)
        {
            // Ground �ɓ��������ꍇ�́A�����҂��Ă��甚���A�j���[�V�����ɑJ�ڂ���
            if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                StartCoroutine(DelayedExplosion(0.1f));
                return;
            }

            // �G�ɓ��������ꍇ�́A�_���[�W������ɏ����x�����Ĕ����A�j���[�V�����Ɉڍs
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                PyreSkillDamage(enemy);
                StartCoroutine(DelayedExplosion(0.1f)); // ��F0.3�b�x��
                return;
            }
        }
        else
        {
            // �P���ڗp�́A�Փˎ��Ƀ_���[�W�����̂ݎ��s
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                PyreSkillDamage(enemy);
            }
        }
    }

    /// <summary>
    /// delay�b��ɔ����A�j���[�V�����ɑJ�ڂ��Ĕj������R���[�`��
    /// </summary>
    private IEnumerator DelayedExplosion(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (anim != null)
        {
            anim.SetTrigger("Explode");
        }
        // �����A�j���[�V�����̍Đ��������邽�߁A����ɏ����҂��Ă���j���i��F0.2�b�j
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }

    private void PyreSkillDamage(Enemy enemy)
    {
        EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
        AudioManager.instance.PlaySFX(5, null);

        int _fireDamage = player.stats.fireDamage.GetValue();
        int _iceDamage = player.stats.iceDamage.GetValue();
        int _lightningDamage = player.stats.lightningDamage.GetValue();
        int baseMagicalDamage = _fireDamage + _iceDamage + _lightningDamage + player.stats.intelligence.GetValue();

        baseMagicalDamage = player.stats.CheckTargetResistance(enemyStats, baseMagicalDamage);
        int finalDamage = Mathf.RoundToInt(baseMagicalDamage * damageMultiplier);
        enemyStats.TakeDamage(finalDamage);

        enemy.FreezeTimeFor(freezeTimeDuration);
        enemyStats.MakeVulnerableFor(freezeTimeDuration);

        ItemData_Equipment equippedAmulet = Inventory.instance.GetEquipment(EquipmentType.Amulet);
        if (equippedAmulet != null)
            equippedAmulet.Effect(enemy.transform);
    }
}
