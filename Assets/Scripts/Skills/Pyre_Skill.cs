using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pyre_Skill : Skill
{
    [Header("Pyre Info")]
    [SerializeField] private UI_SkillTreeSlot pyreUnlockButton;
    public bool pyreUnlocked { get; private set; }
    [SerializeField] private Vector2 launchForce;
    [SerializeField] private Vector3 launchPosition;
    [SerializeField] private GameObject pyrePrefab;
    [SerializeField] private float freezeTimeDuration = 0.3f;
    // インスペクタ上から調整可能なダメージ倍率（例：1.0＝通常、1.5＝1.5倍など）
    [SerializeField] private float damageMultiplier = 1.0f;

    private Pyre_Skill_Controller currentPyre;
    public Transform myEnemy;

    private Vector2 finalDir;

    protected override void Start()
    {
        base.Start();
        pyreUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockPyre);
    }

    protected override void Update()
    {
        base.Update();
        finalDir = new Vector2(player.facingDir * launchForce.x, 0);
    }

    protected override void CheckUnlock()
    {
        UnlockPyre();
    }

    private void UnlockPyre()
    {
        if (pyreUnlockButton.unlocked)
        {
            pyreUnlocked = true;
            isUnlocked = true;
        }
    }

    public override void UseSkill()
    {
        base.UseSkill();
        // スキル発動時の処理があれば記述
    }

    public void CreatePyre()
    {
        Vector3 spawnPosition = player.transform.position + player.facingDir * launchPosition;
        GameObject pyre = Instantiate(pyrePrefab, spawnPosition, transform.rotation);
        currentPyre = pyre.GetComponent<Pyre_Skill_Controller>();
        if (currentPyre != null)
        {
            // damageMultiplier を引数として渡す
            currentPyre.SetupPyre(finalDir, player, freezeTimeDuration, damageMultiplier);
        }
        else
        {
            Debug.LogWarning("Pyre_Skill: Pyre_Skill_Controller not found.");
        }
        player.AssignNewPyre(pyre);
    }
}
