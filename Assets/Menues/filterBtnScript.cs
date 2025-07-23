using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JumpToLetter : MonoBehaviour
{
    public Scrollbar scrollbar;
    public GameObject[] buttons;
    public string letter;

    public void JumpToFirstLetter()
    {
        letter = letter.ToUpper(); // Convert to uppercase for case-insensitive comparison
        
        foreach (GameObject button in buttons)
        {
            if (button.GetComponentInChildren<TextMeshProUGUI>().text.ToUpper().StartsWith(letter))
            {
                RectTransform buttonRect = button.GetComponent<RectTransform>();
                if(scrollbar != null)
                {
                float newValue = 1+(buttonRect.localPosition.y/460);

                if(newValue >= 1)
                    newValue = 0.99f;

                scrollbar.value = newValue;
                break;
                }
            }
        }
    }
}