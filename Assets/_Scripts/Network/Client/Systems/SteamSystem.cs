using Community.Client.Components;
using Network;
using Unity.Entities;
using UnityEngine;


namespace Community.Client.Systems
{ 
    public class SteamSystem : ComponentClient
    {
      
        ClientDataSteam dataClient;
        protected override void OnStartedClient()
        { 

            
            dataClient = ClientManager.manager.clientDataSteam;
            if (dataClient.isDevelop)
            {
                Enabled = false;
                return;
            }
            if (dataClient.AppId == 0)
                throw new System.Exception("You need to set the AppId to your game");
            //
            // Configure us for this unity platform
            //
            Facepunch.Steamworks.Config.ForUnity(Application.platform.ToString());

            //
            // Create a steam_appid.txt (this seems greasy as fuck, but this is exactly
            // what UE's Steamworks plugin does, so fuck it.
            //
            try
            {
                System.IO.File.WriteAllText("steam_appid.txt", dataClient.AppId.ToString());
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Couldn't write steam_appid.txt: " + e.Message);
            }

            // Create the client
            dataClient.client = new Facepunch.Steamworks.Client(dataClient.AppId);

            if (!dataClient.client.IsValid)
            {
                dataClient.client = null;
                Debug.LogWarning("Couldn't initialize Steam");
                return;
            }

            Debug.Log("Steam Initialized: " + dataClient.client.Username + " / " + dataClient.client.SteamId);
        }


        protected override void OnUpdate()
        {
            if (!ClientManager.manager.isClient)
            {
                Enabled = false;
                return;
            }
            if (dataClient.client == null)
                return;

            try
            {
                UnityEngine.Profiling.Profiler.BeginSample("Steam Update");
                dataClient.client.Update();
            }
            finally
            {
                UnityEngine.Profiling.Profiler.EndSample();
            }
        }
        protected override void OnStopRunning()
        {
            if (!ClientManager.manager.isClient)
            {
                Enabled = false;
                return;
            }
            if (dataClient.client != null)
            {
                dataClient.client.Dispose();
                dataClient.client = null;
            }
        }
    }
}
