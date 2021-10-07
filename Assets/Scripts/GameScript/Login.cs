using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;

public class Login : MonoBehaviour
{
    public GameObject username;
    public GameObject password;
    private string Username;
    private string Password;
    private String[] Lines;
    private string DecryptedPass;

    public void LoginButton()
    {
        bool UN = false;
        bool PW = false;

        if (Username != "")
        {
            //if (System.IO.File.Exists(@"/Users/jiehyun/Jenna/UMassBoston/2021 Spring/CS696_Research/final_login/" + Username + ".txt"))
            if (System.IO.File.Exists(@"./final_login/" + Username + ".txt"))
            {
                UN = true;
                //Lines = System.IO.File.ReadAllLines(@"/Users/jiehyun/Jenna/UMassBoston/2021 Spring/CS696_Research/final_login/" + Username + ".txt");
                Lines = System.IO.File.ReadAllLines(@"./final_login/" + Username + ".txt");
            }
            else
            {
                Debug.LogWarning("Username invalid");
            }
        }
        else
        {
            Debug.LogWarning("Username Field Empty");
        }
        if (Password != "")
        {
            //if (System.IO.File.Exists(@"/Users/jiehyun/Jenna/UMassBoston/2021 Spring/CS696_Research/final_login/" + Username + ".txt"))
            if (System.IO.File.Exists(@"./final_login/" + Username + ".txt"))
            {

                int i = 1;
                foreach (char c in Lines[1])
                {
                    i++;
                    char Decrypted = (char)(c / i);
                    DecryptedPass += Decrypted.ToString();
                }
                if (Password == DecryptedPass)
                {
                    PW = true;

                }
                else
                {
                    Debug.LogWarning("Password is invalid");
                }
            }
            else
            {
                Debug.LogWarning("Password is invalid2");
            }
        }
        else
        {
            Debug.LogWarning("Password Field Empty");
        }
        if (UN == true && PW == true)
        {
            username.GetComponent<InputField>().text = "";
            password.GetComponent<InputField>().text = "";
            print("Login Success");
            Application.LoadLevel("Sales");
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
                LoginButton();
            }
        }

        Username = username.GetComponent<InputField>().text;
        Password = password.GetComponent<InputField>().text;
    }
}
