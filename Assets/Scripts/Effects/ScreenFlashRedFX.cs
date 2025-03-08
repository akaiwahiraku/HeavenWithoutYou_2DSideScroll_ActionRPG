using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFlashRedFX : MonoBehaviour
{
    [SerializeField] private Image flashImage; // 赤いイメージをアタッチ
    [SerializeField] private float flashDuration = 0.1f; // フラッシュの持続時間

    private void Awake()
    {
        if (flashImage != null)
        {
            // 初期状態で透明に設定
            Color color = flashImage.color;
            color.a = 0;
            flashImage.color = color;
        }
    }

    public void FlashRedScreen()
    {
        if (flashImage != null)
        {
            gameObject.SetActive(true);
            // フラッシュのコルーチンを開始
            StartCoroutine(FlashCoroutine());
        }
    }

    private IEnumerator FlashCoroutine()
    {
        float elapsedTime = 0f;
        Color originalColor = flashImage.color;

        // フェードイン
        while (elapsedTime < flashDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, elapsedTime / (flashDuration / 2));
            SetImageAlpha(alpha);
            yield return null;
        }

        // フェードアウト
        elapsedTime = 0f;
        while (elapsedTime < flashDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsedTime / (flashDuration / 2));
            SetImageAlpha(alpha);
            yield return null;
        }

        // アルファを0に戻す
        SetImageAlpha(0);
    }

    private void SetImageAlpha(float alpha)
    {
        Color color = flashImage.color;
        color.a = alpha;
        flashImage.color = color;
    }
}