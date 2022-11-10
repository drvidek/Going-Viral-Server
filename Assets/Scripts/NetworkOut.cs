using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

public class NetworkOut : MonoBehaviour
{
    /// <summary>
    /// Send a message to the clients about a tower spawning
    /// </summary>
    public static void SendTowerMessage(ushort playerId, ushort locId, ushort towerId)
    {
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientID.towerSpawn);
        m.AddUShort(playerId);
        m.AddUShort(locId);
        m.AddUShort(towerId);

        NetworkManager.NetworkManagerInstance.GameServer.SendToAll(m);
    }

    /// <summary>
    /// Send a message to the clients about a mob spawning
    /// </summary>
    public static void SendMobMessage(ushort playerId, ushort locId, ushort mobID)
    {
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientID.mobSpawn);
        m.AddUShort(playerId);
        m.AddUShort(locId);
        m.AddUShort(mobID);

        NetworkManager.NetworkManagerInstance.GameServer.SendToAll(m);
    }

    /// <summary>
    /// Randomise and spawn resources at clients
    /// </summary>
    public static void SendResourceSpawnMessage(int spawnMin, int spawnMax)
    {
        //randomise the spawn count
        int spawnCount = Random.Range(spawnMin, spawnMax);

        //spawn that many points at random locations, weighting their 
        for (int i = 0; i < spawnCount; i++)
        {
            ushort resourceId = (ushort)Random.Range(0, 9);
            int resourceValue = Random.Range(10, 15) * (Mathf.FloorToInt((resourceId) / 3) + 1);
            string resourceType = MathExt.Roll(2) ? "DepositCrypto" : "DepositRAM";
            Message m = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientID.resourceSpawn);
            m.AddUShort(resourceId);
            m.AddUShort((ushort)resourceValue);
            m.AddString(resourceType);
            NetworkManager.NetworkManagerInstance.GameServer.SendToAll(m);
        }
    }

    /// <summary>
    /// Send a message containing the game state to clients
    /// </summary>
    /// <param name="state"></param>
    public static void SendStateMessage(GameState state)
    {
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientID.stateChange);
        m.AddUShort((ushort)state);

        NetworkManager.NetworkManagerInstance.GameServer.SendToAll(m);
    }

    /// <summary>
    /// Send the score table to clients
    /// </summary>
    public static void SendPointsMessage()
    {
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientID.points);
        for (var i = 0; i < 2; i++)
        {
            for (var ii = 0; ii < 3; ii++)
            {
                m.AddUShort(GameManager.scoreTable[i, ii]);
            }
        }
        NetworkManager.NetworkManagerInstance.GameServer.SendToAll(m);
    }

}
