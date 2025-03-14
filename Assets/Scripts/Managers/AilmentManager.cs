using UnityEngine;
using System.Collections;

public class AilmentManager
{
    private CharacterStats owner;
    private EntityFX fx;
    private float ailmentsDuration;
    private GameObject shockStrikePrefab;

    private float ignitedTimer;
    private float chilledTimer;
    private float shockedTimer;
    private float igniteDamageCooldown = 0.3f;
    private float igniteDamageTimer;
    private int igniteDamage;
    private int shockDamage;

    public bool IsIgnited { get; private set; }
    public bool IsChilled { get; private set; }
    public bool IsShocked { get; private set; }

    public AilmentManager(CharacterStats owner, EntityFX fx, float ailmentsDuration, GameObject shockStrikePrefab)
    {
        this.owner = owner;
        this.fx = fx;
        this.ailmentsDuration = ailmentsDuration;
        this.shockStrikePrefab = shockStrikePrefab;
    }

    /// <summary>
    /// 毎フレームの更新処理。各タイマーを減算し、状態をリセットします。
    /// また、燃焼状態であれば燃焼ダメージを適用します。
    /// </summary>
    public void Update(float deltaTime)
    {
        ignitedTimer -= deltaTime;
        chilledTimer -= deltaTime;
        shockedTimer -= deltaTime;
        igniteDamageTimer -= deltaTime;

        if (ignitedTimer < 0)
            IsIgnited = false;
        if (chilledTimer < 0)
            IsChilled = false;
        if (shockedTimer < 0)
            IsShocked = false;

        if (IsIgnited)
            ApplyIgniteDamage();
    }

    public void SetupIgniteDamage(int damage)
    {
        igniteDamage = damage;
    }

    public void SetupShockStrikeDamage(int damage)
    {
        shockDamage = damage;
    }

    /// <summary>
    /// 対象（owner）にエイルメントを適用します。
    /// </summary>
    public void ApplyAilments(bool ignite, bool chill, bool shock)
    {
        bool canApplyIgnite = !IsIgnited && !IsChilled && !IsShocked;
        bool canApplyChill = !IsIgnited && !IsChilled && !IsShocked;
        bool canApplyShock = !IsIgnited && !IsChilled;

        if (ignite && canApplyIgnite)
        {
            IsIgnited = true;
            ignitedTimer = ailmentsDuration;
            fx.IgniteFxFor(ailmentsDuration);
        }
        if (chill && canApplyChill)
        {
            chilledTimer = ailmentsDuration;
            IsChilled = true;
            float slowPercentage = 0.2f;
            // SlowEntityBy は owner の Entity コンポーネントで処理
            owner.GetComponent<Entity>().SlowEntityBy(slowPercentage, ailmentsDuration);
            fx.ChillFxFor(ailmentsDuration);
        }
        if (shock && canApplyShock)
        {
            if (!IsShocked)
            {
                ApplyShock(shock);
            }
            else
            {
                // Player の場合は二重適用しない
                if (owner.GetComponent<Player>() != null)
                    return;
                HitNearestTargetWithShockStrike();
            }
        }
    }

    public void ApplyShock(bool shock)
    {
        if (IsShocked)
            return;
        shockedTimer = ailmentsDuration;
        IsShocked = shock;
        fx.ShockFxFor(ailmentsDuration);
    }

    private void HitNearestTargetWithShockStrike()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(owner.transform.position, 25);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && Vector2.Distance(owner.transform.position, hit.transform.position) > 1)
            {
                float distanceToEnemy = Vector2.Distance(owner.transform.position, hit.transform.position);
                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }
        }

        if (closestEnemy == null)
            closestEnemy = owner.transform;

        if (closestEnemy != null)
        {
            GameObject newShockStrike = GameObject.Instantiate(shockStrikePrefab, owner.transform.position, Quaternion.identity);
            newShockStrike.GetComponent<ShockStrike_Controller>().Setup(shockDamage, closestEnemy.GetComponent<CharacterStats>());
        }
    }

    public void ApplyIgniteDamage()
    {
        if (igniteDamageTimer < 0)
        {
            owner.DecreaseHealthBy(igniteDamage);
            if (owner.currentHealth < 0 && !owner.isDead)
                owner.KillEntity();
            igniteDamageTimer = igniteDamageCooldown;
        }
    }

    /// <summary>
    /// エイルメントを適用するための試行。対象の火・氷・雷ダメージを比較し、
    /// 優位な要素に応じたエイルメントを付与します。
    /// </summary>
    public void AttemptToApplyAilments(int fireDamage, int iceDamage, int lightningDamage)
    {
        bool canApplyIgnite = fireDamage > iceDamage && fireDamage > lightningDamage;
        bool canApplyChill = iceDamage > fireDamage && iceDamage > lightningDamage;
        bool canApplyShock = lightningDamage > fireDamage && lightningDamage > iceDamage;

        // 条件が整わない場合、ランダムで決定（元コードの挙動を再現）
        while (!canApplyIgnite && !canApplyChill && !canApplyShock)
        {
            if (Random.value < 0.5f && fireDamage > 0)
            {
                canApplyIgnite = true;
                ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
            if (Random.value < 0.5f && iceDamage > 0)
            {
                canApplyChill = true;
                ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
            if (Random.value < 0.5f && lightningDamage > 0)
            {
                canApplyShock = true;
                ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
        }

        if (canApplyIgnite)
            SetupIgniteDamage(Mathf.RoundToInt(fireDamage * 0.2f));
        if (canApplyShock)
            SetupShockStrikeDamage(Mathf.RoundToInt(lightningDamage * 0.1f));

        ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
    }
}
