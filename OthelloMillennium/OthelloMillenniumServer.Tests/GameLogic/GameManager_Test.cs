using NUnit.Framework;

namespace OthelloMillenniumServer.Tests.GameLogic
{
    [TestFixture]
    class GameManager_Test
    {
        [Test]
        public static void TestGameManager()
        {
            Settings.SIZE_HEIGHT = 8;
            Settings.SIZE_WIDTH = 8;

            GameManager gameManager = new GameManager(GameManager.GameType.SinglePlayer);

            string sequence1 = "D3E3F4G3F3C5H3F2C4C3E2E1B3H4H5A3";
            bool playerOne = true;
            for(int i=0;i< sequence1.Length; i += 2)
            {
                gameManager.PlayMove((sequence1[i], (int) char.GetNumericValue(sequence1[i+1])), playerOne);
                playerOne = !playerOne;
            }

            GameBoard gameState = gameManager.GetGameState();

            Assert.IsTrue(gameState.IsGameEnded(), "Sequence 1 OK");
        }
    }
}
