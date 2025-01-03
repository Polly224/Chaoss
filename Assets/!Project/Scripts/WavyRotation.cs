using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavyRotation : MonoBehaviour
{
    [SerializeField] float rotationSpeed;
    [SerializeField] float rotationAmount;
    float val = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        val += Time.deltaTime * rotationSpeed;
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(val) * rotationAmount);
    }

    
}
