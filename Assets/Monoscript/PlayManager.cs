using System.Collections.Generic;
using UnityEngine;

public class PlayManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject enemy;
    public GameObject tile;
    public int width;
    public int height;
    public Color tileColor1;
    public Color tileColor2;
    public Color enemyColor;
    public Transform board;
    public Transform enemySet;
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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateEnemy(int enemyNumber)
    {
        DeleteEnemy();
        
        HashSet<Vector2Int> coordination = new HashSet<Vector2Int>();
        int currentEnemy = 0;
        while (currentEnemy < enemyNumber)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);
            if (!coordination.Contains(new Vector2Int(x, y)))
            {
                coordination.Add(new Vector2Int(x, y));
                GameObject enemyObj = Instantiate(enemy, calculatePosition(x, y), Quaternion.identity, enemySet);
                enemyObj.GetComponent<SpriteRenderer>().color = enemyColor;
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
