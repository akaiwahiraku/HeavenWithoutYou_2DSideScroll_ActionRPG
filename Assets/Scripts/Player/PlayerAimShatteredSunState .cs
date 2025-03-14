using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerAimShatteredSunState : PlayerState
{
    private float flyTime = 0.3f;
    private bool skillUsed;

    private float defaultGravity;

    PlayerStats playerStats;
    private bool onceTimestop = false;

    private GameObject foreGround;
    private TilemapRenderer foreGroundTilemapRenderer;
    private string originalSortingLayerName; // 元のレイヤー名を保存

    public PlayerAimShatteredSunState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName)
        : base(_player, _stateMachine, _animBoolName)
    {
        playerStats = _player.GetComponent<PlayerStats>();

        // ForegroundオブジェクトとRendererのキャッシュ
        foreGround = GameObject.Find("Level/Grid/Foreground");
        if (foreGround != null)
        {
            foreGroundTilemapRenderer = foreGround.GetComponent<TilemapRenderer>();
            originalSortingLayerName = foreGroundTilemapRenderer.sortingLayerName;
        }
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }

    public override void Enter()
    {
        base.Enter();

        // OverDriveステートに入ったのでフラグをオンし、FXを停止
        playerStats.isInOverdriveState = true;
        if (player.fx != null)
            player.fx.StopOverDriveFX();

        player.stats.overDriveStock -= 1;
        defaultGravity = player.rb.gravityScale;
        skillUsed = false;
        stateTimer = flyTime;
        rb.gravityScale = 0;
        player.stats.MakeInvincible(true);
        onceTimestop = true;

        if (foreGroundTilemapRenderer != null)
        {
            foreGroundTilemapRenderer.sortingLayerName = "Ground";
        }
        SetLockStateTransition(true);
    }

    public override void Exit()
    {
        base.Exit();

        player.rb.gravityScale = defaultGravity;
        player.stats.MakeInvincible(false);

        if (foreGroundTilemapRenderer != null)
        {
            foreGroundTilemapRenderer.sortingLayerName = originalSortingLayerName;
        }

        player.skill.shatteredSun.ResetSkillState();
        SetLockStateTransition(false);

        // ステート終了時はフラグをオフし、残りのOverDriveストックがあればFXを再生
        playerStats.isInOverdriveState = false;
        if (player.stats.overDriveStock > 0 && player.fx != null)
            player.fx.PlayOverDriveFX();
    }


    public override void Update()
    {
        base.Update();

        if (onceTimestop)
        {
            if (playerStats != null && playerStats.screenFlashBlackout != null)
            {
                playerStats.overDriveShatteredSunText.FlashOverDriveText();
                playerStats.screenFlashBlackout.FlashBlackScreen();
            }
            else
            {
                Debug.LogError("PlayerStats or screenFlashBlackout is null.");
            }
            player.StartCoroutine(Restart());
            onceTimestop = false;
        }

        if (stateTimer > 0)
        {
            rb.velocity = new Vector2(0, 10);
        }
        else // stateTimer <= 0
        {
            rb.velocity = new Vector2(0, -0.1f);

            if (!skillUsed)
            {
                if (player.skill.shatteredSun.CanUseOverDrive())
                {
                    skillUsed = true;
                    player.skill.shatteredSun.CreateShatteredSun();
                    player.stats.currentOverDrive = 0;
                }
            }
        }

        // スキルが完了したらエアステートへ移行
        if (player.skill.shatteredSun.SkillCompleted())
        {
            SetLockStateTransition(false);
            stateMachine.ChangeState(player.airState);
        }
    }

    public IEnumerator Restart()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(1.0f);
        Time.timeScale = 1;
    }
}
