using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonProxy : MonoBehaviour
{
    public Button btn;
    // Start is called before the first frame update
    void Start()
    {
        btn = GetComponent<Button>();
    }

    public void AddListener(UnityAction action)
    {
        btn.onClick.AddListener(action);
    }
    public void RemoveAllListeners()
    {
        btn.onClick.RemoveAllListeners();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
