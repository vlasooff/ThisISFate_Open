 
using Community.Core.Serializables;
using Community.Other;
using Community.Server.Components;
using LiteNetLib;
using Network.Core.Attributes;
using Network.Core.Serializables;
using System.Linq;
using UnityEngine;

namespace Community.Server
{
    public delegate bool OnCommandChat(IPlayerBase player, ICommandChat cmd);
    public delegate void OnSayChat(ChatPacket msg);

    public class ChatSystem : ComponentServer
    {
        ServerProxy server;
        ChatComponent chat;
        PlayersInfoComponent playersData;

        protected override void onStartedServer(NetManager manager)
        {  
                server = ServerManager.manager.serverProxy;
                chat = ServerManager.manager.chatData;
                playersData = ServerManager.manager.playersData;
                chat.commands.Add("say", new CommandSay());
                server._packetProcessor.SubscribeReusable<ChatPacket, NetPeer>(OnMessageFromPlayer);
          
        }

        private void OnMessageFromPlayer(ChatPacket obj, NetPeer peer)
        {
            if (playersData.players.ContainsKey(obj.Id))
            {
                if (obj.text.Length <= chat.MAX_MESSAGE_LENGTH)
                {
                    if (obj.text.Length == 0) return;
                    if (obj.text[0] == '/')
                    {
                        string cmd = new string(obj.text.Except("/").ToArray()).ToLower();
                        LogDev("Invoke command: " + cmd);
                        PlayerManager playerManager = playersData.players[obj.Id];
                        foreach (var command in chat.commands.Values) // iterate through all found methods
                        {
                            if (command.name == cmd )
                            {
                                if (ChatComponent.EventCommand != null)
                                    if (ChatComponent.EventCommand.Invoke(playerManager, command))
                                        command.Exception(playerManager, new string[] { cmd });
                            }
                        }
                    }
                    else
                    {
                        ChatComponent.EventSay?.Invoke(obj);
                        Say(obj);
                    }
                }
            }
        }
        public static void CommandConsole(string commad)
        {
            string cmd = new string(commad.Except("/").ToArray()).ToLower();
            Debug.Log("Command name:" + cmd);
            foreach (var command in ServerManager.manager.chatData.commands.Values) // iterate through all found methods
            {
                Debug.Log("Command name method:" + command.name);
                if (command.name == cmd)
                {
                    ChatPacket obj = new ChatPacket();
                    obj.Id = ushort.MaxValue;
                    obj.text = commad;
                    command.Exception(null, new string[] { new string(cmd.Except(" ").ToArray()) });
                }
            }
        }
        public void Say(ChatPacket msg)
        {
            LogDev("Say msg");
            server._netManager.SendToAll(WritePacket(msg), DeliveryMethod.ReliableSequenced);
        }
        protected override void OnUpdate()
        {

        } 
         


    }
    public interface ICommandChat
    {
        string name { get; }
        EtypePermission permission { get; }
        void Exception(Components.PlayerManager player, string[] param);
    }
    public class CommandSay : ICommandChat
    {
        public string name
        {
            get
            {
                return "say";
            }
        }

        public EtypePermission permission
        {
            get
            {
                return EtypePermission.player;
            }
        }

        public void Exception(PlayerManager player, string[] param)
        {

            ChatPacket packet = new ChatPacket();
            packet.Id = ushort.MaxValue;
            packet.text = param[0];
            // World.Active.GetExistingManager<ChatSystem>().Say(packet);
        }
    }
}