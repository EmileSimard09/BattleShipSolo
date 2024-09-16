using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShip_Equipe_BOTL.Class
{
    public class Board
    {
        public int range { get; set; }

        public Case[] board { get; set; }
        public int[] boatRanges; //liste de tailles des bateaux
        public int nbreBoats = 1; //initialement a 1 mais pourra etre modifier pour augmenter le nombre de bateaux
        public int cptBoat;
        public Case[][] listBoat;
        public Gestion gestionnaire;

        public Board(int range)
        {
            this.range = range;
            board = new Case[range * range];
            for (int i = 0; i < range * range; i++)
            {
                board[i] = new Case(i + 1);
            }
            boatRanges = new int[1];
            listBoat = new Case[nbreBoats][];
            gestionnaire = new Gestion();
        }
        public void InitBoard()
        {

            SetBoats();
        }
        public int ChooseNbreBoat()
        {
            int nbreBoat = ChooseNbreUnderRange();
            cptBoat = 0;
            return nbreBoat;
        }
        ///
        public void SetBoats()
        {
            for (int i = 0; i < nbreBoats; i++)
            {
                //Console.WriteLine($"Taille du bateau : .{i + 1}");
                //boatRanges[i] = ChooseNbreUnderRange();
                boatRanges[i] = 2;
                CreateBoat(i + 1, boatRanges[i]);
            }
        }
        public int ChooseNbreUnderRange()
        {
            int nbre;
            do
            {
                string value = Console.ReadLine();
                bool success = int.TryParse(value, out nbre);
                if (success)
                {
                    if (nbre <= 0 || nbre > range)
                    {
                        nbre = -1;
                    }
                }
                else
                {
                    Console.WriteLine("Erreur, une valeur entiere est demamdée.");
                    nbre = -1;
                }
            }
            while (nbre == -1);
            return nbre;
        }
        public void CreateBoat(int id, int boatRange)
        {
            int idCase;
            int[] boatCases = new int[boatRange];
            bool possible;
            int cpt;
            int cptErreur = 0;
            //present dans le cas ou il est impossible de remplir le tableau (valeur a 10 afin de laisser la possibilite au joueur de se tromper un certain nombre de fois s'il est possible d.'ajouter un bateau.)
            do
            {
                cpt = 0;
                possible = true;
                do
                {
                    idCase = ChooseCase();
                    idCase = VerifCase(idCase);
                    if (idCase == -1)
                    {
                        Console.WriteLine("Erreur, la valeur choisie est incorrecte.");

                    }
                }
                while (idCase == -1);
                boatCases[cpt] = idCase - 1;
                cpt++;
                boatCases[cpt] = VerifCaseBoat(idCase);
                if (boatCases[cpt] == -1)
                {
                    Console.WriteLine("Erreur, la case ne possede aucune case disponible autour d'elle.");
                    possible = false;
                    cptErreur++;
                }
                else
                {
                    cpt++;
                    int nextCase = 2 * boatCases[0] - boatCases[1];
                    while (cpt < boatCases.Length || !possible)
                    {
                        if (VerifCase(nextCase) != -1 && IsWall(nextCase) != 1 && IsWall(nextCase) != 2)
                        {
                            boatCases[cpt] = nextCase;
                            cpt++;
                        }
                        else
                        {
                            possible = false;
                            cptErreur++;
                        }
                    }
                }
            }
            while (!possible && cptErreur < 10);
            if (!possible)
            {
                Console.WriteLine("Erreur, plus d'essai disponible ou impossibilite d'ajouter un bateau.");
                //todo : sortir
            }
            else
            {
                InitialisationBoat(boatCases, boatRange, GetListBoat());
                cptBoat++;
            }
        }

        public Case[][] GetListBoat()
        {
            return listBoat;
        }

        public void InitialisationBoat(int[] boatCases, int boatRange, Case[][] listBoat)
        {
            listBoat[cptBoat] = new Case[boatRange];
            for (int i = 0; i < boatCases.Length; i++)
            {
                listBoat[cptBoat][i] = board[boatCases[i]];
                board[boatCases[i]].isBoat = true;
            }
        }
        public int ChooseCase()
        {
            gestionnaire.ShowMyBoard(this);
            Console.WriteLine();
            Console.WriteLine("Choissez une case : ");
            int idCase;
            string value = Console.ReadLine();
            bool success = int.TryParse(value, out idCase);
            if (success)
            {
                return idCase;
            }
            else
            {
                Console.WriteLine("Erreur, une valeur entiere est demandée.");
            }
            return -1;
        }
        public int VerifCase(int idCase)
        {
            if (idCase <= 0 || idCase > (range * range) || board[idCase - 1].isBoat)
            {
                return -1;
            }
            return idCase;
        }
        public int VerifCaseBoat(int idCase)
        {
            int caseBoat;
            int[] possibleCases = PossibleCases(idCase);
            if (possibleCases.Length == 0)
            {
                return -1;
            }
            do
            {
                caseBoat = ChooseCase(possibleCases);

            }
            while (caseBoat == -1);
            return caseBoat-1;
        }
        public int ChooseCase(int[] possibleCases)
        {
            int[] choixCases;
            int idCase;
            Console.WriteLine("Choissez une case : ");
            choixCases = getListesCasespossible(possibleCases);
            for (int i = 0; i < choixCases.Length; i++)
            {

                Console.WriteLine($"{i + 1}. {choixCases[i]}");
            }
            Console.WriteLine("Choissez une valeur allant de 1 a " + $"{choixCases.Length} : ");
            string value = Console.ReadLine();
            bool success = int.TryParse(value, out idCase);
            if (success)
            {
                if (idCase <= 0 || idCase > choixCases.Length)
                {
                    Console.WriteLine("Erreur, la valeur choisie est incorrecte.");
                    return -1;
                }
                return choixCases[idCase - 1];
            }
            else
            {
                Console.WriteLine("Erreur, une valeur entiere est demamdée.");
            }
            return -1;

        }
        public int[] getListesCasespossible(int[] possibleCases)
        {
            int cpt = 0;
            int[] choixCases;
            for (int i = 0; i < possibleCases.Length; i++)
            {
                if (possibleCases[i] != 0)
                {
                    cpt++;
                }
            }
            choixCases = new int[cpt];
            cpt = 0;
            for (int i = 0; i < possibleCases.Length; i++)
            {
                if (possibleCases[i] != 0)
                {
                    choixCases[cpt] = possibleCases[i];
                    cpt++;
                }
            }
            return choixCases;
        }
        public int[] PossibleCases(int idCase)
        {
            int cpt = 0;
            int[] possibleCases = new int[4];
            int possiblecase = idCase - range;
            possiblecase = VerifCase(possiblecase);
            if (possiblecase != -1)
            {
                possibleCases[cpt] = possiblecase;
                cpt++;
            }
            possiblecase = idCase + range;
            possiblecase = VerifCase(possiblecase);
            if (possiblecase != -1)
            {
                possibleCases[cpt] = possiblecase;
                cpt++;

            }
            if (IsWall(idCase) == -1)
            {
                if (!board[idCase + 1].isBoat)
                {
                    possibleCases[cpt] = idCase + 1;
                }
            }
            else if (IsWall(idCase) == -2)
            {
                if (!board[idCase - 1].isBoat)
                {
                    possibleCases[cpt] = idCase - 1;
                }
            }
            else
            {
                if (!board[idCase - 1].isBoat)
                {
                    possibleCases[cpt] = idCase - 1;
                    cpt++;
                }
                if (!board[idCase + 1].isBoat)
                {
                    possibleCases[cpt] = idCase + 1;
                }
            }
            return possibleCases;
        }
        public int IsWall(int idCase)
        {
            if (idCase % range == 1)
            {
                return -1;
            }
            else if (idCase % range == 0)
            {
                return -2;
            }
            return idCase;
        }

        // retourne 1 si la case a un mur a gauche
        // retourne 2 si la case a un mur a droite
        // sinon retourne l'idCase

    }
}

// peut etre ajouter d'autres bateaux via des listes