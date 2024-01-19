using System;
using TMPro;
using UnityEngine;

public class TextAppear : MonoBehaviour
{
    [SerializeField]
    public float timeToDisplayFullText = 1f; // in seconds

    [SerializeField]
    public float timeToDisplayCharacter = 0.1f; // in seconds

    [SerializeField]
    [Tooltip("Check this if you want to set how long it takes until the next character is displayed.")]
    public bool timePerCharacter = false; // if true, timeToDisplayFullText is ignored and timeToDisplayCharacter is used instead

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
        if (Input.GetKeyDown(KeyCode.P))
        {
            Appear();
        }

        if (!timePerCharacter && timePassed < timeToDisplayFullText)
        {
            textMesh.text = textToDisplay.Substring(0, (int)(timePassed / timeToDisplayFullText * textToDisplay.Length + 1));
        }

        if (timePerCharacter && timePassed < timeToDisplayCharacter * textToDisplay.Length)
        {
            textMesh.text = textToDisplay.Substring(0, (int)(timePassed / timeToDisplayCharacter + 1));
        }

        timePassed += Time.deltaTime;

        if (timePassed > 10) {
            textMesh.text = "";
        }
    }

}
