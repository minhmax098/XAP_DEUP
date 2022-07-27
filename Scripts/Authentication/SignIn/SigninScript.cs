using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Text;
using EasyUI.Toast;
using AdvancedInputFieldPlugin;

public class SigninScript : MonoBehaviour
{
    public AdvancedInputField userNameInputField;
    public AdvancedInputField capchaInputField;
    public AdvancedInputField passwordInputField;
    public GameObject EmailWarning;
    public GameObject PassWarning;
    public GameObject CapchaWarning;
    public Button signInBtn;
    public GameObject loadingScreen;
    public GameObject waitingScreen;
    public GameObject headerCapcha;
    public GameObject capcha;
    public GameObject incorrectCapcha;
    private int invalidCount;


    void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        StatusBarManager.statusBarState = StatusBarManager.States.TranslucentOverContent;
        StatusBarManager.navigationBarState = StatusBarManager.States.Hidden;

        signInBtn = transform.GetComponent<Button>();
        signInBtn.onClick.AddListener(ValidateSignin);

        EmailWarning.SetActive(false);
        PassWarning.SetActive(false);
        capcha.SetActive(false);

        loadingScreen.SetActive(false);
        waitingScreen.SetActive(false);

        // userNameInputField.keyboardType = TouchScreenKeyboardType.EmailAddress;
        userNameInputField.OnValueChanged.AddListener(checkUserNameValid);

        // passwordInputField.contentType = InputField.ContentType.Password;
        passwordInputField.OnValueChanged.AddListener(checkPassValid);
        capchaInputField.OnValueChanged.AddListener(HandleCaptcha);

        headerCapcha.SetActive(false);
        CapchaWarning.SetActive(false);
        incorrectCapcha.SetActive(false);
    }

    void HideAllNotifications()
    {
        EmailWarning.SetActive(false);
        PassWarning.SetActive(false);
        CapchaWarning.SetActive(false);
        incorrectCapcha.SetActive(false);
    }

    void HandleCaptcha(string data)
    {
        CapchaWarning.SetActive(false);
        incorrectCapcha.SetActive(false);
    }

    void checkUserNameValid(string data)
    {
        if (data != "")
        {
            changeUIStatus(userNameInputField, EmailWarning, false);
        }
        // else
        // {
        //     changeUIStatus(userNameInputField, EmailWarning, true);
        // }
    }

    void checkPassValid(string data)
    {
        if (data != "")
        {
            changeUIStatus(passwordInputField, PassWarning, false);
        }
    }

    private void ValidateSignin()
    {
        signInBtn.interactable = false;
        Debug.Log("Number of invalid count: " + invalidCount);
        HideAllNotifications();
        string email = userNameInputField.Text;
        string pass = passwordInputField.Text;
        bool check = false;
        if (email == "")
        {
            changeUIStatus(userNameInputField, EmailWarning, true);
            check = true;
        }
        if (pass == "")
        {
            changeUIStatus(passwordInputField, PassWarning, true);
            check = true;
        }
        if (invalidCount >= 5)
        {
            string capchaString = capcha.transform.GetChild(0).GetComponent<AdvancedInputField>().Text;
            Debug.Log(capchaString);
            if (capchaString == "")
            {
                changeUIStatus(capcha.transform.GetChild(0).gameObject.GetComponent<AdvancedInputField>(), CapchaWarning, true);
                incorrectCapcha.SetActive(false);
                check = true;
            }
            if (capchaString != "")
            {
                changeUIStatus(capcha.transform.GetChild(0).gameObject.GetComponent<AdvancedInputField>(), CapchaWarning, false);
                // CapchaWarning.SetActive(false);
                if (!check)
                {
                    Debug.Log("test capcha comparison: ");
                    Debug.Log(capchaString);
                    Debug.Log(PlayerPrefs.GetString("capcha"));
                    CapchaWarning.SetActive(false);
                    if (capchaString.Equals(PlayerPrefs.GetString("capcha")))
                    {
                        // incorrectCapcha.SetActive(false);
                        // capcha.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(SpriteConfig.imageInputFieldWarning);
                        changeUIStatus(capcha.transform.GetChild(0).gameObject.GetComponent<AdvancedInputField>(), incorrectCapcha, false);
                        StartCoroutine(CallSignin(email, pass));
                    }
                    else
                    {
                        Debug.Log("capcha");
                        incorrectCapcha.SetActive(true);
                        CapchaGeneration.Instance.GenCapchaCode(6);
                        CapchaWarning.SetActive(false);
                    }
                }
            }
        }
        if (!check && invalidCount < 5)
        {
            StartCoroutine(CallSignin(email, pass));
        }
        else
        {
            signInBtn.interactable = true;
        }
    }
    private void changeUIStatus(AdvancedInputField input, GameObject warning, bool status)
    {
        warning.SetActive(status);
        if (status)
        {
            input.GetComponent<Image>().sprite = Resources.Load<Sprite>(SpriteConfig.imageInputFieldWarning);
        }
        else
        {
            input.GetComponent<Image>().sprite = Resources.Load<Sprite>(SpriteConfig.normalInputField);
        }
    }
    public IEnumerator LoadAsynchronously(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        waitingScreen.SetActive(true);
        while (!operation.isDone)
        {
            yield return new WaitForSeconds(.0f);
        }
    }
    public IEnumerator WaitForAPIResponse(UnityWebRequest request)
    {
        waitingScreen.SetActive(true);
        Debug.Log("calling API");
        while (!request.isDone)
        {
            yield return new WaitForSeconds(.0f);
        }
    }
    // Signin API
    public IEnumerator CallSignin(string Email, string Password)
    {
        string logindataJsonString = "{\"email\": \"" + Email + "\", \"password\": \"" + Password + "\"}";
        Debug.Log(logindataJsonString);
        var request = new UnityWebRequest(APIUrlConfig.SignIn, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(logindataJsonString);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        StartCoroutine(WaitForAPIResponse(request));
        yield return request.SendWebRequest();
        waitingScreen.SetActive(false);
        if (request.error != null)
        {
            Debug.Log("Error: " + request.error);
            if (request.responseCode == 400)
            {
                // no found user, show message 
                Toast.Show("Username or password incorrect"); // need change later, use message from server
                invalidCount += 1;
                if (invalidCount == 5)
                {
                    headerCapcha.SetActive(true);
                    capcha.SetActive(true);
                }
            }
            signInBtn.interactable = true;
        }
        else
        {
            Debug.Log("Status Code: " + request.responseCode);
            string response = System.Text.Encoding.UTF8.GetString(request.downloadHandler.data);
            Debug.Log("Login response: " + response);
            LoginData userDetail = JsonUtility.FromJson<LoginData>(response);
            if (request.responseCode == 200)
            {
                Debug.Log("Set email: ");
                PlayerPrefs.SetString(PlayerPrefConfig.userEmail, userDetail.data[0].email);
                // PlayerPrefs.SetString("user_name", "Nguyen Thi Huong Giang"); 
                PlayerPrefs.SetString(PlayerPrefConfig.userName, userDetail.data[0].fullName);
                PlayerPrefs.SetString(PlayerPrefConfig.userToken, userDetail.data[0].token);
                // string token = PlayerPrefs.GetString("user_token");
                StartCoroutine(LoadAsynchronously(SceneConfig.home_user));
            }
        }
    }
}
