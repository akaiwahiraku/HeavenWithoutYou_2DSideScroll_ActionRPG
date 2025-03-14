using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OverDriveTextFX : MonoBehaviour
{
    //private TextMeshPro overDriveName;
    [SerializeField] private TextMeshProUGUI overDriveName;
    [SerializeField] private Image frameImage;
    [SerializeField] private float flashDuration = 0.1f; // フラッシュの持続時間

    private void Awake()
    {

        if (frameImage != null)
        {
            // 初期状態で透明に設定
            Color color = frameImage.color;
            color.a = 0;
            frameImage.color = color;
        }

        if (overDriveName != null)
        {
            // 初期状態で透明に設定
            Color textColor = overDriveName.color;
            textColor.a = 0;
            overDriveName.color = textColor;
        }

    }


    public void FlashOverDriveText()
    {
        if (frameImage != null && overDriveName != null)
        {
            gameObject.SetActive(true);
            // フラッシュのコルーチンを開始
            StartCoroutine(FlashOverDriveTextCoroutine());
        }
    }

    private IEnumerator FlashOverDriveTextCoroutine()
    {
        float elapsedTime = 0f;

        //フレームのフェードイン
        while (elapsedTime < flashDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 1, elapsedTime / (flashDuration / 2));
            SetFrameImageAlpha(alpha);
            SetOverDriveNameAlpha(alpha);
            yield return null;
        }

        // アルファを0に戻す
        SetFrameImageAlpha(0);
        SetOverDriveNameAlpha(0);
    }

    private void SetFrameImageAlpha(float alpha)
    {
        Color color = frameImage.color;
        color.a = alpha;
        frameImage.color = color;
    }

    private void SetOverDriveNameAlpha(float alpha)
    {
        Color color = overDriveName.color;
        color.a = alpha;
        overDriveName.color = color;
    }
}


