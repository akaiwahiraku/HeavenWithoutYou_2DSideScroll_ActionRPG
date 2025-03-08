using System.Collections;
using UnityEngine;

public class PlayerAirState : PlayerState
{
    private PhysicsMaterial2D originalMaterial;
    public PhysicsMaterial2D FrictionalMaterial;
    public PhysicsMaterial2D SlippyMaterial;

    private float playerAirStateTimer; // �X�e�[�g�ɓ����Ă���̌o�ߎ���
    [SerializeField] private float timeThreshold = 0.3f; // �ݒ肵�����b��

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
        //timerEventTriggered = false;
    }

    public override void Exit()
    {
        base.Exit();
        player.GetComponent<Collider2D>().sharedMaterial = originalMaterial;
        SetDashingTransition(false);
    }

    public override void Update()
    {
        base.Update();

        joystickInputManager = JoystickInputManager.Instance;
        float horizontalInput = joystickInputManager.Horizontal;
        float verticalInput = joystickInputManager.Vertical;

        // �^�C�}�[�̍X�V
        playerAirStateTimer += Time.deltaTime;

        //���{�^���������Ƌ}�~��
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

        //�ʏ�U��
        if (joystickInputManager.Button2Down())
        {
            //stateMachine.ChangeState(player.primaryAttack);
            stateMachine.ChangeState(player.jumpAttack);
        }

        // ����X�L���iSkillManager �� specialSkill �ɃZ�b�g����Ă��� && �X�L��������ς݁j
        if (joystickInputManager.Button3Down() && !joystickInputManager.Button5())
        {
            if (SkillManager.instance.darkCircle != null && SkillManager.instance.darkCircle.CanUseSkill())
                stateMachine.ChangeState(player.aimDarkCircle);
            //if (SkillManager.instance.shadowFlare != null && SkillManager.instance.shadowFlare.CanUseSkill())
            //    stateMachine.ChangeState(player.aimShadowFlare);
            if (SkillManager.instance.force != null && SkillManager.instance.force.CanUseSkill())
                stateMachine.ChangeState(player.aimForce);

        }

        //��i�W�����v
        if (canDoubleJump && joystickInputManager.Button0Down())
        {
            player.rb.velocity = new Vector2(player.rb.velocity.x, player.jumpForce);
            canDoubleJump = false;
            playerAirStateTimer = 0f; // �W�����v��A�^�C�}�[�����Z�b�g
        }

        //�ǃX���C�h�Ɉڍs
        if (player.IsWallDetected())
        {
            stateMachine.ChangeState(player.wallSlide);
        }

        //�ڒn����
        if (player.IsGroundDetectedFore() || player.IsGroundDetectedBack() || (player.IsThroughGroundDetected() && player.rb.velocity.y <= 0))
        {
            player.GetComponent<Collider2D>().sharedMaterial = originalMaterial;
            stateMachine.ChangeState(player.idleState);
        }

        //��i�W�����v�A�G�A�_�b�V���̃��Z�b�g
        if (player.IsGroundDetectedFore() || player.IsGroundDetectedBack() || (player.IsThroughGroundDetected() && player.rb.velocity.y <= 0) || player.IsWallDetected())
        {
            canDoubleJump = true;
            player.canAirDash = true;
        }

        // ���ړ� & �㏸���Ȃ�A���蔲�����̃R���C�_�[�𖳎�
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
            player.dashDir = horizontalInput;
            player.dashDir = player.dashDir == 0 ? player.facingDir : player.dashDir;
            stateMachine.ChangeState(player.dashState);
            player.canAirDash = false;
        }
    }

    private void CheckForAirRushInput()
    {
        if (joystickInputManager.Button2Down() && joystickInputManager.Button5() && (!player.IsGroundDetectedFore() || !player.IsGroundDetectedBack() || !player.IsThroughGroundDetected()))
        {
            player.rushDir = horizontalInput;
            player.rushDir = player.rushDir == 0 ? player.facingDir : player.rushDir;
            stateMachine.ChangeState(player.shadowBringerOverDrive1st);
            player.canAirRush = false;
        }
    }
}
