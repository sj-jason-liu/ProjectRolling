using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField]
    private float _timeRemaining = 10f; //maximum time which adjustable in Editor
    private bool _isTimerGoing = false; //switch to stop countdown

    [SerializeField]
    private Text _timeText;
    
    void Start()
    {
        _isTimerGoing = true;
        GameManager.Instance.IsTimeGoing = true;
    }

    void Update()
    {
        if(_isTimerGoing) //When countdown is ongoing
        {
            if (_timeRemaining > 0) 
            {
                _timeRemaining -= Time.deltaTime;
                DisplayTime(_timeRemaining);
            }
            else
            {
                _timeRemaining = 0; 
                _isTimerGoing = false;
                GameManager.Instance.IsTimeGoing = false;
                //Make environment elements stop moving
                //or
                //Make environment elements "keep" moving but disable sensor
                Debug.Log("Times up!");
            }
        }      
    }

    void DisplayTime(float timeToDisplay)
    {
        //To prevent time immediately count 1 sec when started,
        //add another second to gain a fully 1 sec countdown at the beginning
        timeToDisplay += 1;

        float minute = Mathf.FloorToInt(timeToDisplay / 60);
        float second = Mathf.FloorToInt(timeToDisplay % 60);
        
        _timeText.text = string.Format("{0:00}:{1:00}", minute, second);
    }
}
