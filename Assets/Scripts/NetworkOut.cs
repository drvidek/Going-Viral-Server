using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

public class NetworkOut : MonoBehaviour
{
    public static void SendTowerMessage(ushort playerId, ushort locId, ushort towerId)
    {
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientID.towerSpawn);
        m.AddUShort(playerId);
        m.AddUShort(locId);
        m.AddUShort(towerId);

        NetworkManager.NetworkManagerInstance.GameServer.SendToAll(m);
    }

    public static void SendMobMessage(ushort playerId, ushort locId, ushort mobID)
    {
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientID.mobSpawn);
        m.AddUShort(playerId);
        m.AddUShort(locId);
        m.AddUShort(mobID);

        NetworkManager.NetworkManagerInstance.GameServer.SendToAll(m);
    }

    private static void SendResourceSpawnMessage()
    {
        int resourceSpawnMin = 2;
        int resourceSpawnMax = 5;
        int spawnCount = Random.Range(resourceSpawnMin, resourceSpawnMax);
        for (int i = 0; i < spawnCount; i++)
        {
            ushort resourceId = (ushort)Random.Range(0, 9);
            int resourceValue = Random.Range(10, 15) * (Mathf.FloorToInt((resourceId) / 3) + 1);
            string resourceType = MathExt.Roll(2) ? "typeA" : "typeB";
            Message m = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientID.resourceSpawn);
            m.AddUShort(resourceId);
            m.AddUShort((ushort)resourceValue);
            m.AddString("Type");
            NetworkManager.NetworkManagerInstance.GameServer.SendToAll(m);
        }
    }

    public static void SendStateMessage(GameState state)
    {
        Message m = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientID.stateChange);
        m.AddUShort((ushort)state);

        NetworkManager.NetworkManagerInstance.GameServer.SendToAll(m);
    }

}
