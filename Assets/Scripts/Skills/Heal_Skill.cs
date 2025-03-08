using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Heal_Skill : Skill
{
    [Header("Heal Info")]
    [SerializeField] private UI_SkillTreeSlot healUnlockButton;
    public bool healUnlocked { get; private set; }
    [SerializeField] private float healPercent; // âÒïúó¶ÅiÅìÅj

    protected override void Start()
    {
        base.Start();
        //healUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockHeal);
    }

    protected override void CheckUnlock()
    {
        //UnlockHeal();
    }

    public override bool CanUseSkill()
    {
        Debug.Log($"[Heal_Skill] isUnlocked = {isUnlocked}");
        if (!isUnlocked)
        {
            player.fx.CreatePopUpText("Skill not unlocked!");
            return false;
        }
        if (cooldownTimer > 0)
        {
            player.fx.CreatePopUpText("Cooldown");
            return false;
        }
        return true;
    }

    public override void UseSkill()
    {
        base.UseSkill();
        CreateHeal();
    }

    public void CreateHeal()
    {
        PlayerStats playerStats = CurrencyManager.instance.player.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            int healAmount = Mathf.RoundToInt(playerStats.GetMaxHealthValue() * healPercent / 100f);
            playerStats.IncreaseHealthBy(healAmount);
            Debug.Log($"Heal_Skill: {skillName} healed {healAmount} HP.");
        }
        cooldownTimer = cooldown;
    }

    //private void UnlockHeal()
    //{
    //    if (healUnlockButton.unlocked)
    //    {
    //        healUnlocked = true;
    //        isUnlocked = true;
    //    }
    //}
}
