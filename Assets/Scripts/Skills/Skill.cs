using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public float cooldown;       // クールダウン時間（秒）
    public float cooldownTimer;  // 現在のクールダウン時間

    public SkillCategory category;   // このスキルが属するカテゴリ
    public string skillName;         // スキルの名前
    [TextArea]
    public string description;       // スキルの説明

    public bool isUnlocked = false;  // スキルツリーでアンロックされるまで false

    protected Player player;

    protected virtual void Start()
    {
        // プレイヤー参照の取得（CurrencyManager などから）
        player = CurrencyManager.instance.player;

        // クールダウンタイマーの初期化
        cooldownTimer = 0f;

        CheckUnlock();
    }

    protected virtual void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// 派生クラスで実装。ここでスキルのアンロック条件などをチェックする。
    /// デフォルトでは何もしない。
    /// </summary>
    protected virtual void CheckUnlock()
    {
        // 初期状態はロック状態
        isUnlocked = false;
    }

    /// <summary>
    /// スキルが使用可能かどうかを判定する。
    /// 使用可能なのは、(1) スキルツリーでアンロック済み、(2) SkillManager でそのカテゴリに選択されている、(3) クールダウンが終了している場合。
    /// </summary>
    public virtual bool CanUseSkill()
    {
        if (!isUnlocked)
        {
            //player.fx.CreatePopUpText("Skill not unlocked!");
            return false;
        }

        // 選択されているスキルかどうかを SkillManager から確認する
        Skill selected = SkillManager.instance.GetSelectedSkill(category);
        if (selected != this)
        {
            //player.fx.CreatePopUpText("Skill not set!");
            return false;
        }

        if (cooldownTimer > 0)
        {
            player.fx.CreatePopUpText("Cooldown");
            return false;
        }

        return true;
    }

    /// <summary>
    /// スキルの実際の効果を発動する（派生クラスで上書き）。
    /// </summary>
    public virtual void UseSkill()
    {
        Debug.Log("Using skill: " + skillName);
        cooldownTimer = cooldown;
        // ここに各スキル固有の処理を実装する
    }

    public virtual bool CanUseOverDrive()
    {
        UseSkill();
        return true;
    }

    /// <summary>
    /// 指定位置から最も近い敵を探して返す
    /// </summary>
    protected virtual Transform FindClosestEnemy(Transform _checkTransform)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_checkTransform.position, 25);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                float distanceToEnemy = Vector2.Distance(_checkTransform.position, hit.transform.position);
                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }
        }
        return closestEnemy;
    }
}
