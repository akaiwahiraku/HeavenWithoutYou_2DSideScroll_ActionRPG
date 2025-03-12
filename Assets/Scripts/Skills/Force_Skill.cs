using UnityEngine;
using UnityEngine.UI;

public class Force_Skill : Skill
{
    [Header("Force Skill Settings")]
    [SerializeField] private UI_SkillTreeSlot forceUnlockButton;
    [SerializeField] private GameObject forcePrefab;
    [SerializeField] private float freezeTimeDuration = 0.3f;
    [SerializeField] private float damageMultiplier = 10.0f;
    [SerializeField, Tooltip("�������ǂꂾ���g�傷�邩�i��F2.0�Ȃ�2�{�Ɋg��j")]
    private float forceScaleMultiplier = 17.5f;
    [SerializeField, Tooltip("�������g�傷��̂ɂ����鎞�ԁi�b�j")]
    private float forceExpansionDuration = 0.25f;
    [SerializeField, Tooltip("�����ɂ��m�b�N�o�b�N�̈З�")]
    private float knockbackPower = 75f;

    public bool forceUnlocked { get; private set; }

    private Force_Skill_Controller currentForce;

    protected override void Start()
    {
        base.Start();
        forceUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockForce);
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void CheckUnlock()
    {
        UnlockForce();
    }

    private void UnlockForce()
    {
        if (forceUnlockButton.unlocked)
        {
            forceUnlocked = true;
            isUnlocked = true;
        }
    }

    public override void UseSkill()
    {
        base.UseSkill();
        // �K�v�ɉ����ăX�L���������̒ǉ��������L�q
    }

    public void CreateForce()
    {
        // �v���C���[�̌��݈ʒu�Ő���
        Vector3 spawnPosition = player.transform.position;
        GameObject force = Instantiate(forcePrefab, spawnPosition, Quaternion.identity);
        currentForce = force.GetComponent<Force_Skill_Controller>();

        if (currentForce != null)
        {
            // �C���X�y�N�^�Őݒ肳�ꂽ�e�p�����[�^��n��
            currentForce.SetupForce(player, freezeTimeDuration, damageMultiplier, forceScaleMultiplier, forceExpansionDuration, knockbackPower);
        }
        else
        {
            Debug.LogWarning("Force_Skill: Force_Skill_Controller not found.");
        }
        player.AssignNewForce(force);
    }
}
