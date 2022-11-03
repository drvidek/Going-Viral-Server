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
    private float timerMax = 120;
    private float timer;
    public static bool[] playerReady = new bool[2] { false, false };

    private void Start()
    {
        RestartTimer();
    }

    private void RestartTimer()
    {
        timer = timerMax;
    }

    private void FixedUpdate()
    {
        switch (_currentState)
        {
            case GameState.PreGame:
                PregameUpdate();
                break;
            case GameState.Build:
                BuildUpdate();
                break;
            case GameState.Play:
                PlayUpdate();
                break;
            case GameState.PostGame:
                break;
            default:
                break;
        }
        ReturnToPregameCheck();
        Debug.Log(playerReady[0] + " " + playerReady[1]);
    }

    private void BuildUpdate()
    {
        //if we run out of time or if both players are ready
        if (timer == 0 || PlayersReady())
        {
            //change our state to the gameplay state and return
            UpdateGameState(GameState.Play);
            return;
        }
        //otherwise we count down towards 0 by real time
        timer = Mathf.MoveTowards(timer, 0, Time.deltaTime);
    }

    private void PlayUpdate()
    {
        //if both players have no more mobs left
        if (PlayersReady())
        {
            UpdateGameState(GameState.Build);
            RestartTimer();
            return;
        }
    }

    private void PregameUpdate()
    {
        if (NetworkManager.NetworkManagerInstance.GameServer.ClientCount == 2)
            UpdateGameState(GameState.Build);
    }

    private void ReturnToPregameCheck()
    {
        if (NetworkManager.NetworkManagerInstance.GameServer.ClientCount < 2)
            UpdateGameState(GameState.PreGame);
    }

    /// <summary>
    /// returns true if both players have sent a ready message
    /// </summary>
    private bool PlayersReady()
    {
        
        return playerReady[0] && playerReady[1];
    }

    private void UpdateGameState(GameState state)
    {
        //set the server gamestate to the input state
        _currentState = state;
        //reset the ready status of both players
        playerReady[0] = false;
        playerReady[1] = false;
        //send the current state to the clients
        NetworkOut.SendStateMessage(_currentState);
    }

}
