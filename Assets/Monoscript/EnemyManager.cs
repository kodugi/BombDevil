using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemy;
    public Color enemyColor;
    public Transform enemySet;
    public GameManager gameManager;
    
    private int width;
    private int height;
    
    void Awake()
    {
        width = gameManager.width;
        height = gameManager.height;
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
    
    private Vector3 calculatePosition(int x, int y)
    {
        float x_coordination = x - (width - 1) / 2f;
        float y_coordination = y - (height - 1) / 2f;
        return new Vector3(x_coordination, y_coordination, 0);
    }
}
