using System.Collections.Generic;
using UnityEngine;

namespace FlowWords
{
    /// <summary>
    /// Possible states the UI can be in
    /// </summary>
    public enum UIState
    {
        NONE,
        LOGIN,
    }

    /// <summary>
    /// Manages which panel is currently displayed.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("Game Panels")]
        [SerializeField] GameObject LoginPanelPrefab;


        private UIState m_currentUiState = UIState.NONE;
        private GameObject m_currentUiPanel = null;
        private Canvas m_canvas;

        public static UIManager Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = FindObjectOfType<UIManager>();
                }

                return m_instance;
            }
        }
        private static UIManager m_instance = null;

        private void Awake()
        {
            m_canvas = GetComponentInChildren<Canvas>();
        }

        private void Start()
        {
            if (Instance != this)
            {
                Destroy(this);
            }
        }

        /// <summary>
        /// Displays the correct panel depending on game state.
        /// </summary>
        /// <param name="state">The current state of the game</param>
        public void SetUiState(UIState state)
        {
            //Destroy the currently displayed panel.
            if (m_currentUiPanel != null)
            {
                Destroy(m_currentUiPanel);
            }

            m_currentUiState = state;

            //Create the desired panel.
            switch (m_currentUiState)
            {
                case UIState.NONE:
                    break;
                case UIState.LOGIN:
                    m_currentUiPanel = Instantiate(LoginPanelPrefab, m_canvas.transform);
                    break;
            }
        }


        /// <summary>
        /// Sets the status text of the appropriate game object depending on what the game state is
        /// </summary>
        /// <param name="status">The status text that should be displayed</param>
        public void SetStatus(string status)
        {
            switch (m_currentUiState)
            {
                case UIState.NONE:
                    break;
                case UIState.LOGIN:
                    m_currentUiPanel.GetComponent<LoginPanel>().SetStatus(status);
                    break;
            }
        }

    }
}
