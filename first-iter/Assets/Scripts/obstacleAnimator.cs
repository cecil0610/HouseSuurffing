﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class obstacleAnimator : MonoBehaviour
{
    public LeanTweenType easeType;
    public float amplitude;
    public float movementFrequency;

    private void OnEnable()
    {
        LeanTween.moveLocalY(gameObject, amplitude, 1/movementFrequency).setLoopPingPong().setEase(easeType);

        //LeanTween.moveLocalX(gameObject, 0f, 2f).setLoopPingPong();
    }

    public void OnClose()
    {
        Destroy(gameObject);
    }
}
