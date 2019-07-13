using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MainCamera üzerinde çalıştırılır.
[RequireComponent(typeof(GridSystem))]
public class Core : MonoBehaviour
{
    public List<Color> colors = new List<Color> // Renkler, default olarak 5 adettir.
    {
        new Color(1.0f, 0.0f, 1.0f),
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow
    };

    public int move = 0;
    public int point = 0;
    public int hexTiles = 0;
    public int bombs = 0;
    
    public bool onMove = true;

    Vector2 clickPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            MouseDown();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            MouseUp();
        }
    }

    private void MouseDown()
    {
        clickPos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void MouseUp()
    {
        GridSystem gridSystem = GetComponent<GridSystem>();
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Yeni hücre seçimi veya aktif hücre döndürmesi gibi eylemlerden hangisinin gerçekleşeceğini MouseUp ve MouseDown olayları arasındaki mouse mesafesine göre belirliyoruz.
        if (Vector2.Distance(mousePos, clickPos) < 1)
        {
            // Hücre seçilmişse tıklanılan noktaya en yakın pivot'u tespit ediyoruz.
            float distance = float.MaxValue;
            GameObject selected = gridSystem.pivots[0,0];
            foreach (GameObject pivot in gridSystem.pivots)
            {
                if (Vector2.Distance(mousePos, pivot.transform.position) < distance)
                {
                    distance = Vector2.Distance(mousePos, pivot.transform.position);
                    selected = pivot;
                }
            }
            gridSystem.SelectPivot(selected); // En yakın pivot noktası GridSystem'e gönderilir.
        }
        else
        {
            // Döndürme olayı gerçekleşecekse bunun sağa mı yoksa sola mı olacağını belirliyoruz.
            if (gridSystem.activePivot != null)
            {
                onMove = true; // Hareket başladı.

                if (clickPos.y > gridSystem.activePivot.transform.position.y) // Sadece kaydırma yönü değil, üstünden veya altından kaydırmak da dönüş yönünü etkileyecek.
                {
                    if (clickPos.x - mousePos.x < 0)
                    {
                        StartCoroutine(gridSystem.TurnRight());
                    }
                    else
                    {
                        StartCoroutine(gridSystem.TurnLeft());
                    }
                }
                else
                {
                    if (clickPos.x - mousePos.x > 0)
                    {
                        StartCoroutine(gridSystem.TurnRight());
                    }
                    else
                    {
                        StartCoroutine(gridSystem.TurnLeft());
                    }
                }
            }
        }
    }
}

public interface IHex // Tüm Hexagon class'larında kullanılacak olan Interface
{
    bool isBomb { get; set; }
    int color { get; set; }
    Vector2 location { get; set; }
    Vector2Int index { get; set; }
    void Create(bool hexBomb, int hexColor, Vector2 hexPos, Vector2Int hexIndex);
    void Break();
}