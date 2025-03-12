//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Surge2_Skill_Controller : MonoBehaviour
//{
//    private Animator anim;
//    private Rigidbody2D rb;
//    private BoxCollider2D cd;
//    private Player player;
//    private float projectileDuration;
//    private float freezeTimeDuration;
//    private float damageMultiplier;
//    private float knockbackPower; // �m�b�N�o�b�N�̈З�

//    private void Awake()
//    {
//        anim = GetComponentInChildren<Animator>();
//        rb = GetComponent<Rigidbody2D>();
//        cd = GetComponent<BoxCollider2D>();
//    }

//    // knockbackPower ��ǉ������Z�b�g�A�b�v���\�b�h
//    public void SetupSurge(Vector2 _dir, Player _player, float _projectileDuration, float _freezeTimeDuration, float _damageMultiplier, float _knockbackPower)
//    {
//        rb.velocity = _dir;
//        player = _player;
//        projectileDuration = _projectileDuration;
//        freezeTimeDuration = _freezeTimeDuration;
//        damageMultiplier = _damageMultiplier;
//        knockbackPower = _knockbackPower;

//        SpriteRenderer sr = GetComponent<SpriteRenderer>();
//        if (sr != null && _dir.x < 0)
//        {
//            sr.flipX = true;
//        }

//        Destroy(gameObject, projectileDuration);
//    }

//    private void Update()
//    {
//        // �K�v�ɉ������X�V�����i��F�ړ��␳�Ȃǁj
//    }

//    private void OnTriggerEnter2D(Collider2D collision)
//    {
//        Enemy enemy = collision.GetComponent<Enemy>();
//        if (enemy != null)
//        {
//            SurgeSkillDamage(enemy);
//            ApplyKnockback(enemy);
//        }
//    }

//    private void SurgeSkillDamage(Enemy enemy)
//    {
//        EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
//        AudioManager.instance.PlaySFX(5, null);

//        int _fireDamage = player.stats.fireDamage.GetValue();
//        int _iceDamage = player.stats.iceDamage.GetValue();
//        int _lightningDamage = player.stats.lightningDamage.GetValue();
//        int baseMagicalDamage = _fireDamage + _iceDamage + _lightningDamage + player.stats.intelligence.GetValue();

//        baseMagicalDamage = player.stats.ChechTargetResistance(enemyStats, baseMagicalDamage);
//        int finalDamage = Mathf.RoundToInt(baseMagicalDamage * damageMultiplier);
//        enemyStats.TakeDamage(finalDamage);

//        enemy.FreezeTimeFor(freezeTimeDuration);

//        ItemData_Equipment equippedAmulet = Inventory.instance.GetEquipment(EquipmentType.Amulet);
//        if (equippedAmulet != null)
//            equippedAmulet.Effect(enemy.transform);
//    }

//    // Force_Skill �Ɠ��l�̃m�b�N�o�b�N����
//    private void ApplyKnockback(Enemy enemy)
//    {
//        Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
//        if (enemyRb != null)
//        {
//            Vector2 knockbackDir = (enemy.transform.position - transform.position).normalized;
//            enemyRb.AddForce(knockbackDir * knockbackPower, ForceMode2D.Impulse);
//        }
//    }
//}
