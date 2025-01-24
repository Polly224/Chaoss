using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDisplay : MonoBehaviour
{
    public void ShowSetPieces(bool show)
    {
        if (show)
        {
            for(int i = 0; i < PlayerSet.instance.roundSet.Count; i++)
            {
                GameObject p = PlayerSet.instance.roundSet[i];
                p.transform.SetParent(transform, false);
                p.transform.GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sortingOrder += 60;
                p.transform.localPosition = new(i - 3.3f, 0, 0);
            }
        }
        else
        {
            for (int i = 0; i < PlayerSet.instance.roundSet.Count; i++)
            {
                GameObject p = PlayerSet.instance.roundSet[i];
                p.transform.SetParent(null, false);
                p.transform.GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sortingOrder -= 60;
                p.transform.position = new(8.6f, -1, 1);
            }
        }
    }
}
