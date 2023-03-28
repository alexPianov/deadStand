using UnityEngine;
using PlayFab.CloudScriptModels;
using PlayFab;

namespace Playstel
{
    public class CloudFunction : MonoBehaviour
    {
        public static CloudFunction func;

        [Header("Status")]
        public bool executing;

        #region Singleton

        public void Awake()
        {
            if (!func)
            {
                func = this;
            }
            else
            {
                if (func == this)
                {
                    Debug.Log("Destroy Cloud Function");
                    Destroy(func.gameObject);
                    func = this;
                }
            }

            DontDestroyOnLoad(gameObject);
        }

        #endregion

        public enum Functions
        {
            SetToDB, GetFromDB, CreateCustomToken, DeleteFromDB, GetAllFromDB
        }

        public void ExecuteRequest(Functions function, object args)
        {
            requestResult = null;
            executing = true;

            Debug.Log("Execute Request: " + function.ToString());

            var request = new ExecuteFunctionRequest()
            {
                FunctionName = function.ToString(),
                FunctionParameter = args
            };

            PlayFabCloudScriptAPI.ExecuteFunction(request,
                (result) =>
                {
                    executing = false;

                    Debug.Log("Result: " + result.FunctionResult.ToString());

                    requestResult = result.FunctionResult;
                }, 
                (error) =>
                {
                    executing = false;

                    Debug.LogError("Error: " + error.ErrorMessage.ToString() + " | Code: " + error.HttpCode);
                });
        }

        object requestResult;
        public object GetRequestResult()
        {
            return requestResult;
        }
    }
}
