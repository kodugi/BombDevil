using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public GameObject tile;
    public Color tileColor1;
    public Color tileColor2;
    public Transform board;
    public GameManager gameManager;
    
    private int _width;
    private int _height;

    void Awake()
    {
        _width = gameManager.width;
        _height = gameManager.height;
    }
    void Start()
    {
        int tileXIndex;
        int tileYIndex;
        for (tileXIndex = 0; tileXIndex < _width; tileXIndex++)
        {
            for (tileYIndex = 0; tileYIndex < _height; tileYIndex++)
            {
                if (tileXIndex % 2 == tileYIndex % 2)
                    CreateTile(tileXIndex, tileYIndex, tileColor1);
                else
                    CreateTile(tileXIndex, tileYIndex, tileColor2);
            }
        }
    }

    private void CreateTile(int x, int y, Color color)
    {
        GameObject tileObj = Instantiate(tile, CalculatePosition(x, y),  Quaternion.identity, board);
        tileObj.GetComponent<SpriteRenderer>().color = color;
    }
    
    private Vector3 CalculatePosition(int x, int y)
    {
        float x_coordination = x - (_width - 1) / 2f;
        float y_coordination = y - (_height - 1) / 2f;
        return new Vector3(x_coordination, y_coordination, 0);
    }
}
