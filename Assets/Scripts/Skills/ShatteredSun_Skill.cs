using UnityEngine;
using UnityEngine.UI;

public class ShatteredSun_Skill : Skill
{
    [Header("ShatteredSun Skill Settings")]
    [SerializeField] private UI_SkillTreeSlot shatteredSunUnlockButton;
    [SerializeField] private GameObject shatteredSunPrefab;
    [SerializeField] private float freezeTimeDuration = 0.3f;
    [SerializeField] private float damageMultiplier = 10.0f;
    [SerializeField, Tooltip("�G�t�F�N�g���ǂꂾ���g�傷�邩�i��F2.0�Ȃ�2�{�Ɋg��j")]
    private float shatteredSunScaleMultiplier = 17.5f;
    [SerializeField, Tooltip("�G�t�F�N�g���g�傷��̂ɂ����鎞�ԁi�b�j")]
    private float shatteredSunExpansionDuration = 0.25f;
    [SerializeField, Tooltip("�G�t�F�N�g�ɂ��m�b�N�o�b�N�̈З�")]
    private float knockbackForce = 75f;

    // �V�K�F�����_���[�W�̎������Ԃƃ_���[�W�Ԋu�i�C���X�y�N�^�ŕύX�\�j
    [SerializeField, Tooltip("�������ԁi�b�j")]
    private float sustainedDamageDuration = 2.0f;
    [SerializeField, Tooltip("�_���[�W�̊Ԋu�i�b�j")]
    private float damageInterval = 0.5f;

    public bool shatteredSunUnlocked { get; private set; }

    private ShatteredSun_Skill_Controller currentShatteredSun;
    private bool hasLaunched = false;

    protected override void Start()
    {
        base.Start();
        shatteredSunUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockShatteredSun);
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void CheckUnlock()
    {
        UnlockShatteredSun();
    }

    private void UnlockShatteredSun()
    {
        if (shatteredSunUnlockButton.unlocked)
        {
            shatteredSunUnlocked = true;
            isUnlocked = true;
        }
    }

    public override void UseSkill()
    {
        base.UseSkill();
        // �ǉ�����������΋L�q
    }

    // �X�L����Ԃ̃��Z�b�g
    public void ResetSkillState()
    {
        hasLaunched = false;
        currentShatteredSun = null;
    }

    public void CreateShatteredSun()
    {
        // ��ʒ����ɃG�t�F�N�g�𐶐�
        Vector3 spawnPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        spawnPosition.z = 0f;
        GameObject shatteredSun = Instantiate(shatteredSunPrefab, spawnPosition, Quaternion.identity);
        currentShatteredSun = shatteredSun.GetComponent<ShatteredSun_Skill_Controller>();

        if (currentShatteredSun != null)
        {
            currentShatteredSun.SetupShatteredSun(
                player,
                freezeTimeDuration,
                damageMultiplier,
                shatteredSunScaleMultiplier,
                shatteredSunExpansionDuration,
                knockbackForce,
                sustainedDamageDuration,
                damageInterval);
        }
        player.AssignNewShatteredSun(shatteredSun);
        hasLaunched = true;
    }

    public bool SkillCompleted()
    {
        if (!hasLaunched)
            return false;
        if (currentShatteredSun == null)
            return true;
        if (currentShatteredSun.playerCanExitState)
        {
            currentShatteredSun = null;
            return true;
        }
        return false;
    }
}
