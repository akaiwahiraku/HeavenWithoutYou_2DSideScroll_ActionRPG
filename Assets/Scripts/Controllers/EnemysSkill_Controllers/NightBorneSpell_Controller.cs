using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightBorneSpell_Controller : MonoBehaviour, ISlowable
{
    private SpriteRenderer sr;

    [SerializeField] private int damage;
    [SerializeField] private string targetLayerName = "Player";

    [SerializeField] private float xVelocity;
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private bool canMove;
    [SerializeField] private bool flipped;

    private CharacterStats myStats;

    private int facingDir = 1;

    private void Update()
    {
        if (canMove)
            rb.velocity = new Vector2(xVelocity, rb.velocity.y);

        if (facingDir == 1 && rb.velocity.x < 0)
        {
            facingDir = -1;
            sr.flipX = true;
        }
    }

    private void Start()
    {
        // �����̏���������������΂��̂܂�

        // Unleashed�X�L�����������Ȃ�A�X���[���ʂ�K�p����
        if (Unleashed_Skill.IsUnleashedActive)
        {
            // Unleashed_Skill �̃C���X�^���X���擾
            Unleashed_Skill unleashedSkill = FindObjectOfType<Unleashed_Skill>();
            if (unleashedSkill != null)
            {
                // �X�L������ slowFactor �� duration ���g�p
                ApplySlow(unleashedSkill.slowFactor, unleashedSkill.duration);
            }
            else
            {
                Debug.LogWarning("Unleashed_Skill �C���X�^���X��������܂���B");
            }
        }
    }



    public void SetupArcane(float _speed, CharacterStats _myStats)
    {
        sr = GetComponent<SpriteRenderer>();
        xVelocity = _speed;
        myStats = _myStats;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(targetLayerName))
        {
            myStats.DoPhysicalDamage(collision.GetComponent<CharacterStats>());
            Destroy(gameObject);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            Destroy(gameObject);

        if (collision.GetComponent<DarkCircle_Skill_Controller>() != null)
        {
            Destroy(gameObject);
        }

        if (collision.GetComponent<Blackhole_Skill_Controller>() != null)
        {
            Destroy(gameObject);
        }

        if (collision.GetComponent<Force_Skill_Controller>() != null)
        {
            Destroy(gameObject);
        }
    }

    public void FlipSpell()
    {
        if (flipped)
            return;

        xVelocity = xVelocity * -1;
        flipped = true;
        transform.Rotate(0, 180, 0);
        targetLayerName = "Enemy";
    }

    // �� ��������V���ɒǉ�����X���[�����֘A�̊֐� ��

    /// <summary>
    /// ISlowable �C���^�[�t�F�[�X�̎���
    /// </summary>
    /// <param name="slowFactor">���x�Ɋ|����{���i��F0.5f �Ȃ甼���̑����j</param>
    /// <param name="slowDuration">�X���[���ʂ̎�������</param>
    public void ApplySlow(float slowFactor, float slowDuration)
    {
        // xVelocity �ɃX���[���ʂ�K�p
        xVelocity *= slowFactor;
        // ���݂� rb.velocity �ɂ������{����K�p
        rb.velocity = new Vector2(rb.velocity.x * slowFactor, rb.velocity.y);
        // �w�莞�Ԍ�Ɍ��̑��x�ɖ߂��������J�n
        StartCoroutine(RestoreSpeedAfter(slowFactor, slowDuration));
    }

    /// <summary>
    /// �X���[���ʏI����Ɍ��̑��x�ɖ߂����߂̃R���[�`��
    /// </summary>
    private IEnumerator RestoreSpeedAfter(float slowFactor, float slowDuration)
    {
        // ���݂� xVelocity �͂��ł� slowFactor �{�ɂȂ��Ă���̂ŁA���̒l���v�Z���Ă���
        float originalXVelocity = xVelocity / slowFactor;
        yield return new WaitForSeconds(slowDuration);
        xVelocity = originalXVelocity;
        // ���� Update �� rb.velocity �� xVelocity �ɍ��킹�čX�V����邽�߁A�ǉ������͕s�v
    }
}
