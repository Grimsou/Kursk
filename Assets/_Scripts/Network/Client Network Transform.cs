using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class ClientNetworkTransform : NetworkTransform
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        CanCommitToTransform = IsOwner;
        
        if(!IsOwner) return;
        PlayerManager.Instance.CreatePlayerData(this.GetComponent<NetworkObject>());
    }

    protected override void Update()
    {
        //We ensure that the client can commit to this transform
        CanCommitToTransform = IsOwner;
        
        base.Update();
        
        if (NetworkManager != null)
        {
            if (NetworkManager.IsConnectedClient || NetworkManager.IsListening)
            {
                if (CanCommitToTransform)
                {
                    //As long as the client is connected and server running, we'll try commiting transform updates
                    TryCommitTransformToServer(this.transform, NetworkManager.LocalTime.Time);
                }
            }    
        }
    }

    //Server hasn't authority on this object, client does
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
