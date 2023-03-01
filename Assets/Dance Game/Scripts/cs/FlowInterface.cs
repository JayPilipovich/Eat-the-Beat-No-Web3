using DapperLabs.Flow.Sdk;
using DapperLabs.Flow.Sdk.Cadence;
using DapperLabs.Flow.Sdk.DataObjects;
using DapperLabs.Flow.Sdk.Unity;
using DapperLabs.Flow.Sdk.DevWallet;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace FlowWords
{
    public class FlowInterface : MonoBehaviour
    {
        // FLOW account object - set via Login screen.
        [Header("FLOW Account")]
        public FlowControl.Account FLOW_ACCOUNT = null;

        // The TextAssets containing Cadence scripts and transactions that will be used for the game.
        [Header("Scripts and Transactions")]
        [SerializeField] TextAsset loginTxn;
        /*[SerializeField] TextAsset getCurrentGameStateTxn;
        [SerializeField] TextAsset checkWordScript;
        [SerializeField] TextAsset submitGuessTxn;*/

        // Cadence scripts to get the data needed to display the High Scores panel
        /*[Header("Highscore Scripts")]
        [SerializeField] TextAsset GetHighScores;
        [SerializeField] TextAsset GetPlayerCumulativeScore;
        [SerializeField] TextAsset GetPlayerWinningStreak;
        [SerializeField] TextAsset GetPlayerMaxWinningStreak;
        [SerializeField] TextAsset GetGuessDistribution; */

        private static FlowInterface m_instance = null;
        public static FlowInterface Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = FindObjectOfType<FlowInterface>();
                }

                return m_instance;
            }
        }

        private void Start()
        {
            if (Instance != this)
            {
                Destroy(this);
            }

            // Register DevWallet
            FlowSDK.RegisterWalletProvider(ScriptableObject.CreateInstance<DevWalletProvider>());
        }

        /// <summary>
        /// Attempts to log in by executing a transaction using the provided credentials
        /// </summary>
        /// <param name="username">An arbitrary username the player would like to be known by on the leaderboards</param>
        /// <param name="onSuccessCallback">Function that should be called when login is successful</param>
        /// <param name="onFailureCallback">Function that should be called when login fails</param>
        public void Login(string username, System.Action<string, string> onSuccessCallback, System.Action onFailureCallback)
        {
            // Authenticate an account with DevWallet
            FlowSDK.GetWalletProvider().Authenticate("", // blank string will show list of accounts from Accounts tab of Flow Control Window
                                                    (string flowAddress) => StartCoroutine(OnAuthSuccess(username, flowAddress, onSuccessCallback, onFailureCallback)), 
                                                    onFailureCallback);
        }

        /// <summary>
        /// Success callback for Wallet Provider's Authenticate method. 
        /// </summary>
        /// <param name="username">The name that the user has provided (for leaderboard)</param>
        /// <param name="flowAddress">The address of the authenticated Flow Account</param>
        /// <param name="onSuccessCallback">Game callback for successful login</param>
        /// <param name="onFailureCallback">Game callback for failed login</param>
        /// <returns></returns>
        private IEnumerator OnAuthSuccess(string username, string flowAddress, System.Action<string, string> onSuccessCallback, System.Action onFailureCallback)
        {
            // get flow account
            FLOW_ACCOUNT = FlowControl.Data.Accounts.FirstOrDefault(x => x.AccountConfig["Address"] == flowAddress);
            
            // execute log in transaction on chain
            Task<FlowTransactionResult> task = FLOW_ACCOUNT.SubmitAndWaitUntilExecuted(loginTxn.text, new CadenceString(username));
            while (!task.IsCompleted)
            {
                int dots = ((int)(Time.time * 2.0f) % 4);
                UIManager.Instance.SetStatus("Connecting" + new string('.', dots));
                yield return null;
            }

            // check for error. if there was an error, break.
            if (task.Result.Error != null || task.Result.ErrorMessage != string.Empty || task.Result.Status == FlowTransactionStatus.EXPIRED)
            {
                onFailureCallback();
                yield break;
            }

            // login successful!
            onSuccessCallback(username, flowAddress);
        }

        /// <summary>
        /// Clear the FLOW account object
        /// </summary>
        internal void Logout()
        {
            FLOW_ACCOUNT = null;
            FlowSDK.GetWalletProvider().Unauthenticate();
        }

    }
}
