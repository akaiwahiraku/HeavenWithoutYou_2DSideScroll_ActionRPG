using UnityEngine;
using UnityEngine.UI;

public class DarkCircle_Skill : Skill
{
    [Header("DarkCircle Info")]
    [SerializeField] private UI_SkillTreeSlot darkCircleUnlockButton;
    public bool darkCircleUnlocked { get; private set; }
    [SerializeField] private float hitCooldown = 0.35f;
    [SerializeField] private float maxTravelDistance = 3f;
    [SerializeField] private float darkCircleDuration = 5f;
    [SerializeField] private float gravity = 0f;
    [SerializeField] private Vector2 launchForce;
    [SerializeField] private Vector3 launchPosition;
    [SerializeField] private GameObject darkCirclePrefab;
    [SerializeField] private float freezeTimeDuration;

    [SerializeField] private float maxSize;
    [SerializeField] private float growSpeed;
    [SerializeField] private float shrinkSpeed;

    private DarkCircle_Skill_Controller currentDarkCircle;
    public Transform myEnemy;

    private Vector2 finalDir;

    protected override void Start()
    {
        base.Start();
        darkCircleUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockDarkCircle);
    }

    protected override void Update()
    {
        base.Update();
        finalDir = new Vector2(player.facingDir * launchForce.x, 0);
    }

    protected override void CheckUnlock()
    {
        UnlockDarkCircle();
    }

    private void UnlockDarkCircle()
    {
        if (darkCircleUnlockButton.unlocked)
        {
            darkCircleUnlocked = true;
            isUnlocked = true;
        }
    }

    public override void UseSkill()
    {
        base.UseSkill();
        // DarkCircleスキル自体は UseSkill() で生成処理を呼ぶなど、用途に応じて実装する
    }

    public void CreateDarkCircle()
    {
        Vector3 spawnPosition = player.transform.position + player.facingDir * launchPosition;
        GameObject darkCircle = Instantiate(darkCirclePrefab, spawnPosition, transform.rotation);
        currentDarkCircle = darkCircle.GetComponent<DarkCircle_Skill_Controller>();
        if (currentDarkCircle != null)
        {
            currentDarkCircle.SetupDarkCircle(maxSize, growSpeed, shrinkSpeed, myEnemy, finalDir, gravity, player, freezeTimeDuration, maxTravelDistance, darkCircleDuration, hitCooldown);
        }
        else
        {
            Debug.LogWarning("DarkCircle_Skill: DarkCircle_Skill_Controller not found.");
        }
        player.AssignNewDarkCircle(darkCircle);
    }
}
