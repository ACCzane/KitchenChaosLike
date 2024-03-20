using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }


    public event EventHandler OnStateChanged;

    private enum State
    {
        WaitingToStart,
        CountDownToStart,
        GamePlaying,
        GameOver
    }

    private State state;
    private float waitingToStartTime = 1f;
    private float countDownToStartTime = 3f;
    private float gamePlayingTimeMax = 10f;
    private float gamePlayingTime = 10f;
    

    private void Awake()
    {
        Instance = this;
        state = State.WaitingToStart;
    }

    private void Update()
    {
        switch (state)
        {
            case State.WaitingToStart:
                waitingToStartTime -= Time.deltaTime;
                if(waitingToStartTime < 0f)
                {
                    state = State.CountDownToStart;
                    OnStateChanged?.Invoke(this, new EventArgs());
                }
                break;
            case State.CountDownToStart:
                countDownToStartTime -= Time.deltaTime;
                if (countDownToStartTime < 0f)
                {
                    state = State.GamePlaying;
                    OnStateChanged?.Invoke(this, new EventArgs());
                }
                break;
            case State.GamePlaying:
                gamePlayingTime -= Time.deltaTime;
                if (gamePlayingTime < 0f)
                {
                    state = State.GameOver;
                    OnStateChanged?.Invoke(this, new EventArgs());
                }
                break;
            case State.GameOver:
                break;
        }
        Debug.Log(state);
    }

    public bool IsGamePlaying()
    {
        return state == State.GamePlaying;
    }

    public bool IsCountDownToStartActive()
    {
        return state == State.CountDownToStart;
    }

    public bool IsGameOver()
    {
        return state == State.GameOver;
    }

    public float GetCountDownToStartTime()
    {
        return countDownToStartTime;
    }

    public float GetGamePlayingTimeNormalized()
    {
        return 1 - (gamePlayingTime / gamePlayingTimeMax);
    }
}
