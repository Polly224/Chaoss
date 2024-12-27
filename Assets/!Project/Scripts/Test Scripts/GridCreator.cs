using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCreator : MonoBehaviour
{
    [SerializeField] GameObject square;
    void Start()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                GameObject intSquare = Instantiate(square, new Vector3(i, j, 0), Quaternion.identity);
                intSquare.transform.SetParent(transform, true);
                intSquare.GetComponent<SpriteRenderer>().material.color = (i + j) % 2 != 0 ? new Color(134f / 255f, 165f / 255f, 190f / 255f) : new Color(75f / 255f, 92f / 255f, 106f / 255f);
            }
        }
    }
}
