using System.Collections;
using UnityEngine;

public class Force_Skill_Controller : MonoBehaviour
{
    private Animator anim;
    private Player player;
    private float freezeTimeDuration;
    private float damageMultiplier;
    private float knockbackPower; // ノックバックの威力

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }

    // インスペクタから渡す各パラメータを含めたセットアップ
    public void SetupForce(Player _player, float _freezeTimeDuration, float _damageMultiplier, float forceScaleMultiplier, float forceExpansionDuration, float _knockbackPower)
    {
        player = _player;
        freezeTimeDuration = _freezeTimeDuration;
        damageMultiplier = _damageMultiplier;
        knockbackPower = _knockbackPower;

        // 拡大演出を開始
        StartCoroutine(ExpandForce(forceScaleMultiplier, forceExpansionDuration));

        // 一定時間後にオブジェクトを破棄
        Invoke("DestroyMe", 0.25f);
    }

    // 指定倍率・時間で拡大するコルーチン
    private IEnumerator ExpandForce(float targetMultiplier, float duration)
    {
        float elapsed = 0f;
        Vector3 initialScale = transform.localScale;
        Vector3 targetScale = initialScale * targetMultiplier;

        while (elapsed < duration)
        {
            transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = targetScale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            ForceSkillDamage(enemy);
        }
    }

    private void ForceSkillDamage(Enemy enemy)
    {
        EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();

        int _fireDamage = player.stats.fireDamage.GetValue();
        int _iceDamage = player.stats.iceDamage.GetValue();
        int _lightningDamage = player.stats.lightningDamage.GetValue();
        int baseMagicalDamage = _fireDamage + _iceDamage + _lightningDamage + player.stats.intelligence.GetValue();

        baseMagicalDamage = player.stats.CheckTargetResistance(enemyStats, baseMagicalDamage);
        int finalDamage = Mathf.RoundToInt(baseMagicalDamage * damageMultiplier);
        enemyStats.TakeDamage(finalDamage);

        enemy.FreezeTimeFor(freezeTimeDuration);

        // ノックバック処理：爆発の中心から敵へ向かう方向に力を加える
        Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
        if (enemyRb != null)
        {
            Vector2 knockbackDir = (enemy.transform.position - transform.position).normalized;
            enemyRb.AddForce(knockbackDir * knockbackPower, ForceMode2D.Impulse);
        }

        ItemData_Equipment equippedAmulet = Inventory.instance.GetEquipment(EquipmentType.Amulet);
        if (equippedAmulet != null)
            equippedAmulet.Effect(enemy.transform);
    }
}
