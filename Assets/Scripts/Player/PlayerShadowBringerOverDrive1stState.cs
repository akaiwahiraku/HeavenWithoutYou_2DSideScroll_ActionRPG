using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerShadowBringerOverDrive1stState : PlayerState
{
    private Color originalColor;
    private PhysicsMaterial2D originalMaterial;
    public PhysicsMaterial2D FrictionalMaterial;
    public PhysicsMaterial2D SlippyMaterial;

    private float originalGravityScale;

    private Vector3 originalWallCheckPosition;
    private Vector3 originalAttackCheckPosition;
    private Transform wallCheckTransform;
    private Transform attackCheckTransform;

    PlayerStats playerStats;
    private bool onceTimestop = false;


    public PlayerShadowBringerOverDrive1stState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
        joystickInputManager = JoystickInputManager.Instance;

        playerStats = _player.GetComponent<PlayerStats>();
    }

    public override void Enter()
    {
        base.Enter();

        AudioManager.instance.PlaySFX(11, null);

        playerStats.isInOverdriveState = true;
        if (player.fx != null)
            player.fx.StopOverDriveFX();

        player.stats.overDriveStock--;

        // 無敵状態を開始
        player.stats.MakeInvincible(true);

        // WallCheckを無効化
        player.wallCheckEnabled = false;

        // プレイヤーのスプライトの元の色を取得して、#00FFFFに変更
        originalColor = player.sr.color;
        player.sr.color = new Color(0f, 1f, 1f);

        originalGravityScale = player.rb.gravityScale;
        player.rb.gravityScale = 0f;

        // AttackCheckの位置を変更
        attackCheckTransform = player.transform.Find("AttackCheck");
        if (attackCheckTransform != null)
        {
            originalAttackCheckPosition = attackCheckTransform.localPosition;
            attackCheckTransform.localPosition = new Vector3(1.0f, attackCheckTransform.localPosition.y, attackCheckTransform.localPosition.z);
        }

        stateTimer = player.rushDuration;

        onceTimestop = true;

        //player.SetVelocity(attackDir, 0);

        player.rushDir = player.rushDir == 0 ? player.facingDir : player.rushDir;

        player.stats.isInShadowBringerOverDrive1stState = true; // フラグをtrueに設定


        // ステート遷移をロック
        SetLockStateTransition(true);
    }

    public override void Exit()
    {
        base.Exit();

        playerStats.isInOverdriveState = false;
        if (player.stats.overDriveStock > 0 && player.fx != null)
            player.fx.PlayOverDriveFX();

        // スプライトの色を元に戻す
        player.sr.color = originalColor;

        player.rb.gravityScale = originalGravityScale;



        // AttackCheckの位置を元に戻す
        if (attackCheckTransform != null)
        {
            attackCheckTransform.localPosition = originalAttackCheckPosition;
        }

        // ダッシュ状態終了時に無敵を解除
        player.stats.MakeInvincible(false);

        // WallCheckを有効化
        player.wallCheckEnabled = true;

        player.stats.isInShadowBringerOverDrive1stState = false; // フラグをfalseに設定

        // ステート終了後に遷移ロックを解除
        SetLockStateTransition(false);
    }

    public override void Update()
    {
        base.Update();

        joystickInputManager = JoystickInputManager.Instance;


        if (player.sr.color == originalColor)
            player.sr.color = new Color(0f, 1f, 1f);

        if (onceTimestop)
        {
            if (playerStats != null && playerStats.screenFlashBlackout != null)
            {
                playerStats.overDriveDreamtideText.FlashOverDriveText();
                playerStats.screenFlashBlackout.FlashBlackScreen();
            }
            else
            {
                Debug.LogError("PlayerStats or screenFlashBlackout is null.");
            }

            player.StartCoroutine(Restart());
            onceTimestop = false;
        }



        player.SetVelocity(player.rushSpeed * player.rushDir, 0);

        if (stateTimer < 0)
        {
            stateMachine.ChangeState(player.idleState);
        }

        // プレイヤーの速度を取得
        Vector3 playerVelocity = player.rb.velocity; // Rigidbodyから速度を取得

        // AfterImageの生成
        player.fx.CreateAfterImage(playerVelocity);


        if ((!player.IsGroundDetectedFore() || !player.IsGroundDetectedBack()) && player.IsWallDetected())
        {
            stateMachine.ChangeState(player.wallSlide);
        }

        if (triggerCalled)
        {
            SetLockStateTransition(false);
            stateMachine.ChangeState(player.idleState);
        }

    }

    public IEnumerator Restart()
    {

        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(1.0f);
        Time.timeScale = 1;
    }
}
