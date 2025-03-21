using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Runtime.CompilerServices;

public class SwitchCars : NetworkBehaviour
{
    [SerializeField] private List<GGCar> Cars = new List<GGCar>();

    public override void OnNetworkSpawn()
    {
        // Disable all cars initially
        foreach (GGCar car in Cars)
        {
            car.gameObject.SetActive(false);
        }

        // Enable the car that matches the client ID
        foreach ((ulong clientId, _) in NetworkManager.Singleton.ConnectedClients)
        {
            if (clientId < (ulong)Cars.Count)
            {
                Cars[(int)clientId].gameObject.SetActive(true);
            }
        }
    }
}