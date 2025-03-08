using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFlashBlackoutFX : MonoBehaviour
{
    [SerializeField] private SpriteRenderer blackOutImage; //�����C���[�W���A�^�b�`
    [SerializeField] private float flashDuration = 0.1f; // �t���b�V���̎�������

    private void Awake()
    {
        if (blackOutImage != null)
        {
            // ������Ԃœ����ɐݒ�
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
            // �t���b�V���̃R���[�`�����J�n
            StartCoroutine(FlashBlackCoroutine());
        }
    }

    private IEnumerator FlashBlackCoroutine()
    {
        float elapsedTime = 0f;

        // �t�F�[�h�C��
        while (elapsedTime < flashDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 1, elapsedTime / (flashDuration / 2));
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
        Color color = blackOutImage.color;
        color.a = alpha;
        blackOutImage.color = color;
    }
}