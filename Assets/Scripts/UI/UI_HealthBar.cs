using System.Collections;
using System.Collections.Generic;
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

        // currentHealthがゼロの場合、スライダー全体を非表示
        if (myStats.currentHealth <= 0)
        {
            slider.gameObject.SetActive(false);
        }
        else
        {
            // スライダーの最大値と現在の値を更新
            slider.maxValue = myStats.GetMaxHealthValue();
            slider.value = myStats.currentHealth;

            // currentHealthがゼロでない場合は表示
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
