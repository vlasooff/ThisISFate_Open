namespace Community.Client
{
    public delegate void OnConnectedClient();
    public delegate void OnStartedClient();
    public delegate void OnStartClient();
    public delegate void OnDisconnectedClient();
    public delegate void OnShutdownClient();
    public static class ClientCallBlack
    {
        public static bool isConnected = false;
        public static bool isClientRun = false;
        public static OnConnectedClient OnConnectedClient;
        public static OnStartClient OnStartClient;
        public static OnStartedClient onStartedClient;
        public static OnDisconnectedClient OnDisconnectedClient;
        public static OnShutdownClient OnShutdownClient;
        public static ushort ServerTick;

    }

}