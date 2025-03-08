using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFlashBlackoutFX : MonoBehaviour
{
    [SerializeField] private SpriteRenderer blackOutImage; //黒いイメージをアタッチ
    [SerializeField] private float flashDuration = 0.1f; // フラッシュの持続時間

    private void Awake()
    {
        if (blackOutImage != null)
        {
            // 初期状態で透明に設定
            Color color = blackOutImage.color;
            color.a = 1;
            blackOutImage.color = color;
        }
    }

    public void FlashBlackScreen()
    {
        if (blackOutImage != null)
        {
            gameObject.SetActive(true);
            // フラッシュのコルーチンを開始
            StartCoroutine(FlashBlackCoroutine());
        }
    }

    private IEnumerator FlashBlackCoroutine()
    {
        float elapsedTime = 0f;

        // フェードイン
        while (elapsedTime < flashDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 1, elapsedTime / (flashDuration / 2));
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
        Color color = blackOutImage.color;
        color.a = alpha;
        blackOutImage.color = color;
    }
}