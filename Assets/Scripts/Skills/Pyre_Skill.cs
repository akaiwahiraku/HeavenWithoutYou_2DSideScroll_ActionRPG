using UnityEngine;
using UnityEngine.UI;

public class Pyre_Skill : Skill
{
    [Header("First Attack Settings")]
    [SerializeField] private UI_SkillTreeSlot pyreUnlockButton;
    public bool pyreUnlocked { get; private set; }
    [SerializeField] private GameObject pyrePrefab;                   // �P���ڗp�v���n�u
    [SerializeField] private Vector2 firstAttackLaunchForce;            // �ˏo���x�i�P���ځj
    [SerializeField] private Vector3 firstAttackLaunchPosition;         // ���ˈʒu�I�t�Z�b�g�i�P���ځj
    [SerializeField] private float firstAttackProjectileDuration = 0.32f; // �v���W�F�N�^�C���̎������ԁi�P���ځj
    [SerializeField] private float firstAttackFreezeTimeDuration = 0.3f;  // FreezeTimeDuration�i�P���ځj
    [SerializeField] private float firstAttackDamageMultiplier = 1.0f;    // �_���[�W�}���`�v���C���[�i�P���ځj

    [Header("Second Attack Settings")]
    [SerializeField] private GameObject pyreSecondPrefab;               // �Q���ڗp�v���n�u
    [SerializeField] private Vector2 secondAttackLaunchForce;            // �ˏo���x�i�Q���ځj ��Inspector��ł͐��̒l�Őݒ�
    [SerializeField] private Vector3 secondAttackLaunchPosition;         // ���ˈʒu�I�t�Z�b�g�i�Q���ځj
    [SerializeField] private float secondAttackProjectileDuration = 0.5f;  // �v���W�F�N�^�C���̎������ԁi�Q���ځj
    [SerializeField] private float secondAttackFreezeTimeDuration = 0.5f;  // FreezeTimeDuration�i�Q���ځj
    [SerializeField] private float secondAttackDamageMultiplier = 1.5f;    // �_���[�W�}���`�v���C���[�i�Q���ځj
    // Collider�T�C�Y��A�j���[�V�����́A�Q���ڗpPrefab���Őݒ�ς�

    private Pyre_Skill_Controller currentPyre;
    private Vector2 finalDir; // �P���ڗp�̔��˕���

    protected override void Start()
    {
        base.Start();
        if (pyreUnlockButton != null)
        {
            pyreUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockPyre);
        }
    }

    protected override void Update()
    {
        base.Update();
        // �P���ڗp�́Aplayer.facingDir ���g���Ĕ��˕������v�Z
        finalDir = new Vector2(player.facingDir * firstAttackLaunchForce.x, firstAttackLaunchForce.y);
    }

    protected override void CheckUnlock()
    {
        UnlockPyre();
    }

    private void UnlockPyre()
    {
        if (pyreUnlockButton != null && pyreUnlockButton.unlocked)
        {
            pyreUnlocked = true;
            isUnlocked = true;
        }
    }

    public override void UseSkill()
    {
        base.UseSkill();
        // �������̒ǉ�����������΋L�q
    }

    /// <summary>
    /// isSecondAttack �� true �Ȃ�Q���ڗp�v���n�u�ƃp�����[�^�Ŕ��˂���
    /// </summary>
    public void CreatePyre(bool isSecondAttack = false)
    {
        if (isSecondAttack)
        {
            // �v���C���[�̌����ɍ��킹�����ˈʒu
            Vector3 spawnPosition = player.transform.position +
                new Vector3(player.facingDir * secondAttackLaunchPosition.x, secondAttackLaunchPosition.y, secondAttackLaunchPosition.z);
            GameObject pyre = Instantiate(pyreSecondPrefab, spawnPosition, Quaternion.identity);
            currentPyre = pyre.GetComponent<Pyre_Skill_Controller>();
            if (currentPyre != null)
            {
                // �v���C���[�̌����𔽉f�����ˏo���x���v�Z�iplayer.facingDir ���|����j
                Vector2 secondFinalDir = new Vector2(player.facingDir * secondAttackLaunchForce.x, secondAttackLaunchForce.y);
                // �C���|�C���g�F_launchForce �Ƃ��� secondFinalDir ��n��
                currentPyre.SetupPyre(secondFinalDir, player, secondAttackProjectileDuration,
                                       secondAttackFreezeTimeDuration, secondAttackDamageMultiplier, secondFinalDir);
            }
            else
            {
                Debug.LogWarning("Pyre_Skill: Pyre_Skill_Controller not found on PyreSecond prefab.");
            }
            player.AssignNewPyre(pyre);
        }
        else
        {
            Vector3 spawnPosition = player.transform.position +
                new Vector3(player.facingDir * firstAttackLaunchPosition.x, firstAttackLaunchPosition.y, firstAttackLaunchPosition.z);
            GameObject pyre = Instantiate(pyrePrefab, spawnPosition, Quaternion.identity);
            currentPyre = pyre.GetComponent<Pyre_Skill_Controller>();
            if (currentPyre != null)
            {
                currentPyre.SetupPyre(finalDir, player, firstAttackProjectileDuration, firstAttackFreezeTimeDuration, firstAttackDamageMultiplier);
            }
            else
            {
                Debug.LogWarning("Pyre_Skill: Pyre_Skill_Controller not found on pyrePrefab.");
            }
            player.AssignNewPyre(pyre);
        }
    }
}
