using System.Collections;
using UnityEngine;

public class UI_EndScreenController : MonoBehaviour
{
    [SerializeField] private UI_FadeScreen fadeScreen;
    [SerializeField] private GameObject endText;
    [SerializeField] private GameObject restartButton;

    private void Awake()
    {
        fadeScreen.gameObject.SetActive(true);

    }

    public void SwitchOnEndScreen()
    {
        if (fadeScreen != null) fadeScreen.FadeOut();
        StartCoroutine(EndScreenCoroutine());
    }

    private IEnumerator EndScreenCoroutine()
    {
        yield return new WaitForSeconds(.5f);
        if (endText != null) endText.SetActive(true);
        yield return new WaitForSeconds(.5f);
        if (restartButton != null) restartButton.SetActive(true);
    }

    public void ResetEndScreen()
    {
        if (endText != null) endText.SetActive(false);
        if (restartButton != null) restartButton.SetActive(false);
    }

    public void RestartGameButton()
    {
        GameManager.instance.RestartScene();
    }
}
