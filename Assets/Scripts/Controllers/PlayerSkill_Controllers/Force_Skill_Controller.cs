using System.Collections;
using UnityEngine;

public class Force_Skill_Controller : MonoBehaviour
{
    private Animator anim;
    private Player player;
    private float freezeTimeDuration;
    private float damageMultiplier;
    private float knockbackPower; // �m�b�N�o�b�N�̈З�

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }

    // �C���X�y�N�^����n���e�p�����[�^���܂߂��Z�b�g�A�b�v
    public void SetupForce(Player _player, float _freezeTimeDuration, float _damageMultiplier, float forceScaleMultiplier, float forceExpansionDuration, float _knockbackPower)
    {
        player = _player;
        freezeTimeDuration = _freezeTimeDuration;
        damageMultiplier = _damageMultiplier;
        knockbackPower = _knockbackPower;

        // �g�剉�o���J�n
        StartCoroutine(ExpandForce(forceScaleMultiplier, forceExpansionDuration));

        // ��莞�Ԍ�ɃI�u�W�F�N�g��j��
        Invoke("DestroyMe", 0.25f);
    }

    // �w��{���E���ԂŊg�傷��R���[�`��
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

        baseMagicalDamage = player.stats.ChechTargetResistance(enemyStats, baseMagicalDamage);
        int finalDamage = Mathf.RoundToInt(baseMagicalDamage * damageMultiplier);
        enemyStats.TakeDamage(finalDamage);

        enemy.FreezeTimeFor(freezeTimeDuration);

        // �m�b�N�o�b�N�����F�����̒��S����G�֌����������ɗ͂�������
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
