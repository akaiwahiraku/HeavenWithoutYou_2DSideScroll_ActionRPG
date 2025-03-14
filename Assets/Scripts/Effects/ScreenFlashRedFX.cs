using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFlashRedFX : MonoBehaviour
{
    [SerializeField] private Image flashImage; // �Ԃ��C���[�W���A�^�b�`
    [SerializeField] private float flashDuration = 0.1f; // �t���b�V���̎�������

    private void Awake()
    {
        if (flashImage != null)
        {
            // ������Ԃœ����ɐݒ�
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
            // �t���b�V���̃R���[�`�����J�n
            StartCoroutine(FlashCoroutine());
        }
    }

    private IEnumerator FlashCoroutine()
    {
        float elapsedTime = 0f;
        Color originalColor = flashImage.color;

        // �t�F�[�h�C��
        while (elapsedTime < flashDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, elapsedTime / (flashDuration / 2));
            SetImageAlpha(alpha);
            yield return null;
        }

        // �t�F�[�h�A�E�g
        elapsedTime = 0f;
        while (elapsedTime < flashDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsedTime / (flashDuration / 2));
            SetImageAlpha(alpha);
            yield return null;
        }

        // �A���t�@��0�ɖ߂�
        SetImageAlpha(0);
    }

    private void SetImageAlpha(float alpha)
    {
        Color color = flashImage.color;
        color.a = alpha;
        flashImage.color = color;
    }
}