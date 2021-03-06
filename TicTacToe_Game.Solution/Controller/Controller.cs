﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TicTacToe_Game
{
    public class GameController
    {
        #region FIELDS
        //
        // track game and round status
        //
        private bool _playingGame;
        private bool _playingRound;

        private int _roundNumber;

        //
        // track the results of multiple rounds
        //
        private int _playerXNumberOfWins;
        private int _playerONumberOfWins;
        private int _numberOfCatsGames;

        //
        // instantiate a Gameboard object
        // instantiate a GameView object and give it access to the Gameboard object
        // instantiate a Menu object
        //
        private static Gameboard _gameboard = new Gameboard();
        private static ConsoleView _gameView = new ConsoleView(_gameboard);
        private static Menu _gameMenu = new Menu();

        #endregion

        #region PROPERTIES



        #endregion

        #region CONSTRUCTORS

        public GameController()
        {
            InitializeGame();
            ManageApplicationLoop();
        }
        
        #endregion

        #region METHODS

        /// <summary>
        /// Initialize the multi-round game.
        /// </summary>
        public void InitializeGame()
        {
            //
            // Initialize game variables
            //
            _playingGame = true;
            _playingRound = false;
            _roundNumber = 0;
            _playerONumberOfWins = 0;
            _playerXNumberOfWins = 0;
            _numberOfCatsGames = 0;

            //
            // Initialize game board status
            //
            _gameboard.InitializeGameboard();

            //
            // add the event handler for a user choosing to quit during a game
            //
            _gameView.UserQuit += HandleUserQuit;
        }

         /// <summary>
        /// method called by the UserQuit event 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="e"></param>
        private void HandleUserQuit(object o, EventArgs e)
        {
            _playingGame = false;
            QuitGame();
        }

        /// <summary>
        /// method to manage the application setup and application loop
        /// </summary>
        private void ManageApplicationLoop()
        {
      
            //
            // display welcome screen
            //
            _playingGame = _gameView.DisplayWelcomeScreen();

            Console.Clear();

            //
            // player chooses to quit
            //
            if (!_playingGame)
            {
                QuitGame();
            }
            else
            {

                //
                // game loop
                //
                while (_playingGame)
                {

                    //
                    // get next menu choice from player
                    //
                    MenuOption menuChoice = GetPlayerMenuChoice(_gameMenu);

                    //
                    // choose an action based on the player's menu choice
                    //
                    switch (menuChoice)
                    {
                        case MenuOption.None:
                            break;
                        case MenuOption.PlayNewRound:
                            _playingRound = true;
                            PlayGame();
                            break;
                        case MenuOption.ViewGameRules:
                            _gameView.DisplayGameRules();
                            break;
                        case MenuOption.ViewCurrentGameResults:
                            _gameView.DisplayCurrentGameStatus(_roundNumber, _playerXNumberOfWins, _playerONumberOfWins, _numberOfCatsGames);
                            break;
                        //case MenuOption.ViewPastGameResultsScores:
                        //    _gameView.DisplayCurrentGameStatus(_roundNumber, _playerXNumberOfWins, _playerONumberOfWins, _numberOfCatsGames);
                        //    break;
                        //case MenuOption.SaveGameResults:
                        //    _gameView.DisplaySaveGameScreen();
                        //    break;
                        case MenuOption.Quit:
                            _playingGame = false;
                            break;                        
                        default:
                            break;
                    }
                }

                //
                // close the application
                //
                QuitGame();
            }
        }

        /// <summary>
        /// Game Loop
        /// </summary>
        public void PlayGame()
        {
            //
            // initialize gameboard 
            //
            if (_roundNumber > 0)
            {
                _gameboard.InitializeGameboard();
            }

            //
            // Round loop
            //
            while (_playingRound)
                {
                    //
                    // Perform the task associated with the current game and round state
                    //
                    ManageGameStateTasks();

                    //
                    // Evaluate and update the current game board state
                    //
                    _gameboard.UpdateGameboardState();
                }

                //
                // Round Complete: Display the results
                //
                if (_gameView.CurrentViewState != ConsoleView.ViewState.PlayerQuit)
                {
                     _gameView.DisplayCurrentGameStatus(_roundNumber, _playerXNumberOfWins, _playerONumberOfWins, _numberOfCatsGames);
                }
                //
                // Confirm no major user errors
                //
                if (_gameView.CurrentViewState != ConsoleView.ViewState.PlayerUsedMaxAttempts &&
                    _gameView.CurrentViewState != ConsoleView.ViewState.PlayerTimedOut &&
                    _gameView.CurrentViewState != ConsoleView.ViewState.PlayerQuit)
                {
                    //
                    // Prompt user to play another round
                    //
                    if (_gameView.DisplayNewRoundPrompt())
                    {
                        _gameboard.InitializeGameboard();
                        _gameView.InitializeView();
                        _playingRound = true;
                        PlayGame();
                    }
                    else
                    {
                    _playingRound = false;

                    }
                }
                //
                // Major user error recorded, end game
                //
                else
                {
                    _playingRound = false;

                }
        }

        /// <summary>
        /// Set the game token of player one
        /// </summary>
        private void SetPlayerOne()
        {
            switch (_gameView.GetPlayerOne())
            {
                case 'X':
                    _gameboard.CurrentRoundState = Gameboard.GameboardState.PlayerXTurn;
                    break;
                case 'O':
                    _gameboard.CurrentRoundState = Gameboard.GameboardState.PlayerOTurn;
                    break;
                case 'R':
                    Random rnd = new Random();

                    int nbr = rnd.Next(1, 100);

                    if (nbr % 2 == 0)
                    {
                        _gameboard.CurrentRoundState = Gameboard.GameboardState.PlayerOTurn;
                    }
                    else
                    {
                        _gameboard.CurrentRoundState = Gameboard.GameboardState.PlayerXTurn;
                    }
                    break;
                default:
                    _gameboard.CurrentRoundState = Gameboard.GameboardState.PlayerXTurn;
                    break;
            }
        }


        /// <summary>
        /// manage each new task based on the current game state
        /// </summary>
        private void ManageGameStateTasks()
        {
            switch (_gameView.CurrentViewState)
            {
                case ConsoleView.ViewState.Active:
                    _gameView.DisplayGameArea();

                    switch (_gameboard.CurrentRoundState)
                    {
                        case Gameboard.GameboardState.NewRound:
                            _roundNumber++;

                            SetPlayerOne();
                            break;

                        case Gameboard.GameboardState.PlayerXTurn:
                            ManagePlayerTurn(Gameboard.PlayerPiece.X);
                            break;

                        case Gameboard.GameboardState.PlayerOTurn:
                            ManagePlayerTurn(Gameboard.PlayerPiece.O);
                            break;

                        case Gameboard.GameboardState.PlayerXWin:
                            _playerXNumberOfWins++;
                            _playingRound = false;
                            break;

                        case Gameboard.GameboardState.PlayerOWin:
                            _playerONumberOfWins++;
                            _playingRound = false;
                            break;

                        case Gameboard.GameboardState.CatsGame:
                            _numberOfCatsGames++;
                            _playingRound = false;
                            break;

                        default:
                            break;
                    }
                    break;
                case ConsoleView.ViewState.PlayerTimedOut:
                    _gameView.DisplayTimedOutScreen();
                    _playingRound = false;
                    break;
                case ConsoleView.ViewState.PlayerUsedMaxAttempts:
                    _gameView.DisplayMaxAttemptsReachedScreen();
                    _playingRound = false;
                    _playingGame = false;
                    break;
                case ConsoleView.ViewState.PlayerQuit:
                    _playingRound = false;
                    _playingGame = false;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Attempt to get a valid player move. 
        /// If the player chooses a location that is taken, the CurrentRoundState remains unchanged,
        /// the player is given a message indicating so, and the game loop is cycled to allow the player
        /// to make a new choice.
        /// </summary>
        /// <param name="currentPlayerPiece">identify as either the X or O player</param>
        private void ManagePlayerTurn(Gameboard.PlayerPiece currentPlayerPiece)
        {
            if(_gameView.CurrentViewState != ConsoleView.ViewState.PlayerQuit)
            {
            GameboardPosition gameboardPosition = _gameView.GetPlayerPositionChoice();

                if (_gameView.CurrentViewState == ConsoleView.ViewState.Active)
                {
                    //
                    // player chose an open position on the game board, add it to the game board
                    //
                    if (_gameboard.GameboardPositionAvailable(gameboardPosition))
                    {
                        _gameboard.SetPlayerPiece(gameboardPosition, currentPlayerPiece);
                    }
                    //
                    // player chose a taken position on the game board
                    //
                    else
                    {
                        _gameView.DisplayGamePositionChoiceNotAvailableScreen();
                    }
                }
            }
        }

        /// <summary>
        /// quit the game
        /// </summary>
        private void QuitGame()
        {
            _playingGame = false;
            _gameView.DisplayExitScreen();
            Timer timer = new Timer(1750);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        /// <summary>
        /// event handler for timer elapse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Environment.Exit(1);
        }

        /// <summary>
        /// returns a player action, based on the currentMenu variable
        /// </summary>
        /// <returns>PlayerAction</returns>
        private MenuOption GetPlayerMenuChoice(Menu gameMenu)
        {
            MenuOption playerMenuChoice = MenuOption.None;

            playerMenuChoice = _gameView.GetMenuChoice(gameMenu);

            return playerMenuChoice;
        }

        #endregion
    }
}