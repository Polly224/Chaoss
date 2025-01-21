using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavyRotation : MonoBehaviour
{
    [SerializeField] float rotationSpeed;
    [SerializeField] float rotationAmount;
    float val = 0;
    // Rotates the piece's info window a bit. Looks cool. Looks awesome. I love sine waves.
    void Update()
    {
        val += Time.deltaTime * rotationSpeed;
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(val) * rotationAmount);
    }

    
}
