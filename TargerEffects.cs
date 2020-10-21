using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargerEffects : MonoBehaviour
{
    public GameObject obj;
    public Material[] colorList;
    public List<Color> oricolorList;
    // Start is called before the first frame update
    public void changeColor(int t)
    {
        colorList = gameObject.GetComponent<MeshRenderer>().materials;
        if (t==1)
        {
            foreach (Material m in colorList)
            {
                Color c = Color.green;
                c.a = 0.60f;
                m.color = c;
            }
        }
        else
        { 
            for (int i = 0; i<colorList.Length; i++)
            {
                colorList[i].color = oricolorList[i];
            }
        }

    }
    void Start()
    {
        
        colorList = gameObject.GetComponent<MeshRenderer>().materials;
        foreach (Material m in colorList)
        {
            oricolorList.Add(m.color);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
       
    }
}
