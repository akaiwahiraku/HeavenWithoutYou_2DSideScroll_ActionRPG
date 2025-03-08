using System.Collections.Generic;
using UnityEngine;

public class Blackhole_Skill_Controller : MonoBehaviour
{
    private float maxSize;
    private float growSpeed;
    private float shrinkSpeed;
    private float blackholeTimer;

    private bool canGrow = true;
    private bool canShrink;

    private bool cloneAttackReleased;
    private bool playerCanDisappear = true;

    private int amoutOfAttacks = 4;
    private float cloneAttackCooldown = .3f;
    private float cloneAttackTimer;

    private Rigidbody2D rb;  // Rigidbody2Dを追加
    private CircleCollider2D circleCollider;  // CircleCollider2Dを追加

    public Transform myEnemy;
    public List<Transform> targets = new List<Transform>();

    public bool playerCanExitState { get; private set; }

    private void Awake()
    {
        // Rigidbody2DとCircleCollider2Dのセットアップ
        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;  // gravityScaleをゼロにして無重力状態にする
        rb.isKinematic = true;  // 物理演算に影響しないようにする

        circleCollider = gameObject.AddComponent<CircleCollider2D>();
        circleCollider.isTrigger = true;  // isTriggerを有効にしてトリガーとして動作させる
    }

    public void SetupBlackhole(float _maxSize, float _growSpeed, float _shrinkSpeed, int _amoutOfAttacks, float _cloneAttackCooldown, float _blackholeDuration, Transform _myEnemy)
    {
        maxSize = _maxSize;
        growSpeed = _growSpeed;
        shrinkSpeed = _shrinkSpeed;
        amoutOfAttacks = _amoutOfAttacks;
        cloneAttackCooldown = _cloneAttackCooldown;

        myEnemy = _myEnemy;

        blackholeTimer = _blackholeDuration;
    }

    private void Update()
    {
        cloneAttackTimer -= Time.deltaTime;
        blackholeTimer -= Time.deltaTime;

        if (blackholeTimer < 0)
        {
            blackholeTimer = Mathf.Infinity;

            if (targets.Count > 0)
                ReleaseCloneAttack();
            else
            {
                FinishBlackHoleAbility();
            }
        }

        CloneAttackLogic();

        if (canGrow && !canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);
        }

        if (canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-1, -1), shrinkSpeed * Time.deltaTime);

            if (transform.localScale.x < 0)
                Destroy(gameObject);
        }
    }

    private void ReleaseCloneAttack()
    {
        if (targets.Count <= 0)
            return;

        cloneAttackReleased = true;

        if (playerCanDisappear)
        {
            playerCanDisappear = false;
            CurrencyManager.instance.player.fx.MakeTransparent(true);
        }
    }

    private void CloneAttackLogic()
    {
        if (cloneAttackTimer < 0 && cloneAttackReleased && amoutOfAttacks > 0)
        {
            cloneAttackTimer = cloneAttackCooldown;

            int randomIndex = Random.Range(0, targets.Count);

            float xOffset;

            if (Random.Range(0, 101) > 50)
                xOffset = 2;
            else
                xOffset = -2;

            SkillManager.instance.clone.CreateClone(targets[randomIndex], new Vector3(xOffset, 0), false, true);

            amoutOfAttacks--;

            if (amoutOfAttacks <= 0)
            {
                Invoke("FinishBlackHoleAbility", 1f);
            }
        }
    }

    private void FinishBlackHoleAbility()
    {
        playerCanExitState = true;
        canShrink = true;
        cloneAttackReleased = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            targets.Add(collision.transform);
            collision.GetComponent<Enemy>().FreezeTime(true);
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
            collision.GetComponent<Enemy>().FreezeTime(false);
    }

    public void AddEnemyToList(Transform _myEnemy)
    {
        targets.Add(myEnemy);
    }
}