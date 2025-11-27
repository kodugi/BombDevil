using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Up,
    Down,
    Right,
    Left,
    UpRight,
    UpLeft,
    DownRight,
    DownLeft
}

public class EnemyManager : MonoBehaviour
{

    public GameObject enemy;
    public Transform enemySet;
    
    // internal variables (get from GameManager)
    private int _width;
    private int _height;
    private Color _enemyColor;
    
    public void Initialize(GameObject enemy, Transform enemySet, GameManager gameManager)
    {
        this.enemy = enemy;
        this.enemySet = enemySet;
        _width = gameManager.GetWidth();
        _height = gameManager.GetHeight();
        _enemyColor = gameManager.GetEnemyColor();
    }

    // create enemy API (call from GameManager)
    public GameObject CreateEnemy(int x, int y)
    {
        GameObject enemyObj = Instantiate(enemy, CalculatePosition(x, y), Quaternion.identity, enemySet);
        enemyObj.GetComponent<SpriteRenderer>().color = _enemyColor;
        return enemyObj;
    }
    
    // grid coordinate -> global coordinate
    private Vector3 CalculatePosition(int x, int y)
    {
        float xCoordination = x - (_width - 1) / 2f;
        float yCoordination = y - (_height - 1) / 2f;
        return new Vector3(xCoordination, yCoordination, 0);
    }
}
