using System.Collections.Generic;
using UnityEngine;

public class PlayManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject enemy;
    public int width;
    public int height;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateEnemy(int enemyNumber)
    {
        DeleteEnemy();
        
        HashSet<Vector2Int> trajectory = new HashSet<Vector2Int>();
        int currentEnemy = 0;
        while (currentEnemy < enemyNumber)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);
            if (!trajectory.Contains(new Vector2Int(x, y)))
            {
                trajectory.Add(new Vector2Int(x, y));
                Instantiate(enemy, new Vector3(x - (width - 1) / 2f, y - (height - 1) / 2f, 0), Quaternion.identity);
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
}
