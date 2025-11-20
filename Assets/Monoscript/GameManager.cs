using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // singleton
    public static GameManager Instance;
    
    public int width;
    public int height;
    public int enemyNumber;
    public EnemyManager enemyManager;
    public BombManager bombManager;
    public int knockbackDistance;
    public float walkDuration;
    public float knockbackDuration;
    public Color enemyColor;
    public int initialAuxiliaryBomb;
    public Color auxiliaryBombColor;
    public Color tileColor1;
    public Color tileColor2;

    public GameObject[,] Board;
    public List<Vector2Int> AuxiliaryBombs;

    void Awake()
    {
        Instance = this;
        Board = new GameObject[width, height];
        AuxiliaryBombs = new List<Vector2Int>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            MouseClickProcess();
    }

    public void CreateEnemy()
    {
        DeleteEnemy();
        
        int currentEnemy = 0;
        while (currentEnemy < enemyNumber)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);
            if (Board[x, y] == null)
            {
                Board[x, y] = enemyManager.CreateEnemy(x, y);
                currentEnemy++;
            }
        }
    }
    
    public void DeleteEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject e in enemies)
        {
            Destroy(e);
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (Board[x, y] && Board[x, y].GetComponent<Enemy>() != null)
                    Board[x, y] = null;
            }
        }
    }

    public void ExplodeAuxiliaryBomb()
    {
        while (AuxiliaryBombs.Count > 0)
        {
            Vector2Int bombCoordinate = AuxiliaryBombs[0];
            AuxiliaryBombs.RemoveAt(0);
            int x = bombCoordinate.x;
            int y = bombCoordinate.y;
            GameObject bomb = Board[x, y];

            bomb.GetComponent<AuxiliaryBomb>().Explode();

            Knockback(x, y);
        }
    }

    private void Knockback(int x, int y)
    {
        if (y < height - 1 && Board[x, y + 1]
                           && Board[x, y + 1].GetComponent<Enemy>() != null)
        {
            Board[x, y + 1].GetComponent<Enemy>().Knockback(Direction.North, knockbackDistance);
        }
        if (y > 0 && Board[x, y - 1]
                  && Board[x, y - 1].GetComponent<Enemy>() != null)
        {
            Board[x, y - 1].GetComponent<Enemy>().Knockback(Direction.South, knockbackDistance);
        }
        if (x < width - 1 && Board[x + 1, y]
                          && Board[x + 1, y].GetComponent<Enemy>() != null)
        {
            Board[x + 1, y].GetComponent<Enemy>().Knockback(Direction.East, knockbackDistance);
        }
        if (x > 0 && Board[x - 1, y]
                  && Board[x - 1, y].GetComponent<Enemy>() != null)
        {
            Board[x - 1, y].GetComponent<Enemy>().Knockback(Direction.West, knockbackDistance);
        }
    }

    private void MouseClickProcess()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        int x = GlobalToGridX(worldPos.x);
        int y = GlobalToGridY(worldPos.y);

        if (x >= 0 && x < width && y >= 0 && y < height
            && Board[x, y] == null)
        {
            CreateAuxiliaryBomb(x, y);
        }
    }
    private void CreateAuxiliaryBomb(int x, int y)
    {
        if (Board[x, y] != null)
            return;

        Board[x, y] = bombManager.PlantAuxiliaryBomb(x, y);
        AuxiliaryBombs.Add(new Vector2Int(x, y));
    }
    
    // global coordinate -> board coordinate
    private int GlobalToGridX(float x)
    {
        return Mathf.FloorToInt(x + width / 2f);
    }
    
    private int GlobalToGridY(float y)
    {   
        return Mathf.FloorToInt(y + height / 2f);
    }
}
