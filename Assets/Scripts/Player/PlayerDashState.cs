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

    // ���G���Ԃ̊Ǘ�
    private float invincibilityDuration = 0.25f;
    private float invincibilityTimer;

    // �X�v���C�g�����_���[�ƌ��̐F�̕ۑ�
    private float colorLoosingOnDashDuration = 0.4f;
    private float colorLoosingOnDashTimer;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Color invincibleColor;

    //private float stateTimer; // �X�e�[�g�ɓ����Ă���̌o�ߎ���


    public PlayerDashState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
        joystickInputManager = JoystickInputManager.Instance;
    }

    public override void Enter()
    {
        base.Enter();

        //AudioManager.instance.PlaySFX(13, null);


        // ���G��Ԃ��J�n
        player.stats.MakeInvincible(true);
        invincibilityTimer = invincibilityDuration;
        colorLoosingOnDashTimer = colorLoosingOnDashDuration;

        // Player�̎q�I�u�W�F�N�g����Animator��SpriteRenderer���擾
        Transform animatorTransform = player.transform.Find("Animator");
        if (animatorTransform != null)
        {
            spriteRenderer = animatorTransform.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                originalColor = spriteRenderer.color; // ���̐F��ۑ�
                invincibleColor = new Color(0f, 1f, 1f, 0f); // �V�A���F�A�����x0.8
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

        // Rigidbody2D�̃}�e���A����SlippyMaterial�ɕύX
        originalMaterial = player.rb.sharedMaterial; // ���݂̃}�e���A����ۑ�
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

        // �_�b�V����ԏI�����ɖ��G�������i�O�̂��߁j�ƃX�v���C�g�̐F�����ɖ߂�
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

        // ���G���Ԃ̃J�E���g�_�E��
        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
            {
                player.stats.MakeInvincible(false); // ���G��Ԃ�����
                invincibilityTimer = 0;
            }
        }

        // ���G���̐F��� (���X�Ɍ��̐F�Ɠ����x�ɖ߂�)
        if (colorLoosingOnDashTimer > 0)
        {
            colorLoosingOnDashTimer -= Time.deltaTime;
            if (spriteRenderer != null)
            {
                float t = 1 - (colorLoosingOnDashTimer / colorLoosingOnDashDuration); // �i�s�x
                spriteRenderer.color = Color.Lerp(invincibleColor, originalColor, t);
                //colorLoosingOnDashTimer = 0;
            }
        }


        player.SetVelocity(player.dashSpeed * player.dashDir, 0);

        if (stateTimer < 0)
        {
            stateMachine.ChangeState(player.idleState);
        }

        // �v���C���[�̑��x���擾
        Vector3 playerVelocity = player.rb.velocity; // Rigidbody���瑬�x���擾

        // AfterImage�̐���
        player.fx.CreateAfterImage(playerVelocity);

        // �W�����v�ւ̈ڍs
        if (joystickInputManager.Button0Down() && (player.IsGroundDetectedFore() || player.IsGroundDetectedBack() || player.IsThroughGroundDetected()))
        {
            float boostedVelocityX = player.rb.velocity.x * 0.8f;
            player.jumpState.SetJumpVelocity(boostedVelocityX);

            player.airState.SetDashingTransition(true);
            stateMachine.ChangeState(player.jumpState);
        }

        //�ǃX���C�h�ւ̈ڍs
        if ((!player.IsGroundDetectedFore() || !player.IsGroundDetectedBack() || !player.IsThroughGroundDetected()) && player.IsWallDetected())
        {
            stateMachine.ChangeState(player.wallSlide);
        }

        // �ʏ�U��
        if (joystickInputManager.Button2Down() && !joystickInputManager.Button5())
        {
            float attackDir = horizontalInput;
            attackDir = attackDir == 0 ? player.facingDir : Mathf.Sign(attackDir); // ���͂��Ȃ��ꍇ�� facingDir ���g�p

            // GroundedState��AirState���őJ�ڐ��؂�ւ���
            if (player.IsGroundDetectedFore() || player.IsGroundDetectedBack() || player.IsThroughGroundDetected())
            {
                // GroundedState�̏ꍇ��PrimaryAttackState�֑J��
                player.primaryAttack.SetAttackDirection(attackDir); // �U��������ݒ�
                stateMachine.ChangeState(player.primaryAttack);
            }
            else
            {
                // AirState�̏ꍇ��JumpAttackState�֑J��
                player.jumpAttack.SetAttackDirection(attackDir); // �U��������ݒ�
                stateMachine.ChangeState(player.jumpAttack);
            }
        }

        ////���{�^���������ꂽ�牺�~
        //if (verticalInput < 0 && (!player.IsGroundDetectedFore() || !player.IsGroundDetectedBack()))
        //{
        //    stateMachine.ChangeState(player.airState);
        //    //player.rb.velocity = new Vector2(player.rb.velocity.x, player.airState.fastFallSpeed * 0);
        //}
    }
}
