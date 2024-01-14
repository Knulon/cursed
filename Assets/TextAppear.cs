using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextAppear : MonoBehaviour
{
    [SerializeField]
    public float timeToDisplayFullText = 1f; // in seconds

    private float timePassed = 0f;

    private String textToDisplay = "Hello World!";

    [SerializeField]
    private TextMeshProUGUI textMesh;

    void Start()
    {
        textToDisplay = textMesh.text;
    }

    public void SetText(String text)
    {
        textToDisplay = text;
    }

    public void Appear()
    {
        timePassed = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Appear();


        if (timePassed < timeToDisplayFullText)
        {
            textMesh.text = textToDisplay.Substring(0, (int)(timePassed / timeToDisplayFullText * textToDisplay.Length));
        }

        timePassed += Time.deltaTime;
    }

}
