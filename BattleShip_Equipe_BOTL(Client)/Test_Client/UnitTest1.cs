using BattleShip_Equipe_BOTL.Class;

namespace Test_serveur
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void TestTriche()
        {
            Gestion gestion = new Gestion();
            Board enemyBoard = new Board(4);
            Board oldEnemyBoard = new Board(4);
            Board alliedBoard = new Board(4);
            Board oldAllieBoard = new Board(4);

            oldEnemyBoard.board[1].isBoat = true;
            oldEnemyBoard.board[2].isBoat = true;
            oldEnemyBoard.board[5].isHit = true;

            enemyBoard.board[1].isBoat = true;
            enemyBoard.board[2].isBoat = true;
            enemyBoard.board[5].isHit = true;
            enemyBoard.board[8].isHit = true;
            enemyBoard.board[13].isHit = true;
            enemyBoard.board[15].isHit = true;

            Assert.IsTrue(gestion.CheatCheck(alliedBoard, enemyBoard, oldAllieBoard, oldEnemyBoard));
        }

        [TestMethod]
        public void TestNonTriche()
        {
            Gestion gestion = new Gestion();
            Board enemyBoard = new Board(4);
            Board oldEnemyBoard = new Board(4);
            Board alliedBoard = new Board(4);
            Board oldAllieBoard = new Board(4);

            oldEnemyBoard.board[1].isBoat = true;
            oldEnemyBoard.board[2].isBoat = true;
            oldEnemyBoard.board[5].isHit = true;

            enemyBoard.board[1].isBoat = true;
            enemyBoard.board[2].isBoat = true;
            enemyBoard.board[5].isHit = true;
            enemyBoard.board[8].isHit = true;

            Assert.IsFalse(gestion.CheatCheck(alliedBoard, enemyBoard, oldAllieBoard, oldEnemyBoard));
        }

        [TestMethod]
        public void TestVictoire()
        {
            Gestion gestion = new Gestion();
            Board enemyBoard = new Board(4);
            Board oldEnemyBoard = new Board(4);
            Board alliedBoard = new Board(4);
            Board oldAllieBoard = new Board(4);

            enemyBoard.board[1].isBoat = true;
            enemyBoard.board[2].isBoat = true;
            enemyBoard.board[1].isHit = true;
            enemyBoard.board[2].isHit = true;

            Assert.IsTrue(gestion.isWinner(enemyBoard));
        }

        [TestMethod]
        public void TestNonVictoire()
        {
            Gestion gestion = new Gestion();
            Board enemyBoard = new Board(4);
            Board oldEnemyBoard = new Board(4);
            Board alliedBoard = new Board(4);
            Board oldAllieBoard = new Board(4);

            enemyBoard.board[1].isBoat = true;
            enemyBoard.board[2].isBoat = true;

            Assert.IsFalse(gestion.isWinner(enemyBoard));
        }
    }
}