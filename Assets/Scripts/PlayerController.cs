﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Mode { Truck, Person }

public class ModeChangedEventArgs : EventArgs
{
    public readonly Transform Follow;
    public readonly Mode Mode;
    public ModeChangedEventArgs(Transform follow, Mode mode)
    {
        Mode = mode;
        Follow = follow;
    }
}

public class PlayerController : MonoBehaviour
{
    public GameObject truck;
    public GameObject person;

    public Transform REALTransform
    {
        get => IsTruck ? truck.transform : person.transform;
    }

    private bool IsTruck = true;

    public event EventHandler<ModeChangedEventArgs> ModeChanged;
    private void OnModeChanged(ModeChangedEventArgs e)
    {
        EventHandler<ModeChangedEventArgs> handler = ModeChanged;
        handler?.Invoke(this, e);
    }

    void Start()
    {
        SwitchToInCar();
        //person.SetActive(false);
    }

    void Update()
    {
    }

    public void SwitchToOnFoot()
    {
        IsTruck = false;
        truck.GetComponent<CarController>().ToggleOff();
        person.transform.position = new Vector2(truck.transform.position.x - 1, truck.transform.position.y);
        person.SetActive(true);
        OnModeChanged(new ModeChangedEventArgs(person.transform, Mode.Person));
    }

    public void SwitchToInCar()
    {
        IsTruck = true;
        person.SetActive(false);
        truck.GetComponent<CarController>().ToggleOn();
        OnModeChanged(new ModeChangedEventArgs(truck.transform, Mode.Truck));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }
}
