using Unity.VisualScripting.Antlr3.Runtime.Misc;
using System.Collections;
using UnityEngine;
using System;

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

    // ���x���ƌo���l�̃t�B�[���h
    [Header("Level&Experience")]
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

        if (ScreenFlashRedFX != null )
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

    // �o���l��ǉ����郁�\�b�h
    public void AddExperience(int amount)
    {
        currentExperience += amount;

        // �o���l���K�v�ʂɒB�����ꍇ�A���x���A�b�v����
        while (currentExperience >= experienceToNextLevel)
        {
            currentExperience -= experienceToNextLevel;
            LevelUp();
        }

        SaveManager.instance?.SaveGame();  // �X�e�[�^�X�X�V��ɕۑ�
    }

    // ���x���A�b�v�̏���
    private void LevelUp()
    {
        currentLevel++;

        // ���̃��x���ɕK�v�Ȍo���l�𑝂₷�i��Ƃ���1.5�{�j
        experienceToNextLevel = Mathf.RoundToInt(experienceToNextLevel * 1.5f);

        // ���x���A�b�v���̃X�e�[�^�X����
        IncreaseStatsOnLevelUp();

        Debug.Log("���x���A�b�v�I ���݂̃��x��: " + currentLevel);

        SaveManager.instance?.SaveGame();  // ���x���A�b�v��ɕۑ�

    }

    // ���x���A�b�v���̃X�e�[�^�X����
    private void IncreaseStatsOnLevelUp()
    {
        strength.AddModifier(2);
        agility.AddModifier(1);
        vitality.AddModifier(1);
        intelligence.AddModifier(1);
        maxHealth.AddModifier(10);
        //currentHealth = GetMaxHealthValue();  // HP���ő�ɉ�
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
    }

    protected override void Die()
    {
        base.Die();
        player.Die();

        // currency��Currency�v���p�e�B�o�R�ő���
        //GameManager.instance.lostCurrencyAmount = CurrencyManager.instance.Currency;
        //CurrencyManager.instance.Currency = 0;

        //GetComponent<PlayerItemDrop>()?.GenerateDrop();
    }


    //�_���[�W�����̏���
    public override void DecreaseHealthBy(int _damage)
    {
        base.DecreaseHealthBy(_damage);

        if (isDead)
            return;

        if (_damage > GetMaxHealthValue() * .15f && !isInGuardState)
        {
            if (screenFlashRed != null)
                screenFlashRed.FlashRedScreen();

            //player.SetupKnockbackPower(new Vector2(30, 2));
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

            if (_damage > GetMaxHealthValue() * .225f)
            {
                //if (screenFlashRed != null)
                //    screenFlashRed.FlashRedScreen();
            }
            else
            {

            }
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
        base.DoPhysicalDamage(_targetStats);

        AudioManager.instance.PlaySFX(1, null);

        //overDriveText.FlashOverDriveText();


        // �����U���q�b�g���A�^�[�Q�b�g�Ƀq�b�g�X�g�b�v�ƃV�F�C�N��K�p
        StartCoroutine(ApplyHitStopAndShake(_targetStats));
        player.fx.ScreenShake(player.fx.shakeNormalAttack);

        // ���݂̃X�e�[�g��PlayerShadowBringerOverDrive1stState���ǂ������m�F���A�q�b�g�X�g�b�v�ƃV�F�C�N��K�p
        if (player.stateMachine.currentState is PlayerShadowBringerOverDrive1stState)
        {
            PlayerAnimationTriggers animationTriggers = player.GetComponentInChildren<PlayerAnimationTriggers>();
            StartCoroutine(ApplyHitStopAndShake(_targetStats, animationTriggers.PushTrigger));
            StartCoroutine(TriggerHitStop());
            StartCoroutine(ApplyHitStopAndShake(_targetStats, animationTriggers.PushTrigger));
        }

        // �N���[���̑��x�����Z�b�g���鏈����ǉ�
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;  // �N���[���̑��x�����Z�b�g
        }
    }

    public override void DoPhysicalDamageCharge(CharacterStats _targetStats)
    {
        base.DoPhysicalDamage(_targetStats);

        AudioManager.instance.PlaySFX(1, null);

        PlayerAnimationTriggers animationTriggers = player.GetComponentInChildren<PlayerAnimationTriggers>();
        StartCoroutine(ApplyHitStopAndShake(_targetStats, animationTriggers.PushTrigger));
        StartCoroutine(TriggerHitStop());

        // �����U���q�b�g���A�^�[�Q�b�g�Ƀq�b�g�X�g�b�v�ƃV�F�C�N��K�p
        StartCoroutine(ApplyHitStopAndShake(_targetStats));
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
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
        }


        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        _targetStats.TakeDamage(totalDamage);

        DoMagicalDamage(_targetStats); // remove if you don't want to apply magic hit on primary attack

        //�q�b�gFX�̎���
        player.fx.CreateCloneHitFx(_targetStats.transform, facingDir);

        // ���x���[���Ƀ��Z�b�g���ăN���[�������ɍs���߂��Ȃ��悤�ɂ���
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;  // �N���[���̑��x�����Z�b�g

        }

        IncreaseOverDriveValue();
    }

    //�_���[�W��
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �G�ꂽ�I�u�W�F�N�g���_���[�W��^���鏰���ǂ������m�F
        if (collision.CompareTag("DamageFloor"))
        {
            TakeDamage(damageFromFloor);
        }
    }
}
