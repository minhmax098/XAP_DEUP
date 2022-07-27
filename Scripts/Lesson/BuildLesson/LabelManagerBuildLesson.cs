using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Text;
using UnityEngine.UI;

namespace BuildLesson 
{
    public class LabelManagerBuildLesson : MonoBehaviour
    {
        private static LabelManagerBuildLesson instance; 
        public static LabelManagerBuildLesson Instance
        {
            get 
            {
                if (instance == null)
                {
                    // centerPosition = Helper.CalculateCentroid(ObjectManager.Instance.OriginObject);
                    instance = FindObjectOfType<LabelManagerBuildLesson>(); 
                }
                return instance; 
            }
        }
        private int calculatedSize = 10;
        public GameObject btnLabel;
        
        private List<Vector3> pointPositions = new List<Vector3>();
        public List<GameObject> listLabelObjects = new List<GameObject>(); 
        public List<GameObject> listLabelObjectsOnEditMode = new List<GameObject>(); 

        private bool isLabelOnEdit = false;
        private bool isShowingLabel = true;

        public bool IsLabelOnEdit { get; set;}
        
        public bool IsShowingLabel 
        {    
            get
            {
                return isShowingLabel;
            }
            set
            {
                Debug.Log("LabelManagerBuildLesson IsShowingLabel call"); 
                isShowingLabel = value;
                btnLabel.GetComponent<Image>().sprite = isShowingLabel ? Resources.Load<Sprite>(PathConfig.LABEL_UNCLICK_IMAGE) : Resources.Load<Sprite>(PathConfig.LABEL_CLICKED_IMAGE);
            }
        }

        void Start()
        {
            InitUI();
        }

        void InitUI()
        {
            btnLabel = GameObject.Find("BtnLabel");
            this.IsShowingLabel = true;  // Force the UI update 
        }

        public void Update()
        {
            pointPositions.Add(transform.position);
        }
        
        private static Vector3 centerPosition;

        public void updateCenterPosition()
        {
            Debug.Log("After init object: " + ObjectManagerBuildLesson.Instance.OriginObject);
            centerPosition = Helper.CalculateCentroid(ObjectManagerBuildLesson.Instance.OriginObject);
        }

        public Bounds GetParentBound(GameObject parentObject, Vector3 center)
        {
            foreach (Transform child in parentObject.transform)
            {
                center += child.gameObject.GetComponent<Renderer>().bounds.center;
            }
            center /= parentObject.transform.childCount;
            Bounds bounds = new Bounds(center, Vector3.zero);
            foreach(Transform child in parentObject.transform)
            {
                bounds.Encapsulate(child.gameObject.GetComponent<Renderer>().bounds);
            }
            return bounds;
        }

        public IEnumerator SaveCoordinate(int lessonId, int modelId, string labelName, Vector3 coordinate, string level)
        {
            var webRequest = new UnityWebRequest(APIUrlConfig.CreateModelLabel, "POST");
            string requestBody = "{\"lessonId\": \"" + lessonId + "\", \"modelId\": \"" + modelId + "\" , \"labelName\": \"" + labelName + "\", \"coordinates\" :{\"x\": "+ coordinate.x + ",\"y\": " + coordinate.y + ",\"z\": "+ coordinate.z +"}, \"level\":  \"" + level + "\"}";
            Debug.Log("CHECKKK Test create label Test data: " + requestBody);
            byte[] jsonToSend = Encoding.UTF8.GetBytes(requestBody);
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", PlayerPrefs.GetString("user_token"));
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
                    Debug.Log("CHECKKK Test create Label done: ");
                    // Use JsonTool to convert from JSONG String to Object
                    string jsonResponse = webRequest.downloadHandler.text;
                    Debug.Log("CHECKKK Test create Label Json response: " + jsonResponse);
                    //  IEnumerator not CoordinateJson
                    CoordinateJson rs =  JsonUtility.FromJson<CoordinateJson>(jsonResponse);
                    Debug.Log("CHECKKK Test create Label: " + rs.data.labelId);
                    // Add coresponding labelId with the addedTags 
                    TagHandler.Instance.AddLabelId(rs.data.labelId); 
                } 
            }
        }

        public string getIndexGivenGameObject(GameObject rootObject, GameObject targetObject)
        {
            var result = new System.Text.StringBuilder();
            while(targetObject != rootObject)
            {
                result.Insert(0, targetObject.transform.GetSiblingIndex().ToString());
                result.Insert(0, "-");
                targetObject = targetObject.transform.parent.gameObject;
            }
            result.Insert(0, "0");
            return result.ToString();
        }

        public void HandleLabelView(bool currentLabelStatus) 
        {
            IsShowingLabel = currentLabelStatus;
            ShowHideLabels(IsShowingLabel);
            TagHandler.Instance.ShowHideTags(IsShowingLabel);
        }

        private void ShowHideLabels(bool isShowing)
        {
            foreach(GameObject label in listLabelObjects)
            {
                label.SetActive(isShowingLabel);
            }    
        }
    }
}
