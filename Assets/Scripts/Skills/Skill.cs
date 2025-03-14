using UnityEngine;

public class Skill : MonoBehaviour
{
    public float cooldown;       // �N�[���_�E�����ԁi�b�j
    public float cooldownTimer;  // ���݂̃N�[���_�E������

    public SkillCategory category;   // ���̃X�L����������J�e�S��
    public string skillName;         // �X�L���̖��O
    [TextArea]
    public string description;       // �X�L���̐���

    public bool isUnlocked = false;  // �X�L���c���[�ŃA�����b�N�����܂� false

    protected Player player;

    protected virtual void Start()
    {
        // �v���C���[�Q�Ƃ̎擾�iCurrencyManager �Ȃǂ���j
        player = CurrencyManager.instance.player;

        // �N�[���_�E���^�C�}�[�̏�����
        cooldownTimer = 0f;

        CheckUnlock();
    }

    protected virtual void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// �h���N���X�Ŏ����B�����ŃX�L���̃A�����b�N�����Ȃǂ��`�F�b�N����B
    /// �f�t�H���g�ł͉������Ȃ��B
    /// </summary>
    protected virtual void CheckUnlock()
    {
        // ������Ԃ̓��b�N���
        isUnlocked = false;
    }

    /// <summary>
    /// �X�L�����g�p�\���ǂ����𔻒肷��B
    /// �g�p�\�Ȃ̂́A(1) �X�L���c���[�ŃA�����b�N�ς݁A(2) SkillManager �ł��̃J�e�S���ɑI������Ă���A(3) �N�[���_�E�����I�����Ă���ꍇ�B
    /// </summary>
    public virtual bool CanUseSkill()
    {
        if (!isUnlocked)
        {
            //player.fx.CreatePopUpText("Skill not unlocked!");
            return false;
        }

        // �I������Ă���X�L�����ǂ����� SkillManager ����m�F����
        Skill selected = SkillManager.instance.GetSelectedSkill(category);
        if (selected != this)
        {
            //player.fx.CreatePopUpText("Skill not set!");
            return false;
        }

        if (cooldownTimer > 0)
        {
            player.fx.CreatePopUpText("Cooldown");
            return false;
        }

        return true;
    }

    /// <summary>
    /// �X�L���̎��ۂ̌��ʂ𔭓�����i�h���N���X�ŏ㏑���j�B
    /// </summary>
    public virtual void UseSkill()
    {
        Debug.Log("Using skill: " + skillName);
        cooldownTimer = cooldown;
        // �����Ɋe�X�L���ŗL�̏�������������
    }

    public virtual bool CanUseOverDrive()
    {
        UseSkill();
        return true;
    }

    /// <summary>
    /// �w��ʒu����ł��߂��G��T���ĕԂ�
    /// </summary>
    protected virtual Transform FindClosestEnemy(Transform _checkTransform)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_checkTransform.position, 25);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                float distanceToEnemy = Vector2.Distance(_checkTransform.position, hit.transform.position);
                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }
        }
        return closestEnemy;
    }
}
