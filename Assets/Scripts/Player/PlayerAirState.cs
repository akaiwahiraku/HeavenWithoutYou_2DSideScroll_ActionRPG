using System.Collections;
using UnityEngine;

public class PlayerAirState : PlayerState
{
    private PhysicsMaterial2D originalMaterial;
    public PhysicsMaterial2D FrictionalMaterial;
    public PhysicsMaterial2D SlippyMaterial;

    private float playerAirStateTimer; // ステートに入ってからの経過時間
    [SerializeField] private float timeThreshold = 0.3f; // 設定したい秒数

    public float fastFallSpeed = -18f;
    private float horizontalVelocity;

    private bool isDashingTransition = false;

    private Collider2D throughGroundCollider;

    public PlayerAirState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
        joystickInputManager = JoystickInputManager.Instance;
    }

    public override void Enter()
    {
        base.Enter();
        var collider = player.GetComponent<Collider2D>();
        originalMaterial = collider.sharedMaterial;
        collider.sharedMaterial = SlippyMaterial;
        horizontalVelocity = player.rb.velocity.x;
        player.rb.velocity = new Vector2(horizontalVelocity, player.rb.velocity.y);

        stateTimer = 0f;
        playerAirStateTimer = 0f;
    }

    public override void Exit()
    {
        base.Exit();
        player.GetComponent<Collider2D>().sharedMaterial = originalMaterial;
        canDoubleJump = true;
        player.canAirDash = true;
        SetDashingTransition(false);
    }

    public override void Update()
    {
        base.Update();

        joystickInputManager = JoystickInputManager.Instance;
        float horizontalInput = joystickInputManager.Horizontal;
        float verticalInput = joystickInputManager.Vertical;

        // タイマーの更新
        playerAirStateTimer += Time.deltaTime;

        // 下ボタンを押すと急降下
        if (verticalInput < 0 && (playerAirStateTimer >= timeThreshold || player.canAirDash == false))
        {
            player.rb.velocity = new Vector2(player.rb.velocity.x, fastFallSpeed);
        }

        if (isDashingTransition)
        {
            Vector3 playerVelocity = player.rb.velocity;
            player.fx.CreateAfterImage(playerVelocity);
            if (horizontalInput != 0)
            {
                player.SetVelocity(player.moveSpeed * 1.1f * horizontalInput, player.rb.velocity.y);
            }
        }
        else if (horizontalInput != 0)
        {
            player.SetVelocity(player.moveSpeed * 0.8f * horizontalInput, player.rb.velocity.y);
        }

        // ジャンプアタック入力時の処理
        if (joystickInputManager.Button2Down())
        {
            // 攻撃開始時刻を記録
            //player.attackButtonPressTime = Time.time;
            if (SkillManager.instance.surge != null && SkillManager.instance.surge.CanUseSkill())
            {
                float attackDir = horizontalInput;
                attackDir = (attackDir == 0 ? player.facingDir : Mathf.Sign(attackDir));
                player.primaryAttackCharge.SetAttackDirection(attackDir);
                stateMachine.ChangeState(player.jumpAttackCharge);
            }
            else
                stateMachine.ChangeState(player.jumpAttack);
        }


        // 特殊スキル（例：ダークサークル、フォース）
        if (joystickInputManager.Button3Down() && !joystickInputManager.Button5())
        {
            if (SkillManager.instance.darkCircle != null && SkillManager.instance.darkCircle.CanUseSkill())
                stateMachine.ChangeState(player.aimDarkCircle);
            if (SkillManager.instance.force != null && SkillManager.instance.force.CanUseSkill())
                stateMachine.ChangeState(player.aimForce);
        }

        // 二段ジャンプ
        if (canDoubleJump && joystickInputManager.Button0Down())
        {
            player.rb.velocity = new Vector2(player.rb.velocity.x, player.jumpForce);
            canDoubleJump = false;
            playerAirStateTimer = 0f; // ジャンプ後、タイマーをリセット
        }

        // 壁スライドに移行
        if (player.IsWallDetected())
        {
            stateMachine.ChangeState(player.wallSlide);
        }

        // 接地判定
        if (player.IsGroundDetectedFore() || player.IsGroundDetectedBack() || (player.IsThroughGroundDetected() && player.rb.velocity.y <= 0))
        {
            player.GetComponent<Collider2D>().sharedMaterial = originalMaterial;
            stateMachine.ChangeState(player.idleState);
        }

        // 二段ジャンプ、エアダッシュのリセット
        if (player.IsGroundDetectedFore() || player.IsGroundDetectedBack() || (player.IsThroughGroundDetected() && player.rb.velocity.y <= 0) || player.IsWallDetected())
        {
            canDoubleJump = true;
            player.canAirDash = true;
        }

        // 横移動 & 上昇中なら、すり抜け床のコライダーを無視
        if (player.rb.velocity.x != 0 && player.rb.velocity.y > 0)
        {
            IgnoreThroughGroundCollision();
        }

        CheckForAirDashInput();
        CheckForAirRushInput();
    }

    public void SetDashingTransition(bool value)
    {
        isDashingTransition = value;
    }

    private void IgnoreThroughGroundCollision()
    {
        Vector2 detectionPosition = new Vector2(player.throughGroundCheck.position.x + player.facingDir * 0.5f, player.throughGroundCheck.position.y + 0.5f);
        Collider2D throughGroundCollider = Physics2D.OverlapCircle(detectionPosition, player.throughGroundCheckDistance, player.whatIsThroughGround);

        if (throughGroundCollider != null && throughGroundCollider != player.GetComponent<Collider2D>())
        {
            Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), throughGroundCollider, true);
            player.StartCoroutine(RestoreCollisionAfterTime(throughGroundCollider));
        }
    }

    private IEnumerator RestoreCollisionAfterTime(Collider2D collider)
    {
        yield return new WaitForSeconds(0.5f);
        Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), collider, false);
    }

    private void CheckForAirDashInput()
    {
        if (joystickInputManager.Button1Down() && (!player.IsGroundDetectedFore() || !player.IsGroundDetectedBack() || !player.IsThroughGroundDetected()) && player.canAirDash)
        {
            player.dashDir = joystickInputManager.Horizontal;
            player.dashDir = (player.dashDir == 0 ? player.facingDir : player.dashDir);
            stateMachine.ChangeState(player.dashState);
            player.canAirDash = false;
        }
    }

    private void CheckForAirRushInput()
    {
        if (joystickInputManager.Button2Down() && joystickInputManager.Button5() && (!player.IsGroundDetectedFore() || !player.IsGroundDetectedBack() || !player.IsThroughGroundDetected()))
        {
            player.rushDir = joystickInputManager.Horizontal;
            player.rushDir = (player.rushDir == 0 ? player.facingDir : player.rushDir);
            stateMachine.ChangeState(player.shadowBringerOverDrive1st);
            player.canAirRush = false;
        }
    }
}
