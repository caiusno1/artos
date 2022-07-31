using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonProxy : MonoBehaviour
{
    public bool isMRTK = true;
    private Button btn;
    private Interactable btnMRTK;
    // Start is called before the first frame update
    void Awake()
    {
        if (!isMRTK)
        {
            btn = GetComponent<Button>();
        }
        else
        {
            btnMRTK = GetComponent<Interactable>();
        }

    }

    public void AddListener(UnityAction action)
    {
        if (!isMRTK)
        {
            btn.onClick.AddListener(action);
        }
        else
        {
            btnMRTK.OnClick.AddListener(action);
        }
    }
    public void RemoveAllListeners()
    {
        if (!isMRTK)
        {
            btn.onClick.RemoveAllListeners();
        }
        else
        {
            btnMRTK.OnClick.RemoveAllListeners();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
