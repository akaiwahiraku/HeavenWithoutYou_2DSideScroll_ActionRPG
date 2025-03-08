using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShadowFlare_Skill : Skill
{
    [Header("ShadowFlare Info")]
    [SerializeField] private UI_SkillTreeSlot shadowFlareUnlockButton;
    public bool shadowFlareUnlocked { get; private set; }
    [SerializeField] private Vector2 launchForce;
    [SerializeField] private Vector3 launchPosition;
    [SerializeField] private GameObject shadowFlarePrefab;
    [SerializeField] private float freezeTimeDuration = 0.3f;
    // �C���X�y�N�^�ォ�璲���\�ȃ_���[�W�{���i��F1.0���ʏ�A1.5��1.5�{�Ȃǁj
    [SerializeField] private float damageMultiplier = 1.0f;

    private ShadowFlare_Skill_Controller currentShadowFlare;
    public Transform myEnemy;

    private Vector2 finalDir;

    protected override void Start()
    {
        base.Start();
        shadowFlareUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockShadowFlare);
    }

    protected override void Update()
    {
        base.Update();
        finalDir = new Vector2(player.facingDir * launchForce.x, 0);
    }

    protected override void CheckUnlock()
    {
        UnlockShadowFlare();
    }

    private void UnlockShadowFlare()
    {
        if (shadowFlareUnlockButton.unlocked)
        {
            shadowFlareUnlocked = true;
            isUnlocked = true;
        }
    }

    public override void UseSkill()
    {
        base.UseSkill();
        // �X�L���������̏���������΋L�q
    }

    public void CreateShadowFlare()
    {
        Vector3 spawnPosition = player.transform.position + player.facingDir * launchPosition;
        GameObject shadowFlare = Instantiate(shadowFlarePrefab, spawnPosition, transform.rotation);
        currentShadowFlare = shadowFlare.GetComponent<ShadowFlare_Skill_Controller>();
        if (currentShadowFlare != null)
        {
            // damageMultiplier �������Ƃ��ēn��
            currentShadowFlare.SetupShadowFlare(finalDir, player, freezeTimeDuration, damageMultiplier);
        }
        else
        {
            Debug.LogWarning("ShadowFlare_Skill: ShadowFlare_Skill_Controller not found.");
        }
        player.AssignNewShadowFlare(shadowFlare);
    }
}
