using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Text;
using System.IO;
using UnityEngine.UI;
using System.Threading.Tasks;

namespace BuildLesson 
{
    public class LoadDataListItemPanel : MonoBehaviour
    {
        private static LoadDataListItemPanel instance;
        public static LoadDataListItemPanel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<LoadDataListItemPanel>();
                }
                return instance; 
            }
        }
        public GameObject lessonInfoPanel;
        private int calculatedSize = 30;
        public Button btnDeleteAudio;
        public Button btnDeleteVideo;
        public GameObject showListItem;

        // panel PopUpDeleteActions
        public GameObject panelPopUpDeleteActions;
        public Button btnExitPopupDeleteActions;
        public Button btnDeleteActions;
        public Button btnCancelDeleteActions; 

        // panel PopUpDeleteActionsVideo
        public GameObject panelPopUpDeleteActionsVideo;
        public Button btnExitPopupDeleteActionsVideo;
        public Button btnDeleteActionsVideo;
        public Button btnCancelDeleteActionsVideo; 

        void Update()
        {
            btnExitPopupDeleteActions.onClick.AddListener(HandlerExitPopupDeleteActions);
            btnCancelDeleteActions.onClick.AddListener(HandlerCancelDeleteActions);
            btnExitPopupDeleteActionsVideo.onClick.AddListener(HandlerExitPopupDeleteActionsVideo);
            btnCancelDeleteActionsVideo.onClick.AddListener(HandlerCancelDeleteActionsVideo);
        }
        
        void HandlerExitPopupDeleteActions()
        {
            panelPopUpDeleteActions.SetActive(false);
        }
        void HandlerCancelDeleteActions()
        {
            panelPopUpDeleteActions.SetActive(false);
        }
        void HandlerExitPopupDeleteActionsVideo()
        {
            panelPopUpDeleteActionsVideo.SetActive(false);
        }
        void HandlerCancelDeleteActionsVideo()
        {
            panelPopUpDeleteActionsVideo.SetActive(false); 
        }

        // Update the Pannel 
        public async Task UpdateLessonInforPannel(int lessonId)
        {
            Debug.Log("Update lesson info pannel ...");
            try
            {
                APIResponse<List<LessonDetail>> lessonDetailResponse = await UnityHttpClient.CallAPI<APIResponse<List<LessonDetail>>>(String.Format(APIUrlConfig.GET_LESSON_BY_ID, lessonId), UnityWebRequest.kHttpVerbGET); 
                if (lessonDetailResponse.code  == APIUrlConfig.SUCCESS_RESPONSE_CODE)
                {
                    // Use static class to store 
                    StaticLesson.SetValueForStaticLesson(lessonDetailResponse.data[0]);
                    Debug.Log("Update lesson info pannel Get lesson info: " + StaticLesson.LessonTitle);
                    Debug.Log("Update lesson info pannel Get lesson info: " + StaticLesson.Audio);
                    
                    // Refresh the item 
                    foreach (Transform child in lessonInfoPanel.transform)
                    {
                        GameObject.Destroy(child.gameObject);
                    }

                    // Load all infomation from the api result 
                    if (StaticLesson.Audio != "")
                    {
                        loadAudio(StaticLesson.Audio, lessonId);
                    }
                    if (StaticLesson.Video != "")
                    {
                        StartCoroutine(loadVideo(StaticLesson.Video, lessonId));
                    } 

                    if (StaticLesson.ListLabel.Length > 0) 
                    {
                        foreach (Label label in StaticLesson.ListLabel)
                        {
                            if (label.audioLabel != "")
                            {
                                loadAudio(label.audioLabel, lessonId);
                            }
                            if (label.videoLabel != "")
                            {
                                StartCoroutine(loadVideo(label.videoLabel, lessonId));
                            }
                        }
                    }
                    
                    if (StaticLesson.Audio == "" && StaticLesson.Video == "")
                    {
                        showListItem.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().text = $"No item";
                    }
                }
                else 
                {
                    throw new Exception(lessonDetailResponse.message);
                }
            }
            catch (Exception e)
            {
                Debug.Log($"Update lesson info panel failed: {e.Message}");
            }
        }

        // private IEnumerator loadAudio(string audioUrl, int lessonId)
        // {
        //     GameObject audioComp = Instantiate(Resources.Load(PathConfig.ADD_AUDIO) as GameObject);
        //     audioComp.transform.GetChild(0).GetChild(3).GetComponent<Button>().onClick.AddListener(() => HandlerDeleteAudio(lessonId));
        //     Debug.Log("Load audio url: " + String.Format(APIUrlConfig.LoadLesson, audioUrl));
        //     UnityWebRequest webRequest = UnityWebRequest.Get(String.Format(APIUrlConfig.LoadLesson, audioUrl));
        //     audioComp.transform.parent = lessonInfoPanel.transform;     
        //     audioComp.transform.localScale = new Vector3(1f, 1f, 1f);                   
        //     yield return webRequest.SendWebRequest();
        //     if (webRequest.isNetworkError || webRequest.isHttpError)
        //     {   
        //         Debug.Log("An error has occur");
        //         Debug.Log(webRequest.error);
        //     }
        //     else
        //     {
        //         // Check when response is received
        //         if (webRequest.isDone)
        //         {
        //             byte[] audio = webRequest.downloadHandler.data;
        //             // Convert to AudioClip 
        //             AudioClip audioData = Helper.ToAudioClip(audio);
        //             // Convert 
        //             audioComp.GetComponent<AudioSource>().clip = audioData;
        //             AudioManager1.Instance.DisplayAudio(true);
        //             // audioComp.GetComponent<AudioSource>().Play();
        //         }
        //     }
        // }

        private async Task loadAudio(string audioUrl, int lessonId)
        {
            GameObject audioComp = Instantiate(Resources.Load(PathConfig.ADD_AUDIO) as GameObject);
            audioComp.transform.GetChild(0).GetChild(3).GetComponent<Button>().onClick.AddListener(() => HandlerDeleteAudio(lessonId));

            audioComp.transform.parent = lessonInfoPanel.transform;     
            audioComp.transform.localScale = new Vector3(1f, 1f, 1f);  

            AudioClip audioData = await UnityHttpClient.GetAudioClip(audioUrl); 
            audioComp.GetComponent<AudioSource>().clip = audioData;
            // audioComp.GetComponent<AudioSource>().Play();
            AudioManager1.Instance.DisplayAudio(true);
        }

        // private async Task loadVideo(string videoUrl, int lessonId)
        // {
        //     GameObject videoComp = Instantiate(Resources.Load(PathConfig.ADD_VIDEO) as GameObject); 
        //     videoComp.transform.GetChild(0).GetChild(3).GetComponent<Button>().onClick.AddListener(() => HandlerDeleteVideo(lessonId));
        //     videoComp.transform.parent = lessonInfoPanel.transform;
        //     videoComp.transform.localScale = new Vector3(1f, 1f, 1f);
        //     InfoLinkVideo dataResp = JsonUtility.FromJson<InfoLinkVideo>(webRequest.downloadHandler.text); 
        //     videoComp.transform.GetChild(1).GetChild(0).GetChild(2).GetComponent<Text>().text = Helper.FormatString(dataResp.title.ToLower(), calculatedSize);
        //     LoadVideoThumbnail(videoComp, dataResp.thumbnail_url, videoUrl);
        // }

        private IEnumerator loadVideo(string videoUrl, int lessonId)
        {
            GameObject videoComp = Instantiate(Resources.Load(PathConfig.ADD_VIDEO) as GameObject); 
            videoComp.transform.GetChild(0).GetChild(3).GetComponent<Button>().onClick.AddListener(() => HandlerDeleteVideo(lessonId));
            Debug.Log("Load video url: " + String.Format(APIUrlConfig.LoadLesson, videoUrl));
            UnityWebRequest webRequest = UnityWebRequest.Get(String.Format(APIUrlConfig.GetLinkVideo, videoUrl));
            videoComp.transform.parent = lessonInfoPanel.transform;
            videoComp.transform.localScale = new Vector3(1f, 1f, 1f);
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.Log("An error has occur");
                Debug.Log(webRequest.error);
            }
            else
            {
                // Check when response is received 
                if (webRequest.isDone)
                {
                    Debug.Log("Update video: " + webRequest.downloadHandler.text);
                    InfoLinkVideo dataResp = JsonUtility.FromJson<InfoLinkVideo>(webRequest.downloadHandler.text); 
                    videoComp.transform.GetChild(1).GetChild(0).GetChild(2).GetComponent<Text>().text = Helper.FormatString(dataResp.title.ToLower(), calculatedSize);
                    StartCoroutine(LoadVideoThumbnail(videoComp, dataResp.thumbnail_url, videoUrl));
                }
            }
        }

        IEnumerator LoadVideoThumbnail(GameObject videoObj, string imageUri, string videoUri)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUri);
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {

            }
            if (request.isDone)
            {
                Texture2D tex = ((DownloadHandlerTexture) request.downloadHandler).texture;
                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));

                videoObj.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = sprite;
                videoObj.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(() => NavigageToVideo(videoUri));
            }
        }

        void NavigageToVideo(string videoUri)
        {
            Application.OpenURL(videoUri);
        }

        void HandlerDeleteAudio(int lessonId)
        {
            panelPopUpDeleteActions.SetActive(true);
            btnDeleteActions = GameObject.Find("BtnDeleteActions").GetComponent<Button>();
            btnDeleteActions.onClick.AddListener(() => StartCoroutine(DeleteAudioLesson(lessonId)));
        }

        void HandlerDeleteVideo(int lessonId)
        {
            panelPopUpDeleteActionsVideo.SetActive(true);
            btnDeleteActionsVideo = GameObject.Find("BtnDeleteActionsVideo").GetComponent<Button>();
            btnDeleteActionsVideo.onClick.AddListener(() => StartCoroutine(DeleteVideoLesson(lessonId)));
        }

        IEnumerator DeleteAudioLesson(int lessonId)
        {
            panelPopUpDeleteActions.SetActive(false);
            Debug.Log("Trigger delete audio: ");
            UnityWebRequest webRequest = UnityWebRequest.Delete(String.Format(APIUrlConfig.DeleteAudioLesson, lessonId));
            webRequest.SetRequestHeader("Authorization", PlayerPrefs.GetString("user_token"));
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.Log("An error has occur");
                Debug.Log(webRequest.error);
            }
            else
            {
                if (webRequest.isDone)
                {
                    Debug.Log("Delete audio: ");
                    UpdateLessonInforPannel(lessonId);
                }
            }
        }

        IEnumerator DeleteVideoLesson(int lessonId)
        {
            Debug.Log("Trigger delete video: ");
            UnityWebRequest webRequest = UnityWebRequest.Delete(String.Format(APIUrlConfig.DeleteVideoLesson, lessonId));
            webRequest.SetRequestHeader("Authorization", PlayerPrefs.GetString("user_token"));
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.Log("An error has occur");
                Debug.Log(webRequest.error);
            }
            else
            {
                if (webRequest.isDone)
                {
                    Debug.Log("Delete video: ");
                    UpdateLessonInforPannel(lessonId);
                    panelPopUpDeleteActionsVideo.SetActive(false);
                }
            }
        }
    }
}

