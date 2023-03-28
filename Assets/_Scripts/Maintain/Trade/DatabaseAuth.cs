/*using UnityEngine;
using Firebase;
using Firebase.Auth;
using System.Collections;
using Proyecto26;
using Playstel;
using System.Collections.Generic;   
using System.Threading.Tasks;
using Firebase.Database;
using PlayFab;
using PlayFab.ClientModels;

namespace NukeFactory
{
    public class DatabaseAuth : MonoBehaviour
    {
        private DependencyStatus dependencyStatus;
        private FirebaseAuth auth;
        private FirebaseUser user;

        class SignResponse
        {
            public string idToken;
            public string refreshToken;
            public string expiresIn;
        }

        [ContextMenu("Cloud Script")]
        public void CloudScriptTest()
        {
            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = "helloWorld",

                GeneratePlayStreamEvent = true,

            }, OnCloudScript, OnCloudScriptError);
        }

        private void OnCloudScript(ExecuteCloudScriptResult result)
        {
            Debug.Log("OnCloudScript: " + result.FunctionResult);
        }

        private void OnCloudScriptError(PlayFabError obj)
        {
            Debug.Log("OnCloudScriptError: " + obj.ErrorMessage);
        }

        void Awake()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    InitializeFirebase();
                }
                else
                {
                    Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
                }
            });
        }

        private void InitializeFirebase()
        {
            auth = FirebaseAuth.DefaultInstance;
        }

        [ContextMenu("Anon Auth")]
        public void Auth()
        {
            //StartCoroutine(AuthProcess());
        }

        private void AuthProcess()
        {
            var signInTask = auth.SignInAnonymouslyAsync();

            yield return new WaitUntil(() => signInTask.IsCompleted);

            if(signInTask.Exception == null)
            {
                user = signInTask.Result;

                Debug.LogFormat("User signed in successfully| Anon: {0} | Id: {1})", 
                    user.IsAnonymous, user.UserId);

                var setValueTask = FirebaseDatabase.DefaultInstance.GetReference
                    ($"users/{signInTask.Result.UserId}/nickname").SetValueAsync("Player1");

                yield return new WaitUntil(() => setValueTask.IsCompleted);

                Debug.Log("SetValueTask");

            }
            else
            {
                ExceptionInfo(signInTask.Exception.GetBaseException() as FirebaseException);
            }
        }

        string verifyCustomToken = "https://identitytoolkit.googleapis.com/v1/accounts:signInWithCustomToken?key=";
        private IEnumerator GetSecureToken(string uid)
        {
            Debug.Log("GetSecureToken");

            Dictionary<string, string> args = new Dictionary<string, string>()
            {
                {"uid", uid},
            };

            CloudFunction.func.ExecuteRequest(CloudFunction.Functions.CreateCustomToken, args);

            yield return new WaitUntil(() => !CloudFunction.func.executing);

            var customToken = CloudFunction.func.GetRequestResult();

            if(customToken == null)
            {
                Debug.LogError("Custom Token is null");
                yield break;
            }

            Debug.Log("Custom Token: " + customToken);

            string userData = "{\"token\":\"" + customToken.ToString() + "\",\"returnSecureToken\":true}";

            RestClient.Post<SignResponse>(verifyCustomToken + apiKey, userData).Then(
                response =>
                {
                    secureToken = response.idToken;

                    Debug.LogFormat("secureToken: {0})", secureToken);

                }).Catch(error =>
                {
                    Debug.Log(error);
                });
        }

        string signInAnonymosly = "https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=";
        string apiKey = "AIzaSyAaR0h9cjbt2uP2Bw39ldbv3uOB0QY6z54";

        [Header("Auth Info")]
        public string secureToken;
        public string localId;

        private void SignInAnonymosly()
        {
            string userData = "{\"returnSecureToken\":true}";

            RestClient.Post<SignResponse>(signInAnonymosly + apiKey, userData).Then(
                response =>
                {
                    secureToken = response.idToken;

                    Debug.LogFormat("User signed in successfully| LocalId: {0} | IdToken: {1})",
                        localId, secureToken);

                }).Catch(error =>
                {
                    Debug.Log(error);
                });
        }


        [ContextMenu("Sign Out")]
        private void SignOut()
        {
            auth.SignOut();
        }

        private void ExceptionInfo(FirebaseException exception)
        {
            AuthError errorCode = (AuthError)exception.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }

            Debug.LogError("Error: " + message);
        }
    }
}*/
