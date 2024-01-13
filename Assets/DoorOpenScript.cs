using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class DoorOpenScript : MonoBehaviour
{
    enum AnimationStep
    {
        Closed = 0,
        Open = 1,
        Close= 2,
        Opened = 3
    }
    private GameObject player;
    private AnimationStep currentStep = AnimationStep.Closed;
    private float animationCounter = 1;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(player != null && player.GetComponent<PlayerMoveScript>().HasKey()) {
            
            if (!nearDoor(player.transform.position))
            {
                if(currentStep != AnimationStep.Closed) {
                    animationCounter += Time.deltaTime;
                }
            }
            else
            {
                if (currentStep != AnimationStep.Opened)
                {
                    animationCounter -= Time.deltaTime;
                    currentStep = AnimationStep.Open;
                }
            }
            if (animationCounter < 0.01)
            {
                currentStep = AnimationStep.Opened;
                animationCounter = 0;
                transform.localScale = new Vector3(1, 0, 1);
            }
            else if(animationCounter > 0.999f) { 
                currentStep = AnimationStep.Closed;
                animationCounter = 1;
                transform.localScale = new Vector3(1,5f,1);
            }

            if (currentStep != AnimationStep.Closed )
            { 
                transform.localScale = new Vector3(1, Easing.OutCubic(animationCounter)*5f,1);
            }
        }
    }

    private bool nearDoor(Vector3 position)
    {
        return Math.Abs(position.x - transform.position.x) < 3
            && Math.Abs(position.y - transform.position.y) < 3;
    }
}
