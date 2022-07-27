using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

namespace LessonDescription
{
    public class InteractionUI : MonoBehaviour
    {
        // public GameObject waitingScreen;
        private GameObject startLessonBtn; 
        private GameObject startMeetingBtn; 
        private GameObject backToHomeBtn;
        private static InteractionUI instance; 
        public static InteractionUI Instance
        {
            get 
            {
                if(instance == null)
                {
                    instance = FindObjectOfType<InteractionUI>(); 
                }
                return instance;
            }
        }
        void Start()
        {
            InitUI(); 
            SetActions(); 
        }

        void Update()
        {
            
        }
        void InitUI()
        {
            backToHomeBtn = GameObject.Find("BackBtn"); 
            startLessonBtn = GameObject.Find("StartLessonBtn");
            if (PlayerPrefs.GetString("user_email") != "")
            {
                startMeetingBtn = GameObject.Find("StartMeetingBtn");
            } 
        }
        void SetActions()
        {
            backToHomeBtn.GetComponent<Button>().onClick.AddListener(BackToRenalSystem); 
            startLessonBtn.GetComponent<Button>().onClick.AddListener(StartExperience);
            if (PlayerPrefs.GetString(PlayerPrefConfig.userToken) != "")
            {
                startMeetingBtn.GetComponent<Button>().onClick.AddListener(StartMeeting);
            } 
        }
        void BackToRenalSystem()
        {
            if (PlayerPrefs.GetString(PlayerPrefConfig.userToken) != "")
            {
                StartCoroutine(Helper.LoadAsynchronously(SceneConfig.home_user));
            } 
            else
            {
                StartCoroutine(Helper.LoadAsynchronously(SceneConfig.home_nosignin));
            }
        }
        void StartExperience()
        {
            SceneNameManager.setPrevScene(SceneManager.GetActiveScene().name);
            StartCoroutine(Helper.LoadAsynchronously(SceneConfig.experience)); 
        }
        void StartMeeting()
        {
            SceneNameManager.setPrevScene(SceneManager.GetActiveScene().name);
            StartCoroutine(Helper.LoadAsynchronously(SceneConfig.meetingStarting));
        }
    }
}


