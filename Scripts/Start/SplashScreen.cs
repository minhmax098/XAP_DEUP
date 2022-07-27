using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.Networking;
using EasyUI.Toast;
public class SplashScreen : MonoBehaviour
{
    void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait; 
        SetOrganCache();
    }

    async void SetOrganCache()
    {
        try
        {
            APIResponse<List<ListOrganLesson>> organsResponse = await UnityHttpClient.CallAPI<APIResponse<List<ListOrganLesson>>>(APIUrlConfig.GET_ORGAN_LIST, UnityWebRequest.kHttpVerbGET);
            if (organsResponse.code == APIUrlConfig.SUCCESS_RESPONSE_CODE)
            {
                OrganCaching.organList = organsResponse.data;
                StartCoroutine(LoadMainScene());
            }
            else
            {
                throw new Exception(organsResponse.message);
            }
        }
        catch (Exception e)
        {
            Toast.Show(e.Message);
        }
    }

    IEnumerator LoadMainScene ()
    {
        string nextSceneName = string.IsNullOrEmpty(PlayerPrefs.GetString(PlayerPrefConfig.userToken)) ? SceneConfig.home_nosignin : SceneConfig.home_user;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextSceneName);
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
