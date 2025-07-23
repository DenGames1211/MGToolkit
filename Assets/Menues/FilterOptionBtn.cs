using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilterOptionBtn : MonoBehaviour
{
    bool isOpen = false; // Cambiato da 'const' a 'bool' per poter modificare il valore
    public GameObject filterMenu;
    public GameObject background;

    public void FilterManager()
    {
        isOpen = !isOpen; // Cambiato da 'isOpen = false'/'isOpen = true' a un toggle usando l'operatore '!'
        
        if(filterMenu != null)
        {
            filterMenu.SetActive(isOpen);
        }
        if(background!= null)
        {
            background.SetActive(isOpen);
        }

    }
}
