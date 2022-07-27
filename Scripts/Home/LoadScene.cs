using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Linq;
using EasyUI.Toast;

namespace Home
{
    public class LoadScene : MonoBehaviour
    {
        public GameObject waitingScreen;
        public Transform contentItemCategoryWithLesson;
        public GameObject searchBox;
        // add 3 record: searchBtn, xBtn, sumLesson

        [SerializeField]
        public GameObject organPanelResource;
        [SerializeField]
        public GameObject lessonObjectResource;
        void Start()
        {
            InitScreen();
            LoadDataFromServer();
        }

        void InitScreen()
        {
            Debug.Log("Init screen");
            Screen.orientation = ScreenOrientation.Portrait;
            StatusBarManager.statusBarState = StatusBarManager.States.TranslucentOverContent;
            StatusBarManager.navigationBarState = StatusBarManager.States.Hidden;
        }

        async Task LoadDataFromServer()
        {
            Debug.Log("Loading data from server...");
            try
            {
                if (OrganCaching.organList == null)
                {
                    APIResponse<List<ListOrganLesson>> organsResponse = await UnityHttpClient.CallAPI<APIResponse<List<ListOrganLesson>>>(APIUrlConfig.GET_ORGAN_LIST, UnityWebRequest.kHttpVerbGET);
                    if (organsResponse.code == APIUrlConfig.SUCCESS_RESPONSE_CODE)
                    {
                        OrganCaching.organList = organsResponse.data;
                    }
                    else
                    {
                        throw new Exception(organsResponse.message);
                    }
                }
                foreach (ListOrganLesson organ in OrganCaching.organList)
                {
                    await InitOrganPanel(organ);
                    if (waitingScreen.activeInHierarchy)
                    {
                        waitingScreen.SetActive(false);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log($"sonvdh load organ failed: {e.Message}");
            }
        }

        async Task InitOrganPanel(ListOrganLesson organ)
        {
            GameObject lessonsByOrganPanel = CreateOrganPanel(organ);
            await LoadLessonsByOrgan(lessonsByOrganPanel, organ.organsId);
        }

        GameObject CreateOrganPanel(ListOrganLesson organ)
        {
            // copy from minhlh17
            GameObject lessonByOrganPanel = Instantiate(organPanelResource) as GameObject;
            lessonByOrganPanel.name = organ.organsId.ToString();
            lessonByOrganPanel.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = organ.organsName;
            lessonByOrganPanel.transform.SetParent(contentItemCategoryWithLesson, false);

            Button moreLessonBtn = lessonByOrganPanel.transform.GetChild(0).GetChild(1).gameObject.GetComponent<Button>();
            moreLessonBtn.onClick.AddListener(() => updateOrganManager(organ.organsId, organ.organsName));

            return lessonByOrganPanel;
        }
        async Task LoadLessonsByOrgan(GameObject lessonsByOrganPanel, int organsId)
        {
            try
            {
                string getLessonsByOrganURL = string.Format(APIUrlConfig.GET_LESSON_BY_ORGAN, organsId, APIUrlConfig.DEFAULT_OFFSET, APIUrlConfig.DEFAULT_LIMIT);
                APIResponse<List<LessonForHome>> lessonsByOrgan = await UnityHttpClient.CallAPI<APIResponse<List<LessonForHome>>>(getLessonsByOrganURL, UnityWebRequest.kHttpVerbGET);
                if (lessonsByOrgan.code == APIUrlConfig.SUCCESS_RESPONSE_CODE)
                {
                    if (lessonsByOrgan.data.Count < 1)
                    {
                        Destroy(lessonsByOrganPanel);
                        return;
                    }
                    
                    GameObject subContent = lessonsByOrganPanel.transform.GetChild(1).GetChild(0).GetChild(0).gameObject;
                    foreach (LessonForHome lesson in lessonsByOrgan.data)
                    {
                        InitLessonItem(lesson, subContent.transform);
                    }
                }
                else
                {
                    throw new Exception(lessonsByOrgan.message);
                }
            }
            catch (Exception e)
            {
                Debug.Log($"sonvdh load lessons failed: {e.Message}");
            }
        }

        

        void InitLessonItem(LessonForHome lesson, Transform parentTransform)
        {
            // copy from minhlh17
            // UI component for one Lesson
            GameObject lessonObject = Instantiate(lessonObjectResource) as GameObject;
            try
            {
                string imageURL = APIUrlConfig.BASE_URL + lesson.lessonThumbnail;
                RawImage targetImage = lessonObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<RawImage>();
                StartCoroutine(UnityHttpClient.LoadRawImageAsync(imageURL, targetImage, (isSuccess) => {
                    if (isSuccess)
                    {
                        lessonObject.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                    }
                }));

                lessonObject.name = lesson.lessonId.ToString();
                lessonObject.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = Helper.FormatString(lesson.lessonTitle, ItemConfig.calculatedSize);
                Button lessonBtn = lessonObject.GetComponent<Button>();
                lessonBtn.onClick.AddListener(() => onClickItemLesson(lesson.lessonId));
                lessonObject.transform.SetParent(parentTransform, false);
            }
            catch(Exception e)
            {
                Debug.Log($"sonvdh {e.Message}");
            }     
        }

        void Update()
        {
            if (searchBox.GetComponent<InputField>().isFocused == true)
            {
                StartCoroutine(Helper.LoadAsynchronously(SceneConfig.xrLibrary_edit));
            }

            // if (!pool.Any())
            // {
            //     // pool is empty
            //     int remain = POOL_SIZE;
            //     if (RequestsAndItems.Count > 0 && remain > 0)
            //     {
            //         remain -= 1;
            //         var temp = RequestsAndItems.Dequeue();
            //         pool.Add((temp.Item1.SendWebRequest(), temp.Item2));
            //     }
            // }
            // if (!pool.Any() == false)
            // {
            //     if(AllRequestDone(pool))
            //     {
            //         HandleAllRequestsWhenFinished(pool);
            //         pool.Clear();
            //     }
            // }
        }

        // Update the UI 
        void LateUpdate()
        {
            
        }

        void updateOrganManager(int id, string name)
        {
            waitingScreen.SetActive(true);
            OrganManager.InitOrgan(id, name);
            StopAllCoroutines();
            StartCoroutine(Helper.LoadAsynchronously(SceneConfig.listOrgan_edit));
        }

        public void onClickItemLesson(int lessonId)
        {
            waitingScreen.SetActive(true);
            InteractionUI.Instance.onClickItemLesson(lessonId);
        }
    }
}