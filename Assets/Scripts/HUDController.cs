﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public Color IdleColor;
    public Color RunningColor;

    public GameObject Player;
    public GameObject Destination;

    private float refTime = 0;
    private bool timerIsRunning = false;
    private Text timerText;
    private GameObject wayfinder;
    private GameObject toolbox;
    private GameObject toolboxbg;
    private GameObject minimap;
    private GameMaster GM;

    private Rect minimapRect;
    void Start()
    {
        this.wayfinder = transform.Find("Wayfinder").gameObject;
        this.toolbox = GameObject.Find("Toolbox");
        this.toolboxbg = GameObject.Find("ToolboxBg");
        this.toolbox.SetActive(false);
        this.toolboxbg.SetActive(false);

        this.timerText = GameObject.Find("Timer").GetComponent<Text>();
        this.timerText.text = "0:00";
        this.timerText.color = IdleColor;

        this.minimap = GameObject.Find("MinimapMapMap");
        this.minimapRect = minimap.GetComponent<RectTransform>().rect;

        GM = GameObject.Find("GameMaster").GetComponent<GameMaster>();

        //StartCoroutine(TestTheTimers());

    }

    void Update()
    {
        if (this.timerIsRunning) {
            this.timerText.text = this.FormattedTime();
        }

        if (this.Destination != null) {
            this.wayfinder.SetActive(true);
            var vec = this.Destination.transform.InverseTransformDirection(this.Destination.transform.position - this.Player.GetComponent<PlayerController>().REALTransform.position);
            var ang = Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;
            this.wayfinder.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, ang);
        } else {
            this.wayfinder.SetActive(false);
        }
    }

    IEnumerator TestTheTimers() {
        yield return new WaitForSeconds(2);
        this.StartTimer(Time.time);
        yield return new WaitForSeconds(6.3f);
        this.StopTimer();
        this.ShowToolbox();
    }

    public void StartTimer(float time)
    {
        this.refTime = time;
        this.timerIsRunning = true;
        this.timerText.color = RunningColor;
    }

    public float StopTimer() {
        this.timerIsRunning = false;
        float duration = Time.time - this.refTime;
        StartCoroutine(StopAnimation());
        return duration;
    }

    public void SetActiveTool(Tools tool) {
        Debug.Log(tool);
        GameObject.Find("GameMaster").GetComponent<GameMaster>().CurrentJobScript.SetTool(tool);
        this.HideToolbox();
    }

    public void ShowToolbox() {
        this.toolbox.SetActive(true);
        this.toolboxbg.SetActive(true);
    }

    public void HideToolbox() {
        this.toolbox.SetActive(false);
        this.toolboxbg.SetActive(false);
    }

    public void UpdatePosition(float xpos, float ypos) {
        minimap.GetComponent<RectTransform>().anchoredPosition = new Vector2(
            minimapRect.width * (1 - xpos) - minimapRect.width / 2,
            minimapRect.height * (1 - ypos) - minimapRect.height / 2
        );
    }

    IEnumerator StopAnimation()
    {
        var originalColor = this.timerText.color;
        for (int i = 0; i < 4; i++)
        {
            var flashTime = 0.3f;
            this.timerText.color = Color.clear;
            yield return new WaitForSeconds(flashTime);
            this.timerText.color = originalColor;
            yield return new WaitForSeconds(flashTime);
        }
        this.refTime = 0;
        this.timerText.text = "0:00"; 
        this.timerText.color = IdleColor;
    }

    string FormattedTime()
    {
        float duration = Time.time - this.refTime;
        var span = new TimeSpan(0, 0, 0, (int)duration);

        if (span.Hours > 0) {
            return $"{span.Hours}:{pad(span.Minutes)}:{pad(span.Seconds)}";
        }
        return $"{span.Minutes}:{pad(span.Seconds)}.{pad((int)((duration % 1)*100))}";
    }

    string pad(int number) {
        return number < 10 ? $"0{number}" : number.ToString();
    }
}
