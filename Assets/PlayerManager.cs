using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager Instance;

    private Dictionary<ulong, PlayerData> m_playersInRoom = new Dictionary<ulong, PlayerData>();

    private void Start()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    [Rpc]
    public void CreatePlayerData(NetworkObject player)
    {
        PlayerData data = new PlayerData(player, null);
        
    }
    
    [Rpc]
    public void TryAddPlayer(ulong playerId, PlayerData pData)
    {
        m_playersInRoom.TryAdd(playerId, pData);
    }

    private void SetPlayerModel(Vehicle vehicleData, ulong playerID)
    {
        if(m_playersInRoom.ContainsKey(playerID))
        {
            if (m_playersInRoom.TryGetValue(playerID, out PlayerData data))
            {
                data.PlayerNObject.TryGetComponent(out TankController tController);
                
                tController.m_vehicleData = vehicleData;

                var controllerTransform = tController.transform;

                var instance = Instantiate(tController.m_vehicleData.m_tankModel.gameObject,
                    controllerTransform.position,
                    quaternion.identity, controllerTransform);

                var instanceNetworkObject = instance.GetComponent<NetworkObject>();

                instanceNetworkObject.SpawnWithOwnership(tController.GetComponent<NetworkObject>().OwnerClientId);

                instanceNetworkObject.TrySetParent(tController.gameObject);

                m_playerModel = instanceNetworkObject.GetComponent<TankModel>();

                m_playerModel.SetupPlayerTank(tController);

                tController.m_isSetup = true;
            }
        }
    }
}

public class PlayerData
{
    public NetworkObject PlayerNObject { get; private set; }
    public TankModel PlayerTankModel { get; private set; }

    public PlayerData(NetworkObject player, TankModel model)
    {
        PlayerNObject = player;
        PlayerTankModel = model;
    }
}
