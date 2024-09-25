using BattleShip_Equipe_BOTL.Class;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace BattleShip_Equipe_BOTL
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            while (true)
            {
                //Déclaration des variables
                bool verifReplay = false;
                bool continueGame = true;

                Gestion gestion = new Gestion();

                //Init le serveur
                ConnexionServer laCo = await InitServ();

                if (await GestionTaille(laCo,gestion))
                {
                    try
                    {
                        do
                        {
                            bool readyToStart = false;

                            // Wait for client to be ready
                            while (!readyToStart)
                            {
                                int clientStatus = await laCo.RecevoirConfirmation();
                                if (clientStatus == 1)
                                {
                                    readyToStart = true;
                                    await laCo.EnvoyerTaille(1);
                                }
                            }

                            // Start the game
                            await gestion.StartGame(laCo);

                            
                            int confirm = await laCo.RecevoirConfirmation();
                            continueGame = (confirm == 1);

                            
                            await laCo.EnvoyerTaille(continueGame ? 1 : 2);

                        } while (continueGame);
                    }
                    catch (Exception ex)
                    {
                        //Gere la connexion perdu
                        Console.WriteLine("Erreur de connexion");
                    }
                }

                await laCo.FermerLaCo();
                Console.Clear();
            }
        }

        async static Task<ConnexionServer> InitServ()
        {
            ConnexionServer connexionServer = new ConnexionServer();
            Console.WriteLine("Attente de connexion sur le port 46000");
            await connexionServer.Initialize();
            Console.WriteLine("The devise is connected");
            return connexionServer;
        }

        async static Task<bool> GestionTaille(ConnexionServer laCo, Gestion gestion)
        {
            //Ajouts de la descision de la taille par le serveur
            Console.Clear();
            int size = gestion.SaisirEntier("Entrer la longeur de la grille, de 4 à 10: ", 4, 10);

            //Envoie la taille
            await laCo.EnvoyerTaille(size);

            //Recoit la confirmation 
            return await laCo.Recevoirconf();
        }
    }
}
