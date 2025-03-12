using UnityEngine;
using System.Collections;

public class Explosive_Controller : MonoBehaviour, ISlowable
{
    private Animator anim;
    private CharacterStats myStats;
    private float growSpeed = 15;
    private float maxSize = 6;
    private float explosionRadius;

    private bool canGrow = true;

    private void Awake()
    {
        // Animator コンポーネントを取得
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        // Unleashedスキルが発動中なら、スロー効果を適用する
        if (Unleashed_Skill.IsUnleashedActive)
        {
            Unleashed_Skill unleashedSkill = FindObjectOfType<Unleashed_Skill>();
            if (unleashedSkill != null)
            {
                ApplySlow(unleashedSkill.slowFactor, unleashedSkill.duration);
            }
            else
            {
                Debug.LogWarning("Unleashed_Skill インスタンスが見つかりません。");
            }
        }
    }

    private void Update()
    {
        if (canGrow)
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);

        if (maxSize - transform.localScale.x < .5f)
        {
            canGrow = false;
            if (anim != null)
                anim.SetTrigger("Explode");
        }
    }

    public void SetupExplosive(CharacterStats _myStats, float _growSpeed, float _maxSize, float _radius)
    {
        // SetupExplosive 呼び出し時に Animator を取得（すでに Awake で取得済みの場合は不要ですが、念のため）
        anim = GetComponent<Animator>();
        myStats = _myStats;
        growSpeed = _growSpeed;
        maxSize = _maxSize;
        explosionRadius = _radius;
    }

    private void AnimationExplodeEvent()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<CharacterStats>() != null)
            {
                hit.GetComponent<Entity>().SetupKnockbackDir(transform);
                myStats.DoPhysicalDamage(hit.GetComponent<CharacterStats>());
            }
        }
    }

    private void SelfDestroy() => Destroy(gameObject);

    // ▼ ISlowable インターフェースの実装 ▼

    /// <summary>
    /// スロー効果を適用します。
    /// ここでは、爆発までの成長速度（growSpeed）と Animator の再生速度に slowFactor を適用します。
    /// </summary>
    /// <param name="slowFactor">速度に掛ける倍率（例：0.5f なら半分の速さ）</param>
    /// <param name="slowDuration">スロー効果の持続時間</param>
    public void ApplySlow(float slowFactor, float slowDuration)
    {
        // 成長速度にスロー効果を適用
        growSpeed *= slowFactor;
        // Animator の再生速度にスロー効果を適用（アタッチされている場合）
        if (anim != null)
        {
            anim.speed *= slowFactor;
        }
        // 一定時間後に元の速度に戻す処理を開始
        StartCoroutine(RestoreSlowAfter(slowFactor, slowDuration));
    }

    /// <summary>
    /// 指定時間後に、成長速度と Animator の再生速度を元に戻すためのコルーチン
    /// </summary>
    private IEnumerator RestoreSlowAfter(float slowFactor, float slowDuration)
    {
        // 現在の値はすでに slowFactor 倍になっているため、元の値を計算しておく
        float originalGrowSpeed = growSpeed / slowFactor;
        float originalAnimSpeed = anim != null ? anim.speed / slowFactor : 1f;
        yield return new WaitForSeconds(slowDuration);
        growSpeed = originalGrowSpeed;
        if (anim != null)
        {
            anim.speed = originalAnimSpeed;
        }
    }
}
