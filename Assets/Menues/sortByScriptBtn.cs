using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class SortButtons : MonoBehaviour
{
    public GameObject[] buttons;

    private bool ascendingOrder = true;

    public void ToggleSortOrder()
    {
        ascendingOrder = !ascendingOrder;
        SortButtonsAlphabetically();
    }

    public void SortButtonsAlphabetically()
    {
        buttons = buttons.OrderBy(b => b.GetComponentInChildren<TextMeshProUGUI>().text, StringComparer.OrdinalIgnoreCase).ToArray();

        if (!ascendingOrder)
            Array.Reverse(buttons);

        RepositionButtons();
    }

    private void RepositionButtons()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].transform.SetSiblingIndex(i);
        }
    }
}