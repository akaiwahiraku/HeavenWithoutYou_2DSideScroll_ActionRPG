using System.Collections;
using UnityEngine;

public class ShatteredSun_Skill_Controller : MonoBehaviour
{
    private Animator anim;
    private Player player;
    private float freezeTimeDuration;
    private float damageMultiplier;
    private float knockbackPower;

    private float sustainedDamageDuration;
    private float damageInterval;
    public bool playerCanExitState { get; private set; } = false;

    // �\���p��SpriteRenderer��񖇗p�Ӂi�q�I�u�W�F�N�g�ɐݒ�j
    public SpriteRenderer staticSpriteRenderer1;
    public SpriteRenderer staticSpriteRenderer2;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// �e�p�����[�^���Z�b�g�A�b�v���A�G�t�F�N�g�̊g�剉�o���J�n����B
    /// </summary>
    public void SetupShatteredSun(Player _player, float _freezeTimeDuration, float _damageMultiplier, float shatteredSunScaleMultiplier, float shatteredSunExpansionDuration, float _knockbackPower, float _sustainedDamageDuration, float _damageInterval)
    {
        player = _player;
        freezeTimeDuration = _freezeTimeDuration;
        damageMultiplier = _damageMultiplier;
        knockbackPower = _knockbackPower;
        sustainedDamageDuration = _sustainedDamageDuration;
        damageInterval = _damageInterval;

        StartCoroutine(ExpandShatteredSun(shatteredSunScaleMultiplier, shatteredSunExpansionDuration));
    }

    /// <summary>
    /// �G�t�F�N�g�̊g�剉�o��A�������ԓ��ɑS�G�Ƀ_���[�W��^����B
    /// �\���p��Sprite�͌Œ�T�C�Y�ɕۂB
    /// </summary>
    private IEnumerator ExpandShatteredSun(float targetMultiplier, float duration)
    {
        float elapsed = 0f;
        Vector3 initialScale = transform.localScale;
        Vector3 targetScale = initialScale * targetMultiplier;

        // �e�\���p�X�v���C�g�̌��̃��[�J���X�P�[�����L�^
        Vector3 staticOriginalScale1 = staticSpriteRenderer1.transform.localScale;
        Vector3 staticOriginalScale2 = staticSpriteRenderer2.transform.localScale;

        while (elapsed < duration)
        {
            // �e�I�u�W�F�N�g�̊g��
            transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsed / duration);

            // �e�̃X�P�[�����擾
            Vector3 parentScale = transform.localScale;
            // �e�q�I�u�W�F�N�g�̊g���ł��������߁A�t���ŕ␳
            staticSpriteRenderer1.transform.localScale = new Vector3(
                staticOriginalScale1.x / parentScale.x,
                staticOriginalScale1.y / parentScale.y,
                staticOriginalScale1.z / parentScale.z
            );
            staticSpriteRenderer2.transform.localScale = new Vector3(
                staticOriginalScale2.x / parentScale.x,
                staticOriginalScale2.y / parentScale.y,
                staticOriginalScale2.z / parentScale.z
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
        // �ŏI�I�ɌŒ�̃X�v���C�g�͌��̃X�P�[���ɖ߂�
        staticSpriteRenderer1.transform.localScale = staticOriginalScale1;
        staticSpriteRenderer2.transform.localScale = staticOriginalScale2;

        // �����_���[�W�̏���
        float elapsedDamageTime = 0f;
        while (elapsedDamageTime < sustainedDamageDuration)
        {
            DamageAllEnemies();
            yield return new WaitForSeconds(damageInterval);
            elapsedDamageTime += damageInterval;
        }

        playerCanExitState = true;
        DestroyMe();
    }

    // �����̏����͂��̂܂�
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            ShatteredSunSkillDamage(enemy);
        }
    }

    private void ShatteredSunSkillDamage(Enemy enemy)
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

    private void DamageAllEnemies()
    {
        Vector3 screenCenter = transform.position;
        Camera cam = Camera.main;
        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;
        float radius = Mathf.Sqrt(Mathf.Pow(camWidth / 2f, 2) + Mathf.Pow(camHeight / 2f, 2));

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(screenCenter, radius);
        foreach (Collider2D collider in hitColliders)
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                ShatteredSunSkillDamage(enemy);
            }
        }
    }
}
