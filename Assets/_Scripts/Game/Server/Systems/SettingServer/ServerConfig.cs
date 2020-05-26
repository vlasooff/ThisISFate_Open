
using System;
using System.IO;

public class ServerConfig : SaveBehaviour
{
    public string name;
    public ushort port;
    public byte MaxPlayers;

    public static string patchServer;
    public ServerConfig(string _name)
    {
        name = _name;
        port = 10515;
        MaxPlayers = 64;
        patchServer =  $"Servers/Server_{port}/";
        Directory.CreateDirectory(Environment.CurrentDirectory + $"/Servers/Server_{port}/");
    }
    public void Load()
    {
        ServerConfig config =  LoadJSON<ServerConfig>($"/Servers/Server_{port}/Server.config");
        name = config.name;
        port = config.port;
        MaxPlayers = config.MaxPlayers;

    }
    public void Save()
    {
        SaveJSON(this, $"/Servers/Server_{port}/Server.config");
    }
}

 