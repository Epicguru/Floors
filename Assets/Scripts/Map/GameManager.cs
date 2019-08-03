using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Pawn Player;

    [Header("Floor")]
    // TODO make this stuff floor specific
    public Vector2Int FloorSize = new Vector2Int(100, 100);

    [Header("Game Over")]
    public CanvasGroup GameOverFade;
    public AnimationCurve FadeCurve;
    public float FadeTime = 1.5f;
    public TextMeshProUGUI LoadingText;

    [Header("Floor & Room")]
    public int Floor = 0;
    public TextMeshProUGUI FloorText;

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

    private void SetText(TextMeshProUGUI text, string msg)
    {
        if(text.text != msg)
        {
            text.text = msg;
        }
    }

    private void Update()
    {
        if((Player == null || Player.Health.IsDead) && !IsGameOver)
        {
            IsGameOver = true;
            fadeTimer = 0f;
        }

        SetText(FloorText, $"Floor {Floor}");

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
            LoadingText.text = $"Loading {LoadingOp.progress * 100f}%";
        }
    }
}
