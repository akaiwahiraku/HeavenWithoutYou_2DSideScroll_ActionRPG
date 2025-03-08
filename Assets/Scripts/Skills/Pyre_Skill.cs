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
    // �C���X�y�N�^�ォ�璲���\�ȃ_���[�W�{���i��F1.0���ʏ�A1.5��1.5�{�Ȃǁj
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
        // �X�L���������̏���������΋L�q
    }

    public void CreatePyre()
    {
        Vector3 spawnPosition = player.transform.position + player.facingDir * launchPosition;
        GameObject pyre = Instantiate(pyrePrefab, spawnPosition, transform.rotation);
        currentPyre = pyre.GetComponent<Pyre_Skill_Controller>();
        if (currentPyre != null)
        {
            // damageMultiplier �������Ƃ��ēn��
            currentPyre.SetupPyre(finalDir, player, freezeTimeDuration, damageMultiplier);
        }
        else
        {
            Debug.LogWarning("Pyre_Skill: Pyre_Skill_Controller not found.");
        }
        player.AssignNewPyre(pyre);
    }
}
