using BattleShip_Equipe_BOTL_Client_;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShip_Equipe_BOTL.Class
{
    public class Gestion
    {
        Board alliedBoard;
        Board enemyBoard;
        Board oldAlliedBoard;
        Board oldEnemyBoard;
        bool verifGame = true;
        bool winner = true;
        bool lost = true;

        public async Task StartGame(int size, ConnexionClient connexion)
        {
            //TODO Refactor cette methode pour qu'elle soit moins longue
            while (verifGame)
            {
                alliedBoard = new Board(size);
                enemyBoard = new Board(size);
                oldAlliedBoard = new Board(size);
                oldEnemyBoard = new Board(size);

                //TODO Client: Placer bateau (modifier alliedBoard et oldAllied)
                alliedBoard.SetBoats();
                ShowMyBoard(alliedBoard);

                //TODO Client:Envoyer son board
                await connexion.Envoyer(alliedBoard);

                //Recevoir boat
                enemyBoard = await connexion.Recevoir();

                Array.Copy(alliedBoard.board, oldAlliedBoard.board, alliedBoard.board.Length);
                Array.Copy(enemyBoard.board, oldEnemyBoard.board, enemyBoard.board.Length);

                verifGame = false;
            }

            do
            {
                //Tour normal
                bool cheats = CheatCheck(alliedBoard, enemyBoard, oldAlliedBoard, oldEnemyBoard);
                //Check Winner
                //isWinner(alliedBoard)
                if (!cheats)
                {
                    lost = isWinner(alliedBoard);
                    if (!lost)
                    {
                        Console.Clear();
                        //Enregistrement des tableaux
                        Array.Copy(alliedBoard.board, oldAlliedBoard.board, alliedBoard.board.Length);
                        Array.Copy(enemyBoard.board, oldEnemyBoard.board, enemyBoard.board.Length);

                        //Affichage
                        Console.WriteLine("Océan allié");
                        ShowMyBoard(alliedBoard);
                        Console.WriteLine("");
                        Console.WriteLine("Océan ennemi");
                        ShowEnemyBoard(enemyBoard);

                        //Tir
                        Shoot(enemyBoard);
                        winner = isWinner(enemyBoard);
                        //TODO Si isWinner == false, send son board à l'ennemi.
                        if (!winner)
                        {
                            await connexion.Envoyer(enemyBoard);
                            alliedBoard = await connexion.Recevoir();
                        }
                        else
                        {
                            await connexion.Envoyer(enemyBoard);
                            Console.Clear();
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("You WIN!!!!!");
                            Console.ForegroundColor = ConsoleColor.White;
                            winner = true;
                            verifGame = true;
                        }
                    }
                    else
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("You Lost!!!!!");
                        Console.ForegroundColor = ConsoleColor.White;
                        winner = true;
                        verifGame = true;
                    }

                }
            }
            while (!winner);
        }

        /// <summary>
        /// Écriture du board en console
        /// </summary>
        /// <param name="board">Board à afficher</param>
        /// <param name="affichage">Board traduit en lettres</param>
        public void AfficherBoard(Board board, string[] affichage)
        {
            //Affichage de lignes
            for (int i = 0; i < board.range; i++)
            {
                //Affichage de la ligne horizontale supérieure
                for (int k = 0; k < board.range; k++)
                {
                    Console.Write("______");
                }
                Console.WriteLine();

                //affichage de chaque case de la ligne
                for (int j = 0; j < board.range; j++)
                {
                    int position = (i * board.range) + j;
                    Console.Write(" | " + affichage[position]);
                }
                Console.WriteLine(" | ");

            }

            //Affichage de la ligne horizontale finale
            for (int h = 0; h < board.range; h++)
            {
                Console.Write("______");
            }
        }

        /// <summary>
        /// Montre le board allié, avec son propre bateau
        /// </summary>
        /// <param name="board">Board allié (PAS celui qu'on tire dessus)</param>
        public void ShowMyBoard(Board board)
        {
            int nbCases = board.range * board.range;
            string[] affichage = new string[nbCases];
            Console.ForegroundColor = ConsoleColor.Green;
            //Traduction du board en symboles
            for (int i = 0; i < nbCases; i++)
            {
                Case currentCase = board.board[i];
                if (currentCase.isHit == false && currentCase.isBoat == false)
                {
                    int numCase = i + 1;
                    string stringCase = numCase.ToString();
                    //Rajout d'un zéro pour égaliser l'affichage
                    if (numCase < 10)
                        stringCase = "0" + stringCase;

                    affichage[i] = stringCase;
                }
                else if (currentCase.isHit == true && currentCase.isBoat == true)
                    affichage[i] = " X";
                else if (currentCase.isHit == true && currentCase.isBoat == false)
                    affichage[i] = " O";
                else
                    affichage[i] = " B";
            }
            AfficherBoard(board, affichage);
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Montre le board allié, avec son propre bateau
        /// </summary>
        /// <param name="board">Board ennemi (celui qu'on tire dessus)</param>
        public void ShowEnemyBoard(Board board)
        {
            int nbCases = board.range * board.range;
            string[] affichage = new string[nbCases];
            Console.ForegroundColor = ConsoleColor.Red;
            //Traduction du board en symboles
            for (int i = 0; i < nbCases; i++)
            {
                Case currentCase = board.board[i];
                if (currentCase.isHit == false)
                {
                    int numCase = i + 1;
                    string stringCase = numCase.ToString();
                    //Rajout d'un zéro pour égaliser l'affichage
                    if (numCase < 10)
                        stringCase = "0" + stringCase;

                    affichage[i] = stringCase;
                }
                else if (currentCase.isHit == true && currentCase.isBoat == true)
                    affichage[i] = " X";
                else if (currentCase.isHit == true && currentCase.isBoat == false)
                    affichage[i] = " O";
            }
            AfficherBoard(board, affichage);
            Console.ForegroundColor = ConsoleColor.White;
        }
        /// <summary>
        /// Vérifie un board pour voir si le joueur a gagné
        /// </summary>
        /// <param name="board">Le board à vérifier</param>
        /// <returns>true pour une vctoire</returns>
        public bool isWinner(Board board)
        {
            int nbCases = board.range * board.range;
            string[] affichage = new string[nbCases];
            bool win = true;

            for (int i = 0; i < nbCases; i++)
            {
                Case currentCase = board.board[i];
                if (currentCase.isHit == false && currentCase.isBoat == true)
                    win = false;
            }
            return win;
        }
        /// <summary>
        /// Seulement pour une paire de bateaux, appeler 2 fois
        /// </summary>
        /// <param name="currentBoard"></param>
        /// <param name="oldBoard"></param>
        /// <returns>false si aucune triche</returns>
        private bool DetectCheatBoard(Board currentBoard, Board oldBoard)
        {
            int nbCases = currentBoard.range * currentBoard.range;
            int changed = 0;

            for (int i = 0; i < nbCases; i++)
            {
                Case currentCase = currentBoard.board[i];
                if (currentCase.isBoat != oldBoard.board[i].isBoat)
                    changed += 2;
                if (currentCase.isHit == false && oldBoard.board[i].isHit == true)
                    changed += 2;
                if (currentCase.isHit != oldBoard.board[i].isHit)
                    changed += 1;
            }
            if (changed <= 1)
                return false;
            else
                return true;
        }
        /// <summary>
        /// Vérifie les deux boards
        /// </summary>
        /// <returns>false si aucune triche</returns>
        public bool CheatCheck(Board currentAlliedBoard, Board currentEnemyBoard, Board oldAlliedBoard, Board oldEnemyBoard)
        {
            bool cheat = false;
            if (DetectCheatBoard(currentAlliedBoard, oldAlliedBoard))
            {
                cheat = true;
                Console.WriteLine("Triche détectée sur le tableau ennemi!");
            }
            if (DetectCheatBoard(currentEnemyBoard, oldEnemyBoard))
            {
                cheat = true;
                Console.WriteLine("Triche détectée sur le tableau allié!");
            }
            return cheat;
        }
        /// <summary>
        /// Demande à l'utilisateur sa position de tir, vérifie, et affiche le board ennemi.
        /// </summary>
        /// <param name="board">Board ennemi</param>
        public void Shoot(Board board)
        {
            int nbCases = board.range * board.range;
            int caseTir = SaisirEntier("Entrer la case à bombarder : ", 1, nbCases);
            bool shotFired = false;

            do
            {
                if (board.board[caseTir - 1].isHit == true)
                {
                    Console.WriteLine("Case déjà bombardée. Entrer une autre case : ");
                    caseTir = SaisirEntier("Entrer la case à bombarder", 1, nbCases);
                }
                else if (board.board[caseTir - 1].isBoat == false)
                {
                    Console.WriteLine("Splash! Raté.");
                    board.board[caseTir - 1].isHit = true;
                    shotFired = true;
                }
                else
                {
                    Console.WriteLine("BOOM! Touché!");
                    board.board[caseTir - 1].isHit = true;
                    shotFired = true;
                }
            } while (!shotFired);
            ShowEnemyBoard(board);
        }
        /// <summary>
        /// vérifie que l'utilisateur entre un entier, min et max inclus
        /// </summary>
        /// <param name="message"></param>
        /// <param name="min">minimum inclus</param>
        /// <param name="max">maximum inclus</param>
        /// <returns></returns>
        public int SaisirEntier(string message, int min, int max)
        {
            Console.Write(message);
            int entier;
            while (int.TryParse(Console.ReadLine(), out entier) == false || entier < min || entier > max)
                Console.Write("Ceci n'est pas un entier valide. Réessayez : ");
            return entier;
        }


    }
}
