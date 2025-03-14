using UnityEngine;
using UnityEngine.UI;

public class DreamtideDriving_Skill : Skill
{
    [Header("DreamtideDriving Info")]
    [SerializeField] private UI_SkillTreeSlot dreamtideDrivingUnlockButton;
    public bool dreamtideDrivingUnlocked { get; private set; }

    protected override void Start()
    {
        base.Start();
        dreamtideDrivingUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockDreamtideDriving);
    }

    protected override void CheckUnlock()
    {
        UnlockDreamtideDriving();
    }

    private void UnlockDreamtideDriving()
    {
        if (dreamtideDrivingUnlockButton.unlocked)
        {
            dreamtideDrivingUnlocked = true;
            isUnlocked = true;
        }
    }

    public override void UseSkill()
    {
        base.UseSkill();
        cooldownTimer = cooldown;
    }
}
