using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadSceneHomeUser : MonoBehaviour
{
    public Text username;
    public Text email;
    void Start()
    {
        username.text = PlayerPrefs.GetString(PlayerPrefConfig.userName);
        email.text = PlayerPrefs.GetString(PlayerPrefConfig.userEmail);
    }
}

