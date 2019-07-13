using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex : MonoBehaviour, IHex
{
    public bool isBomb { get; set; }
    public int color { get; set; } // Hücre renginin Core'daki Index'i
    public Vector2 location { get; set; } // Hücre lokasyonu
    public Vector2Int index { get; set; } // GridSystem'de kayıtlı olduğu index

    void Update()
    {
        if (Vector2.Distance(transform.position, location) > 0.0f)
        {
            transform.position = Vector2.MoveTowards(transform.position, location, Time.deltaTime * 30);
        }
    }

    public void Create(bool hexBomb, int hexColor, Vector2 hexPos, Vector2Int hexIndex) // Altıgen hücrenin yaratım metodu, Renk ve pozisyon altıgene atanır
    {
        isBomb = hexBomb;
        color = hexColor; // Renk Index'i atanır
        GetComponent<SpriteRenderer>().color = Camera.main.GetComponent<Core>().colors[hexColor]; // Sprite rengi, index numarasına tanımlanmış renk ile değiştirilir.

        location = hexPos; // Hücre pozisyonu, animasyonlu hareket için kullanılmak üzere değişkene atılır.
        index = hexIndex; // GridSystem'de kayıtlı olduğu index.
    }

    public void Break() // Hücreyi yok etmek için çağırılacak metot.
    {
        Camera.main.GetComponent<Core>().point += 5;
        Camera.main.GetComponent<Core>().hexTiles += 1;

        Camera.main.GetComponent<GridSystem>().grid[index.y, index.x] = null;
        Destroy(this.gameObject);
    }
}
