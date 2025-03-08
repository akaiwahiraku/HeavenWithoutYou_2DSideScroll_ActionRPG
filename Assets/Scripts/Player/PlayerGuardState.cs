using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGuardState : PlayerState
{
    public PlayerStats currentOverDrive;
    public PlayerStats maxOverDrive;
    public PlayerStats damage;

    private float savedDirectionInput; // 保存する方向キーの入力

    private PhysicsMaterial2D originalMaterial;
    private CapsuleCollider2D capsuleCollider;

    public PlayerGuardState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
        joystickInputManager = JoystickInputManager.Instance;
    }

    public override void Enter()
    {
        base.Enter();
        player.stats.isInGuardState = true;


        //if (capsuleCollider != null)
        //{
        //    originalMaterial = capsuleCollider.sharedMaterial;
        //    capsuleCollider.sharedMaterial = player.SlippyMaterial;
        //}


    }

    public override void Exit()
    {
        base.Exit();
        player.stats.isInGuardState = false;

        //player.rb.sharedMaterial = originalMaterial;
    }

    public override void Update()
    {
        base.Update();

        joystickInputManager = JoystickInputManager.Instance;
        float horizontalInput = joystickInputManager.Horizontal;
        float verticalInput = joystickInputManager.Vertical;

        player.SetZeroVelocity();

        if ((joystickInputManager.Button3Down()) && (player.stats.overDriveStock >= 2))
        {
            if (SkillManager.instance.blackhole != null && SkillManager.instance.blackhole.CanUseSkill())
            {
                stateMachine.ChangeState(player.shadowBringerOverDrive2nd);
            }
            else
            {
                player.fx.CreatePopUpText("Not Enough OverDrive");
                return;
            }
        }

        // 方向キーの入力を反映してOverDrive1stに移行
        float directionInput = horizontalInput;

        if ((joystickInputManager.Button2Down()) && (player.stats.overDriveStock >= 1))
        {
            if (SkillManager.instance.dreamtideDriving != null && SkillManager.instance.dreamtideDriving.CanUseSkill())
            {
                player.rushDir = directionInput;
                stateMachine.ChangeState(player.shadowBringerOverDrive1st);
            }
            if (SkillManager.instance.shatteredSun != null && SkillManager.instance.shatteredSun.CanUseSkill())
            {
                stateMachine.ChangeState(player.aimShatteredSun);

            }
            else
            {
                //player.fx.CreatePopUpText("Not Enough OverDrive");
                return;
            }
            
        }


        if (!joystickInputManager.Button5())
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }

        // 空中状態への移行
        if (!player.IsGroundDetectedFore() && !player.IsGroundDetectedBack() && !player.IsThroughGroundDetected())
            stateMachine.ChangeState(player.airState);
    }
}
