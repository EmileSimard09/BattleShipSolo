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
                ConnexionServer laCo = await InitServ();

                bool verifReplay = false;
                int conf;

                Gestion gestion = new Gestion();
                int replay;
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
