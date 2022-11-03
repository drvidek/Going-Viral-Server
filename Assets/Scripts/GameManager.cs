using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

public enum GameState
{
    PreGame,
    Build,
    Play,
    PostGame
}
public class GameManager : MonoBehaviour
{

    public GameState _currentState = GameState.PreGame;
    private float timerMax;
    private float timer;
    public static bool[] playerReady = new bool[2] { false,false};

    private void FixedUpdate()
    {
        switch (_currentState)
        {
            case GameState.Build:
                BuildUpdate();
                break;
            case GameState.Play:
                PlayUpdate();
                break;
            default:
                break;
        }
        Debug.Log(playerReady[0] + " " + playerReady[1]);
    }

    private void BuildUpdate()
    {
        if (/*timer == 0 ||*/ PlayersReady())
        {
            UpdateGameState(GameState.Play);
            return;
        }

        timer = Mathf.MoveTowards(timer, 0, Time.deltaTime);
    }

    private void PlayUpdate()
    {
        if (PlayersReady())
        {
            UpdateGameState(GameState.Build);
            timer = timerMax;
            return;
        }
    }

    private bool PlayersReady()
    {
        return playerReady[0] && playerReady[1];
    }

    private void UpdateGameState(GameState state)
    {
        _currentState = state;
        //NetworkOut.SendStateMessage(state);
        playerReady[0] = false;
        playerReady[1] = false;
        NetworkOut.SendStateMessage(_currentState);
    }

}
