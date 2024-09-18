﻿using BattleShip_Equipe_BOTL.Class;
using System;
using System.Net;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace BattleShip_Equipe_BOTL_Client_
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            
            //Création de la connexion utilisée
            ConnexionClient? LaConnexion = null;

            //Établissement de la connexion
            LaConnexion = await TentativeConnexion(getIp());

            while (LaConnexion == null)
            {
                Console.WriteLine("Connexion invalide, veuillez ressayer");
                LaConnexion = await TentativeConnexion(getIp());
            }

            //Get le tuple du la méthode qui gere la connexion
            (bool play,int size) = await GestionTaille(LaConnexion);

            if (play)
            {
                int replay;
                do
                {
                    Gestion gestion = new Gestion();

                    await gestion.StartGame(size, LaConnexion);

                    replay = gestion.SaisirEntier("Rejouer? 1.Oui.  2.Non.", 1, 2);

                    await LaConnexion.EnvoyerConfirmatioon(replay);

                } while (replay != 2);
            }
 
        }

        static async Task<(bool,int)> GestionTaille(ConnexionClient LaConnexion)
        {
            //Recevoir la taille du serveur et accepter la taille du champ de bataille
            char Réponse;
            bool VérifChar;
            bool StartGame;


            int size = await LaConnexion.RecevoirTaille();

            do
            {
                Console.Clear();
                Console.WriteLine($"Le serveur souhaite jouer avec un grille de {size}x{size} acceptez-vous?\n Vous allez être deconnecté si vous refusez (y/n)");
                string rep = Console.ReadLine().ToLower();
                VérifChar = char.TryParse(rep, out Réponse);

            } while (!VérifChar || (Réponse != 'y' && Réponse != 'n'));

            //envoie la bonne réponse et et mets le bool a true ou false
            if (Réponse == 'y')
            {
                await LaConnexion.EnvoyerConf(true);
                StartGame = true;
            }
            else
            {
                await LaConnexion.EnvoyerConf(false);
                StartGame = false;
            }

            //Renvoie un tuple avec la bool et le int
            return (StartGame, size);   
        }
        static string getIp()
        {
            string adresseEntree;

            do
            {
                Console.Write("\nVeuillez entrer l'adresse de votre hote : ");
                adresseEntree = Console.ReadLine();
            }
            while (!IPAddress.TryParse(adresseEntree, out IPAddress i));

            Console.Clear();
            return adresseEntree;
        }

        static async Task<ConnexionClient?> TentativeConnexion(string ip)
        {
            ConnexionClient laCo = null;

            Console.WriteLine($"Tentative de conexion avec l'Ip {ip}");

            //Création de la connexion
            ConnexionClient laConnexion = new ConnexionClient(ip);

            //Tentative de connexion
            bool verif = await laConnexion.TryConnect();

            if (verif)
                laCo = laConnexion;

            return laCo;
        }
    }
}
