using NUnit.Framework;
using Tools;
using Tools.Properties;

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

            GameManager gameManager = new GameManager(BattleType.AgainstAI);

            string sequence1 = "D3E3F4G3F3C5H3F2C4C3E2E1B3H4H5A3";
            for(int i=0;i< sequence1.Length; i += 2)
            {
                var player = i % 2 == 0 ? Color.Black: Color.White;
                var coords = new System.Tuple<char, int>(sequence1[i], (int)char.GetNumericValue(sequence1[i + 1]));
                gameManager.PlayMove(coords, player);
            }

            Tools.GameState gameState = gameManager.Export();

            Assert.IsTrue(gameState.GameEnded, "Sequence 1 OK");
        }
    }
}
