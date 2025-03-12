using UnityEngine;
using System.Collections;

public class Explosive_Controller : MonoBehaviour, ISlowable
{
    private Animator anim;
    private CharacterStats myStats;
    private float growSpeed = 15;
    private float maxSize = 6;
    private float explosionRadius;

    private bool canGrow = true;

    private void Awake()
    {
        // Animator �R���|�[�l���g���擾
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        // Unleashed�X�L�����������Ȃ�A�X���[���ʂ�K�p����
        if (Unleashed_Skill.IsUnleashedActive)
        {
            Unleashed_Skill unleashedSkill = FindObjectOfType<Unleashed_Skill>();
            if (unleashedSkill != null)
            {
                ApplySlow(unleashedSkill.slowFactor, unleashedSkill.duration);
            }
            else
            {
                Debug.LogWarning("Unleashed_Skill �C���X�^���X��������܂���B");
            }
        }
    }

    private void Update()
    {
        if (canGrow)
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);

        if (maxSize - transform.localScale.x < .5f)
        {
            canGrow = false;
            if (anim != null)
                anim.SetTrigger("Explode");
        }
    }

    public void SetupExplosive(CharacterStats _myStats, float _growSpeed, float _maxSize, float _radius)
    {
        // SetupExplosive �Ăяo������ Animator ���擾�i���ł� Awake �Ŏ擾�ς݂̏ꍇ�͕s�v�ł����A�O�̂��߁j
        anim = GetComponent<Animator>();
        myStats = _myStats;
        growSpeed = _growSpeed;
        maxSize = _maxSize;
        explosionRadius = _radius;
    }

    private void AnimationExplodeEvent()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<CharacterStats>() != null)
            {
                hit.GetComponent<Entity>().SetupKnockbackDir(transform);
                myStats.DoPhysicalDamage(hit.GetComponent<CharacterStats>());
            }
        }
    }

    private void SelfDestroy() => Destroy(gameObject);

    // �� ISlowable �C���^�[�t�F�[�X�̎��� ��

    /// <summary>
    /// �X���[���ʂ�K�p���܂��B
    /// �����ł́A�����܂ł̐������x�igrowSpeed�j�� Animator �̍Đ����x�� slowFactor ��K�p���܂��B
    /// </summary>
    /// <param name="slowFactor">���x�Ɋ|����{���i��F0.5f �Ȃ甼���̑����j</param>
    /// <param name="slowDuration">�X���[���ʂ̎�������</param>
    public void ApplySlow(float slowFactor, float slowDuration)
    {
        // �������x�ɃX���[���ʂ�K�p
        growSpeed *= slowFactor;
        // Animator �̍Đ����x�ɃX���[���ʂ�K�p�i�A�^�b�`����Ă���ꍇ�j
        if (anim != null)
        {
            anim.speed *= slowFactor;
        }
        // ��莞�Ԍ�Ɍ��̑��x�ɖ߂��������J�n
        StartCoroutine(RestoreSlowAfter(slowFactor, slowDuration));
    }

    /// <summary>
    /// �w�莞�Ԍ�ɁA�������x�� Animator �̍Đ����x�����ɖ߂����߂̃R���[�`��
    /// </summary>
    private IEnumerator RestoreSlowAfter(float slowFactor, float slowDuration)
    {
        // ���݂̒l�͂��ł� slowFactor �{�ɂȂ��Ă��邽�߁A���̒l���v�Z���Ă���
        float originalGrowSpeed = growSpeed / slowFactor;
        float originalAnimSpeed = anim != null ? anim.speed / slowFactor : 1f;
        yield return new WaitForSeconds(slowDuration);
        growSpeed = originalGrowSpeed;
        if (anim != null)
        {
            anim.speed = originalAnimSpeed;
        }
    }
}
