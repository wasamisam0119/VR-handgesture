using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OnTriggerButton : MonoBehaviour
{
    public UnityEvent UEvent;

    [Range(0.1f, 2)]
    public float InvokeTime = 1;
    private float timeCounter = 0;


    void Update()
    {
        if (timeCounter > 0)
        {
            timeCounter -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (timeCounter > 0)
        {
            return;
        }
        timeCounter = InvokeTime;
        UEvent.Invoke();
    }

    private void OnTriggerStay(Collider other)
    {
        this.GetComponent<Image>().color = Color.red;
    }
    private void OnTriggerExit(Collider other)
    {
        this.GetComponent<Image>().color = Color.white;
    }
}
