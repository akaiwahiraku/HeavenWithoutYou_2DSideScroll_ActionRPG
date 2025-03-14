using System.Collections;
using UnityEngine;

public class PlayerAimUnleashedState : PlayerState
{
    // �X�e�[�g�̎������ԁi�b�j
    private float stateDuration = 0.2f;
    private float timer;

    // �v���C���[�̌��̐F��ۑ�
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

        // Unleashed_Skill �𔭓����āA�S�̂̃G�t�F�N�g�i�G�X���[�A�G�F�ύX�A�����X�N���[���A���G��ԁj���J�n
        if (player.skill.unleashed.CanUseOverDrive())
        {
            player.skill.unleashed.ActivateUnleashedSkill();
        }

        onceTimestop = true;

        // �X�e�[�g�̎������Ԃ�������
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

        // �^�C�}�[�����������A0.2�b�o�߂����� IdleState �ɑJ�ڂ���
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
