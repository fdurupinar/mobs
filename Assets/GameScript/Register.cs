using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;

public class Register : MonoBehaviour
{
    public GameObject username;
    public GameObject password;
    private string Username;
    private string Password;
    private string form;


    public void RegisterButton()
    {
        //username
        bool UN = false;
        //password
        bool PW = false;

        if (Username != "")
        {
            //use local storage
            //if (!System.IO.File.Exists(@"/Users/jiehyun/Jenna/UMassBoston/2021 Spring/CS696_Research/final_login/" + Username + ".txt"))
            if (!System.IO.File.Exists(@"./final_login/" + Username + ".txt"))
            {
                UN = true;
            }
            else
            {
                Debug.LogWarning("Username Taken");
            }
        }
        else
        {
            Debug.LogWarning("Username field empty");
        }
        if (Password != "")
        {
            if (Password.Length > 5)
            {
                PW = true;
            }
            else
            {
                Debug.LogWarning("Password Must be at least 6 characters long");
            }
        }
        else
        {
            Debug.LogWarning("Password field empty");
        }
        if (UN == true && PW == true)
        {
            bool Clear = true;
            int i = 1;
            foreach (char c in Password)
            {
                if (Clear)
                {
                    Password = "";
                    Clear = false;
                }
                i++;
                char Encrypted = (char)(c * i);
                Password += Encrypted.ToString();
            }
            form = (Username + "\n" + Password);
            //System.IO.File.WriteAllText(@"/Users/jiehyun/Jenna/UMassBoston/2021 Spring/CS696_Research/final_login/" + Username + ".txt", form);
            System.IO.File.WriteAllText(@"./final_login/" + Username + ".txt", form);

            username.GetComponent<InputField>().text = "";
            password.GetComponent<InputField>().text = "";

            print("Registration Complete");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (username.GetComponent<InputField>().isFocused)
            {
                password.GetComponent<InputField>().Select();
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (Password != "" && Password != "")
            {
                RegisterButton();
            }
        }
        Username = username.GetComponent<InputField>().text;
        Password = password.GetComponent<InputField>().text;
    }
}
