using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerAimUnleashedState : PlayerState
{
    // ステートの持続時間（秒）
    private float stateDuration = 0.2f;
    private float timer;

    // プレイヤーの元の色を保存
    private Color originalColor;
    PlayerStats playerStats;
    private bool onceTimestop = false;

    public PlayerAimUnleashedState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName)
        : base(_player, _stateMachine, _animBoolName)
    {
        joystickInputManager = JoystickInputManager.Instance;

        playerStats = _player.GetComponent<PlayerStats>();
    }

    public override void Enter()
    {
        base.Enter();

        player.stats.MakeInvincible(true);

        player.stats.overDriveStock = player.stats.overDriveStock - 2;

        AudioManager.instance.PlaySFX(6, null);

        playerStats.isInOverdriveState = true;
        if (player.fx != null)
            player.fx.StopOverDriveFX();

        // Unleashed_Skill を発動して、全体のエフェクト（敵スロー、敵色変更、黒いスクリーン、無敵状態）を開始
        if (player.skill.unleashed.CanUseOverDrive())
        {
            player.skill.unleashed.ActivateUnleashedSkill();
        }

        onceTimestop = true;

        // ステートの持続時間を初期化
        timer = stateDuration;
    }


    public override void Exit()
    {
        base.Exit();
        player.stats.MakeInvincible(false);

    }

    public override void Update()
    {
        base.Update();

        if (onceTimestop)
        {
            if (playerStats != null && playerStats.screenFlashBlackout != null)
            {
                playerStats.overDriveUnleashedText.FlashOverDriveText();
                playerStats.screenFlashBlackout.FlashBlackScreen();
            }
            else
            {
                Debug.LogError("PlayerStats or screenFlashBlackout is null.");
            }

            player.StartCoroutine(Restart());
            onceTimestop = false;
        }

        player.fx.CreateAfterImage(player.rb.velocity);

        // タイマーを減少させ、0.2秒経過したら IdleState に遷移する
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
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
