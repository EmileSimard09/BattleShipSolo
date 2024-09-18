using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BattleShip_Equipe_BOTL.Class
{
    public class ConnexionServer
    {
        public bool isConnected {  get; set; }
        private Socket ecouteur {  get; set; }
        public bool running { get; set; }
        private Socket handleLaCo { get; set; }

        public ConnexionServer() 
        { 
            running = false;
            isConnected = false;
        }

        public async Task Initialize()
        {
            IPAddress adresseIp = IPAddress.Any;
            IPEndPoint endpointLocal = new IPEndPoint(adresseIp, 46000);
            ecouteur = new Socket(adresseIp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            ecouteur.Bind(endpointLocal);
            running = true;
            try
            {
                ecouteur.Listen();
                handleLaCo = await ecouteur.AcceptAsync();
                isConnected = true;
            }
            catch (Exception ex)
            {
                //later 
            }
        }

        public async Task<bool> Envoyer(Board board)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(board);
                byte[] leBoard = Encoding.ASCII.GetBytes(jsonString);
                int bytesEnvoye = handleLaCo.Send(leBoard);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public async Task<Board> Recevoir()
        {
            try
            {
                byte[] bytes = new byte[64000];
                int bytesRecu = await handleLaCo.ReceiveAsync(new ArraySegment<byte>(bytes), SocketFlags.None);

                string data = Encoding.ASCII.GetString(bytes, 0, bytesRecu);
                Board leBoard = JsonSerializer.Deserialize<Board>(data);
                return leBoard;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> EnvoyerTaille(int taille)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(taille);
                byte[] laConf = Encoding.ASCII.GetBytes(jsonString);
                int bytesEnvoye = handleLaCo.Send(laConf);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public async Task<int> RecevoirTaille()
        {
            try
            {
                byte[] bytes = new byte[64000];
                int bytesRecu = await handleLaCo.ReceiveAsync(new ArraySegment<byte>(bytes), SocketFlags.None);

                string data = Encoding.ASCII.GetString(bytes, 0, bytesRecu);
                int taille = JsonSerializer.Deserialize<int>(data);
                return taille;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<bool> EnvoyerConf(bool conf)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(conf);
                byte[] laConf = Encoding.ASCII.GetBytes(jsonString);
                int bytesEnvoye = handleLaCo.Send(laConf);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> Recevoirconf()
        {
            try
            {
                byte[] bytes = new byte[64000];
                int bytesRecu = await handleLaCo.ReceiveAsync(new ArraySegment<byte>(bytes), SocketFlags.None);

                string data = Encoding.ASCII.GetString(bytes, 0, bytesRecu);
                bool taille = JsonSerializer.Deserialize<bool>(data);
                return taille;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<int> RecevoirConfirmation()
        {
            try
            {
                byte[] bytes = new byte[64000];
                int bytesRecu = await handleLaCo.ReceiveAsync(new ArraySegment<byte>(bytes), SocketFlags.None);

                string data = Encoding.ASCII.GetString(bytes, 0, bytesRecu);
                int laconf = JsonSerializer.Deserialize<int>(data);
                return laconf;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task FermerLaCo()
        {
            handleLaCo.Shutdown(SocketShutdown.Both);
            handleLaCo.Close();
            ecouteur.Close();
            isConnected = false;
            running = false;
        }

    }
}
