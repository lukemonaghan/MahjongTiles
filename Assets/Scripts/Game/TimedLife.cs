﻿using UnityEngine;
using System.Collections;

public class TimedLife : MonoBehaviour 
{
    public float lifeTime;
	void Start() 
    {
        Destroy(gameObject, lifeTime);
	}
}
