using UnityEngine;
using UnityEngine.UI;

public class UI_HealthBar : MonoBehaviour
{

    [SerializeField] private Entity entity;
    [SerializeField] private CharacterStats myStats;

    private RectTransform myTransform;
    private Slider slider;

    private void Start()
    {
        entity = GetComponentInParent<Entity>();
        myStats = GetComponentInParent<CharacterStats>();
        myTransform = GetComponent<RectTransform>();
        slider = GetComponentInChildren<Slider>();

        UpdateHealthUI();
    }

    private void Update()
    {
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        //slider.maxValue = myStats.GetMaxHealthValue();
        //slider.value = myStats.currentHealth;

        // currentHealth���[���̏ꍇ�A�X���C�_�[�S�̂��\��
        if (myStats.currentHealth <= 0)
        {
            slider.gameObject.SetActive(false);
        }
        else
        {
            // �X���C�_�[�̍ő�l�ƌ��݂̒l���X�V
            slider.maxValue = myStats.GetMaxHealthValue();
            slider.value = myStats.currentHealth;

            // currentHealth���[���łȂ��ꍇ�͕\��
            slider.gameObject.SetActive(true);
        }

    }

    private void OnEnable()
    {
        entity.onFlipped += FlipUI;
        myStats.onHealthChanged += UpdateHealthUI;
    }

    private void OnDisable()
    {
        if (entity != null)
            entity.onFlipped -= FlipUI;

        if (myStats != null)
            myStats.onHealthChanged -= UpdateHealthUI;
    }
    private void FlipUI()
    {
        myTransform.Rotate(0, 180, 0);
    }
}
