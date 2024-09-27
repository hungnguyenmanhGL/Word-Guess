using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarSlot : MonoBehaviour
{
    [SerializeField] private Image imgStarFill;
    
    public void Toggle(bool on) {
        imgStarFill.gameObject.SetActive(on);
    }
}
