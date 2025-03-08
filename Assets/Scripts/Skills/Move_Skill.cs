using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Move_Skill : Skill
{
    [Header("Clone on Dash")]
    [SerializeField] private UI_SkillTreeSlot cloneOnDashUnlockButton;
    public bool cloneOnDashUnlocked { get; private set; }
    [Header("Double Jump")]
    [SerializeField] private UI_SkillTreeSlot doubleJumpUnlockButton;
    public bool doubleJumpUnlocked { get; private set; }
    [Header("Air Dash")]
    [SerializeField] private UI_SkillTreeSlot airDashUnlockButton;
    public bool airDashUnlocked { get; private set; }

    protected override void Start()
    {
        base.Start();
        cloneOnDashUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneOnDash);
        doubleJumpUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockDoubleJump);
        airDashUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockAirDash);
    }

    protected override void CheckUnlock()
    {
        UnlockCloneOnDash();
        UnlockDoubleJump();
        UnlockAirDash();
    }

    private void UnlockCloneOnDash()
    {
        if (cloneOnDashUnlockButton.unlocked)
            cloneOnDashUnlocked = true;
    }

    private void UnlockDoubleJump()
    {
        if (doubleJumpUnlockButton.unlocked)
            doubleJumpUnlocked = true;
    }

    private void UnlockAirDash()
    {
        if (airDashUnlockButton.unlocked)
            airDashUnlocked = true;
    }

    public void CloneOnDash()
    {
        if (cloneOnDashUnlocked)
            SkillManager.instance.clone.CreateClone(player.transform, Vector3.zero, false, true);
    }

    public override void UseSkill()
    {
        base.UseSkill();
        // Move スキル固有の処理をここに実装（必要に応じて）
    }
}
