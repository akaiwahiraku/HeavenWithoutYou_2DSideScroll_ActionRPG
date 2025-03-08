using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    // �ڍs��̃V�[�������w��
    [SerializeField] private string targetSceneName = "StageSelect";

    // �g���K�[�ɓ������Ƃ��̏���
    private void OnTriggerEnter2D(Collider2D other)
    {
        // �v���C���[���ǂ����̃`�F�b�N
        if (other.CompareTag("Player"))
        {
            // �V�[����񓯊��Ń��[�h
            StartCoroutine(LoadSceneAsync());
        }
    }

    // �V�[����񓯊��Ń��[�h����R���[�`��
    private IEnumerator LoadSceneAsync()
    {
        // ���[�f�B���O�V�[�������ޏꍇ�ɂ́A�����Ń��[�f�B���O�V�[�������[�h���܂�
        //SceneManager.LoadScene("LoadingScene");

        // �񓯊��Ń^�[�Q�b�g�V�[�������[�h
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetSceneName);
        asyncLoad.allowSceneActivation = false;

        // ���[�f�B���O�i�s���Ď��iUI�Ȃǂɐi�s�x��\���\�j
        while (!asyncLoad.isDone)
        {
            // �V�[���ǂݍ��݂�90%�𒴂�����A�V�[���J�ڂ�����
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
