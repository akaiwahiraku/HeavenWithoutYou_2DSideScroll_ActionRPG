using System;
using System.Collections;
using UnityEngine;

public enum StatType
{
    strength,
    agility,
    intelligence,
    vitality,
    damage,
    critChance,
    critPower,
    health,
    overDrive,
    armor,
    evasion,
    magicRes,
    fireDamage,
    iceDamage,
    lightningDamage
}

public class CharacterStats : MonoBehaviour
{
    protected Player player;
    protected EntityFX fx;
    public bool isInShadowBringerOverDrive1stState;
    public bool isInShadowBringerOverDrive2ndState;
    public bool isInGuardState;

    [Header("Major stats")]
    public Stat strength; // 1 point increase damage by 1 and crit.power by 1%
    public Stat agility; // 1 point increase evasion by 1% and crit.chance by 1%
    public Stat intelligence; // 1 point increace magic damage by 1 and magic resistance by 3
    public Stat vitality; // 1 point increase health by 5 points

    [Header("Offensive stats")]
    public Stat damage;
    public Stat critChance;
    public Stat critPower;  // default value 150%
    public Stat maxOverDrive;

    [Header("Defensive stats")]
    public Stat maxHealth;
    public Stat armor;
    public Stat evasion;
    public Stat magicResistance;

    [Header("Magic stats")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightningDamage;

    [Space]
    public bool isIgnited;  // does damage over time
    public bool isChilled;  // reduce armor by 20%
    public bool isShocked;  // reduce accuracy by 20%

    [SerializeField] private float ailmentsDuration = 4;
    private float ignitedTimer;
    private float chilledTimer;
    private float shockedTimer;
    private float igniteDamageCooldown = .3f;
    private float igniteDamageTimer;
    private int igniteDamage;
    [SerializeField] private GameObject shockStrikePrefab;
    private int shockDamage;
    public int currentHealth;

    [Header("OverDrive")]
    public int currentOverDrive;
    public int overDriveStock = 0;  // OverDrive�X�g�b�N
    private const int maxOverDriveStock = 3;  // �ő�X�g�b�N��

    public System.Action onHealthChanged;
    public bool isDead { get; private set; }
    public bool isInvincible { get; private set; }
    private bool isVulnerable;

    [Space]
    public float crisisPercent;

    public PlayerAnimationTriggers animationTriggers;

    [Header("Hit Stop Settings")]
    [SerializeField] private float hitStopDuration = 0.03f; // �U�����󂯂��ꍇ�̃q�b�g�X�g�b�v�̎�������
    [SerializeField] private float shakeMagnitude = 0.4f;  // ��L�̏ꍇ�̃V�F�C�N�̋��x
    private bool isHitStopping = false;
    [SerializeField] private float doHitStopDuration = 0.03f; // �U���𓖂Ă��ꍇ�̃q�b�g�X�g�b�v�̎�������
    [SerializeField] private float doShakeMagnitude = 0.9f;  // ��L�̏ꍇ�̃V�F�C�N�̋��x

    protected virtual void Start()
    {
        critPower.SetDefaultValue(150);
        currentHealth = GetMaxHealthValue();
        //currentOverDrive = GetMaxOverDriveValue();

        // �v���C���[�̎q�I�u�W�F�N�g�ɂ���PlayerAnimationTriggers���擾
        animationTriggers = GetComponentInChildren<PlayerAnimationTriggers>();

        fx = GetComponent<EntityFX>();
        player = GetComponent<Player>();
    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;

        igniteDamageTimer -= Time.deltaTime;

        if (ignitedTimer < 0)
            isIgnited = false;
        if (chilledTimer < 0)
            isChilled = false;
        if (shockedTimer < 0)
            isShocked = false;

        if (isIgnited)
            ApplyIgniteDamage();
    }


    public void MakeVulnerableFor(float _duration) => StartCoroutine(VulnerableForCoroutine(_duration));

    private IEnumerator VulnerableForCoroutine(float _duration)
    {
        isVulnerable = true;

        yield return new WaitForSeconds(_duration);

        isVulnerable = false;
    }

    public virtual void IncreaseStatBy(int _modifier, float _duration, Stat _statToModify)
    {
        StartCoroutine(StatModCoroutine(_modifier, _duration, _statToModify));
    }

    private IEnumerator StatModCoroutine(int _modifier, float _duration, Stat _statToModify)
    {
        _statToModify.AddModifier(_modifier);

        yield return new WaitForSeconds(_duration);

        _statToModify.RemoveModifier(_modifier);
    }

    public virtual void DoPhysicalDamage(CharacterStats _targetStats)
    {
        bool criticalStrike = false;

        if (_targetStats.isInvincible)
            return;

        if (TargetCanAvoidAttack(_targetStats))
            return;

        _targetStats.GetComponent<Entity>().SetupKnockbackDir(transform);

        int totalDamage = damage.GetValue() + strength.GetValue();

        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
            criticalStrike = true;
        }

        fx.CreateHitFx(_targetStats.transform, criticalStrike);

        // �N���C�V�X���[�h�̏���
        if (currentHealth <= GetMaxHealthValue() * crisisPercent)
        {
            totalDamage = Mathf.RoundToInt(totalDamage * 1.25f);
        }

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        _targetStats.TakeDamage(totalDamage);

        DoMagicalDamage(_targetStats);

        IncreaseOverDriveValue();

        StartCoroutine(TriggerHitStop());

    }

    public virtual void DoPhysicalDamageCharge(CharacterStats _targetStats)
    {
        bool criticalStrike = false;

        if (_targetStats.isInvincible)
            return;

        if (TargetCanAvoidAttack(_targetStats))
            return;

        _targetStats.GetComponent<Entity>().SetupKnockbackDir(transform);

        // �ʏ��2�{�̔{��
        float multiplier = 2f;
        // �U���҂�Player�̏ꍇ�A�K�[�h������0.5�b�ȓ��Ȃ�{����1.5�{�A�b�v�i��3�{�ɂȂ�j
        Player playerAttacker = GetComponent<Player>();
        if (playerAttacker != null && playerAttacker.lastGuardExitTime > 0 &&
            Time.time - playerAttacker.lastGuardExitTime <= 0.8f)
        {
            StartCoroutine(ShakeEffectCharge());
    

            multiplier *= 1.5f; // multiplier �� 3.0f �ɂȂ�
        }

        int totalDamage = Mathf.RoundToInt((damage.GetValue() + strength.GetValue()) * multiplier);

        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
            criticalStrike = true;
        }

        fx.CreateHitFx(_targetStats.transform, criticalStrike);

        // �N���C�V�X���[�h�̏���
        if (currentHealth <= GetMaxHealthValue() * crisisPercent)
        {
            totalDamage = Mathf.RoundToInt(totalDamage * 1.25f);
        }

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        _targetStats.TakeDamage(totalDamage);

        DoMagicalDamage(_targetStats);

        IncreaseOverDriveValue();

        StartCoroutine(TriggerHitStopCharge());
    }


    protected IEnumerator TriggerHitStop()
    {
        if (isHitStopping)
            yield break;

        isHitStopping = true;
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(hitStopDuration);

        Time.timeScale = 1f;
        isHitStopping = false;

        StartCoroutine(ShakeEffect());
    }

    protected IEnumerator TriggerHitStopCharge()
    {
        if (isHitStopping)
            yield break;

        isHitStopping = true;
        Time.timeScale = 0f;

        float chargeStopDuration = hitStopDuration * 5;

        yield return new WaitForSecondsRealtime(chargeStopDuration);

        Time.timeScale = 1f;
        isHitStopping = false;

        StartCoroutine(ShakeEffectCharge());
    }

    // �q�b�g�X�g�b�v�����������Ƃ��A�ΏۃI�u�W�F�N�g��Animator�R���|�[�l���g��speed��0�ɂ���
    IEnumerator TriggerLocalHitStop(GameObject target, float duration)
    {
        // Animator ���ꎞ��~
        Animator anim = target.GetComponent<Animator>();
        float originalSpeed = anim.speed;
        anim.speed = 0f;

        // �K�v�Ȃ�A�Ώۂ̃X�N���v�g���ꎞ�I�ɒ�~�i��Fenabled �� false �ɂ���j
        // target.GetComponent<YourMovementScript>().enabled = false;

        // duration �b�҂i�������Z�͒ʏ�ʂ�i�ށj
        yield return new WaitForSecondsRealtime(duration);

        // Animator �̑��x�𕜋A
        anim.speed = originalSpeed;

        // �Ώۂ̃X�N���v�g���ėL��������
        // target.GetComponent<YourMovementScript>().enabled = true;
    }


    private IEnumerator ShakeEffect()
    {
        Vector3 originalPosition = transform.position;
        float elasped = 0.0f;

        while (elasped < hitStopDuration)
        {
            float x = UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;
            float y = UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;

            transform.position = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elasped += Time.unscaledDeltaTime;
            yield return null;
        }

    }

    private IEnumerator ShakeEffectCharge()
    {
        Vector3 originalPosition = transform.position;
        float elasped = 0.0f;

        while (elasped < hitStopDuration)
        {
            float x = UnityEngine.Random.Range(-10f, 10f) * shakeMagnitude;
            float y = UnityEngine.Random.Range(-10f, 10f) * shakeMagnitude;

            transform.position = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elasped += Time.unscaledDeltaTime;
            yield return null;
        }

    }


    protected IEnumerator ApplyHitStopAndShake(CharacterStats target, Action onHitStopComplete = null)
    {
        // �R���[�`���J�n���ɑΏۂ��L�����`�F�b�N
        if (target == null)
        {
            Debug.LogWarning("ApplyHitStopAndShake: Target is null or destroyed at coroutine start.");
            onHitStopComplete?.Invoke();
            yield break;
        }

        // Rigidbody2D �̎擾�Ə���
        Rigidbody2D targetRigidbody = target.GetComponent<Rigidbody2D>();
        if (targetRigidbody != null)
        {
            Vector2 originalVelocity = targetRigidbody.velocity;
            targetRigidbody.velocity = Vector2.zero; // �^�[�Q�b�g����u��~

            yield return new WaitForSecondsRealtime(hitStopDuration); // �q�b�g�X�g�b�v�̑ҋ@

            // �ҋ@��ɍēx�Ώۂ̗L�������`�F�b�N
            if (target == null)
            {
                Debug.LogWarning("ApplyHitStopAndShake: Target is null or destroyed after hit stop wait.");
                onHitStopComplete?.Invoke();
                yield break;
            }
            targetRigidbody.velocity = originalVelocity; // ���̑��x�ɖ߂�
        }
        else
        {
            Debug.LogWarning("ApplyHitStopAndShake: Rigidbody2D is null or destroyed.");
            yield return new WaitForSecondsRealtime(hitStopDuration);
        }

        // �V�F�C�N�����̑O�ɑΏۂ��܂��L�����m�F
        if (target == null)
        {
            Debug.LogWarning("ApplyHitStopAndShake: Target is null or destroyed before shake effect.");
            onHitStopComplete?.Invoke();
            yield break;
        }

        // �V�F�C�N�̏���
        Vector3 originalPosition = target.transform.position;
        float elapsed = 0.0f;
        while (elapsed < hitStopDuration)
        {
            // �r���ł��Ώۂ̗L�������`�F�b�N
            if (target == null)
            {
                Debug.LogWarning("ApplyHitStopAndShake: Target is null or destroyed during shake effect.");
                onHitStopComplete?.Invoke();
                yield break;
            }

            float x = UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;
            float y = UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;
            target.transform.position = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        // �ŏI�I�Ɍ��̈ʒu�֖߂��i�Ώۂ��L���ȏꍇ�̂݁j
        if (target != null)
            target.transform.position = originalPosition;

        onHitStopComplete?.Invoke();
    }


    #region Magical damage and ailements
    public virtual void DoMagicalDamage(CharacterStats _targetStats)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightningDamage = lightningDamage.GetValue();

        int totalMagicalDamage = _fireDamage + _iceDamage + _lightningDamage + intelligence.GetValue();

        totalMagicalDamage = ChechTargetResistance(_targetStats, totalMagicalDamage);
        _targetStats.TakeDamage(totalMagicalDamage);

        if (Mathf.Max(_fireDamage, _iceDamage, _lightningDamage) <= 0)
            return;


        AttemptyToApplyAilments(_targetStats, _fireDamage, _iceDamage, _lightningDamage);

    }


    private void AttemptyToApplyAilments(CharacterStats _targetStats, int _fireDamage, int _iceDamage, int _lightningDamage)
    {
        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightningDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightningDamage;
        bool canApplyShock = _lightningDamage > _fireDamage && _lightningDamage > _iceDamage;

        while (!canApplyIgnite && !canApplyChill && !canApplyShock)
        {
            if (UnityEngine.Random.value < .5f && _fireDamage > 0)
            {
                canApplyIgnite = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }

            if (UnityEngine.Random.value < .5f && _iceDamage > 0)
            {
                canApplyChill = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }

            if (UnityEngine.Random.value < .5f && _lightningDamage > 0)
            {
                canApplyShock = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
        }

        if (canApplyIgnite)
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * .2f));

        if (canApplyShock)
            _targetStats.SetupShockStrikeDamage(Mathf.RoundToInt(_lightningDamage * .1f));

        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
    }


    public void ApplyAilments(bool _ignite, bool _chill, bool _shock)
    {
        bool canApplyIgnite = !isIgnited && !isChilled && !isShocked;
        bool canApplyChill = !isIgnited && !isChilled && !isShocked;
        bool canApplyShock = !isIgnited && !isChilled;


        if (_ignite && canApplyIgnite)
        {
            isIgnited = _ignite;
            ignitedTimer = ailmentsDuration;

            fx.IgniteFxFor(ailmentsDuration);
        }

        if (_chill && canApplyChill)
        {
            chilledTimer = ailmentsDuration;
            isChilled = _chill;

            float slowPercentage = .2f;

            GetComponent<Entity>().SlowEntityBy(slowPercentage, ailmentsDuration);
            fx.ChillFxFor(ailmentsDuration);
        }
        if (_shock && canApplyShock)
        {
            if (!isShocked)
            {
                ApplyShock(_shock);
            }
            else
            {
                if (GetComponent<Player>() != null)
                    return;
                HitNearestTargetWithShockStrike();
            }

        }
    }

    public void ApplyShock(bool _shock)
    {
        if (isShocked)
            return;

        shockedTimer = ailmentsDuration;
        isShocked = _shock;

        fx.ShockFxFor(ailmentsDuration);
    }

    private void HitNearestTargetWithShockStrike()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && Vector2.Distance(transform.position, hit.transform.position) > 1)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;

                }
            }

            if (closestEnemy == null)  // delete if you don't want shocked target to be hit by shock strike
                closestEnemy = transform;
        }

        if (closestEnemy != null)
        {
            GameObject newShockStrike = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity);
            newShockStrike.GetComponent<ShockStrike_Controller>().Setup(shockDamage, closestEnemy.GetComponent<CharacterStats>());

        }
    }

    private void ApplyIgniteDamage()
    {
        if (igniteDamageTimer < 0)
        {
            DecreaseHealthBy(igniteDamage);

            if (currentHealth < 0 && !isDead)
                Die();

            igniteDamageTimer = igniteDamageCooldown;
        }
    }
    public void SetupIgniteDamage(int _damage) => igniteDamage = _damage;
    public void SetupShockStrikeDamage(int _damage) => shockDamage = _damage;

    #endregion

    public virtual void TakeDamage(int _damage)
    {

        if (isInvincible)
            return;

        DecreaseHealthBy(_damage);

        GetComponent<Entity>().DamageImpact();
        fx.StartCoroutine("FlashFX");

        if (currentHealth < 0 && !isDead)
            Die();
    }

    public virtual void IncreaseHealthBy(int _amount)
    {
        currentHealth += _amount;

        if (currentHealth > GetMaxHealthValue())
            currentHealth = GetMaxHealthValue();

        if (onHealthChanged != null)
            onHealthChanged();
    }

    public virtual void DecreaseHealthBy(int _damage)
    {
        //�Ǝ���
        if (isVulnerable)
            _damage = Mathf.RoundToInt(_damage * 1.1f);

        //�N���C�V�X���[�h�̏���
        if (currentHealth <= GetMaxHealthValue() * crisisPercent)
            _damage = Mathf.RoundToInt(_damage * 0.7f);

        //�K�[�h�X�e�[�g�ɓ����Ă���ꍇ�̃_���[�W��������
        if (isInGuardState)
        {
            _damage = Mathf.RoundToInt(_damage * 0.2f);
        }

        currentHealth -= _damage;

        if (_damage > 0)
            fx.CreatePopUpText(_damage.ToString());

        if (onHealthChanged != null)
            onHealthChanged();
    }

    //�I�[�o�[�h���C�u�Q�[�W�𑝂₷����
    public virtual void IncreaseOverDriveValue()
    {
        // ����̃X�e�[�g�� OverDrive �Q�[�W�������Ȃ��悤�ɂ������
        if (isInShadowBringerOverDrive2ndState || isInShadowBringerOverDrive1stState || overDriveStock == 3)
            return;

        // �N���C�V�X���[�h�̏ꍇ�� OverDrive �Q�[�W�𑽂߂ɑ���
        int increaseAmount = (currentHealth <= GetMaxHealthValue() * crisisPercent) ? 2 : 1;
        currentOverDrive += increaseAmount;

        // OverDrive��MAX�ɒB�����ꍇ�̏���
        if (currentOverDrive >= GetMaxOverDriveValue())
        {
            if (overDriveStock < maxOverDriveStock)
            {
                overDriveStock++;  // �X�g�b�N��1���₷
            }
            currentOverDrive = 0;  // OverDrive�Q�[�W�����Z�b�g
        }
    }


    protected virtual void Die()
    {
        isDead = true;
    }

    public void KillEntity()
    {
        if (!isDead)
            Die();
    }

    public void MakeInvincible(bool _invincible) => isInvincible = _invincible;

    public void MakeTransparent(bool _invincible)
    {
        isInvincible = _invincible;

        // Rigidbody2D��Collider2D���擾
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Collider2D collider = GetComponent<Collider2D>();

        if (_invincible)
        {
            // �U����������Ȃ��悤��Rigidbody2D�𖳌�������
            if (rb != null)
                rb.simulated = false;  // �������Z���~����

            // Collider�𖳌�������
            if (collider != null)
                collider.enabled = false;  // �Փ˔���𖳌���
        }
        else
        {
            // �U�����Ăѓ��Ă���悤��Rigidbody2D��Collider2D��L��������
            if (rb != null)
                rb.simulated = true;  // �������Z���ĊJ����

            if (collider != null)
                collider.enabled = true;  // �Փ˔����L����
        }
    }

    #region Stat calculations
    protected int CheckTargetArmor(CharacterStats _targetStats, int totalDamage)
    {
        if (_targetStats.isChilled)
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * .8f);
        else
            totalDamage -= _targetStats.armor.GetValue();

        totalDamage -= _targetStats.armor.GetValue();
        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }

    public int ChechTargetResistance(CharacterStats _targetStats, int totalMagicalDamage)
    {
        totalMagicalDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);
        totalMagicalDamage = Mathf.Clamp(totalMagicalDamage, 0, int.MaxValue);
        return totalMagicalDamage;
    }

    public virtual void OnEvasion()
    {

    }

    protected bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        if (isShocked)
            totalEvasion += 20;

        if (UnityEngine.Random.Range(0, 100) < totalEvasion)
        {
            _targetStats.OnEvasion();
            return true;
        }

        return false;
    }


    protected bool CanCrit()
    {
        int totalCriticalChance = critChance.GetValue() + agility.GetValue();

        if (UnityEngine.Random.Range(0, 100) <= totalCriticalChance)
        {
            return true;
        }

        return false;
    }

    protected int CalculateCriticalDamage(int _damage)
    {
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * .01f;
        float critDamage = _damage * totalCritPower;

        return Mathf.RoundToInt(critDamage);
    }

    public int GetMaxHealthValue()
    {
        return maxHealth.GetValue() + vitality.GetValue() * 5;
    }

    public int GetMaxOverDriveValue()
    {
        return maxOverDrive.GetValue();
    }


    #endregion

    public Stat GetStat(StatType _statType)
    {
        if (_statType == StatType.strength) return strength;
        else if (_statType == StatType.agility) return agility;
        else if (_statType == StatType.intelligence) return intelligence;
        else if (_statType == StatType.vitality) return vitality;
        else if (_statType == StatType.damage) return damage;
        else if (_statType == StatType.critChance) return critChance;
        else if (_statType == StatType.critPower) return critPower;
        else if (_statType == StatType.health) return maxHealth;
        else if (_statType == StatType.overDrive) return maxOverDrive;
        else if (_statType == StatType.armor) return armor;
        else if (_statType == StatType.evasion) return evasion;
        else if (_statType == StatType.magicRes) return magicResistance;
        else if (_statType == StatType.fireDamage) return fireDamage;
        else if (_statType == StatType.iceDamage) return iceDamage;
        else if (_statType == StatType.lightningDamage) return lightningDamage;

        return null;
    }
}
