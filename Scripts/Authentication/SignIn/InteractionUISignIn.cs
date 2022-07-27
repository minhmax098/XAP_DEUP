using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems; 

public class InteractionUISignIn : MonoBehaviour
{
    public GameObject waitingScreen; 
    public Button backToHomeBtn; 
    public Button nextForgotPassBtn; 
    public Button nextSignUpBtn; 
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
        waitingScreen.SetActive(false); 

    }
    void SetActions()
    {
        backToHomeBtn.GetComponent<Button>().onClick.AddListener(BackToHomeNoSignIn); 
        nextForgotPassBtn.GetComponent<Button>().onClick.AddListener(NextToForgotPass); 
        nextSignUpBtn.GetComponent<Button>().onClick.AddListener(NextToSignUp); 
    }

    void BackToHomeNoSignIn()
    {
        StartCoroutine(Helper.LoadAsynchronously(SceneConfig.home_nosignin));
    }
    void NextToForgotPass()
    {
        StartCoroutine(Helper.LoadAsynchronously(SceneConfig.forgotPass)); 
    }
    void NextToSignUp()
    {
        StartCoroutine(Helper.LoadAsynchronously(SceneConfig.signUp)); 
    }
}
