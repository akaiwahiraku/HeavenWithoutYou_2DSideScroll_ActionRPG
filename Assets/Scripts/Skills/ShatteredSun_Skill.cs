using UnityEngine;
using UnityEngine.UI;

public class ShatteredSun_Skill : Skill
{
    [Header("ShatteredSun Skill Settings")]
    [SerializeField] private UI_SkillTreeSlot shatteredSunUnlockButton;
    [SerializeField] private GameObject shatteredSunPrefab;
    [SerializeField] private float freezeTimeDuration = 0.3f;
    [SerializeField] private float damageMultiplier = 10.0f;
    [SerializeField, Tooltip("エフェクトがどれだけ拡大するか（例：2.0なら2倍に拡大）")]
    private float shatteredSunScaleMultiplier = 17.5f;
    [SerializeField, Tooltip("エフェクトが拡大するのにかかる時間（秒）")]
    private float shatteredSunExpansionDuration = 0.25f;
    [SerializeField, Tooltip("エフェクトによるノックバックの威力")]
    private float knockbackForce = 75f;

    // 新規：持続ダメージの持続時間とダメージ間隔（インスペクタで変更可能）
    [SerializeField, Tooltip("持続時間（秒）")]
    private float sustainedDamageDuration = 2.0f;
    [SerializeField, Tooltip("ダメージの間隔（秒）")]
    private float damageInterval = 0.5f;

    public bool shatteredSunUnlocked { get; private set; }

    private ShatteredSun_Skill_Controller currentShatteredSun;
    private bool hasLaunched = false;

    protected override void Start()
    {
        base.Start();
        shatteredSunUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockShatteredSun);
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void CheckUnlock()
    {
        UnlockShatteredSun();
    }

    private void UnlockShatteredSun()
    {
        if (shatteredSunUnlockButton.unlocked)
        {
            shatteredSunUnlocked = true;
            isUnlocked = true;
        }
    }

    public override void UseSkill()
    {
        base.UseSkill();
        // 追加処理があれば記述
    }

    // スキル状態のリセット
    public void ResetSkillState()
    {
        hasLaunched = false;
        currentShatteredSun = null;
    }

    public void CreateShatteredSun()
    {
        // 画面中央にエフェクトを生成
        Vector3 spawnPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        spawnPosition.z = 0f;
        GameObject shatteredSun = Instantiate(shatteredSunPrefab, spawnPosition, Quaternion.identity);
        currentShatteredSun = shatteredSun.GetComponent<ShatteredSun_Skill_Controller>();

        if (currentShatteredSun != null)
        {
            currentShatteredSun.SetupShatteredSun(
                player,
                freezeTimeDuration,
                damageMultiplier,
                shatteredSunScaleMultiplier,
                shatteredSunExpansionDuration,
                knockbackForce,
                sustainedDamageDuration,
                damageInterval);
        }
        player.AssignNewShatteredSun(shatteredSun);
        hasLaunched = true;
    }

    public bool SkillCompleted()
    {
        if (!hasLaunched)
            return false;
        if (currentShatteredSun == null)
            return true;
        if (currentShatteredSun.playerCanExitState)
        {
            currentShatteredSun = null;
            return true;
        }
        return false;
    }
}
