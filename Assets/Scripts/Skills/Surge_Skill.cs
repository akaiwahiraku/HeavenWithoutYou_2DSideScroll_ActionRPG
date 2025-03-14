using UnityEngine;
using UnityEngine.UI;

public class Surge_Skill : Skill
{
    [Header("Surge Settings")]
    [SerializeField] private UI_SkillTreeSlot surgeUnlockButton;
    public bool surgeUnlocked { get; private set; }
    [SerializeField] private GameObject surgePrefab;
    [SerializeField] private Vector2 launchForce;
    [SerializeField] private Vector3 launchPosition;
    [SerializeField] private float projectileDuration = 0.4f;
    [SerializeField] private float freezeTimeDuration = 0.3f;
    [SerializeField] private float damageMultiplier = 20.0f;
    [SerializeField] private float knockbackPower = 5f; // ノックバックの威力

    private Surge_Skill_Controller currentSurge;
    private Vector2 finalDir;

    protected override void Start()
    {
        base.Start();
        if (surgeUnlockButton != null)
        {
            surgeUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSurge);
        }
    }

    protected override void Update()
    {
        base.Update();
        finalDir = new Vector2(player.facingDir * launchForce.x, launchForce.y);
    }

    protected override void CheckUnlock()
    {
        UnlockSurge();
    }

    private void UnlockSurge()
    {
        if (surgeUnlockButton != null && surgeUnlockButton.unlocked)
        {
            surgeUnlocked = true;
            isUnlocked = true;
        }
    }

    public override void UseSkill()
    {
        base.UseSkill();
    }

    public void CreateSurge()
    {
        Vector3 spawnPosition = player.transform.position +
            new Vector3(player.facingDir * launchPosition.x, launchPosition.y, launchPosition.z);
        GameObject surge = Instantiate(surgePrefab, spawnPosition, Quaternion.identity);
        currentSurge = surge.GetComponent<Surge_Skill_Controller>();
        if (currentSurge != null)
        {
            currentSurge.SetupSurge(finalDir, player, projectileDuration, freezeTimeDuration, damageMultiplier, knockbackPower);
        }
        else
        {
            Debug.LogWarning("Surge_Skill: Surge_Skill_Controller not found on surgePrefab.");
        }
        player.AssignNewSurge(surge);
    }
}

