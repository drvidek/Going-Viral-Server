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
    [SerializeField] private float timerMax = 120;
    private float timer;
    public static bool[] playerReady = new bool[2] { false, false };

    public static ushort[,] scoreTable = new ushort[2, 3];

    private ushort _turnCounter;
    [SerializeField] private ushort _turnCounterMax = 3;
    public static ushort mobCounter;



    private void Start()
    {
        NextState();
    }

    private void RestartTimer()
    {
        timer = timerMax;
    }

    private void NextState()
    {
        switch (_currentState)
        {
            case GameState.PreGame:
                StartCoroutine("StatePreGame");
                break;
            case GameState.Build:
                StartCoroutine("StateBuild");
                break;
            case GameState.Play:
                StartCoroutine("StatePlay");
                break;
            case GameState.PostGame:
                StartCoroutine("StatePostGame");
                break;
            default:
                break;
        }
    }

    private void FixedUpdate()
    {
        //check if a player disconnected, and return to pregame if so
        ReturnToPregameCheck();
        //return the ready status of the two players
        Debug.Log($"Player 1: {(playerReady[0] ? "Ready" : "Not ready")} / Player 2: {(playerReady[1] ? "Ready" : "Not ready")}");
    }

    IEnumerator StatePreGame()
    {
        while (_currentState == GameState.PreGame)
        {
            if (NetworkManager.NetworkManagerInstance.GameServer.ClientCount == 2)
            {
                _turnCounter = 0;
                _currentState = GameState.Build;
            }
            yield return null;
        }
        UpdateGameState(_currentState);

        NextState();
    }

    IEnumerator StateBuild()
    {
        RestartTimer();
        while (_currentState == GameState.Build)
        {
            //if we run out of time or if both players are ready
            if (timer == 0 || PlayersReady())
            {
                //change our state to the gameplay state and return
                _currentState = GameState.Play;
            }
            //otherwise we count down towards 0 by real time
            timer = Mathf.MoveTowards(timer, 0, Time.deltaTime);

            NetworkOut.SendTimerMessage((ushort)timer);

            yield return null;

        }

        NetworkOut.SendMobCountMessage(mobCounter);

        //update the game state
        UpdateGameState(_currentState);

        NextState();
    }

    IEnumerator StatePlay()
    {
        int resourceSpawnMin = 2;
        int resourceSpawnMax = 5;
        NetworkOut.SendResourceSpawnMessage(resourceSpawnMin, resourceSpawnMax);

        while (_currentState == GameState.Play)
        {
            //if both players have no more mobs left
            if (PlayersReady())
            {
                _currentState = GameState.Build;
            }
            yield return null;

        }

        _turnCounter++;
        mobCounter = 0;
        if (_turnCounter == _turnCounterMax)
            _currentState = GameState.PostGame;

        UpdateGameState(_currentState);
        NextState();
    }

    IEnumerator StatePostGame()
    {

        while (_currentState == GameState.PostGame)
        {
 
            yield return null;

        }

        NextState();
    }


    /// <summary>
    /// Returns to the PreGame state if less than 2 players are present
    /// </summary>
    private void ReturnToPregameCheck()
    {
        if (NetworkManager.NetworkManagerInstance.GameServer.ClientCount < 2)
            UpdateGameState(GameState.PreGame);
    }

    /// <summary>
    /// Read and store a player's resources (takes params from message handler method)
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="points"></param>
    /// <param name="resourceA"></param>
    /// <param name="resourceB"></param>
    public static void UpdatePlayerTotals(ushort playerId, ushort points, ushort resourceA, ushort resourceB)
    {
        scoreTable[playerId, 0] = points;
        scoreTable[playerId, 1] = resourceA;
        scoreTable[playerId, 2] = resourceB;
    }

    /// <summary>
    /// returns true if both players have sent a ready message
    /// </summary>
    private bool PlayersReady()
    {
        return playerReady[0] && playerReady[1];
    }
    /// <summary>
    /// Change state, reset Player Ready, and send a message to the clients
    /// </summary>
    /// <param name="state" ></param>
    private void UpdateGameState(GameState state)
    {
        //set the server gamestate to the input state
        _currentState = state;
        //reset the ready status of both players
        playerReady[0] = false;
        playerReady[1] = false;

        //synchornise the points
        NetworkOut.SendPointsMessage();

        //send the current state to the clients
        NetworkOut.SendStateMessage(_currentState, _turnCounter);

    }

}
