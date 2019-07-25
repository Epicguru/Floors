using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Pawn Player;

    [Header("Game Over")]
    public CanvasGroup GameOverFade;
    public AnimationCurve FadeCurve;
    public float FadeTime = 1.5f;
    public TextMeshProUGUI LoadingText;

    [Header("State")]
    public bool IsRestarting = false;
    public bool IsGameOver = false;

    private AsyncOperation LoadingOp;
    private float fadeTimer;

    private void RestartLevel()
    {
        if (IsRestarting)
            return;

        IsRestarting = true;
        LoadingOp = SceneManager.LoadSceneAsync("Dev Scene", LoadSceneMode.Single);
    }

    private void Update()
    {
        if((Player == null || Player.Health.IsDead) && !IsGameOver)
        {
            IsGameOver = true;
            fadeTimer = 0f;
        }

        if (IsGameOver)
        {
            fadeTimer += Time.unscaledDeltaTime;
            float p = fadeTimer / FadeTime;
            if (p >= 1f)
                p = 1f;
            float x = FadeCurve.Evaluate(p);

            GameOverFade.alpha = x;

            if(p >= 0.5f)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    RestartLevel();
                }
            }
        }

        if (IsRestarting)
        {
            LoadingText.text = $"Loading {LoadingOp.progress * 100f : F0}%";
        }
    }
}
