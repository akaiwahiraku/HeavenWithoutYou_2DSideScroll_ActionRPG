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

        // ���G��Ԃ��J�n
        player.stats.MakeInvincible(true);

        // WallCheck�𖳌���
        player.wallCheckEnabled = false;

        // �v���C���[�̃X�v���C�g�̌��̐F���擾���āA#00FFFF�ɕύX
        originalColor = player.sr.color;
        player.sr.color = new Color(0f, 1f, 1f);

        originalGravityScale = player.rb.gravityScale;
        player.rb.gravityScale = 0f;

        // AttackCheck�̈ʒu��ύX
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

        player.stats.isInShadowBringerOverDrive1stState = true; // �t���O��true�ɐݒ�


        // �X�e�[�g�J�ڂ����b�N
        SetLockStateTransition(true);
    }

    public override void Exit()
    {
        base.Exit();

        playerStats.isInOverdriveState = false;
        if (player.stats.overDriveStock > 0 && player.fx != null)
            player.fx.PlayOverDriveFX();

        // �X�v���C�g�̐F�����ɖ߂�
        player.sr.color = originalColor;

        player.rb.gravityScale = originalGravityScale;



        // AttackCheck�̈ʒu�����ɖ߂�
        if (attackCheckTransform != null)
        {
            attackCheckTransform.localPosition = originalAttackCheckPosition;
        }

        // �_�b�V����ԏI�����ɖ��G������
        player.stats.MakeInvincible(false);

        // WallCheck��L����
        player.wallCheckEnabled = true;

        player.stats.isInShadowBringerOverDrive1stState = false; // �t���O��false�ɐݒ�

        // �X�e�[�g�I����ɑJ�ڃ��b�N������
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

        // �v���C���[�̑��x���擾
        Vector3 playerVelocity = player.rb.velocity; // Rigidbody���瑬�x���擾

        // AfterImage�̐���
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
