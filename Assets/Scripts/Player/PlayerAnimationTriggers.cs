using System.Collections;
using System.Collections.Generic;
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

                if(_target != null)
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

    public void PushTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        foreach (var hit in colliders)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                float pushForce = 75f; // 調整可能な力の大きさ
                Vector2 pushDirection = new Vector2(player.facingDir, .01f).normalized;

                // 一時的にノックバック状態を解除し、力を加える
                bool originalKnockedState = enemy.isKnocked; // 元の状態を保存
                enemy.isKnocked = false; // ノックバック状態を解除
                enemy.rb.AddForce(pushDirection * pushForce, ForceMode2D.Impulse);
                enemy.isKnocked = originalKnockedState; // 元の状態に戻す
            }
        }
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

    // Pyreスキル追加時
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
