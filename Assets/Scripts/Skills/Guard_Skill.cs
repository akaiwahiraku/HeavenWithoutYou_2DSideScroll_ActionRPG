using UnityEngine;
using UnityEngine.UI;

public class Guard_Skill : Skill
{
    [Header("Guard Info")]
    [SerializeField] private UI_SkillTreeSlot guardUnlockButton;
    public bool guardUnlocked { get; private set; }

    protected override void Start()
    {
        base.Start();
        guardUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockGuard);
    }

    protected override void CheckUnlock()
    {
        UnlockGuard();
    }

    private void UnlockGuard()
    {
        if (guardUnlockButton.unlocked)
        {
            guardUnlocked = true;
            isUnlocked = true;
        }
    }

    public override void UseSkill()
    {
        base.UseSkill();
        Debug.Log("Guard_Skill: " + skillName + " is used.");
        cooldownTimer = cooldown;
    }
}
