using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Slider odSlider;

    [Header("OverDrive Stock UI")]
    [SerializeField] private Image[] overDriveStockIcons;  // OverDriveストックのアイコン
    private int lastOverDriveStock = 0;  // 前回のストック数を保持する変数

    [Header("Level & XP UI")]
    [SerializeField] private TextMeshProUGUI levelText;  // 現在のレベル表示用テキスト
    [SerializeField] private Slider xpSlider;  // 経験値バー

    public bool isInShadowBringerOverDrive2ndState;
    private bool isOverDriveFXPlaying = false;

    private bool isBlinking = false;
    private float blinkTimer = 0f;
    [SerializeField] private float blinkDuration = 0.2f;
    private Color originalColor;
    [SerializeField] private Color blinkColor = Color.white;

    [SerializeField] private Image darkCircleImage;
    [SerializeField] private Image healImage;

    private SkillManager skills;

    [Header("Souls info")]
    [SerializeField] private TextMeshProUGUI currentSouls;
    [SerializeField] private float soulsAmount;
    [SerializeField] private float increaseRate = 100;

    public Player player;

    void Start()
    {
        hpSlider = GetComponentInChildren<Slider>();
        UpdateHealthUI();

        skills = SkillManager.instance;
        originalColor = hpSlider.fillRect.GetComponent<Image>().color;

        darkCircleImage.fillAmount = 0;
        healImage.fillAmount = 0;

        UpdateLevelUI();       // 初期のレベルを表示
        UpdateExperienceUI();  // 初期の経験値バーを表示
    }

    void Update()
    {
        UpdateHealthUI();
        UpdateOverDriveUI();
        UpdateSoulsUI();
        UpdateOverDriveStockUI();

        // 現在のレベルと経験値バーの表示を更新
        UpdateLevelUI();
        UpdateExperienceUI();

        if (playerStats.overDriveStock != lastOverDriveStock)
        {
            AnimateOverDriveStock(playerStats.overDriveStock);
            lastOverDriveStock = playerStats.overDriveStock;
        }

        if (player.stateMachine.currentState is PlayerHealState)
        {
            SetCooldownOf(healImage);
        }

        if (player.stateMachine.currentState is PlayerAimDarkCircleState)
        {
            SetCooldownOf(darkCircleImage);
        }

        CheckCooldownOf(darkCircleImage, skills.darkCircle.cooldown);
        CheckCooldownOf(healImage, skills.heal.cooldown);

        if (hpSlider.value <= hpSlider.maxValue * playerStats.crisisPercent)
        {
            TriggerBlink();
        }
        else
        {
            StopBlink();
        }

        if (isBlinking)
        {
            blinkTimer += Time.deltaTime;
            if (blinkTimer >= blinkDuration)
            {
                Image fillImage = hpSlider.fillRect.GetComponent<Image>();
                fillImage.color = fillImage.color == originalColor ? blinkColor : originalColor;
                blinkTimer = 0f;
            }
        }

        if (player != null && player.fx != null)
        {
            if (player.stats.overDriveStock >= 1 && !playerStats.isInOverdriveState)
            {
                if (!isOverDriveFXPlaying)
                {
                    player.fx.PlayOverDriveFX();
                    isOverDriveFXPlaying = true;
                }
            }
            else if (isOverDriveFXPlaying)
            {
                player.fx.StopOverDriveFX();
                isOverDriveFXPlaying = false;
            }
        }

    }

    private void UpdateOverDriveStockUI()
    {
        for (int i = 0; i < overDriveStockIcons.Length; i++)
        {
            overDriveStockIcons[i].gameObject.SetActive(i < playerStats.overDriveStock);
        }
    }

    private void AnimateOverDriveStock(int currentStock)
    {
        for (int i = 0; i < overDriveStockIcons.Length; i++)
        {
            if (i < currentStock)
            {
                overDriveStockIcons[i].gameObject.SetActive(true);
                overDriveStockIcons[i].GetComponent<Animator>().SetTrigger("StockAdded");
            }
            else
            {
                overDriveStockIcons[i].gameObject.SetActive(false);
            }
        }
    }

    // レベルを表示するメソッド
    private void UpdateLevelUI()
    {
        if (levelText != null)
        {
            levelText.text = "Lv. " + playerStats.currentLevel;
        }
    }

    // 経験値バーの更新メソッド
    private void UpdateExperienceUI()
    {
        if (xpSlider != null)
        {
            xpSlider.maxValue = playerStats.experienceToNextLevel;  // 次のレベルまでの必要経験値を設定
            xpSlider.value = playerStats.currentExperience;         // 現在の経験値を設定
        }
    }

    public void TriggerBlink()
    {
        isBlinking = true;
    }

    public void StopBlink()
    {
        isBlinking = false;
        Image fillImage = hpSlider.fillRect.GetComponent<Image>();
        fillImage.color = originalColor;
    }

    private void UpdateHealthUI()
    {
        hpSlider.maxValue = playerStats.GetMaxHealthValue();
        hpSlider.value = playerStats.currentHealth;
    }

    private void UpdateOverDriveUI()
    {
        odSlider.maxValue = playerStats.GetMaxOverDriveValue();
        odSlider.value = playerStats.currentOverDrive;
    }

    private void UpdateSoulsUI()
    {
        if (soulsAmount < CurrencyManager.instance.GetCurrency())
            soulsAmount += Time.deltaTime * increaseRate;
        else
            soulsAmount = CurrencyManager.instance.GetCurrency();

        currentSouls.text = ((int)soulsAmount).ToString();
    }

    private void SetCooldownOf(Image _image)
    {
        if (_image.fillAmount <= 0)
            _image.fillAmount = 1;
    }

    private void CheckCooldownOf(Image _image, float _cooldown)
    {
        if (_image.fillAmount > 0)
        {
            _image.fillAmount -= 1 / _cooldown * Time.deltaTime;
            if (_image.fillAmount <= 0)
            {
                _image.fillAmount = 0;
            }
        }
    }
}
