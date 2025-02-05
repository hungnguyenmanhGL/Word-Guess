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
    private TouchScreenKeyboard keyboard;

    public KeyboardDoneEvent onKeyboardDone {
        get { return keyboardDoneEvent; }
        set { keyboardDoneEvent = value; }
    }

    protected override void Start() {
        TouchScreenKeyboard.Android.consumesOutsideTouches = true;
    }

    //m_Keyboard is protected in metadata
    void Update() {
        if (m_SoftKeyboard != null && m_SoftKeyboard.status == TouchScreenKeyboard.Status.Done && m_SoftKeyboard.status != TouchScreenKeyboard.Status.Canceled) {
            keyboardDoneEvent.Invoke();
        }
    }

    public override void OnSelect(BaseEventData eventData) {
        base.OnSelect(eventData);
        //Debug.LogError("Selected");
        if (keyboard == null) {
            keyboard = TouchScreenKeyboard.Open(string.Empty, TouchScreenKeyboardType.Default);
        }

    }

    public override void OnDeselect(BaseEventData eventData) {
        base.OnDeselect(eventData);
        //Debug.LogError("Deselect");
        if (keyboard != null && !keyboard.active) keyboard.active = true;
        SelectSelf();
    }

    public void SelectSelf() {
        ActivateInputField();
    }
}
