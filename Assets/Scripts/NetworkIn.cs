using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

public class NetworkIn : MonoBehaviour
{
    [MessageHandler((ushort)ClientToServerID.name)]
    private static void ManagePlayerName(ushort clientID, Message message)
    {
        
    }

    [MessageHandler((ushort)ClientToServerID.towerSpawn)]
    private static void SpawnTower(ushort clientID, Message message)
    {
        ushort playerID = message.GetUShort();
        ushort locationID = message.GetUShort();
        ushort towerID = message.GetUShort();

        NetworkOut.SendTowerMessage(playerID, locationID, towerID);

    }

    [MessageHandler((ushort)ClientToServerID.mobSpawn)]
    private static void SpawnMob(ushort clientID, Message message)
    {
        ushort playerID = message.GetUShort();
        ushort locationID = message.GetUShort();
        ushort mobID = message.GetUShort();
        GameManager.mobCounter++;
        NetworkOut.SendMobMessage(playerID, locationID, mobID);
    }

    [MessageHandler((ushort)ClientToServerID.points)]
    private static void ManagePoints(ushort clientID, Message message)
    {
        ushort playerID = message.GetUShort();
        ushort points = message.GetUShort();
        ushort resourceA = message.GetUShort();
        ushort resourceB = message.GetUShort();

        GameManager.UpdatePlayerTotals(playerID, points, resourceA, resourceB);

    }

    [MessageHandler((ushort)ClientToServerID.playerReady)]
    private static void GetPlayerReadyMessage(ushort clientID, Message message)
    {
        bool ready = message.GetBool();
        Debug.Log($"Ready message { ready} from player {clientID}");
        GameManager.playerReady[clientID - 1] = ready;
    }

}
