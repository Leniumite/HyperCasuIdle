using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Analytics;

public class AnalyticsManager : MonoBehaviour
{
    async void Start() //Initialize Unity Services
    {
        try
        {
            await UnityServices.InitializeAsync();
        }
        catch (ConsentCheckException e)
        {
            Debug.Log(e.ToString());
        }
    }

    //Example
    /*
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            this.LevelCompletedCustomEvent();
        }
    }
    
    private void LevelCompletedCustomEvent()
    {
        int currentLevel = Random.Range(1, 4); //Gets a random number from 1-3
        //Define Custom Parameters
        //"levelName"  is the name of the custom parameter
        Dictionary<string, object> parameters = new Dictionary<string, object> {
            {
                "levelName", "level" + currentLevel
            }
        };

        // The ‘levelCompleted’ event will get cached locally 
        //"levelCompleted" is the name of the custom event
        //and sent during the next scheduled upload, within 1 minute
        AnalyticsService.Instance.CustomData("levelCompleted", parameters);

        // You can call Events.Flush() to send the event immediately
        AnalyticsService.Instance.Flush();
    }
    */
}
