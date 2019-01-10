using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OthelloMillenniumClient
{
    /// <summary>
    /// Classe wrapper entre l'interface de jeu et la partie communication avec le serveur
    /// </summary>
    class GameHandler
    {
        #region Internal class
        private static GameHandler instance = new GameHandler();

        public enum GameType
        {
            HumanVsHuman = 0,
            HumanVsAI = 1,
            AIVsAI = 2,
        }

        #endregion

        #region Properties
        public static GameHandler Instance { get => instance; private set; }

        public bool SinglePlayer { get; private set; }

        #endregion

        #region Attributes

        #endregion

        private GameHandler()
        {

        }

        public void StartNewGame(bool online, GameType gameType)
        {
            //TODO

            //If local -> start_multiplayer

            // SEP 1
            //Modes:
            // SinglePlayer
            // SinglePlayer online
            // SinglePlayer local with AI XX
            // SinglePlayer online with AI YY

            // Human VS AI
            // SinglePlayer online vs AI
            // SinglePlayer local with AI XX

            // MultiPlayer
            // Multiplayer online
            // Multiplayer local

            // AI battle
            // AI vs AI online
            // AI vs AI local
            // Play online as an AI YY

            // SEP 2
            //Modes
            //Local
            // Human vs Human
            // Human vs AI
            // AI vs AI

            //Online
            //SinglePlayer
            // Human vs online Human
            // Human vs online AI
            // AI vs online AI
            //MultiPlayer -> Si on veut pouvoir diffuser les matchs
            // Human vs local Human
            // Human vs local AI
            // AI vs local AI

            if (online)
            {
                StartOnlineGame();
            }
            else
            {
                StartLocalGame();
            }
        }

        private void StartOnlineGame()
        {

        }

        private void StartLocalGame()
        {

        }
    }
}
