using System;
using System.Collections;
using UnityEngine;

public class LightingManager : MonoBehaviour
{
    private GameManager gameManager;

    private enum HourState
    {
        Day,
        Evening,
        Night
    }

    [Header("Lights")]
    public Light sun;
    public Light moon;

    [Header("Transition Settings")]
    public float transitionDuration = 1f;
    private Quaternion sunDayRot;
    private Quaternion sunEvenRot;
    private Quaternion sunHideRot;
    private Quaternion moonNightRot;
    private Quaternion moonHideRot;

    private bool isTransitioning = false;

    private Coroutine currentSunTransition;
    private Coroutine currentMoonTransition;

    private HourState currentState;

    void Start()
    {
        sun.enabled = true;
        moon.enabled = false;

        gameManager = GameManager.Instance;

        sunDayRot = sun.transform.rotation;
        sunEvenRot = Quaternion.Euler(12.7f, sunDayRot.eulerAngles.y, sunDayRot.eulerAngles.z);
        moonNightRot = moon.transform.rotation;

        sunHideRot = Quaternion.Euler(-14.94f, sunDayRot.eulerAngles.y, sunDayRot.eulerAngles.z);
        moonHideRot = Quaternion.Euler(-6.7f, moonNightRot.eulerAngles.y, moonNightRot.eulerAngles.z);

        currentState = GetCurrentHourState();
    }

    void Update()
    {
        HourState newState = GetCurrentHourState();

        if (newState != currentState)
        {
            currentState = newState;
            ChangeLighting();
        }
    }

    private void ChangeLighting()
    {
        StopAllCoroutines();

        switch (currentState)
        {
            case HourState.Day:
                StartCoroutine(transitionToDay());
                break;

            case HourState.Evening:
                StartCoroutine(transitionToEvening());
                break;

            case HourState.Night:
                StartCoroutine(transitionToNight());
                break;
        }
    }

    private IEnumerator transitionToDay()
    {
        yield return null;
    }

    private IEnumerator transitionToEvening()
    {
        moon.enabled = false;
        sun.enabled = true;

        yield return StartCoroutine(StartSunTransition());
    }

    private IEnumerator transitionToNight()
    {
        moon.enabled = false;
        sun.enabled = true;

        yield return StartCoroutine(StartSunTransition());

        sun.enabled = false;
        moon.enabled = true;

        yield return StartCoroutine(StartMoonTransition());
    }

    private IEnumerator StartSunTransition()
    {
        float elapsedTime = 0f;
        while (elapsedTime < transitionDuration)
        {
            if(GetCurrentHourState() == HourState.Day)
            {
                sun.transform.rotation = Quaternion.Slerp(sunHideRot, sunDayRot, elapsedTime / transitionDuration);
            } else if(GetCurrentHourState() == HourState.Evening)
            {
                sun.transform.rotation = Quaternion.Slerp(sunDayRot, sunEvenRot, elapsedTime / transitionDuration);
            } else if(GetCurrentHourState() == HourState.Night)
            {
                sun.transform.rotation = Quaternion.Slerp(sunEvenRot, sunHideRot, elapsedTime / transitionDuration);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator StartMoonTransition()
    {
        float elapsedTime = 0f;
        while (elapsedTime < transitionDuration)
        {
            if (GetCurrentHourState() == HourState.Night)
            {
                moon.transform.rotation = Quaternion.Slerp(moonHideRot, moonNightRot, elapsedTime / transitionDuration);
            }
            else if (GetCurrentHourState() == HourState.Day)
            {
                moon.transform.rotation = Quaternion.Slerp(moonNightRot, moonHideRot, elapsedTime / transitionDuration);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private HourState GetCurrentHourState()
    {
        float time = gameManager.GetGamePlayingTimerNormalized();

        if (time <= 0.4f)
        {
            return HourState.Day;
        }
        else if (time <= 0.6f)
        {
            return HourState.Evening;
        }
        else
        {
            return HourState.Night;
        }
    }

}
