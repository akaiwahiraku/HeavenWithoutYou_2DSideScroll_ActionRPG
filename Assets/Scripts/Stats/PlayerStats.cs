using System;
using System.Collections;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    public static PlayerStats instance;

    [SerializeField] public GameObject ScreenFlashRedFX;
    private ScreenFlashRedFX screenFlashRed;

    [SerializeField] public GameObject ScreenFlashBlackoutFX;
    public ScreenFlashBlackoutFX screenFlashBlackout;

    [SerializeField] public GameObject OverDriveDreamtideTextFX;
    public OverDriveTextFX overDriveDreamtideText;

    [SerializeField] public GameObject OverDrivePhantasmNightTextFX;
    public OverDriveTextFX overDrivePhantasmNightText;

    [SerializeField] public GameObject OverDriveShatteredSunTextFX;
    public OverDriveTextFX overDriveShatteredSunText;

    [SerializeField] public GameObject OverDriveUnleashedTextFX;
    public OverDriveTextFX overDriveUnleashedText;

    [SerializeField] private float flashDuration = 0.1f; // �t���b�V���̎�������

    public bool isInOverdriveState = false;
    public int damageFromFloor = 50;

    [Header("Level & Experience")]
    public int currentLevel = 1;
    public int currentExperience = 0;
    public int experienceToNextLevel = 100;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    protected override void Start()
    {
        base.Start();
        player = GetComponent<Player>();

        if (ScreenFlashRedFX != null)
            screenFlashRed = ScreenFlashRedFX.GetComponent<ScreenFlashRedFX>();

        if (ScreenFlashBlackoutFX != null)
            screenFlashBlackout = ScreenFlashBlackoutFX.GetComponent<ScreenFlashBlackoutFX>();

        if (SaveManager.instance != null)
        {
            SaveManager.instance.LoadPlayerStats(this);
        }
    }

    protected override void Update()
    {
        base.Update();
    }

    // �o���l�ǉ��ƃ��x���A�b�v����
    public void AddExperience(int amount)
    {
        currentExperience += amount;
        while (currentExperience >= experienceToNextLevel)
        {
            currentExperience -= experienceToNextLevel;
            LevelUp();
        }
        SaveManager.instance?.SaveGame();
    }

    private void LevelUp()
    {
        currentLevel++;
        experienceToNextLevel = Mathf.RoundToInt(experienceToNextLevel * 1.5f);
        IncreaseStatsOnLevelUp();
        Debug.Log("���x���A�b�v�I ���݂̃��x��: " + currentLevel);
        SaveManager.instance?.SaveGame();
    }

    private void IncreaseStatsOnLevelUp()
    {
        strength.AddModifier(2);
        agility.AddModifier(1);
        vitality.AddModifier(1);
        intelligence.AddModifier(1);
        maxHealth.AddModifier(10);
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
    }

    protected override void Die()
    {
        base.Die();
        player.Die();
    }

    public override void DecreaseHealthBy(int _damage)
    {
        base.DecreaseHealthBy(_damage);

        if (isDead)
            return;

        if (_damage > GetMaxHealthValue() * 0.15f && !isInGuardState)
        {
            if (screenFlashRed != null)
                screenFlashRed.FlashRedScreen();

            player.fx.ScreenShake(player.fx.shakeHighDamage);
            AudioManager.instance.PlaySFX(28, null);
        }
        else if (isInGuardState)
        {
            player.fx.ScreenShake(player.fx.shakeNormalDamage);
            player.fx.PlayGuardFX();
            player.fx.PlayDustFX();
            AudioManager.instance.PlaySFX(0, null);
            player.SetupKnockbackPower(new Vector2(70, 5));
        }
        else
        {
            if (screenFlashBlackout != null)
                screenFlashBlackout.FlashBlackScreen();

            player.SetupKnockbackPower(new Vector2(4, 1));
            player.fx.ScreenShake(player.fx.shakeNormalDamage);
            AudioManager.instance.PlaySFX(28, null);
        }

        ItemData_Equipment currentArmor = Inventory.instance.GetEquipment(EquipmentType.Armor);
        if (currentArmor != null)
            currentArmor.Effect(player.transform);

        IncreaseOverDriveValue();
    }

    public override void DoPhysicalDamage(CharacterStats _targetStats)
    {
        // ���N���X�Ŋ��Ƀ_���[�W�v�Z�E�K�p�A�q�b�g�X�g�b�v�^�V�F�C�N���������s���Ă���
        base.DoPhysicalDamage(_targetStats);

        AudioManager.instance.PlaySFX(1, null);

        // �ǉ��̉�ʃV�F�C�N�Ȃǂ̉��o
        player.fx.ScreenShake(player.fx.shakeNormalAttack);

        // �����Ԃ̏ꍇ�A�ǉ��̃G�t�F�N�g�����s
        if (player.stateMachine.currentState is PlayerShadowBringerOverDrive1stState)
        {
            PlayerAnimationTriggers animTriggers = player.GetComponentInChildren<PlayerAnimationTriggers>();
            if (hitStopAndShakeManager != null)
            {
                StartCoroutine(hitStopAndShakeManager.ApplyHitStopAndShake(_targetStats, animTriggers.PushTrigger));
            }
        }

        // �N���[�����Ŏg�p����ꍇ�͑��x���Z�b�g
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.velocity = Vector2.zero;
    }

    public override void DoPhysicalDamageCharge(CharacterStats _targetStats)
    {
        base.DoPhysicalDamageCharge(_targetStats);
        AudioManager.instance.PlaySFX(1, null);

        PlayerAnimationTriggers animTriggers = player.GetComponentInChildren<PlayerAnimationTriggers>();
        if (hitStopAndShakeManager != null)
        {
            StartCoroutine(hitStopAndShakeManager.ApplyHitStopAndShake(_targetStats, animTriggers.PushTrigger));
        }

        player.fx.ScreenShake(player.fx.shakeNormalAttack);
    }

    public void CloneDoDamage(CharacterStats _targetStats, float _multiplier, int facingDir)
    {
        if (TargetCanAvoidAttack(_targetStats) || _targetStats.isDead)
            return;

        int totalDamage = damage.GetValue() + strength.GetValue();
        if (_multiplier > 0)
            totalDamage = Mathf.RoundToInt(totalDamage * _multiplier);

        if (CanCrit())
            totalDamage = CalculateCriticalDamage(totalDamage);

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        _targetStats.TakeDamage(totalDamage);
        DoMagicalDamage(_targetStats);

        player.fx.CreateCloneHitFx(_targetStats.transform, facingDir);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.velocity = Vector2.zero;

        IncreaseOverDriveValue();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DamageFloor"))
            TakeDamage(damageFromFloor);
    }
}
