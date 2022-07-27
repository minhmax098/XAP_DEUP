using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 
using UnityEngine.EventSystems; 

namespace List3DStore
{
    public class InteractionUI : MonoBehaviour
    {
        public GameObject waitingScreen; 
        public Button backToCreateLessonBtn; 
        private static InteractionUI instance; 
        public static InteractionUI Instance
        { 
            get 
            { 
                if (instance == null)
                {
                    instance = FindObjectOfType<InteractionUI>();
                }
                return instance;
            }
        }
        void Start()
        {
            InitEvents();
        }
       
        void InitEvents()
        {
            backToCreateLessonBtn.onClick.AddListener(BackToCreateLesson); 
        }
        void BackToCreateLesson()
        {
            StopAllCoroutines();
            waitingScreen.SetActive(true);
            StartCoroutine(Helper.LoadAsynchronously(SceneConfig.createLesson_main));
        }
        public void onClickItemModel(int modelId, string modelName)
        {
            Debug.Log("On click item lesson: ");
            ModelStoreManager.InitModelStore(modelId, modelName);
            SceneNameManager.setPrevScene(SceneConfig.storeModel); 
            if (PlayerPrefs.GetString(PlayerPrefConfig.userToken) != "")
            {
                StopAllCoroutines();
                StartCoroutine(Helper.LoadAsynchronously(SceneConfig.createLesson)); 
            }
        }
    }
}
