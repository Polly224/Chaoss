using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class NewBehaviourScript : MonoBehaviour
{
    public UIDocument document;
    void Start()
    {
        Button btn = document.rootVisualElement.Query<Button>();
        btn.clicked += DebugLog;
    }

    private void DebugLog()
    {
        Debug.Log("button pressed");
    }
}
