using System.Collections;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    private PhysicsMaterial2D originalMaterial;

    private float originalColliderWidth;
    private float originalColliderHeight;
    private CapsuleCollider2D capsuleCollider;

    private Vector2 originalOffset;
    private CapsuleDirection2D originalDirection;

    private float originalGravityScale;

    private Vector3 originalWallCheckPosition;
    private Transform wallCheckTransform;

    // 無敵時間の管理
    private float invincibilityDuration = 0.25f;
    private float invincibilityTimer;

    // スプライトレンダラーと元の色の保存
    private float colorLoosingOnDashDuration = 0.4f;
    private float colorLoosingOnDashTimer;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Color invincibleColor;

    //private float stateTimer; // ステートに入ってからの経過時間


    public PlayerDashState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
        joystickInputManager = JoystickInputManager.Instance;
    }

    public override void Enter()
    {
        base.Enter();

        //AudioManager.instance.PlaySFX(13, null);


        // 無敵状態を開始
        player.stats.MakeInvincible(true);
        invincibilityTimer = invincibilityDuration;
        colorLoosingOnDashTimer = colorLoosingOnDashDuration;

        // Playerの子オブジェクトからAnimatorのSpriteRendererを取得
        Transform animatorTransform = player.transform.Find("Animator");
        if (animatorTransform != null)
        {
            spriteRenderer = animatorTransform.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                originalColor = spriteRenderer.color; // 元の色を保存
                invincibleColor = new Color(0f, 1f, 1f, 0f); // シアン色、透明度0.8
                spriteRenderer.color = invincibleColor;
            }
        }

        capsuleCollider = player.GetComponent<CapsuleCollider2D>();
        originalGravityScale = player.rb.gravityScale;
        player.rb.gravityScale = 0f;

        wallCheckTransform = player.transform.Find("WallCheck");
        if (wallCheckTransform != null)
        {
            originalWallCheckPosition = wallCheckTransform.localPosition;
            wallCheckTransform.localPosition = new Vector3(0.75f, -0.48f, 0.0f);
        }

        if (capsuleCollider != null)
        {
            originalMaterial = capsuleCollider.sharedMaterial;
            capsuleCollider.sharedMaterial = player.SlippyMaterial;

            originalColliderWidth = capsuleCollider.size.x;
            originalColliderHeight = capsuleCollider.size.y;
            originalOffset = capsuleCollider.offset;
            originalDirection = capsuleCollider.direction;

            capsuleCollider.size = new Vector2(originalColliderWidth * 1.5f, originalColliderHeight / 2);
            capsuleCollider.offset = new Vector2(.5f, -0.5f);
            capsuleCollider.direction = CapsuleDirection2D.Horizontal;
        }

        // Rigidbody2DのマテリアルをSlippyMaterialに変更
        originalMaterial = player.rb.sharedMaterial; // 現在のマテリアルを保存
        player.rb.sharedMaterial = player.SlippyMaterial;

        stateTimer = player.dashDuration;
    }

    public override void Exit()
    {
        base.Exit();

        player.rb.sharedMaterial = originalMaterial;

        player.rb.gravityScale = originalGravityScale;

        if (wallCheckTransform != null)
        {
            wallCheckTransform.localPosition = originalWallCheckPosition;
        }

        if (capsuleCollider != null)
        {
            capsuleCollider.sharedMaterial = originalMaterial;
            capsuleCollider.size = new Vector2(originalColliderWidth, originalColliderHeight);
            capsuleCollider.offset = originalOffset;
            capsuleCollider.direction = originalDirection;
        }

        // ダッシュ状態終了時に無敵を解除（念のため）とスプライトの色を元に戻す
        player.stats.MakeInvincible(false);
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }

    public override void Update()
    {
        base.Update();

        joystickInputManager = JoystickInputManager.Instance;
        float horizontalInput = joystickInputManager.Horizontal;
        float verticalInput = joystickInputManager.Vertical;

        // 無敵時間のカウントダウン
        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
            {
                player.stats.MakeInvincible(false); // 無敵状態を解除
                invincibilityTimer = 0;
            }
        }

        // 無敵中の色補間 (徐々に元の色と透明度に戻る)
        if (colorLoosingOnDashTimer > 0)
        {
            colorLoosingOnDashTimer -= Time.deltaTime;
            if (spriteRenderer != null)
            {
                float t = 1 - (colorLoosingOnDashTimer / colorLoosingOnDashDuration); // 進行度
                spriteRenderer.color = Color.Lerp(invincibleColor, originalColor, t);
                //colorLoosingOnDashTimer = 0;
            }
        }


        player.SetVelocity(player.dashSpeed * player.dashDir, 0);

        if (stateTimer < 0)
        {
            stateMachine.ChangeState(player.idleState);
        }

        // プレイヤーの速度を取得
        Vector3 playerVelocity = player.rb.velocity; // Rigidbodyから速度を取得

        // AfterImageの生成
        player.fx.CreateAfterImage(playerVelocity);

        // ジャンプへの移行
        if (joystickInputManager.Button0Down() && (player.IsGroundDetectedFore() || player.IsGroundDetectedBack() || player.IsThroughGroundDetected()))
        {
            float boostedVelocityX = player.rb.velocity.x * 0.8f;
            player.jumpState.SetJumpVelocity(boostedVelocityX);

            player.airState.SetDashingTransition(true);
            stateMachine.ChangeState(player.jumpState);
        }

        //壁スライドへの移行
        if ((!player.IsGroundDetectedFore() || !player.IsGroundDetectedBack() || !player.IsThroughGroundDetected()) && player.IsWallDetected())
        {
            stateMachine.ChangeState(player.wallSlide);
        }

        // 通常攻撃
        if (joystickInputManager.Button2Down() && !joystickInputManager.Button5())
        {
            float attackDir = horizontalInput;
            attackDir = attackDir == 0 ? player.facingDir : Mathf.Sign(attackDir); // 入力がない場合は facingDir を使用

            // GroundedStateかAirStateかで遷移先を切り替える
            if (player.IsGroundDetectedFore() || player.IsGroundDetectedBack() || player.IsThroughGroundDetected())
            {
                // GroundedStateの場合はPrimaryAttackStateへ遷移
                player.primaryAttack.SetAttackDirection(attackDir); // 攻撃方向を設定
                stateMachine.ChangeState(player.primaryAttack);
            }
            else
            {
                // AirStateの場合はJumpAttackStateへ遷移
                player.jumpAttack.SetAttackDirection(attackDir); // 攻撃方向を設定
                stateMachine.ChangeState(player.jumpAttack);
            }
        }

        ////下ボタンが押されたら下降
        //if (verticalInput < 0 && (!player.IsGroundDetectedFore() || !player.IsGroundDetectedBack()))
        //{
        //    stateMachine.ChangeState(player.airState);
        //    //player.rb.velocity = new Vector2(player.rb.velocity.x, player.airState.fastFallSpeed * 0);
        //}
    }
}
