using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pyre_Skill_Controller : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private CapsuleCollider2D cd;
    private Player player;
    private float freezeTimeDuration;
    // インスペクタから渡されたダメージ倍率を保持
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

    // damageMultiplier などのパラメータを受け取るようにシグネチャを変更
    public void SetupPyre(Vector2 _dir, Player _player, float _freezeTimeDuration, float _damageMultiplier)
    {
        rb.velocity = _dir;
        player = _player;
        freezeTimeDuration = _freezeTimeDuration;
        damageMultiplier = _damageMultiplier;

        // プレイヤーが左向きの場合、スプライトを反転
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && _dir.x < 0)
        {
            sr.flipX = true;
        }

        Invoke("DestroyMe", .32f);
    }

    private void Update()
    {
        // 必要に応じた更新処理
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            PyreSkillDamage(enemy);
        }
    }

    private void PyreSkillDamage(Enemy enemy)
    {
        // ターゲットの EnemyStats を取得
        EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();

        AudioManager.instance.PlaySFX(5, null);


        // ★ここで、通常の DoMagicalDamage と同様の計算を行います★
        int _fireDamage = player.stats.fireDamage.GetValue();
        int _iceDamage = player.stats.iceDamage.GetValue();
        int _lightningDamage = player.stats.lightningDamage.GetValue();
        int baseMagicalDamage = _fireDamage + _iceDamage + _lightningDamage + player.stats.intelligence.GetValue();

        // ターゲットの耐性などを考慮する処理（元の DoMagicalDamage 内の処理に準じる）
        baseMagicalDamage = player.stats.ChechTargetResistance(enemyStats, baseMagicalDamage);
        // ここで damageMultiplier を掛け合わせた最終ダメージを算出
        int finalDamage = Mathf.RoundToInt(baseMagicalDamage * damageMultiplier);
        enemyStats.TakeDamage(finalDamage);

        enemy.FreezeTimeFor(freezeTimeDuration);
        enemyStats.MakeVulnerableFor(freezeTimeDuration);

        ItemData_Equipment equippedAmulet = Inventory.instance.GetEquipment(EquipmentType.Amulet);
        if (equippedAmulet != null)
            equippedAmulet.Effect(enemy.transform);
    }
}