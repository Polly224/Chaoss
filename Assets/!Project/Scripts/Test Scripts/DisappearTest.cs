using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DisappearTest : MonoBehaviour
{
    float holeSize = 1f;
    [SerializeField] float disappearSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<SpriteRenderer>().material.SetFloat("_HoleSize", holeSize);
        holeSize = Mathf.Lerp(holeSize, -0.2f, disappearSpeed * Time.deltaTime);
    }


    void Example()
    {
        int[] ints = { 1, 2, 3 };
    }
}
