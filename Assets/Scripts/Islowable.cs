public interface ISlowable
{
    /// <summary>
    /// スロー効果を適用します。
    /// </summary>
    /// <param name="slowFactor">速度に掛ける倍率（例：0.5f なら半分の速さ）</param>
    /// <param name="slowDuration">スロー効果の持続時間</param>
    void ApplySlow(float slowFactor, float slowDuration);
}

