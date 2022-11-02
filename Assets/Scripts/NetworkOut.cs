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
}
