using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RandomStringGenerator : MonoBehaviour
{
    public TextMeshProUGUI code;
    int length = 6;
    public void GenerateRandomString()
    {
        const string allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        System.Text.StringBuilder randomStringBuilder = new System.Text.StringBuilder(length);

        for (int i = 0; i < length; i++)
        {
            int randomIndex = Random.Range(0, allowedChars.Length);
            randomStringBuilder.Append(allowedChars[randomIndex]);
        }

        code.text = randomStringBuilder.ToString();
    }
}