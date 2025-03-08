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

    [SerializeField] public GameObject OverDrive1stTextFX;
    public OverDriveTextFX overDrive1stText;

    [SerializeField] public GameObject OverDrive2ndTextFX;
    public OverDriveTextFX overDrive2ndText;

    [SerializeField] public GameObject OverDrive2ndShatteredTextFX;
    public OverDriveTextFX overDrive2ndShatteredText;

    [SerializeField] private float flashDuration = 0.1f; // フラッシュの持続時間

    public bool isInOverdriveState = false;

    public int damageFromFloor = 50;

    // レベルと経験値のフィールド
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

    // 経験値を追加するメソッド
    public void AddExperience(int amount)
    {
        currentExperience += amount;

        // 経験値が必要量に達した場合、レベルアップ処理
        while (currentExperience >= experienceToNextLevel)
        {
            currentExperience -= experienceToNextLevel;
            LevelUp();
        }

        SaveManager.instance?.SaveGame();  // ステータス更新後に保存
    }

    // レベルアップの処理
    private void LevelUp()
    {
        currentLevel++;

        // 次のレベルに必要な経験値を増やす（例として1.5倍）
        experienceToNextLevel = Mathf.RoundToInt(experienceToNextLevel * 1.5f);

        // レベルアップ時のステータス強化
        IncreaseStatsOnLevelUp();

        Debug.Log("レベルアップ！ 現在のレベル: " + currentLevel);

        SaveManager.instance?.SaveGame();  // レベルアップ後に保存

    }

    // レベルアップ時のステータス強化
    private void IncreaseStatsOnLevelUp()
    {
        strength.AddModifier(2);
        agility.AddModifier(1);
        vitality.AddModifier(1);
        intelligence.AddModifier(1);
        maxHealth.AddModifier(10);
        //currentHealth = GetMaxHealthValue();  // HPを最大に回復
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
    }

    protected override void Die()
    {
        base.Die();
        player.Die();

        // currencyをCurrencyプロパティ経由で操作
        //GameManager.instance.lostCurrencyAmount = CurrencyManager.instance.Currency;
        //CurrencyManager.instance.Currency = 0;

        //GetComponent<PlayerItemDrop>()?.GenerateDrop();
    }


    //ダメージ減少の処理
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


        // 物理攻撃ヒット時、ターゲットにヒットストップとシェイクを適用
        StartCoroutine(ApplyHitStopAndShake(_targetStats));
        player.fx.ScreenShake(player.fx.shakeNormalAttack);

        // 現在のステートがPlayerShadowBringerOverDrive1stStateかどうかを確認し、ヒットストップとシェイクを適用
        if (player.stateMachine.currentState is PlayerShadowBringerOverDrive1stState)
        {
            PlayerAnimationTriggers animationTriggers = player.GetComponentInChildren<PlayerAnimationTriggers>();
            StartCoroutine(ApplyHitStopAndShake(_targetStats, animationTriggers.PushTrigger));
            StartCoroutine(TriggerHitStop());
            StartCoroutine(ApplyHitStopAndShake(_targetStats, animationTriggers.PushTrigger));
        }

        // クローンの速度をリセットする処理を追加
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;  // クローンの速度をリセット
        }
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

        //ヒットFXの実装
        player.fx.CreateCloneHitFx(_targetStats.transform, facingDir);

        // 速度をゼロにリセットしてクローンが下に行き過ぎないようにする
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;  // クローンの速度をリセット

        }

        IncreaseOverDriveValue();
    }

    //ダメージ床
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 触れたオブジェクトがダメージを与える床かどうかを確認
        if (collision.CompareTag("DamageFloor"))
        {
            TakeDamage(damageFromFloor);
        }
    }
}
