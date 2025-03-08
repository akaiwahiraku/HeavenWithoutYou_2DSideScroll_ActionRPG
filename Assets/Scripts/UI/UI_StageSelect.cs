using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_StageSelect : MonoBehaviour
{
    [SerializeField] private Button stage1Button;      // �X�e�[�W1�{�^��
    [SerializeField] private Button stage2Button;      // �X�e�[�W2�{�^��
    [SerializeField] private UI_FadeScreen fadeScreen;

    private void Start()
    {
        // �e�{�^���ɃV�[������ݒ肵�A�N���b�N���ɃV�[���J�ڂ�����
        stage1Button.onClick.AddListener(() => SelectStage("SnowStageScene"));
        stage2Button.onClick.AddListener(() => SelectStage("CastleStageScene"));

        //fadeScreen.FadeIn();
    }

    // �V�[���J�ڂ��s�����\�b�h
    public void SelectStage(string sceneName)
    {
        StartCoroutine(LoadSceneWithFadeEffect(sceneName, 1.5f));
    }

    // �V�[���J�ڂ��R���[�`���ōs���A�t�F�[�h�A�E�g���ʂ�t��
    private IEnumerator LoadSceneWithFadeEffect(string sceneName, float delay)
    {
        if (fadeScreen != null)
        {
            // �t�F�[�h�A�E�g���s
            fadeScreen.FadeOut();
        }

        // �x����ҋ@
        yield return new WaitForSeconds(delay);

        // �V�[�������[�h
        SceneManager.LoadScene(sceneName);
    }
}
