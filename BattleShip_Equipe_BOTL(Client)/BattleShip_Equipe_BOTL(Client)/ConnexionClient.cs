using BattleShip_Equipe_BOTL.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BattleShip_Equipe_BOTL_Client_
{
    public class ConnexionClient
    {
        public string adresseIP { get; set; }
        const int PORT = 46000;
        private Socket client;

        public ConnexionClient(string ip)
        {
            adresseIP = ip;
        }

        public async Task<bool> TryConnect()
        {
            try
            {
                Console.Clear();
                IPAddress adressIparse = IPAddress.Parse(adresseIP);
                IPEndPoint remoteEp = new IPEndPoint(adressIparse, PORT);

                 client = new Socket(adressIparse.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    await client.ConnectAsync(remoteEp);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public async Task<bool> Envoyer(Board board)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(board);
                byte[] leBoard = Encoding.ASCII.GetBytes(jsonString);
                int bytesEnvoye = client.Send(leBoard);
            }
            catch(Exception ex)
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
                int bytesRecu = await client.ReceiveAsync(new ArraySegment<byte>(bytes), SocketFlags.None);

                string data = Encoding.ASCII.GetString(bytes,0,bytesRecu);
                Board leBoard = JsonSerializer.Deserialize<Board>(data);
                return leBoard;
            }
            catch( Exception ex )
            {
                return null;
            } 
        }

        public async Task<bool> EnvoyerConfirmatioon(int conf)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(conf);
                byte[] laConf = Encoding.ASCII.GetBytes(jsonString);
                int bytesEnvoye = client.Send(laConf);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> EnvoyerTaille(int taille)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(taille);
                byte[] laConf = Encoding.ASCII.GetBytes(jsonString);
                int bytesEnvoye = client.Send(laConf);
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
                int bytesRecu = await client.ReceiveAsync(new ArraySegment<byte>(bytes), SocketFlags.None);

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
                int bytesEnvoye = client.Send(laConf);
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
                int bytesRecu = await client.ReceiveAsync(new ArraySegment<byte>(bytes), SocketFlags.None);

                string data = Encoding.ASCII.GetString(bytes, 0, bytesRecu);
                bool taille = JsonSerializer.Deserialize<bool>(data);
                return taille;
            }
            catch (Exception ex)
            {
                return false;
            }
        }



    }
}
