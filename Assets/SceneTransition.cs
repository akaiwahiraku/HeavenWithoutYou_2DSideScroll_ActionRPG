using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    // 移行先のシーン名を指定
    [SerializeField] private string targetSceneName = "StageSelect";

    // トリガーに入ったときの処理
    private void OnTriggerEnter2D(Collider2D other)
    {
        // プレイヤーかどうかのチェック
        if (other.CompareTag("Player"))
        {
            // シーンを非同期でロード
            StartCoroutine(LoadSceneAsync());
        }
    }

    // シーンを非同期でロードするコルーチン
    private IEnumerator LoadSceneAsync()
    {
        // ローディングシーンを挟む場合には、ここでローディングシーンをロードします
        //SceneManager.LoadScene("LoadingScene");

        // 非同期でターゲットシーンをロード
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetSceneName);
        asyncLoad.allowSceneActivation = false;

        // ローディング進行を監視（UIなどに進行度を表示可能）
        while (!asyncLoad.isDone)
        {
            // シーン読み込みが90%を超えたら、シーン遷移を許可
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
