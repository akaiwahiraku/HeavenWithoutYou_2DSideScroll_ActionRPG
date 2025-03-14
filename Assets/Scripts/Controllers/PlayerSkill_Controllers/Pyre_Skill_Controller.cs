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

    // 二撃目かどうかのフラグ（trueなら二撃目用）
    private bool isSecondAttack = false;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CapsuleCollider2D>();
    }

    /// <summary>
    /// 爆発アニメーションに遷移してから、所定時間後にオブジェクトを破棄する
    /// </summary>
    private void ExplosionAndDestroy()
    {
        if (anim != null)
        {
            anim.SetTrigger("Explode");
        }
        // 爆発アニメーションを見せるため、0.2秒後に破棄（必要に応じて調整）
        Destroy(gameObject, 0.2f);
    }

    /// <summary>
    /// １撃目用セットアップ
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
    /// ２撃目用セットアップ
    /// Collider や Animator の設定はPrefab内で設定済みのため、射出速度は外部から計算済みの値を使用
    /// </summary>
    public void SetupPyre(Vector2 _dir, Player _player, float _projectileDuration, float _freezeTimeDuration, float _damageMultiplier, Vector2 _launchForce)
    {
        player = _player;
        projectileDuration = _projectileDuration;
        freezeTimeDuration = _freezeTimeDuration;
        damageMultiplier = _damageMultiplier;
        isSecondAttack = true;

        // プレイヤーの向きを反映した _launchForce（プレイヤーの向きを反映済み）をそのまま設定
        rb.velocity = _launchForce;

        // Collider や Animator の設定はPrefab内に任せる

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && _dir.x < 0)
        {
            if (sr.flipX == true)
                sr.flipX = false;
            else
                sr.flipX = true;
        }

        // ２撃目は、設定した持続時間後に爆発アニメーションに遷移して破棄する
        Invoke("ExplosionAndDestroy", projectileDuration);
    }

    private void Update()
    {
        // 必要に応じた更新処理（例：移動補正など）
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isSecondAttack)
        {
            // Ground に当たった場合は、少し待ってから爆発アニメーションに遷移する
            if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                StartCoroutine(DelayedExplosion(0.1f));
                return;
            }

            // 敵に当たった場合は、ダメージ処理後に少し遅延して爆発アニメーションに移行
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                PyreSkillDamage(enemy);
                StartCoroutine(DelayedExplosion(0.1f)); // 例：0.3秒遅延
                return;
            }
        }
        else
        {
            // １撃目用は、衝突時にダメージ処理のみ実行
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                PyreSkillDamage(enemy);
            }
        }
    }

    /// <summary>
    /// delay秒後に爆発アニメーションに遷移して破棄するコルーチン
    /// </summary>
    private IEnumerator DelayedExplosion(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (anim != null)
        {
            anim.SetTrigger("Explode");
        }
        // 爆発アニメーションの再生を見せるため、さらに少し待ってから破棄（例：0.2秒）
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
