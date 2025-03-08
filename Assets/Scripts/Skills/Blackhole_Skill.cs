using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blackhole_Skill : Skill
{
    [Header("Blackhole Specific Data")]
    [SerializeField] private UI_SkillTreeSlot blackHoleUnlockButton;
    public bool blackholeUnlocked { get; private set; }

    [SerializeField] private int amountOfAttacks;
    [SerializeField] private float cloneCooldown;
    [SerializeField] private float blackholeCloneAttackPower;
    [SerializeField] private float blackholeDuration;

    [Space]
    [SerializeField] private GameObject blackHolePrefab;
    [SerializeField] private float maxSize;
    [SerializeField] private float growSpeed;
    [SerializeField] private float shrinkSpeed;

    public Transform myEnemy;

    // �����ŊǗ�����ꎞ�I�� Blackhole_Skill_Controller �Q��
    private Blackhole_Skill_Controller currentBlackhole;

    // launchPosition �� launchForce ���K�v�i�����̎������Q�l�j
    [SerializeField] private Vector3 launchPosition;
    [SerializeField] private Vector2 launchForce;

    protected override void Start()
    {
        base.Start();
        if (blackHoleUnlockButton != null)
        {
            Button btn = blackHoleUnlockButton.GetComponent<Button>();
            if (btn != null)
                btn.onClick.AddListener(UnlockBlackhole);
        }
    }

    protected override void Update()
    {
        base.Update();

    }

    protected override void CheckUnlock()
    {
        UnlockBlackhole();
    }

    private void UnlockBlackhole()
    {
        if (blackHoleUnlockButton != null && blackHoleUnlockButton.unlocked)
        {
            blackholeUnlocked = true;
            isUnlocked = true;
        }
    }

    public override void UseSkill()
    {
        base.UseSkill();
        Debug.Log("Using Blackhole skill: " + skillName);
        cooldownTimer = cooldown;  // �N�[���_�E�����J�n

        // �����ʒu�̌v�Z�F�v���C���[�̈ʒu + (�v���C���[�̌��� * launchPosition)
        Vector3 spawnPosition = player.transform.position + player.facingDir * launchPosition;
        if (blackHolePrefab == null)
        {
            Debug.LogWarning("Blackhole_Skill: blackHolePrefab is not set.");
            return;
        }
        GameObject newBlackHole = Instantiate(blackHolePrefab, spawnPosition, Quaternion.identity);
        currentBlackhole = newBlackHole.GetComponent<Blackhole_Skill_Controller>();
        if (currentBlackhole != null)
        {
            // �ŏI�I�Ȍ������v�Z�F�v���C���[�� facingDir ��p���� launchForce �� x �����𔽉f
            Vector2 finalDir = new Vector2(player.facingDir * launchForce.x, 0);
            currentBlackhole.SetupBlackhole(maxSize, growSpeed, shrinkSpeed, amountOfAttacks, cloneCooldown, blackholeDuration, myEnemy);
        }
        else
        {
            Debug.LogWarning("Blackhole_Skill: Blackhole_Skill_Controller not found on blackHolePrefab.");
        }
        AudioManager.instance.PlaySFX(6, player.transform);
    }

    /// <summary>
    /// �I�v�V�����F�X�L��������Ԃ̃`�F�b�N
    /// </summary>
    public bool SkillCompleted()
    {
        if (currentBlackhole == null)
            return false;
        if (currentBlackhole.playerCanExitState)
        {
            currentBlackhole = null;
            return true;
        }
        return false;
    }

    /// <summary>
    /// �I�v�V�����F���݂̃u���b�N�z�[���̔��a��Ԃ�
    /// </summary>
    public float GetBlackholeRadius()
    {
        return maxSize / 2f;
    }
}
