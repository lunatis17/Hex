using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexBomb : MonoBehaviour, IHex
{
    public int color { get; set; }
    public Vector2 location { get; set; }
    public int counter = 7;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void Create(int hexColor, Vector2 hexPos)
    {

    }


    public void Break()
    {

    }
}
