using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Core))] // Çekirdek Sistem Component'ini gerektirir.
public class GridSystem : MonoBehaviour
{
    [Header("Altıgen Prefab'ı (Beyaz Renkte Olmalı)")]
    public GameObject HexSample; // Altıgen Hücre Örneği.
    [Header("Bomba Prefab'ı (Beyaz Renkte Olmalı)")]
    public GameObject BombSample; // Bomba Örneği.
    [Header("Pivot Prefab'ı")]
    public GameObject PivotSample; // Pivot örneği.

    [Header("Offset'ler")] // Hücreler arası yatay ve dikey uzaklıkları ifade eder.
    public float XOffset = 3.0f;
    public float YOffset = 3.5f;

    [Header("Yatay Hücre Sayısı")]
    [Range(2, 98)]
    public int XTile = 8; // Yatay eksendeki hücre sayısını bildirir.

    [Header("Dikey Hücre Sayısı")]
    [Range(2, 98)]
    public int YTile = 9; // Dikey eksendeki hücre sayısını bildirir.

    public GameObject[,] grid; // Hücreler GameObject olarak bu 2 boyutlu Array içinde tutulacak.
    public GameObject[,] pivots;

    public GameObject activePivot;



    // Start is called before the first frame update
    void Start()
    {
        grid = new GameObject[YTile, XTile]; // Belirlenen sayılarda altıgen hücre için 2 boyutlu Array'da yer açılıyor.
        pivots = new GameObject[((YTile - 1) * 2), (XTile - 1)]; // Altıgen hücre sayısına bağlı olacak şekilde pivotlara yer açılıyor.
        Build(); // Hücreler oluşturuluyor
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    void Build() // Desenin oluşturulduğu metot.
    {
        // Önce altıgen hücrelerimizi oluşturuyoruz.
        StartCoroutine(FillHexDelayed());

        // Sonra pivotlarımızı yerleştiriyoruz.
        CreatePivots();
    }



    public Vector2 ConvertToHexPos(int x, int y) // Normal koordinatları altıgen koordinatlara dönüştüren metot. (0,0) noktası her zaman merkezde tutulur.
    {
        return new Vector2(((x + 0.5f) * XOffset) - (XTile * XOffset / 2), ((y + 0.5f) * YOffset) + (x % 2 * (YOffset / 2)) - (YTile * YOffset / 2));
    }

    public Vector2 ConvertToPivotPos(float x, float y) // Normal koordinatları pivot koordinatlarına dönüştüren metot. (0,0) noktası her zaman merkezde tutulur.
    {
        // Pivotları doğru noktalarda konumlandırmak için mantıksal sihrimizi yapıyoruz :)
        float PivotOffset = y % 2 == 0 ? (x % 2 == 0 ? -0.5f : 0.5f) : (x % 2 == 0 ? 0.5f : -0.5f);
        return new Vector2(((x + 1.0f) * XOffset) - (XTile * XOffset / 2) + PivotOffset, (y * (YOffset / 2)) + (YOffset) - (YTile * YOffset / 2));
    }



    public void SelectPivot(GameObject pivot)
    {
        activePivot = pivot;

        Debug.Log("Pivot 0: " + pivot.GetComponent<Pivot>().linkedHexagons[0]);
        Debug.Log("Pivot 1: " + pivot.GetComponent<Pivot>().linkedHexagons[1]);
        Debug.Log("Pivot 2: " + pivot.GetComponent<Pivot>().linkedHexagons[2]);
        Debug.Log("Vector: " + pivot.GetComponent<Pivot>().pivotVector);
    }

    public void TurnLeft()
    {
        Debug.Log("Turn Left");

        Pivot pivot = activePivot.GetComponent<Pivot>();
        Vector2 b = grid[pivot.linkedHexagons[0].y, pivot.linkedHexagons[0].x].GetComponent<IHex>().location;
        Vector2 c = grid[pivot.linkedHexagons[1].y, pivot.linkedHexagons[1].x].GetComponent<IHex>().location;
        Vector2 a = grid[pivot.linkedHexagons[2].y, pivot.linkedHexagons[2].x].GetComponent<IHex>().location;

        grid[pivot.linkedHexagons[0].y, pivot.linkedHexagons[0].x].GetComponent<IHex>().location = a;
        grid[pivot.linkedHexagons[1].y, pivot.linkedHexagons[1].x].GetComponent<IHex>().location = b;
        grid[pivot.linkedHexagons[2].y, pivot.linkedHexagons[2].x].GetComponent<IHex>().location = c;

        GameObject[] hex = new GameObject[3];
        hex[2] = grid[pivot.linkedHexagons[2].y, pivot.linkedHexagons[2].x];
        hex[1] = grid[pivot.linkedHexagons[1].y, pivot.linkedHexagons[1].x];
        hex[0] = grid[pivot.linkedHexagons[0].y, pivot.linkedHexagons[0].x];

        grid[pivot.linkedHexagons[2].y, pivot.linkedHexagons[2].x] = hex[0];
        grid[pivot.linkedHexagons[1].y, pivot.linkedHexagons[1].x] = hex[2];
        grid[pivot.linkedHexagons[0].y, pivot.linkedHexagons[0].x] = hex[1];
    }

    public void TurnRight()
    {
        Debug.Log("Turn Right");

        Pivot p = activePivot.GetComponent<Pivot>();
        Vector2 c = grid[p.linkedHexagons[0].y, p.linkedHexagons[0].x].GetComponent<IHex>().location;
        Vector2 a = grid[p.linkedHexagons[1].y, p.linkedHexagons[1].x].GetComponent<IHex>().location;
        Vector2 b = grid[p.linkedHexagons[2].y, p.linkedHexagons[2].x].GetComponent<IHex>().location;

        grid[p.linkedHexagons[0].y, p.linkedHexagons[0].x].GetComponent<IHex>().location = a;
        grid[p.linkedHexagons[1].y, p.linkedHexagons[1].x].GetComponent<IHex>().location = b;
        grid[p.linkedHexagons[2].y, p.linkedHexagons[2].x].GetComponent<IHex>().location = c;

        GameObject[] hex = new GameObject[3];
        hex[2] = grid[p.linkedHexagons[2].y, p.linkedHexagons[2].x];
        hex[1] = grid[p.linkedHexagons[1].y, p.linkedHexagons[1].x];
        hex[0] = grid[p.linkedHexagons[0].y, p.linkedHexagons[0].x];

        grid[p.linkedHexagons[2].y, p.linkedHexagons[2].x] = hex[1];
        grid[p.linkedHexagons[1].y, p.linkedHexagons[1].x] = hex[0];
        grid[p.linkedHexagons[0].y, p.linkedHexagons[0].x] = hex[2];
    }

    IEnumerator FillHexDelayed()
    {
        for (int y = 0; y < YTile; y++)
        {
            for (int x = 0; x < XTile; x++)
            {
                if (grid[y, x] == null)
                {
                    yield return new WaitForSeconds(0.005f * x); // Kısa süreli gecikmelerle taşları sahneye çağrıyoruz.
                    GameObject hex = Instantiate(HexSample); // Hücre sahneye çağrılıyor.
                    grid[y, x] = hex; // Hücre Array'ımıza ekleniyor.
                    hex.transform.position = ConvertToHexPos(x, 15 + y); // Koordinatlar altıgen sıralamaya göre dönüştürülerek hücre konumlandırılıyor.
                    int randomColor = Random.Range(0, GetComponent<Core>().colors.Count); // Core'dan alınmak üzere rastgele renk kodu belirleniyor.
                    hex.GetComponent<IHex>().Create(randomColor, ConvertToHexPos(x, y)); // Hücre yaratım metodu çağrılıyor.
                }
            }
        }

        Camera.main.GetComponent<Core>().onMove = false; // Hareket sonlanır. Oyuncu artık hamle yapabilir.
    }

    void CreatePivots()
    {
        for (int y = 0; y < (YTile - 1) * 2; y++)
        {
            for (int x = 0; x < XTile - 1; x++)
            {

                GameObject pivot = Instantiate(PivotSample);
                Pivot pivotData = pivot.GetComponent<Pivot>();

                pivots[y, x] = pivot;
                pivot.transform.position = ConvertToPivotPos(x, y);

                // Pivotlarımıza onları çevreleyen hücrelerin Array Index'lerini veriyoruz.
                pivotData.pivotVector = new Vector2Int(x, y); //
                float q = y % 2 == 0 ? 1 : 0.5f;
                if (y % 2 == 0)
                {
                    if (x % 2 == 0)
                    {
                        // (P,P)
                        pivotData.linkedHexagons[0] = new Vector2Int(x, (int)(y / 2 + q));
                        pivotData.linkedHexagons[1] = new Vector2Int(x + 1, (int)(y / 2));
                        pivotData.linkedHexagons[2] = new Vector2Int(x, (int)(y / 2));
                    }
                    else
                    {
                        // (N,P)
                        pivotData.linkedHexagons[0] = new Vector2Int(x + 1, (int)(y / 2 + q));
                        pivotData.linkedHexagons[1] = new Vector2Int(x + 1, (int)(y / 2));
                        pivotData.linkedHexagons[2] = new Vector2Int(x, (int)(y / 2));
                    }
                }
                else
                {
                    if (x % 2 == 0)
                    {
                        // (P,N)
                        pivotData.linkedHexagons[0] = new Vector2Int(x + 1, (int)(y / 2 + (2* q)));
                        pivotData.linkedHexagons[1] = new Vector2Int(x + 1, (int)(y / 2));
                        pivotData.linkedHexagons[2] = new Vector2Int(x, (int)(y / 2 + (2 * q)));
                    }
                    else
                    {
                        // (N,N)
                        pivotData.linkedHexagons[0] = new Vector2Int(x, (int)(y / 2 + (2 * q)));
                        pivotData.linkedHexagons[1] = new Vector2Int(x + 1, (int)(y / 2 + (2 * q)));
                        pivotData.linkedHexagons[2] = new Vector2Int(x, (int)(y / 2 + q));
                    }
                }
            }
        }
    }
}
