using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;

public class SubmitInputField : TMP_InputField
{
    [System.Serializable] public class KeyboardDoneEvent : UnityEvent { }

    [SerializeField] private KeyboardDoneEvent keyboardDoneEvent = new KeyboardDoneEvent();

    public KeyboardDoneEvent onKeyboardDone {
        get { return keyboardDoneEvent; }
        set { keyboardDoneEvent = value; }
    }

    //m_Keyboard is protected in metadata
    void Update() {
        if (m_SoftKeyboard != null && m_SoftKeyboard.status == TouchScreenKeyboard.Status.Done && m_SoftKeyboard.status != TouchScreenKeyboard.Status.Canceled) {
            keyboardDoneEvent.Invoke();
        }
    }
}
