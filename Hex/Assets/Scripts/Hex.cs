using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex : MonoBehaviour, IHex
{
    public int color { get; set; }
    public Vector2 location { get; set; }

    public void Create(int hexColor)
    {
        color = hexColor;
        GetComponent<SpriteRenderer>().color = Camera.main.GetComponent<Core>().colors[hexColor];
    }

    public void Break()
    {
        Camera.main.GetComponent<Core>().point += 5;
        Camera.main.GetComponent<Core>().hexTiles += 1;
        Destroy(this);
    }
}
