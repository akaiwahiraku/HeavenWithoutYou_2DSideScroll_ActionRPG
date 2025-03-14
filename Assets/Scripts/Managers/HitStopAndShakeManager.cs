using System;
using System.Collections;
using UnityEngine;

public class HitStopAndShakeManager : MonoBehaviour
{
    [SerializeField] private float hitStopDuration = 0.3f;  // 基本のヒットストップ持続時間
    [SerializeField] private float shakeMagnitude = 0.5f;      // シェイクの強度

    private bool isHitStopping = false;

    /// <summary>
    /// 汎用のヒットストップ処理。指定した停止時間後に、shakeEffectMethod で渡されたシェイク処理を実行します。
    /// </summary>
    public IEnumerator TriggerHitStopGeneric(float stopDuration, Func<IEnumerator> shakeEffectMethod)
    {
        if (isHitStopping)
            yield break;

        isHitStopping = true;
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(stopDuration);
        Time.timeScale = 1f;
        isHitStopping = false;

        if (shakeEffectMethod != null)
            yield return StartCoroutine(shakeEffectMethod());
    }

    /// <summary>
    /// 通常攻撃用のヒットストップとシェイク処理を実行
    /// </summary>
    public IEnumerator TriggerHitStop()
    {
        return TriggerHitStopGeneric(hitStopDuration, ShakeEffect);
    }

    /// <summary>
    /// チャージ攻撃用のヒットストップとシェイク処理（通常の5倍の停止時間）
    /// </summary>
    public IEnumerator TriggerHitStopCharge()
    {
        return TriggerHitStopGeneric(hitStopDuration * 2.5f, ShakeEffectCharge);
    }

    /// <summary>
    /// 指定された乱数レンジを用いたシェイク処理の汎用メソッド
    /// </summary>
    private IEnumerator ShakeEffectGeneric(float minRange, float maxRange)
    {
        Vector3 originalPosition = transform.position;
        float elapsed = 0f;

        // ヒットストップと同じ持続時間でシェイク処理を実行
        while (elapsed < hitStopDuration)
        {
            float x = UnityEngine.Random.Range(minRange, maxRange) * shakeMagnitude;
            float y = UnityEngine.Random.Range(minRange, maxRange) * shakeMagnitude;
            transform.position = originalPosition + new Vector3(x, y, 0);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        transform.position = originalPosition;
    }

    /// <summary>
    /// 通常攻撃用のシェイク（乱数レンジ：-1～1）
    /// </summary>
    public IEnumerator ShakeEffect()
    {
        yield return ShakeEffectGeneric(-1f, 1f);
    }

    /// <summary>
    /// チャージ攻撃用のシェイク（乱数レンジ：-10～10）
    /// </summary>
    public IEnumerator ShakeEffectCharge()
    {
        yield return ShakeEffectGeneric(-10f, 10f);
    }

    /// <summary>
    /// 対象オブジェクトに対してヒットストップとシェイクを適用する処理
    /// </summary>
    public IEnumerator ApplyHitStopAndShake(CharacterStats target, Action onHitStopComplete = null)
    {
        // 対象の有無チェック
        if (target == null)
        {
            Debug.LogWarning("ApplyHitStopAndShake: Target is null or destroyed at coroutine start.");
            onHitStopComplete?.Invoke();
            yield break;
        }

        // Rigidbody2D を一時停止（対象の移動を一瞬停止）
        Rigidbody2D targetRigidbody = target.GetComponent<Rigidbody2D>();
        if (targetRigidbody != null)
        {
            Vector2 originalVelocity = targetRigidbody.velocity;
            targetRigidbody.velocity = Vector2.zero;
            yield return new WaitForSecondsRealtime(hitStopDuration);

            // 待機後、対象が破棄されていないか確認
            if (target == null)
            {
                Debug.LogWarning("ApplyHitStopAndShake: Target is null or destroyed after hit stop wait.");
                onHitStopComplete?.Invoke();
                yield break;
            }
            targetRigidbody.velocity = originalVelocity;
        }
        else
        {
            Debug.LogWarning("ApplyHitStopAndShake: Rigidbody2D is null or destroyed.");
            yield return new WaitForSecondsRealtime(hitStopDuration);
        }

        // 対象のシェイク処理を実行（乱数レンジ：-1～1）
        yield return StartCoroutine(ShakeEffectOnTarget(target, -1f, 1f));

        onHitStopComplete?.Invoke();
    }

    /// <summary>
    /// 対象オブジェクトに対してシェイク効果を適用する共通処理
    /// </summary>
    private IEnumerator ShakeEffectOnTarget(CharacterStats target, float minRange, float maxRange)
    {
        Vector3 originalPosition = target.transform.position;
        float elapsed = 0f;

        while (elapsed < hitStopDuration)
        {
            if (target == null)
                yield break;

            float x = UnityEngine.Random.Range(minRange, maxRange) * shakeMagnitude;
            float y = UnityEngine.Random.Range(minRange, maxRange) * shakeMagnitude;
            target.transform.position = originalPosition + new Vector3(x, y, 0);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        if (target != null)
            target.transform.position = originalPosition;
    }
}
