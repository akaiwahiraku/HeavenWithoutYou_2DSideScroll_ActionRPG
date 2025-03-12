using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightBorneSpell_Controller : MonoBehaviour, ISlowable
{
    private SpriteRenderer sr;

    [SerializeField] private int damage;
    [SerializeField] private string targetLayerName = "Player";

    [SerializeField] private float xVelocity;
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private bool canMove;
    [SerializeField] private bool flipped;

    private CharacterStats myStats;

    private int facingDir = 1;

    private void Update()
    {
        if (canMove)
            rb.velocity = new Vector2(xVelocity, rb.velocity.y);

        if (facingDir == 1 && rb.velocity.x < 0)
        {
            facingDir = -1;
            sr.flipX = true;
        }
    }

    private void Start()
    {
        // 既存の初期化処理があればそのまま

        // Unleashedスキルが発動中なら、スロー効果を適用する
        if (Unleashed_Skill.IsUnleashedActive)
        {
            // Unleashed_Skill のインスタンスを取得
            Unleashed_Skill unleashedSkill = FindObjectOfType<Unleashed_Skill>();
            if (unleashedSkill != null)
            {
                // スキル側の slowFactor と duration を使用
                ApplySlow(unleashedSkill.slowFactor, unleashedSkill.duration);
            }
            else
            {
                Debug.LogWarning("Unleashed_Skill インスタンスが見つかりません。");
            }
        }
    }



    public void SetupArcane(float _speed, CharacterStats _myStats)
    {
        sr = GetComponent<SpriteRenderer>();
        xVelocity = _speed;
        myStats = _myStats;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(targetLayerName))
        {
            myStats.DoPhysicalDamage(collision.GetComponent<CharacterStats>());
            Destroy(gameObject);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            Destroy(gameObject);

        if (collision.GetComponent<DarkCircle_Skill_Controller>() != null)
        {
            Destroy(gameObject);
        }

        if (collision.GetComponent<Blackhole_Skill_Controller>() != null)
        {
            Destroy(gameObject);
        }

        if (collision.GetComponent<Force_Skill_Controller>() != null)
        {
            Destroy(gameObject);
        }
    }

    public void FlipSpell()
    {
        if (flipped)
            return;

        xVelocity = xVelocity * -1;
        flipped = true;
        transform.Rotate(0, 180, 0);
        targetLayerName = "Enemy";
    }

    // ▼ ここから新たに追加するスロー処理関連の関数 ▼

    /// <summary>
    /// ISlowable インターフェースの実装
    /// </summary>
    /// <param name="slowFactor">速度に掛ける倍率（例：0.5f なら半分の速さ）</param>
    /// <param name="slowDuration">スロー効果の持続時間</param>
    public void ApplySlow(float slowFactor, float slowDuration)
    {
        // xVelocity にスロー効果を適用
        xVelocity *= slowFactor;
        // 現在の rb.velocity にも同じ倍率を適用
        rb.velocity = new Vector2(rb.velocity.x * slowFactor, rb.velocity.y);
        // 指定時間後に元の速度に戻す処理を開始
        StartCoroutine(RestoreSpeedAfter(slowFactor, slowDuration));
    }

    /// <summary>
    /// スロー効果終了後に元の速度に戻すためのコルーチン
    /// </summary>
    private IEnumerator RestoreSpeedAfter(float slowFactor, float slowDuration)
    {
        // 現在の xVelocity はすでに slowFactor 倍になっているので、元の値を計算しておく
        float originalXVelocity = xVelocity / slowFactor;
        yield return new WaitForSeconds(slowDuration);
        xVelocity = originalXVelocity;
        // 次の Update で rb.velocity が xVelocity に合わせて更新されるため、追加処理は不要
    }
}
