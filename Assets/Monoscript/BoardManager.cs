using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public GameObject tile;
    public Color tileColor1;
    public Color tileColor2;
    public Transform board;
    public GameManager gameManager;
    
    private int width;
    private int height;

    void Awake()
    {
        width = gameManager.width;
        height = gameManager.height;
    }
    void Start()
    {
        int tileXIndex;
        int tileYIndex;
        for (tileXIndex = 0; tileXIndex < width; tileXIndex++)
        {
            for (tileYIndex = 0; tileYIndex < height; tileYIndex++)
            {
                if (tileXIndex % 2 == tileYIndex % 2)
                    createTile(tileXIndex, tileYIndex, tileColor1);
                else
                    createTile(tileXIndex, tileYIndex, tileColor2);
            }
        }
    }

    private void createTile(int x, int y, Color color)
    {
        GameObject tileObj = Instantiate(tile, calculatePosition(x, y),  Quaternion.identity, board);
        tileObj.GetComponent<SpriteRenderer>().color = color;
    }
    
    private Vector3 calculatePosition(int x, int y)
    {
        float x_coordination = x - (width - 1) / 2f;
        float y_coordination = y - (height - 1) / 2f;
        return new Vector3(x_coordination, y_coordination, 0);
    }
}
