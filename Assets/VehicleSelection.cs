using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class VehicleSelection : MonoBehaviour
{
    [SerializeField] private List<Vehicle> m_vehicleList;

    public void SelectVehicle(int index)
    {
        if (index < m_vehicleList.Count)
        {
            PlayerManager.Instance.CreatePlayerData(NetworkManager.Singleton.LocalClient.ClientId, m_vehicleList[index]);
        }
    }
}
