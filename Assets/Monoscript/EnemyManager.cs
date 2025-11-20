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

    public GameObject enemy;
    public Transform enemySet;
    public GameManager gameManager;
    
    private int _width;
    private int _height;
    private Color _enemyColor;
    
    void Awake()
    {
        _width = gameManager.width;
        _height = gameManager.height;
        _enemyColor = gameManager.enemyColor;
    }

    public GameObject CreateEnemy(int x, int y)
    {
        GameObject enemyObj = Instantiate(enemy, CalculatePosition(x, y), Quaternion.identity, enemySet);
        enemyObj.GetComponent<SpriteRenderer>().color = _enemyColor;
        return enemyObj;
    }
    
    private Vector3 CalculatePosition(int x, int y)
    {
        float xCoordination = x - (_width - 1) / 2f;
        float yCoordination = y - (_height - 1) / 2f;
        return new Vector3(xCoordination, yCoordination, 0);
    }
}
