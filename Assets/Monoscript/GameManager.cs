using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Entity;

public class GameManager : MonoBehaviour
{
    
    public EnemyManager enemyManager;
    public BombManager bombManager;
    
    // setting option from StageManager (common set)
    private int _knockbackDistance;
    private float _walkDuration;
    private float _knockbackDuration;
    private Color _enemyColor;
    private Color _auxiliaryBombColor;
    private Color _tileColor1; 
    private Color _tileColor2;
    
    // setting option from JSON file
    private int _width;
    private int _height;
    private int _enemyNumber;
    private int _initialAuxiliaryBomb;
    
    // scoping board situation
    private GameObject[,] _board;
    
    // tracking auxiliary bomb order
    private List<Vector2Int> _auxiliaryBombs;
    
    // boundary coordinate
    private float _minX, _minY, _maxX, _maxY;

    public void Initialize(EnemyManager enemyManager, BombManager bombManager, int stageId, StageCommonData commonData)
    {
        this.enemyManager = enemyManager;
        this.bombManager = bombManager;
        _board = new GameObject[_width, _height];
        _auxiliaryBombs = new List<Vector2Int>();
        _minX = -_width / 2.0f;
        _maxX = _width / 2.0f;
        _minY = -_height / 2.0f;
        _maxY = _height / 2.0f;
        
        SetStageState(stageId);
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
        while (currentEnemy < _enemyNumber)
        {
            int x = Random.Range(0, _width);
            int y = Random.Range(0, _height);
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

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
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
    
    // get boundary coordinate API
    public float GetMaxX()
    {
        return _maxX;
    }
    
    public float GetMinX()
    {
        return _minX;
    }
    
    public float GetMaxY()
    {
        return _maxY;
    }
    
    public float GetMinY()
    {
        return _minY;
    }
    
    // get private variable API
    public int getKnockbackDistance()
    {
        return _knockbackDistance;
    }

    public float getWalkDuration()
    {
        return _walkDuration;
    }

    public float getKnockbackDuration()
    {
        return _knockbackDuration;
    }

    public Color GetEnemyColor()
    {
        return _enemyColor;
    }

    public Color GetAuxiliaryBombColor()
    {
        return _auxiliaryBombColor;
    }

    public Color GetTileColor1()
    {
        return _tileColor1;
    }

    public Color GetTileColor2()
    {
        return _tileColor2;
    }

    public int GetWidth()
    {
        return _width;
    }

    public int GetHeight()
    {
        return _height;
    }

    public int GetEnemyNumber()
    {
        return _enemyNumber;
    }

    public int GetInitialAuxiliaryBomb()
    {
        return _initialAuxiliaryBomb;
    }

    // find enemy nearby (x,y) and push them (call Enemy.Knockback)
    private void Knockback(int x, int y)
    {
        //Up
        if (_board[x, Mod(y + 1, _height)]
                           && _board[x, Mod(y + 1, _height)].GetComponent<Enemy>() != null)
        {
            _board[x, Mod(y + 1, _height)].GetComponent<Enemy>().Knockback(Direction.Up, _knockbackDistance);
            ReflectMoveInBoard(x, Mod(y + 1, _height), Direction.Up, _knockbackDistance);
        }
        //Down
        if (_board[x, Mod(y - 1, _height)]
                  && _board[x, Mod(y - 1, _height)].GetComponent<Enemy>() != null)
        {
            _board[x, Mod(y - 1, _height)].GetComponent<Enemy>().Knockback(Direction.Down, _knockbackDistance);
            ReflectMoveInBoard(x, Mod(y - 1, _height), Direction.Down, _knockbackDistance);
        }
        //Right
        if (_board[Mod(x + 1, _width), y]
                          && _board[Mod(x + 1, _width), y].GetComponent<Enemy>() != null)
        {
            _board[Mod(x + 1, _width), y].GetComponent<Enemy>().Knockback(Direction.Right, _knockbackDistance);
            ReflectMoveInBoard(Mod(x + 1, _width), y, Direction.Right, _knockbackDistance);
        }
        //Left
        if (_board[Mod(x - 1, _width), y]
                  && _board[Mod(x - 1, _width), y].GetComponent<Enemy>() != null)
        {
            _board[Mod(x - 1, _width), y].GetComponent<Enemy>().Knockback(Direction.Left, _knockbackDistance);
            ReflectMoveInBoard(Mod(x - 1, _width), y, Direction.Left, _knockbackDistance);
        }
        //UpRight
        if (_board[Mod(x + 1, _width), Mod(y + 1, _height)]
            && _board[Mod(x + 1, _width), Mod(y + 1, _height)].GetComponent<Enemy>() != null)
        {
            _board[Mod(x + 1, _width), Mod(y + 1, _height)].GetComponent<Enemy>().Knockback(Direction.UpRight, _knockbackDistance);
            ReflectMoveInBoard(Mod(x + 1, _width), Mod(y + 1, _height), Direction.UpRight, _knockbackDistance);
        }
        //DownLeft
        if (_board[Mod(x - 1, _width), Mod(y - 1, _height)]
            && _board[Mod(x - 1, _width), Mod(y - 1, _height)].GetComponent<Enemy>() != null)
        {
            _board[Mod(x - 1, _width), Mod(y - 1, _height)].GetComponent<Enemy>().Knockback(Direction.DownLeft, _knockbackDistance);
            ReflectMoveInBoard(Mod(x - 1, _width), Mod(y - 1, _height), Direction.DownLeft, _knockbackDistance);
        }
        //DownRight
        if (_board[Mod(x + 1, _width), Mod(y - 1, _height)]
            && _board[Mod(x + 1, _width), Mod(y - 1, _height)].GetComponent<Enemy>() != null)
        {
            _board[Mod(x + 1, _width), Mod(y - 1, _height)].GetComponent<Enemy>().Knockback(Direction.DownRight, _knockbackDistance);
            ReflectMoveInBoard(Mod(x + 1, _width), Mod(y - 1, _height), Direction.DownRight, _knockbackDistance);
        }
        //UpLeft
        if (_board[Mod(x - 1, _width), Mod(y + 1, _height)]
            && _board[Mod(x - 1, _width), Mod(y + 1, _height)].GetComponent<Enemy>() != null)
        {
            _board[Mod(x - 1, _width), Mod(y + 1, _height)].GetComponent<Enemy>().Knockback(Direction.UpLeft, _knockbackDistance);
            ReflectMoveInBoard(Mod(x - 1, _width), Mod(y + 1, _height), Direction.UpLeft, _knockbackDistance);
        }
    }
    
    // update board when object movement occurs
    private void ReflectMoveInBoard(int x, int y, Direction direction, int distance)
    {
        GameObject obj = _board[x, y];
        if (obj == null)
            return;

        Vector3 target = GetWrappedTarget(direction, distance, new Vector3(x, y, 0), 0, _width, 0, _height);
        if (_board[(int)target.x, (int)target.y] != null)
        {
            // collision
        }

        _board[(int)target.x, (int)target.y] = obj;
        _board[x, y] = null;

    }
    
    // get destination from direction, distance, start point
    private static Vector3 GetTarget(Direction direction, int distance, Vector3 start)
    {
        switch (direction)
        {
            case Direction.Up:
                return start + new Vector3(0, distance, 0);
            
            case Direction.Down:
                return start - new Vector3(0, distance, 0);
            
            case Direction.Right:
                return start + new Vector3(distance, 0, 0);
            
            case Direction.Left:
                return start - new Vector3(distance, 0, 0);
            
            case Direction.UpRight:
                return start + new Vector3(distance, distance, 0);
            
            case Direction.UpLeft:
                return start + new Vector3(-distance, distance, 0);
            
            case Direction.DownRight:
                return start + new Vector3(distance, -distance, 0);
            
            case Direction.DownLeft:
                return start - new Vector3(distance, distance, 0);
        }

        return new Vector3();
    }
    
    // get destination wrt boundary condition
    private static Vector3 GetWrappedTarget(Direction direction, int distance, Vector3 start,
        float minX = float.NegativeInfinity, float maxX = float.PositiveInfinity,
        float minY = float.NegativeInfinity, float maxY = float.PositiveInfinity,
        float minZ = float.NegativeInfinity, float maxZ = float.PositiveInfinity)
    {

        Vector3 result = GetTarget(direction, distance, start);
        
        if (result.x < minX || result.x >= maxX)
            result.x = Mod(result.x - minX, maxX - minX) + minX;
        if (result.y < minY || result.y >= maxY)
            result.y = Mod(result.y - minY, maxY - minY) + minY;
        if (result.z < minZ || result.z >= maxZ)
            result.z = Mod(result.z - minZ, maxZ - minZ) + minZ;

        return result;
    }

    // when mouse click occurs on the board, plant auxiliary bomb in the cell
    private void MouseClickProcess()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        int x = GlobalToGridX(worldPos.x);
        int y = GlobalToGridY(worldPos.y);

        if (x >= 0 && x < _width && y >= 0 && y < _height
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
        return Mathf.FloorToInt(x + _width / 2f);
    }
    
    private int GlobalToGridY(float y)
    {   
        return Mathf.FloorToInt(y + _height / 2f);
    }

    private static float Mod(float x, int m)
    {
        return (x % m + m) % m;
    }
    
    private static float Mod(float x, float m)
    {
        return (x % m + m) % m;
    }
    
    private static int Mod(int x, int m)
    {
        return (x % m + m) % m;
    }
    
    // init setting from stage json file
    private void SetStageState(int stageId)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Json", "Stage", "stage" + stageId + ".json");
        string json = File.ReadAllText(path);
        StageDifferentData differentData = JsonUtility.FromJson<StageDifferentData>(json);
        _width = differentData.width;
        _height = differentData.height;
        _enemyNumber = differentData.enemyNumber;
        _initialAuxiliaryBomb = differentData.initialAuxiliaryBomb;
    }
}
