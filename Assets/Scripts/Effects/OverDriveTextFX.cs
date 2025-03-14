using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OverDriveTextFX : MonoBehaviour
{
    //private TextMeshPro overDriveName;
    [SerializeField] private TextMeshProUGUI overDriveName;
    [SerializeField] private Image frameImage;
    [SerializeField] private float flashDuration = 0.1f; // �t���b�V���̎�������

    private void Awake()
    {

        if (frameImage != null)
        {
            // ������Ԃœ����ɐݒ�
            Color color = frameImage.color;
            color.a = 0;
            frameImage.color = color;
        }

        if (overDriveName != null)
        {
            // ������Ԃœ����ɐݒ�
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
            // �t���b�V���̃R���[�`�����J�n
            StartCoroutine(FlashOverDriveTextCoroutine());
        }
    }

    private IEnumerator FlashOverDriveTextCoroutine()
    {
        float elapsedTime = 0f;

        //�t���[���̃t�F�[�h�C��
        while (elapsedTime < flashDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 1, elapsedTime / (flashDuration / 2));
            SetFrameImageAlpha(alpha);
            SetOverDriveNameAlpha(alpha);
            yield return null;
        }

        // �A���t�@��0�ɖ߂�
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


