using UnityEngine;
using System.Collections.Generic;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;

    // 各スキルコンポーネントの参照
    public Move_Skill move { get; private set; }
    public Clone_Skill clone { get; private set; }
    public DarkCircle_Skill darkCircle { get; private set; }
    public ShadowFlare_Skill shadowFlare { get; private set; }
    public Force_Skill force { get; private set; }
    public ShatteredSun_Skill shatteredSun { get; private set; }
    public Pyre_Skill pyre { get; private set; }


    public Heal_Skill heal { get; private set; }
    public Blackhole_Skill blackhole { get; private set; }
    public Guard_Skill guard { get; private set; }
    public DreamtideDriving_Skill dreamtideDriving { get; private set; }

    [Header("Selected Skills (Set via Inspector)")]
    public Skill primaryAttackSkill;
    public Skill dashSkill;
    public Skill jumpSkill;
    public Skill specialSkill;
    public Skill guardSkill;
    public Skill healSkill;
    public Skill overdrive1Skill;
    public Skill overdrive2Skill;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("[SkillManager] Another instance already exists, destroying this one.");
            Destroy(gameObject);
            return;
        }
        instance = this;
        Debug.Log("[SkillManager] instance set to this object.");
    }

    private void Start()
    {
        move = GetComponent<Move_Skill>();
        clone = GetComponent<Clone_Skill>();
        darkCircle = GetComponent<DarkCircle_Skill>();
        shadowFlare = GetComponent<ShadowFlare_Skill>();
        force = GetComponent<Force_Skill>();
        shatteredSun = GetComponent<ShatteredSun_Skill>();
        pyre = GetComponent<Pyre_Skill>();
        guard = GetComponent<Guard_Skill>();
        heal = GetComponent<Heal_Skill>();
        dreamtideDriving = GetComponent<DreamtideDriving_Skill>();
        blackhole = GetComponent<Blackhole_Skill>();

        Debug.Log("[SkillManager] Start() - Finished scanning skill components attached to this GameObject.");
    }

    /// <summary>
    /// 指定されたカテゴリに対して、現在選択されているスキルを返します。
    /// </summary>
    public Skill GetSelectedSkill(SkillCategory category)
    {
        Skill result = null;
        switch (category)
        {
            case SkillCategory.PrimaryAttack: result = primaryAttackSkill; break;
            case SkillCategory.Dash: result = dashSkill; break;
            case SkillCategory.Jump: result = jumpSkill; break;
            case SkillCategory.SpecialSkill: result = specialSkill; break;
            case SkillCategory.Guard: result = guardSkill; break;
            case SkillCategory.Heal: result = healSkill; break;
            case SkillCategory.Overdrive1: result = overdrive1Skill; break;
            case SkillCategory.Overdrive2: result = overdrive2Skill; break;
        }
        Debug.Log($"[SkillManager.GetSelectedSkill] category={category} => {(result != null ? result.skillName : "null")}");
        return result;
    }

    /// <summary>
    /// 指定カテゴリでアンロック済みのスキルを全て返す
    /// </summary>
    public List<Skill> GetUnlockedSkills(SkillCategory category)
    {
        Skill[] allSkills = FindObjectsOfType<Skill>();
        Debug.Log($"[SkillManager.GetUnlockedSkills] Searching for unlocked skills in category={category}. totalSkillComponents={allSkills.Length}");

        List<Skill> result = new List<Skill>();
        foreach (Skill s in allSkills)
        {
            Debug.Log($"   Checking skill: {s.skillName}, category={s.category}, isUnlocked={s.isUnlocked}");
            if (s.category == category && s.isUnlocked)
            {
                result.Add(s);
            }
        }
        Debug.Log($"[SkillManager.GetUnlockedSkills] => returning {result.Count} unlocked skills for category={category}");
        return result;
    }

    /// <summary>
    /// 選択中のスキルを差し替えます。
    /// </summary>
    public void SetSelectedSkill(SkillCategory category, Skill newSkill)
    {
        Debug.Log($"[SkillManager.SetSelectedSkill] Trying to set skill for category={category} => {newSkill?.skillName}, isUnlocked={newSkill?.isUnlocked}");

        if (newSkill != null && newSkill.isUnlocked)
        {
            switch (category)
            {
                case SkillCategory.PrimaryAttack: primaryAttackSkill = newSkill; break;
                case SkillCategory.Dash: dashSkill = newSkill; break;
                case SkillCategory.Jump: jumpSkill = newSkill; break;
                case SkillCategory.SpecialSkill: specialSkill = newSkill; break;
                case SkillCategory.Guard: guardSkill = newSkill; break;
                case SkillCategory.Heal: healSkill = newSkill; break;
                case SkillCategory.Overdrive1: overdrive1Skill = newSkill; break;
                case SkillCategory.Overdrive2: overdrive2Skill = newSkill; break;
            }
            Debug.Log($"[SkillManager] {category} skill set to: {newSkill.skillName}");
        }
        else
        {
            Debug.LogWarning($"[SkillManager] Invalid or locked skill for {category} => newSkill={newSkill?.skillName}, isUnlocked={newSkill?.isUnlocked}");
        }
    }
}
