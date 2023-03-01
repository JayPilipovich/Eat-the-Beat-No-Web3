using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace FlowWords
{
    public class GuessResult
    {
        /// <summary>
        /// the guess that was submitted
        /// </summary>
        public string word;

        /// <summary>
        /// A 5 letter code indicating the result and resulting color a cell should be. 
        /// </summary>
        /// <remarks>
        /// "p" = the letter at this position was in the word and in the correct (p)osition, color the cell green.
        /// "w" = the letter at this position was in the (w)ord, but in the incorrect position, color the cell yellow.
        /// "n" = the letter at this position was (n)ot in the word.
        /// </remarks>
        public string colorMap;
    }

    /// <summary>
    /// Enum of the possible states the game can be in.
    /// </summary>
    public enum GameState
    {
        PLAYING,
        WON,
        LOST
    }

    /// <summary>
    /// Handles overall game state and transitions between different states.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = FindObjectOfType<GameManager>();
                }

                return m_instance;
            }
        }
        private static GameManager m_instance = null;

        private List<GuessResult> m_guessResults = new List<GuessResult>();
        private Dictionary<string, string> m_letterStatuses = new Dictionary<string, string>();

        public GameState CurrentGameState { get { return m_currentGameState; } }
        private GameState m_currentGameState;

        public double CurrentGameStartTime { get { return m_currentGameStartTimeUnix; } }
        private double m_currentGameStartTimeUnix;

        /// <summary>
        /// Starts the game.
        /// </summary>
        private void Start()
        {
            if (Instance != this)
            {
                Destroy(this);
                return;
            }

            //Set the starting state to be the LOGIN state.
            UIManager.Instance.SetUiState(UIState.LOGIN);
        }

        /// <summary>
        /// Starts the login coroutine
        /// </summary>
        /// <param name="username">Username the user would like to use</param>
        public void Login(string username)
        {
            FlowInterface.Instance.Login(username, OnLoginSuccess, OnLoginFailure);
        }

        /// <summary>
        /// Function called when login is successful
        /// </summary>
        /// <param name="username">The username chosen by the user</param>
        /// <param name="address">The user's Flow address</param>
        private void OnLoginSuccess(string username, string address)
        {
            /*UIManager.Instance.SetStatus("Login Success - Getting game state");

            NewGame();*/
            Debug.Log("username:" + username);
            Debug.Log("address:" + address);

            SceneManager.LoadScene(1);
        }

        /// <summary>
        /// Function called when login fails
        /// </summary>
        private void OnLoginFailure()
        {
            LogOut();
            UIManager.Instance.SetStatus("Login Failed.\nPlease check your credentials.");
        }


        /// <summary>
        /// Function called if starting a new game failed
        /// </summary>
        private void OnNewGameFailure()
        {
            LogOut();
            UIManager.Instance.SetStatus("Failed to get Game State from chain. Please try again.");
        }

        /// <summary>
        /// Logs out of the game.
        /// </summary>
        public void LogOut()
        {
            FlowInterface.Instance.Logout();
            UIManager.Instance.SetUiState(UIState.LOGIN);
        }
    }
}
