# ExpertProjet1

## Demande 1 - Gestion de la taille au début de la connexion

### Ajouts

Ajouts des méthodes suivants dans les classes de connexion pur faciliter l'envoie de confirmations (bool) et de taille (int)

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

Ensuite, j'ai enlevé l'ancien code qui permettais de setter la taille sur le serveur:

                int size = gestion.SaisirEntier("Entrer la longeur de la grille, de 4 à 10: ", 4, 10);

je l'ai remplacé par ces méthodes qui sont executés avant le jeux dans le client el le serveur, les voici :

Client:
        
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

Serveur

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

## Demande 3 - Loop de l'attaque si le joueur touche

Changement de la méthode Shoot pour qu'elle retoure un void au lieu de void

        public bool Shoot(Board board)
        {
            int nbCases = board.range * board.range;
            int caseTir = SaisirEntier("Entrer la case à bombarder : ", 1, nbCases);
            bool shotFired = false;
            bool Touché = false;

            do
            {
                if (board.board[caseTir - 1].isHit == true)
                {
                    Console.WriteLine();
                    Console.WriteLine("Case déjà bombardée. Entrer une autre case : ");
                    caseTir = SaisirEntier("Entrer la case à bombarder", 1, nbCases);
                }
                else if (board.board[caseTir - 1].isBoat == false)
                {
                    Console.WriteLine();
                    Console.WriteLine("Splash! Raté.");
                    board.board[caseTir - 1].isHit = true;
                    shotFired = true;
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("BOOM! Touché!");
                    Console.WriteLine("Vous allez pouvoir retirer...");
                    board.board[caseTir - 1].isHit = true;
                    shotFired = true;
                    Touché = true;
                }
            } while (!shotFired);
            ShowEnemyBoard(board);
            return Touché;
        }