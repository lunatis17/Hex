using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
    public List<Color> colors = new List<Color>
    {
        Color.white,
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow
    };

    public int point = 0;
    public int hexTiles = 0;
    public int bombs = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public interface IHex
{
    int color { get; set; }
    Vector2 location { get; set; }
    void Create(int hexColor);
    void Break();
}