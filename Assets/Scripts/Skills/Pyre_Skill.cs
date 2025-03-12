using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pyre_Skill : Skill
{
    [Header("First Attack Settings")]
    [SerializeField] private UI_SkillTreeSlot pyreUnlockButton;
    public bool pyreUnlocked { get; private set; }
    [SerializeField] private GameObject pyrePrefab;                   // １撃目用プレハブ
    [SerializeField] private Vector2 firstAttackLaunchForce;            // 射出速度（１撃目）
    [SerializeField] private Vector3 firstAttackLaunchPosition;         // 発射位置オフセット（１撃目）
    [SerializeField] private float firstAttackProjectileDuration = 0.32f; // プロジェクタイルの持続時間（１撃目）
    [SerializeField] private float firstAttackFreezeTimeDuration = 0.3f;  // FreezeTimeDuration（１撃目）
    [SerializeField] private float firstAttackDamageMultiplier = 1.0f;    // ダメージマルチプライヤー（１撃目）

    [Header("Second Attack Settings")]
    [SerializeField] private GameObject pyreSecondPrefab;               // ２撃目用プレハブ
    [SerializeField] private Vector2 secondAttackLaunchForce;            // 射出速度（２撃目） ※Inspector上では正の値で設定
    [SerializeField] private Vector3 secondAttackLaunchPosition;         // 発射位置オフセット（２撃目）
    [SerializeField] private float secondAttackProjectileDuration = 0.5f;  // プロジェクタイルの持続時間（２撃目）
    [SerializeField] private float secondAttackFreezeTimeDuration = 0.5f;  // FreezeTimeDuration（２撃目）
    [SerializeField] private float secondAttackDamageMultiplier = 1.5f;    // ダメージマルチプライヤー（２撃目）
    // Colliderサイズやアニメーションは、２撃目用Prefab内で設定済み

    private Pyre_Skill_Controller currentPyre;
    private Vector2 finalDir; // １撃目用の発射方向

    protected override void Start()
    {
        base.Start();
        if (pyreUnlockButton != null)
        {
            pyreUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockPyre);
        }
    }

    protected override void Update()
    {
        base.Update();
        // １撃目用は、player.facingDir を使って発射方向を計算
        finalDir = new Vector2(player.facingDir * firstAttackLaunchForce.x, firstAttackLaunchForce.y);
    }

    protected override void CheckUnlock()
    {
        UnlockPyre();
    }

    private void UnlockPyre()
    {
        if (pyreUnlockButton != null && pyreUnlockButton.unlocked)
        {
            pyreUnlocked = true;
            isUnlocked = true;
        }
    }

    public override void UseSkill()
    {
        base.UseSkill();
        // 発動時の追加処理があれば記述
    }

    /// <summary>
    /// isSecondAttack が true なら２撃目用プレハブとパラメータで発射する
    /// </summary>
    public void CreatePyre(bool isSecondAttack = false)
    {
        if (isSecondAttack)
        {
            // プレイヤーの向きに合わせた発射位置
            Vector3 spawnPosition = player.transform.position +
                new Vector3(player.facingDir * secondAttackLaunchPosition.x, secondAttackLaunchPosition.y, secondAttackLaunchPosition.z);
            GameObject pyre = Instantiate(pyreSecondPrefab, spawnPosition, Quaternion.identity);
            currentPyre = pyre.GetComponent<Pyre_Skill_Controller>();
            if (currentPyre != null)
            {
                // プレイヤーの向きを反映した射出速度を計算（player.facingDir を掛ける）
                Vector2 secondFinalDir = new Vector2(player.facingDir * secondAttackLaunchForce.x, secondAttackLaunchForce.y);
                // 修正ポイント：_launchForce として secondFinalDir を渡す
                currentPyre.SetupPyre(secondFinalDir, player, secondAttackProjectileDuration,
                                       secondAttackFreezeTimeDuration, secondAttackDamageMultiplier, secondFinalDir);
            }
            else
            {
                Debug.LogWarning("Pyre_Skill: Pyre_Skill_Controller not found on PyreSecond prefab.");
            }
            player.AssignNewPyre(pyre);
        }
        else
        {
            Vector3 spawnPosition = player.transform.position +
                new Vector3(player.facingDir * firstAttackLaunchPosition.x, firstAttackLaunchPosition.y, firstAttackLaunchPosition.z);
            GameObject pyre = Instantiate(pyrePrefab, spawnPosition, Quaternion.identity);
            currentPyre = pyre.GetComponent<Pyre_Skill_Controller>();
            if (currentPyre != null)
            {
                currentPyre.SetupPyre(finalDir, player, firstAttackProjectileDuration, firstAttackFreezeTimeDuration, firstAttackDamageMultiplier);
            }
            else
            {
                Debug.LogWarning("Pyre_Skill: Pyre_Skill_Controller not found on pyrePrefab.");
            }
            player.AssignNewPyre(pyre);
        }
    }
}
