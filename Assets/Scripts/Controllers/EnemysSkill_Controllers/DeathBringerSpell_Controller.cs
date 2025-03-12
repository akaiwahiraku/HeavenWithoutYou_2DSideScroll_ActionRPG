using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBringerSpell_Controller : MonoBehaviour, ISlowable
{
    [SerializeField] private Transform check;
    [SerializeField] private Vector2 boxSize;
    [SerializeField] private LayerMask whatIsPlayer;

    private CharacterStats myStats;
    private Animator animator; // アニメーターを取得して、再生速度を制御

    public void SetupSpell(CharacterStats _stats) => myStats = _stats;

    private void Awake()
    {
        // Animator コンポーネントを取得（もしアタッチされていれば）
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // Unleashedスキルが発動中なら、スロー効果を適用する
        if (Unleashed_Skill.IsUnleashedActive)
        {
            Unleashed_Skill unleashedSkill = FindObjectOfType<Unleashed_Skill>();
            if (unleashedSkill != null)
            {
                // スキル側の slowFactor と duration を使用してアニメーターの速度を低下させる
                ApplySlow(unleashedSkill.slowFactor, unleashedSkill.duration);
            }
            else
            {
                Debug.LogWarning("Unleashed_Skill インスタンスが見つかりません。");
            }
        }
    }

    private void AnimationTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(check.position, boxSize, whatIsPlayer);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Player>() != null)
            {
                hit.GetComponent<Entity>().SetupKnockbackDir(transform);
                myStats.DoPhysicalDamage(hit.GetComponent<CharacterStats>());
            }
        }
    }

    private void OnDrawGizmos() => Gizmos.DrawWireCube(check.position, boxSize);

    private void SelfDestroy() => Destroy(gameObject);

    // ▼ ISlowable インターフェースの実装 ▼

    /// <summary>
    /// スロー効果を適用します。ここでは Animator の再生速度に反映させます。
    /// </summary>
    /// <param name="slowFactor">再生速度に掛ける倍率（例：0.5f なら半分の速さ）</param>
    /// <param name="slowDuration">スロー効果の持続時間</param>
    public void ApplySlow(float slowFactor, float slowDuration)
    {
        if (animator != null)
        {
            // 現在の再生速度に slowFactor を適用
            animator.speed *= slowFactor;
            StartCoroutine(RestoreAnimatorSpeedAfter(slowFactor, slowDuration));
        }
        else
        {
            Debug.LogWarning("Animator コンポーネントが見つかりません。スロー効果を適用できません。");
        }
    }

    /// <summary>
    /// 指定時間後に Animator の再生速度を元に戻すコルーチン
    /// </summary>
    private IEnumerator RestoreAnimatorSpeedAfter(float slowFactor, float slowDuration)
    {
        // 現在の animator.speed はすでに slowFactor 倍になっているため、元の値を計算
        float originalSpeed = animator.speed / slowFactor;
        yield return new WaitForSeconds(slowDuration);
        animator.speed = originalSpeed;
    }
}
