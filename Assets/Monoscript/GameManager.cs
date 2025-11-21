using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // singleton
    public static GameManager Instance;
    
    // setting option
    public int width;
    public int height;
    public int enemyNumber;
    public int knockbackDistance;
    public float walkDuration;
    public float knockbackDuration;
    public Color enemyColor;
    public int initialAuxiliaryBomb;
    public Color auxiliaryBombColor;
    public Color tileColor1;
    public Color tileColor2;

    public EnemyManager enemyManager;
    public BombManager bombManager;
    
    // scoping board situation
    private GameObject[,] _board;
    
    // tracking auxiliary bomb order
    private List<Vector2Int> _auxiliaryBombs;

    void Awake()
    {
        Instance = this;
        _board = new GameObject[width, height];
        _auxiliaryBombs = new List<Vector2Int>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            MouseClickProcess();
    }

    // delete previous enemy
    // and create enemy on the board in random space
    public void CreateEnemy()
    {
        DeleteEnemy();
        
        int currentEnemy = 0;
        while (currentEnemy < enemyNumber)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);
            if (_board[x, y] == null)
            {
                _board[x, y] = enemyManager.CreateEnemy(x, y);
                currentEnemy++;
            }
        }
    }
    
    // delete previous enemy
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
                if (_board[x, y] && _board[x, y].GetComponent<Enemy>() != null)
                    _board[x, y] = null;
            }
        }
    }

    // explode all auxiliary bomb and push enemy
    public void ExplodeAuxiliaryBomb()
    {
        while (_auxiliaryBombs.Count > 0)
        {
            Vector2Int bombCoordinate = _auxiliaryBombs[0];
            _auxiliaryBombs.RemoveAt(0);
            int x = bombCoordinate.x;
            int y = bombCoordinate.y;
            GameObject bomb = _board[x, y];

            bomb.GetComponent<AuxiliaryBomb>().Explode();

            Knockback(x, y);
        }
    }

    // find enemy nearby (x,y) and push them (call Enemy.Knockback)
    private void Knockback(int x, int y)
    {
        //Up
        if (_board[x, (y + 1)%height]
                           && _board[x, (y + 1)%height].GetComponent<Enemy>() != null)
        {
            _board[x, (y + 1)%height].GetComponent<Enemy>().Knockback(Direction.Up, knockbackDistance);
        }
        //Down
        if (_board[x, ((y - 1)%height + height)%height]
                  && _board[x, ((y - 1)%height + height)%height].GetComponent<Enemy>() != null)
        {
            _board[x, ((y - 1)%height + height)%height].GetComponent<Enemy>().Knockback(Direction.Down, knockbackDistance);
        }
        //Right
        if (_board[(x + 1)%width, y]
                          && _board[(x + 1)%width, y].GetComponent<Enemy>() != null)
        {
            _board[(x + 1)%width, y].GetComponent<Enemy>().Knockback(Direction.Right, knockbackDistance);
        }
        //Left
        if (_board[((x - 1)%width + width)%width, y]
                  && _board[((x - 1)%width + width)%width, y].GetComponent<Enemy>() != null)
        {
            _board[((x - 1)%width + width)%width, y].GetComponent<Enemy>().Knockback(Direction.Left, knockbackDistance);
        }
        //UpRight
        if (_board[(x + 1)%width, (y + 1)%height]
            && _board[(x + 1)%width, (y + 1)%height].GetComponent<Enemy>() != null)
        {
            _board[(x + 1)%width, (y + 1)%height].GetComponent<Enemy>().Knockback(Direction.UpRight, knockbackDistance);
        }
        //DownLeft
        if (_board[((x - 1)%width + width)%width, ((y - 1)%height + height)%height]
            && _board[((x - 1)%width + width)%width, ((y - 1)%height + height)%height].GetComponent<Enemy>() != null)
        {
            _board[((x - 1)%width + width)%width, ((y - 1)%height + height)%height].GetComponent<Enemy>().Knockback(Direction.DownLeft, knockbackDistance);
        }
        //DownRight
        if (_board[(x + 1)%width, ((y - 1)%height + height)%height]
            && _board[(x + 1)%width, ((y - 1)%height + height)%height].GetComponent<Enemy>() != null)
        {
            _board[(x + 1)%width, ((y - 1)%height + height)%height].GetComponent<Enemy>().Knockback(Direction.DownRight, knockbackDistance);
        }
        //UpLeft
        if (_board[((x - 1)%width + width)%width, (y + 1)%height]
            && _board[((x - 1)%width + width)%width, (y + 1)%height].GetComponent<Enemy>() != null)
        {
            _board[((x - 1)%width + width)%width, (y + 1)%height].GetComponent<Enemy>().Knockback(Direction.UpLeft, knockbackDistance);
        }
    }

    // when mouse click occurs on the board, plant auxiliary bomb in the cell
    private void MouseClickProcess()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        int x = GlobalToGridX(worldPos.x);
        int y = GlobalToGridY(worldPos.y);

        if (x >= 0 && x < width && y >= 0 && y < height
            && _board[x, y] == null)
        {
            CreateAuxiliaryBomb(x, y);
        }
    }
    
    // if (x,y) is empty, fill the cell by an auxiliary bomb
    private void CreateAuxiliaryBomb(int x, int y)
    {
        if (_board[x, y] != null)
            return;

        _board[x, y] = bombManager.PlantAuxiliaryBomb(x, y);
        _auxiliaryBombs.Add(new Vector2Int(x, y));
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
