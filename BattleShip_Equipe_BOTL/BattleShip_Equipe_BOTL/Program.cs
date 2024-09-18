using BattleShip_Equipe_BOTL.Class;
using System;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace BattleShip_Equipe_BOTL
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            while (true)
            {
                //Déclaration des variables
                bool verifSize = true;
                bool verifReplay = false;
                int conf;
                Gestion gestion = new Gestion();

                //Init le serveur
                ConnexionServer laCo = await InitServ();

                //Ajouts de la descision de la taille par le serveur
                Console.Clear();
                int size = gestion.SaisirEntier("Entrer la longeur de la grille, de 4 à 10: ", 4, 10);

                //Envoie la taille
                await laCo.EnvoyerTaille(size);

                //Recoit la confirmation 
                bool confirmationTaille = await laCo.Recevoirconf();


                if (confirmationTaille)
                {
                    try
                    {
                        do
                        {
                            verifReplay = false;
                            await gestion.StartGame(laCo);
                            //todo verif le replay
                            conf = await laCo.RecevoirConfirmation();

                            if (conf == 1)
                                verifReplay = true;

                        } while (verifReplay);
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
    }
}
