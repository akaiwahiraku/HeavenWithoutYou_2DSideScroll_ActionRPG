using UnityEngine;
using UnityEngine.UI;

public class Force_Skill : Skill
{
    [Header("Force Skill Settings")]
    [SerializeField] private UI_SkillTreeSlot forceUnlockButton;
    [SerializeField] private GameObject forcePrefab;
    [SerializeField] private float freezeTimeDuration = 0.3f;
    [SerializeField] private float damageMultiplier = 10.0f;
    [SerializeField, Tooltip("爆発がどれだけ拡大するか（例：2.0なら2倍に拡大）")]
    private float forceScaleMultiplier = 17.5f;
    [SerializeField, Tooltip("爆発が拡大するのにかかる時間（秒）")]
    private float forceExpansionDuration = 0.25f;
    [SerializeField, Tooltip("爆発によるノックバックの威力")]
    private float knockbackForce = 75f;

    public bool forceUnlocked { get; private set; }

    private Force_Skill_Controller currentForce;

    protected override void Start()
    {
        base.Start();
        forceUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockExplosion);
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void CheckUnlock()
    {
        UnlockExplosion();
    }

    private void UnlockExplosion()
    {
        if (forceUnlockButton.unlocked)
        {
            forceUnlocked = true;
            isUnlocked = true;
        }
    }

    public override void UseSkill()
    {
        base.UseSkill();
        // 必要に応じてスキル発動時の追加処理を記述
    }

    public void CreateForce()
    {
        // プレイヤーの現在位置で生成
        Vector3 spawnPosition = player.transform.position;
        GameObject force = Instantiate(forcePrefab, spawnPosition, Quaternion.identity);
        currentForce = force.GetComponent<Force_Skill_Controller>();

        if (currentForce != null)
        {
            // インスペクタで設定された各パラメータを渡す
            currentForce.SetupForce(player, freezeTimeDuration, damageMultiplier, forceScaleMultiplier, forceExpansionDuration, knockbackForce);
        }
        else
        {
            Debug.LogWarning("Force_Skill: Force_Skill_Controller not found.");
        }
        player.AssignNewForce(force);
    }
}
