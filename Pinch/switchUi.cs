using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class switchUi : MonoBehaviour
{
    public GameObject ui;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SwitchUiAction() 
    {
        if (ui.activeSelf) { ui.SetActive(false); } else { ui.SetActive(true); }
    
    }
}
