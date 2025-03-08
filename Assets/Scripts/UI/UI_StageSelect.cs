using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_StageSelect : MonoBehaviour
{
    [SerializeField] private Button stage1Button;      // ステージ1ボタン
    [SerializeField] private Button stage2Button;      // ステージ2ボタン
    [SerializeField] private UI_FadeScreen fadeScreen;

    private void Start()
    {
        // 各ボタンにシーン名を設定し、クリック時にシーン遷移させる
        stage1Button.onClick.AddListener(() => SelectStage("SnowStageScene"));
        stage2Button.onClick.AddListener(() => SelectStage("CastleStageScene"));

        //fadeScreen.FadeIn();
    }

    // シーン遷移を行うメソッド
    public void SelectStage(string sceneName)
    {
        StartCoroutine(LoadSceneWithFadeEffect(sceneName, 1.5f));
    }

    // シーン遷移をコルーチンで行い、フェードアウト効果を付加
    private IEnumerator LoadSceneWithFadeEffect(string sceneName, float delay)
    {
        if (fadeScreen != null)
        {
            // フェードアウト実行
            fadeScreen.FadeOut();
        }

        // 遅延を待機
        yield return new WaitForSeconds(delay);

        // シーンをロード
        SceneManager.LoadScene(sceneName);
    }
}
