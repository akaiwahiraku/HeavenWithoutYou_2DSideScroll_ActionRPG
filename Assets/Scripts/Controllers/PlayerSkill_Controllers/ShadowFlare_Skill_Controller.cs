using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowFlare_Skill_Controller : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private CapsuleCollider2D cd;
    private Player player;
    private float freezeTimeDuration;
    // �C���X�y�N�^����n���ꂽ�_���[�W�{����ێ�
    private float damageMultiplier;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CapsuleCollider2D>();
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }

    // damageMultiplier �Ȃǂ̃p�����[�^���󂯎��悤�ɃV�O�l�`����ύX
    public void SetupShadowFlare(Vector2 _dir, Player _player, float _freezeTimeDuration, float _damageMultiplier)
    {
        rb.velocity = _dir;
        player = _player;
        freezeTimeDuration = _freezeTimeDuration;
        damageMultiplier = _damageMultiplier;

        // �v���C���[���������̏ꍇ�A�X�v���C�g�𔽓]
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && _dir.x < 0)
        {
            sr.flipX = true;
        }

        Invoke("DestroyMe", 3);
    }

    private void Update()
    {
        // �K�v�ɉ������X�V����
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            ShadowFlareSkillDamage(enemy);
        }
    }

    private void ShadowFlareSkillDamage(Enemy enemy)
    {
        // �^�[�Q�b�g�� EnemyStats ���擾
        EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();

        AudioManager.instance.PlaySFX(5, null);


        // �������ŁA�ʏ�� DoMagicalDamage �Ɠ��l�̌v�Z���s���܂���
        int _fireDamage = player.stats.fireDamage.GetValue();
        int _iceDamage = player.stats.iceDamage.GetValue();
        int _lightningDamage = player.stats.lightningDamage.GetValue();
        int baseMagicalDamage = _fireDamage + _iceDamage + _lightningDamage + player.stats.intelligence.GetValue();

        // �^�[�Q�b�g�̑ϐ��Ȃǂ��l�����鏈���i���� DoMagicalDamage ���̏����ɏ�����j
        baseMagicalDamage = player.stats.ChechTargetResistance(enemyStats, baseMagicalDamage);
        // ������ damageMultiplier ���|�����킹���ŏI�_���[�W���Z�o
        int finalDamage = Mathf.RoundToInt(baseMagicalDamage * damageMultiplier);
        enemyStats.TakeDamage(finalDamage);

        enemy.FreezeTimeFor(freezeTimeDuration);
        enemyStats.MakeVulnerableFor(freezeTimeDuration);

        ItemData_Equipment equippedAmulet = Inventory.instance.GetEquipment(EquipmentType.Amulet);
        if (equippedAmulet != null)
            equippedAmulet.Effect(enemy.transform);
    }
}
