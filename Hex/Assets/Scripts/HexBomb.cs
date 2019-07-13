using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexBomb : MonoBehaviour, IHex
{
    public bool isBomb { get; set; }
    public int color { get; set; }
    public Vector2 location { get; set; }
    public Vector2Int index { get; set; }
    public int counter = 7;

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position, location) > 0.0f)
        {
            transform.position = Vector2.MoveTowards(transform.position, location, Time.deltaTime * 30);
        }
    }



    public void Create(bool hexBomb, int hexColor, Vector2 hexPos, Vector2Int hexIndex)
    {
        isBomb = hexBomb;
        color = hexColor; // Renk Index'i atanır
        GetComponent<SpriteRenderer>().color = Camera.main.GetComponent<Core>().colors[hexColor]; // Sprite rengi index numarasına tanımlanmış renk ile değiştirilir.
        location = hexPos; // Hücre pozisyonu, animasyonlu hareket için kullanılmak üzere değişkene atılır.
        index = hexIndex; // GridSystem'de kayıtlı olduğu index.
    }


    public void Break()
    {
        Camera.main.GetComponent<Core>().point += 5;
        Camera.main.GetComponent<Core>().bombs += 1;

        GridSystem gridSystem = Camera.main.GetComponent<GridSystem>();
        for (int y = 0; y < gridSystem.YTile; y++)
        {
            for (int x = 0; x < gridSystem.XTile; x++)
            {
                if (gridSystem.grid[y, x] != null && gridSystem.grid[y, x].GetComponent<IHex>().color == color)
                {
                    gridSystem.grid[y, x].GetComponent<IHex>().Break();
                }
            }
        }

        Camera.main.GetComponent<GridSystem>().grid[index.y, index.x] = null;
        Destroy(this.gameObject);
    }
}
