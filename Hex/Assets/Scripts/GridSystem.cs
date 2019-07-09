using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [Header("Altıgen Prefab'ı")]
    public GameObject HexSample;

    [Header("Offset'ler")]
    public float XOffset = 3.0f;
    public float YOffset = 3.5f;

    [Header("Altıgen Sayısı")]
    [Range(2, 98)]
    public int XTile = 8;

    [Range(2, 98)]
    public int YTile = 9;

    public GameObject[,] grid;

    // Start is called before the first frame update
    void Start()
    {
        grid = new GameObject[YTile, XTile];
        Build();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Build()
    {
        for (int y = 0; y < YTile; y++)
        {
            for (int x = 0; x < XTile; x++)
            {
                GameObject hex = Instantiate(HexSample);
                hex.transform.position = ConvertToHexPos(x, y);
                int randomColor = Random.Range(0, GetComponent<Core>().colors.Count - 1);
                hex.GetComponent<Hex>().Create(randomColor);
                grid[y, x] = hex;
            }
        }
    }

    public Vector2 ConvertToHexPos(int x, int y)
    {
        return new Vector2(((x + 0.5f) * XOffset) - (XTile * XOffset / 2), ((y + 0.5f) * YOffset) + (x % 2 * (YOffset / 2)) - (YTile * YOffset / 2));
    }
}
