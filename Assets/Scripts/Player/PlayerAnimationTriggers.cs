using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour
{
    private Player player => GetComponentInParent<Player>();

    private void AnimationTrigger()
    {
        player.AnimationTrigger();
    }

    private void AttackTrigger()
    {
        AudioManager.instance.PlaySFX(2, null);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                EnemyStats _target = hit.GetComponent<EnemyStats>();

                if (_target != null)
                    player.stats.DoPhysicalDamage(_target);

                ItemData_Equipment weaponData = Inventory.instance.GetEquipment(EquipmentType.Weapon);

                if (weaponData != null)
                    weaponData.Effect(_target.transform);
            }
        }
    }

    private void AttackTriggerWithoutSound()
    {
        //AudioManager.instance.PlaySFX(2, null);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                EnemyStats _target = hit.GetComponent<EnemyStats>();

                if (_target != null)
                    player.stats.DoPhysicalDamage(_target);

                ItemData_Equipment weaponData = Inventory.instance.GetEquipment(EquipmentType.Weapon);

                if (weaponData != null)
                    weaponData.Effect(_target.transform);
            }
        }
    }

    private void ChargeAttackTrigger()
    {
        //AudioManager.instance.PlaySFX(2, null);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                EnemyStats _target = hit.GetComponent<EnemyStats>();

                if (_target != null)
                {
                    player.stats.DoPhysicalDamageCharge(_target);
                }

                ItemData_Equipment weaponData = Inventory.instance.GetEquipment(EquipmentType.Weapon);

                if (weaponData != null)
                    weaponData.Effect(_target.transform);
            }
        }
    }

    private void ZeroVelocityTrigger()
    {
        player.SetZeroVelocity();
        player.rb.gravityScale = 0;
    }

    public void PushTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        foreach (var hit in colliders)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                float pushForce = 75f; // �����\�ȗ͂̑傫��
                Vector2 pushDirection = new Vector2(player.facingDir, .01f).normalized;

                // �ꎞ�I�Ƀm�b�N�o�b�N��Ԃ��������A�͂�������
                bool originalKnockedState = enemy.isKnocked; // ���̏�Ԃ�ۑ�
                enemy.isKnocked = false; // �m�b�N�o�b�N��Ԃ�����
                enemy.rb.AddForce(pushDirection * pushForce, ForceMode2D.Impulse);
                enemy.isKnocked = originalKnockedState; // ���̏�Ԃɖ߂�
            }
        }
    }


    public void DashTrigger()
    {
        player.SetVelocity(player.facingDir * 12.5f, 3);
    }



    //private void ThrowSword()
    //{
    //    SkillManager.instance.sword.CreateSword();
    //}

    private void ReleaseDarkCircle()
    {
        SkillManager.instance.darkCircle.CreateDarkCircle();
    }

    private void ReleaseShadowFlare()
    {
        SkillManager.instance.shadowFlare.CreateShadowFlare();
    }

    private void ReleaseForce()
    {
        SkillManager.instance.force.CreateForce();
    }

    // Pyre�X�L���ǉ���
    //private void ReleasePyre()
    //{
    //    if (SkillManager.instance.pyre != null && SkillManager.instance.pyre.CanUseSkill())
    //        SkillManager.instance.pyre.CreatePyre();
    //}


    private void ReleaseHeal()
    {
        SkillManager.instance.heal.CreateHeal();
    }

}
