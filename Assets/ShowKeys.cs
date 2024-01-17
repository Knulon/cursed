using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowKeys : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]GameObject W, A, S, D;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Vertical")>0)
        {
            W.GetComponent<Image>().color = Color.red;
        }
        else
        {
            W.GetComponent<Image>().color = Color.white;
        }
        if (Input.GetAxis("Horizontal") < 0)
        {
            A.GetComponent<Image>().color = Color.red;
        }
        else
        {
            A.GetComponent<Image>().color = Color.white;
        }
        if (Input.GetAxis("Vertical") < 0)
        {
            S.GetComponent<Image>().color = Color.red;
        }
        else
        {
            S.GetComponent<Image>().color = Color.white;
        }
        if (Input.GetAxis("Horizontal") > 0)
        {
            D.GetComponent<Image>().color = Color.red;
        }
        else
        {
            D.GetComponent<Image>().color = Color.white;
        }
    }
}
