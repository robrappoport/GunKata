﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UtilScript : MonoBehaviour {
    
    public static float remapRange(float oldValue, float oldMin, float oldMax, float newMin, float newMax ){
        float newValue = 0;
        float oldRange = (oldMax - oldMin);
        float newRange = (newMax - newMin);
        newValue = (((oldValue - oldMin) * newRange) / oldRange) + newMin;
        return newValue;
    }
}
