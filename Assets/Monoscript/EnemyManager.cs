using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    North,
    South,
    East,
    West
}

public class EnemyManager : MonoBehaviour
{
    // singleton
    public static EnemyManager Instance;
    
    public GameObject enemy;
    public Color enemyColor;
    public Transform enemySet;
    public GameManager gameManager;
    public float walkDuration;
    public float knockbackDuration;
    
    private int _width;
    private int _height;
    
    void Awake()
    {
        Instance = this;
        _width = gameManager.width;
        _height = gameManager.height;
    }
    
    public void CreateEnemy(int enemyNumber)
    {
        DeleteEnemy();
        
        HashSet<Vector2Int> coordination = new HashSet<Vector2Int>();
        int currentEnemy = 0;
        while (currentEnemy < enemyNumber)
        {
            int x = Random.Range(0, _width);
            int y = Random.Range(0, _height);
            if (!coordination.Contains(new Vector2Int(x, y)))
            {
                coordination.Add(new Vector2Int(x, y));
                GameObject enemyObj = Instantiate(enemy, CalculatePosition(x, y), Quaternion.identity, enemySet);
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
    
    private Vector3 CalculatePosition(int x, int y)
    {
        float x_coordination = x - (_width - 1) / 2f;
        float y_coordination = y - (_height - 1) / 2f;
        return new Vector3(x_coordination, y_coordination, 0);
    }
}
