using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex : MonoBehaviour, IHex
{
    public int color { get; set; } // Hücre renginin Core'daki Index'i
    public Vector2 location { get; set; } // Hücre lokasyonu

    void Update()
    {
        if (Vector2.Distance(transform.position, location) > 0.0f)
        {
            transform.position = Vector2.MoveTowards(transform.position, location, Time.deltaTime * 30);
        }
    }

    public void Create(int hexColor, Vector2 hexPos) // Altıgen hücrenin yaratım metodu, Renk ve pozisyon altıgene atanır
    {
        color = hexColor; // Renk Index'i atanır
        GetComponent<SpriteRenderer>().color = Camera.main.GetComponent<Core>().colors[hexColor]; // Sprite rengi index numarasına tanımlanmış renk ile değiştirilir.

        location = hexPos; // Hücre pozisyonu, animasyonlu hareket için kullanılmak üzere değişkene atılır.
    }

    public void Break() // Hücreyi yok etmek için çağırılacak metot.
    {
        Camera.main.GetComponent<Core>().point += 5;
        Camera.main.GetComponent<Core>().hexTiles += 1;
        Destroy(this);
    }
}
