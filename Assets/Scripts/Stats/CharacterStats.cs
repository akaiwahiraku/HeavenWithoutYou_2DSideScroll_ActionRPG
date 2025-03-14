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
    public int currentHealth;
    public Stat maxHealth;
    public Stat armor;
    public Stat evasion;
    public Stat magicResistance;

    [Header("Magic stats")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightningDamage;

    [Header("Ailments")]
    [SerializeField] private float ailmentsDuration = 4;
    [SerializeField] private GameObject shockStrikePrefab;

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
    public AilmentManager AilmentManager { get; private set; }

    // HitStopAndShakeManager ���O���Ϗ����邽�߂̎Q�Ɓi���̃R���|�[�l���g�͓��� GameObject �ɃA�^�b�`�j
    protected HitStopAndShakeManager hitStopAndShakeManager;

    protected virtual void Start()
    {
        critPower.SetDefaultValue(150);
        currentHealth = GetMaxHealthValue();

        // �q�I�u�W�F�N�g����A�j���[�V�����g���K�[���擾
        animationTriggers = GetComponentInChildren<PlayerAnimationTriggers>();

        fx = GetComponent<EntityFX>();
        player = GetComponent<Player>();

        // �G�C�������g������ AilmentManager �ɈϏ�
        AilmentManager = new AilmentManager(this, fx, ailmentsDuration, shockStrikePrefab);

        // �q�b�g�X�g�b�v�ƃV�F�C�N������ HitStopAndShakeManager �ɈϏ�
        hitStopAndShakeManager = GetComponent<HitStopAndShakeManager>();
    }

    protected virtual void Update()
    {
        // �����^�C�}�[�� AilmentManager �ɂ��X�V����邽�߁A������͕s�v
        AilmentManager.Update(Time.deltaTime);
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
        if (_targetStats.isInvincible)
            return;

        // ��𔻒���s���Aevaded �� true �Ȃ�_���[�W�y����K�p
        bool evaded = TargetCanAvoidAttack(_targetStats);

        // �m�b�N�o�b�N�����̐ݒ�
        _targetStats.GetComponent<Entity>().SetupKnockbackDir(transform);

        // DamageManager �Ɍv�Z���Ϗ��i�`���[�W�U���ł͂Ȃ����� false�j
        DamageManager damageCalc = new DamageManager();
        int totalDamage = damageCalc.CalculatePhysicalDamage(this, _targetStats, false);

        // ������������Ă����ꍇ�A�_���[�W�� 1/20 �Ɍy��
        if (evaded)
            totalDamage = Mathf.RoundToInt(totalDamage / 20f);

        fx.CreateHitFx(_targetStats.transform, false);
        _targetStats.TakeDamage(totalDamage);

        DoMagicalDamage(_targetStats);
        IncreaseOverDriveValue();

        if (hitStopAndShakeManager != null)
            StartCoroutine(hitStopAndShakeManager.TriggerHitStop());
    }

    public virtual void DoPhysicalDamageCharge(CharacterStats _targetStats)
    {
        if (_targetStats.isInvincible)
            return;

        // ��𔻒�i�`���[�W�U���ł����l�̕␳��K�p�j
        bool evaded = TargetCanAvoidAttack(_targetStats);

        _targetStats.GetComponent<Entity>().SetupKnockbackDir(transform);

        // DamageManager �Ɍv�Z���Ϗ��i�`���[�W�U���Ȃ̂� true�j
        DamageManager damageCalc = new DamageManager();
        int totalDamage = damageCalc.CalculatePhysicalDamage(this, _targetStats, true);

        // ��𐬌��Ȃ�_���[�W�� 1/20 �Ɍy��
        if (evaded)
            totalDamage = Mathf.RoundToInt(totalDamage / 20f);

        fx.CreateHitFx(_targetStats.transform, false);
        _targetStats.TakeDamage(totalDamage);

        DoMagicalDamage(_targetStats);
        IncreaseOverDriveValue();

        if (hitStopAndShakeManager != null)
            StartCoroutine(hitStopAndShakeManager.TriggerHitStopCharge());
    }


    public virtual void DoMagicalDamage(CharacterStats _targetStats)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightningDamage = lightningDamage.GetValue();

        int totalMagicalDamage = _fireDamage + _iceDamage + _lightningDamage + intelligence.GetValue();
        totalMagicalDamage = CheckTargetResistance(_targetStats, totalMagicalDamage);
        _targetStats.TakeDamage(totalMagicalDamage);

        if (Mathf.Max(_fireDamage, _iceDamage, _lightningDamage) <= 0)
            return;

        // �G�C�������g�t�^�����͑Ώۂ� AilmentManager �ɈϏ�
        AilmentManager.AttemptToApplyAilments(_fireDamage, _iceDamage, _lightningDamage);
    }

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
        onHealthChanged?.Invoke();
    }

    public virtual void DecreaseHealthBy(int _damage)
    {
        if (isVulnerable)
            _damage = Mathf.RoundToInt(_damage * 1.1f);
        if (currentHealth <= GetMaxHealthValue() * crisisPercent)
            _damage = Mathf.RoundToInt(_damage * 0.7f);
        if (isInGuardState)
            _damage = Mathf.RoundToInt(_damage * 0.2f);

        currentHealth -= _damage;
        if (_damage > 0)
            fx.CreatePopUpText(_damage.ToString());
        onHealthChanged?.Invoke();
    }

    public virtual void IncreaseOverDriveValue()
    {
        if (isInShadowBringerOverDrive2ndState || isInShadowBringerOverDrive1stState || overDriveStock == 3)
            return;

        int increaseAmount = (currentHealth <= GetMaxHealthValue() * crisisPercent) ? 2 : 1;
        currentOverDrive += increaseAmount;

        if (currentOverDrive >= GetMaxOverDriveValue())
        {
            if (overDriveStock < maxOverDriveStock)
                overDriveStock++;
            currentOverDrive = 0;
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
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Collider2D collider = GetComponent<Collider2D>();

        if (_invincible)
        {
            if (rb != null)
                rb.simulated = false;
            if (collider != null)
                collider.enabled = false;
        }
        else
        {
            if (rb != null)
                rb.simulated = true;
            if (collider != null)
                collider.enabled = true;
        }
    }

    protected int CheckTargetArmor(CharacterStats _targetStats, int totalDamage)
    {
        if (_targetStats.AilmentManager.IsChilled)
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * 0.8f);
        else
            totalDamage -= _targetStats.armor.GetValue();

        totalDamage -= _targetStats.armor.GetValue();
        return Mathf.Clamp(totalDamage, 0, int.MaxValue);
    }

    public int CheckTargetResistance(CharacterStats _targetStats, int totalMagicalDamage)
    {
        totalMagicalDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);
        return Mathf.Clamp(totalMagicalDamage, 0, int.MaxValue);
    }

    public virtual void OnEvasion() { }

    protected bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();
        if (_targetStats.AilmentManager != null && _targetStats.AilmentManager.IsShocked)
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
        return UnityEngine.Random.Range(0, 100) <= totalCriticalChance;
    }

    protected int CalculateCriticalDamage(int _damage)
    {
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * 0.01f;
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

    public Stat GetStat(StatType _statType)
    {
        switch (_statType)
        {
            case StatType.strength: return strength;
            case StatType.agility: return agility;
            case StatType.intelligence: return intelligence;
            case StatType.vitality: return vitality;
            case StatType.damage: return damage;
            case StatType.critChance: return critChance;
            case StatType.critPower: return critPower;
            case StatType.health: return maxHealth;
            case StatType.overDrive: return maxOverDrive;
            case StatType.armor: return armor;
            case StatType.evasion: return evasion;
            case StatType.magicRes: return magicResistance;
            case StatType.fireDamage: return fireDamage;
            case StatType.iceDamage: return iceDamage;
            case StatType.lightningDamage: return lightningDamage;
            default: return null;
        }
    }
}
