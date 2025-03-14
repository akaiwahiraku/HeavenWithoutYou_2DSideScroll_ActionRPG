using UnityEngine;

public class DamageManager
{
    // 各種倍率（必要に応じて外部から変更可能）
    public float CrisisMultiplier { get; set; } = 1.25f;
    public float ChargeBaseMultiplier { get; set; } = 2f;
    public float GuardExitBonusMultiplier { get; set; } = 1.5f;

    /// <summary>
    /// 攻撃側 (attacker) と防御側 (target) の情報から物理ダメージを計算します。
    /// isChargeAttack が true の場合、チャージ攻撃用の倍率が適用されます。
    /// </summary>
    public int CalculatePhysicalDamage(CharacterStats attacker, CharacterStats target, bool isChargeAttack = false)
    {
        // 基本ダメージは、武器ダメージと力の合計とする
        int baseDamage = attacker.damage.GetValue() + attacker.strength.GetValue();
        float multiplier = 1f;

        // チャージ攻撃の場合の倍率
        if (isChargeAttack)
        {
            multiplier = ChargeBaseMultiplier;
            // Player コンポーネントを明示的に取得する
            Player playerAttacker = attacker.GetComponent<Player>();
            if (playerAttacker != null &&
                playerAttacker.lastGuardExitTime > 0 &&
                Time.time - playerAttacker.lastGuardExitTime <= 0.8f)
            {
                multiplier *= GuardExitBonusMultiplier;
            }
        }

        int totalDamage = Mathf.RoundToInt(baseDamage * multiplier);

        // クリティカルヒットの判定と計算
        if (CheckCriticalHit(attacker))
        {
            totalDamage = CalculateCriticalDamage(attacker, totalDamage);
        }

        // クライシスモード：残り体力が閾値以下の場合の倍率適用
        if (attacker.currentHealth <= attacker.GetMaxHealthValue() * attacker.crisisPercent)
        {
            totalDamage = Mathf.RoundToInt(totalDamage * CrisisMultiplier);
        }

        // 防御側のアーマーによるダメージ軽減
        totalDamage = ApplyArmorReduction(target, totalDamage);

        return totalDamage;
    }

    /// <summary>
    /// クリティカルヒットの判定（critChance と agility を合算して乱数と比較）
    /// </summary>
    private bool CheckCriticalHit(CharacterStats attacker)
    {
        int totalCriticalChance = attacker.critChance.GetValue() + attacker.agility.GetValue();
        return Random.Range(0, 100) <= totalCriticalChance;
    }

    /// <summary>
    /// クリティカルヒット時のダメージ計算（critPower と strength を基に乗数を算出）
    /// </summary>
    private int CalculateCriticalDamage(CharacterStats attacker, int damage)
    {
        float critMultiplier = (attacker.critPower.GetValue() + attacker.strength.GetValue()) * 0.01f;
        return Mathf.RoundToInt(damage * critMultiplier);
    }

    /// <summary>
    /// 防御側のアーマーによるダメージ軽減を適用
    /// ※ chilled 状態の場合はアーマーの軽減効果が変化します（サンプル実装）
    /// </summary>
    private int ApplyArmorReduction(CharacterStats target, int damage)
    {
        int armorValue = target.armor.GetValue();
        if (target.AilmentManager != null && target.AilmentManager.IsChilled)
            damage -= Mathf.RoundToInt(armorValue * 0.8f);
        else
            damage -= armorValue;

        // さらにアーマー分を減算（元コードの挙動）
        damage -= armorValue;
        return Mathf.Clamp(damage, 0, int.MaxValue);
    }

}
