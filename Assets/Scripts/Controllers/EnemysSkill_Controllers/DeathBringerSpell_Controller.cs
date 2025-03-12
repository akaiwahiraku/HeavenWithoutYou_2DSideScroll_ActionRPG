using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBringerSpell_Controller : MonoBehaviour, ISlowable
{
    [SerializeField] private Transform check;
    [SerializeField] private Vector2 boxSize;
    [SerializeField] private LayerMask whatIsPlayer;

    private CharacterStats myStats;
    private Animator animator; // �A�j���[�^�[���擾���āA�Đ����x�𐧌�

    public void SetupSpell(CharacterStats _stats) => myStats = _stats;

    private void Awake()
    {
        // Animator �R���|�[�l���g���擾�i�����A�^�b�`����Ă���΁j
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // Unleashed�X�L�����������Ȃ�A�X���[���ʂ�K�p����
        if (Unleashed_Skill.IsUnleashedActive)
        {
            Unleashed_Skill unleashedSkill = FindObjectOfType<Unleashed_Skill>();
            if (unleashedSkill != null)
            {
                // �X�L������ slowFactor �� duration ���g�p���ăA�j���[�^�[�̑��x��ቺ������
                ApplySlow(unleashedSkill.slowFactor, unleashedSkill.duration);
            }
            else
            {
                Debug.LogWarning("Unleashed_Skill �C���X�^���X��������܂���B");
            }
        }
    }

    private void AnimationTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(check.position, boxSize, whatIsPlayer);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Player>() != null)
            {
                hit.GetComponent<Entity>().SetupKnockbackDir(transform);
                myStats.DoPhysicalDamage(hit.GetComponent<CharacterStats>());
            }
        }
    }

    private void OnDrawGizmos() => Gizmos.DrawWireCube(check.position, boxSize);

    private void SelfDestroy() => Destroy(gameObject);

    // �� ISlowable �C���^�[�t�F�[�X�̎��� ��

    /// <summary>
    /// �X���[���ʂ�K�p���܂��B�����ł� Animator �̍Đ����x�ɔ��f�����܂��B
    /// </summary>
    /// <param name="slowFactor">�Đ����x�Ɋ|����{���i��F0.5f �Ȃ甼���̑����j</param>
    /// <param name="slowDuration">�X���[���ʂ̎�������</param>
    public void ApplySlow(float slowFactor, float slowDuration)
    {
        if (animator != null)
        {
            // ���݂̍Đ����x�� slowFactor ��K�p
            animator.speed *= slowFactor;
            StartCoroutine(RestoreAnimatorSpeedAfter(slowFactor, slowDuration));
        }
        else
        {
            Debug.LogWarning("Animator �R���|�[�l���g��������܂���B�X���[���ʂ�K�p�ł��܂���B");
        }
    }

    /// <summary>
    /// �w�莞�Ԍ�� Animator �̍Đ����x�����ɖ߂��R���[�`��
    /// </summary>
    private IEnumerator RestoreAnimatorSpeedAfter(float slowFactor, float slowDuration)
    {
        // ���݂� animator.speed �͂��ł� slowFactor �{�ɂȂ��Ă��邽�߁A���̒l���v�Z
        float originalSpeed = animator.speed / slowFactor;
        yield return new WaitForSeconds(slowDuration);
        animator.speed = originalSpeed;
    }
}
