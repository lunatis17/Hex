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
        StartCoroutine(FillHexDelayed()); // Bu metodu daha sonra kırılan hücrelerin yerlerini doldurmak için de kullanacağız.

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
    }

    public List<Vector2Int> CheckHex(Vector2Int[] index)
    {
        List<Vector2Int> hexToBreak = new List<Vector2Int>();

        foreach (Vector2Int i in index)
        {
            Vector2Int[] vectors = new Vector2Int[6];
            if (i.x % 2 == 0)
            {
                vectors[0] = new Vector2Int(i.x - 1, i.y);
                vectors[1] = new Vector2Int(i.x, i.y + 1);
                vectors[2] = new Vector2Int(i.x + 1, i.y);
                vectors[3] = new Vector2Int(i.x + 1, i.y - 1);
                vectors[4] = new Vector2Int(i.x, i.y - 1);
                vectors[5] = new Vector2Int(i.x - 1, i.y - 1);
            }
            else
            {
                vectors[0] = new Vector2Int(i.x - 1, i.y + 1);
                vectors[1] = new Vector2Int(i.x, i.y + 1);
                vectors[2] = new Vector2Int(i.x + 1, i.y + 1);
                vectors[3] = new Vector2Int(i.x + 1, i.y);
                vectors[4] = new Vector2Int(i.x, i.y - 1);
                vectors[5] = new Vector2Int(i.x - 1, i.y);
            }

            List<GameObject> matches = new List<GameObject>();

            for (int v = 0; v < 5; v++)
            {
                try
                {
                    if (v == 0)
                    {
                        if (grid[i.y, i.x].GetComponent<IHex>().color == grid[vectors[5].y, vectors[5].x].GetComponent<IHex>().color)
                        {
                            matches.Add(grid[vectors[5].y, vectors[5].x]);
                        }
                    }
                    
                    if (grid[i.y, i.x].GetComponent<IHex>().color == grid[vectors[v].y, vectors[v].x].GetComponent<IHex>().color)
                    {
                        matches.Add(grid[vectors[v].y, vectors[v].x]);
                    }
                    else
                    {
                        if (matches.Count > 1)
                        {
                            matches.Add(grid[i.y, i.x]);
                            foreach (GameObject match in matches)
                            {
                                if(!hexToBreak.Exists(x => x == match.GetComponent<IHex>().index))
                                    hexToBreak.Add(match.GetComponent<IHex>().index);
                            }
                            matches.Clear();
                            //break;
                        }
                        else
                        {
                            matches.Clear();
                        }
                    }
                }
                catch
                {
                    if (matches.Count > 1)
                    {
                        matches.Add(grid[i.y, i.x]);
                        foreach (GameObject match in matches)
                        {
                            if (!hexToBreak.Exists(x => x == match.GetComponent<IHex>().index))
                                hexToBreak.Add(match.GetComponent<IHex>().index);
                        }
                        matches.Clear();
                        //break;
                    }
                    else
                    {
                        matches.Clear();
                    }
                }
                
            }
        }
        return hexToBreak;
    }

    public IEnumerator TurnLeft()
    {

        for (int i = 0; i <= 2; i++)
        {
            Pivot pivot = activePivot.GetComponent<Pivot>();

            // Hücrelerin hareket edecekleri lokasyonlar atanıyor.
            Vector2 b = grid[pivot.linkedHexagons[0].y, pivot.linkedHexagons[0].x].GetComponent<IHex>().location;
            Vector2 c = grid[pivot.linkedHexagons[1].y, pivot.linkedHexagons[1].x].GetComponent<IHex>().location;
            Vector2 a = grid[pivot.linkedHexagons[2].y, pivot.linkedHexagons[2].x].GetComponent<IHex>().location;

            grid[pivot.linkedHexagons[0].y, pivot.linkedHexagons[0].x].GetComponent<IHex>().location = a;
            grid[pivot.linkedHexagons[1].y, pivot.linkedHexagons[1].x].GetComponent<IHex>().location = b;
            grid[pivot.linkedHexagons[2].y, pivot.linkedHexagons[2].x].GetComponent<IHex>().location = c;

            // Hücrelerin GridSystem'de kayıtlı yerleri değiştiriliyor
            GameObject[] hex = new GameObject[3];
            hex[2] = grid[pivot.linkedHexagons[2].y, pivot.linkedHexagons[2].x];
            hex[1] = grid[pivot.linkedHexagons[1].y, pivot.linkedHexagons[1].x];
            hex[0] = grid[pivot.linkedHexagons[0].y, pivot.linkedHexagons[0].x];

            grid[pivot.linkedHexagons[2].y, pivot.linkedHexagons[2].x] = hex[0];
            grid[pivot.linkedHexagons[1].y, pivot.linkedHexagons[1].x] = hex[2];
            grid[pivot.linkedHexagons[0].y, pivot.linkedHexagons[0].x] = hex[1];

            // Hücrelerdeki indexler değiştiriliyor.
            hex[0].GetComponent<IHex>().index = new Vector2Int(pivot.linkedHexagons[2].x, pivot.linkedHexagons[2].y);
            hex[2].GetComponent<IHex>().index = new Vector2Int(pivot.linkedHexagons[1].x, pivot.linkedHexagons[1].y);
            hex[1].GetComponent<IHex>().index = new Vector2Int(pivot.linkedHexagons[0].x, pivot.linkedHexagons[0].y);

            yield return new WaitForSeconds(0.2f);

            // Değişen hücreler için eşleşmeler kontrol ediliyor.
            Vector2Int[] checkList = new Vector2Int[] { hex[0].GetComponent<IHex>().index, hex[1].GetComponent<IHex>().index, hex[2].GetComponent<IHex>().index };
            List<Vector2Int> result = CheckHex(checkList);
            if (result.Count > 0)
            {
                foreach (Vector2Int res in result)
                {
                    grid[res.y, res.x].GetComponent<IHex>().Break();
                }

                StartCoroutine(FillHexDelayed());
                break;
            }

        }
    }

    public IEnumerator TurnRight()
    {

        for (int i = 0; i <= 2; i++)
        {
            Pivot pivot = activePivot.GetComponent<Pivot>();

            Vector2 c = grid[pivot.linkedHexagons[0].y, pivot.linkedHexagons[0].x].GetComponent<IHex>().location;
            Vector2 a = grid[pivot.linkedHexagons[1].y, pivot.linkedHexagons[1].x].GetComponent<IHex>().location;
            Vector2 b = grid[pivot.linkedHexagons[2].y, pivot.linkedHexagons[2].x].GetComponent<IHex>().location;

            grid[pivot.linkedHexagons[0].y, pivot.linkedHexagons[0].x].GetComponent<IHex>().location = a;
            grid[pivot.linkedHexagons[1].y, pivot.linkedHexagons[1].x].GetComponent<IHex>().location = b;
            grid[pivot.linkedHexagons[2].y, pivot.linkedHexagons[2].x].GetComponent<IHex>().location = c;

            GameObject[] hex = new GameObject[3];
            hex[2] = grid[pivot.linkedHexagons[2].y, pivot.linkedHexagons[2].x];
            hex[1] = grid[pivot.linkedHexagons[1].y, pivot.linkedHexagons[1].x];
            hex[0] = grid[pivot.linkedHexagons[0].y, pivot.linkedHexagons[0].x];

            grid[pivot.linkedHexagons[2].y, pivot.linkedHexagons[2].x] = hex[1];
            grid[pivot.linkedHexagons[1].y, pivot.linkedHexagons[1].x] = hex[0];
            grid[pivot.linkedHexagons[0].y, pivot.linkedHexagons[0].x] = hex[2];

            yield return new WaitForSeconds(0.2f);

            // Değişen hücreler için eşleşmeler kontrol ediliyor.
            Vector2Int[] checkList = new Vector2Int[] { hex[0].GetComponent<IHex>().index, hex[1].GetComponent<IHex>().index, hex[2].GetComponent<IHex>().index };
            List<Vector2Int> result = CheckHex(checkList);
            if (result.Count > 0)
            {
                foreach (Vector2Int res in result)
                {
                    grid[res.y, res.x].GetComponent<IHex>().Break();
                }

                StartCoroutine(FillHexDelayed());
                break;
            }
        }
    }

    IEnumerator FillHexDelayed()
    {
        for (int y = 0; y < YTile; y++)
        {
            for (int x = 0; x < XTile; x++)
            {
                if (grid[y, x] == null) // Hücre alanı boşsa dolduruyoruz.
                {
                    yield return new WaitForSeconds(0.005f * x); // Kısa süreli gecikmelerle hücreleri sahneye çağrıyoruz.
                    GameObject hex = Instantiate(HexSample); // Hücre sahneye çağrılıyor.
                    grid[y, x] = hex; // Hücre Array'ımıza ekleniyor.
                    hex.transform.position = ConvertToHexPos(x, 15 + y); // Koordinatlar altıgen sıralamaya göre dönüştürülerek hücre konumlandırılıyor.
                    int randomColor = Random.Range(0, GetComponent<Core>().colors.Count); // Core'dan alınmak üzere rastgele renk kodu belirleniyor.
                    hex.GetComponent<IHex>().Create(false, randomColor, ConvertToHexPos(x, y), new Vector2Int(x, y)); // Hücre yaratım metodu çağrılıyor.
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
