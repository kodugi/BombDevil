using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Entity;

public class GameManager : MonoBehaviour
{
    // Manager references (set via Initialize)
    private EnemyManager enemyManager;
    private BombManager bombManager;
    private BoardManager boardManager;
    
    // setting option from StageManager (common set)
    private float _walkDuration;
    private float _knockbackDuration;
    private Color _enemyColor;
    
    // setting option from JSON file
    private int _width;
    private int _height;
    private int _enemyNumber;
    private int _initialBlueBomb;
    private int _initialGreenBomb;
    private int _initialPinkBomb;
    private string _boardSpritePath;  // Resources path to board sprite
    
    // scoping board situation - List to allow multiple objects per cell
    private List<GameObject>[,] _board;
    
    // tracking auxiliary bomb order
    private List<Vector2Int> _auxiliaryBombs;
    
    // boundary coordinate
    private float _minX, _minY, _maxX, _maxY;
    
    // game state management
    private GameState _currentState = GameState.Playing;
    public event Action<GameState> OnGameStateChanged;
    
    // turn processing flag (prevents button spam during animations)
    private bool _isTurnInProgress = false;

    public void Initialize(EnemyManager enemyManager, BombManager bombManager, int stageId, StageCommonData commonData)
    {
        // Load stage data first
        SetStageState(stageId);
        SetCommonData(commonData);
        
        this.enemyManager = enemyManager;
        this.bombManager = bombManager;
        
        // Initialize board with empty lists
        _board = new List<GameObject>[_width, _height];
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                _board[x, y] = new List<GameObject>();
            }
        }
        
        _auxiliaryBombs = new List<Vector2Int>();
        _currentState = GameState.Playing;
    }
    
    // Set BoardManager reference after it's initialized
    public void SetBoardManager(BoardManager boardManager)
    {
        this.boardManager = boardManager;
    }
    
    // Set common data from StageManager
    private void SetCommonData(StageCommonData commonData)
    {
        _walkDuration = commonData.walkDuration;
        _knockbackDuration = commonData.knockbackDuration;
        _enemyColor = commonData.enemyColor;
    }

    void Update()
    {
        // Skip if not initialized or not playing
        if (_board == null || _currentState != GameState.Playing)
            return;
            
        if (Input.GetMouseButtonDown(0))
            MouseClickProcess();
            
        CheckGameState();
    }
    
    // check win/lose conditions
    private void CheckGameState()
    {
        // Check win condition: all enemies eliminated
        if (GetEnemyCount() == 0)
        {
            SetGameState(GameState.Win);
            return;
        }
        
        // Check lose condition: no bombs left and no bombs placed
        if (bombManager.GetTotalLeftoverBombs() <= 0 && 
            bombManager.GetPlantedAuxiliaryBombCount() <= 0)
        {
            SetGameState(GameState.Lose);
            return;
        }
    }
    
    // set game state and trigger event
    private void SetGameState(GameState newState)
    {
        if (_currentState == newState)
            return;
            
        _currentState = newState;
        OnGameStateChanged?.Invoke(_currentState);
        Debug.Log($"Game State Changed: {_currentState}");
    }
    
    // get current game state
    public GameState GetCurrentState()
    {
        return _currentState;
    }
    
    // count enemies on the board
    private int GetEnemyCount()
    {
        int count = 0;
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                foreach (var obj in _board[x, y])
                {
                    if (obj != null && obj.GetComponent<Enemy>() != null)
                        count++;
                }
            }
        }
        return count;
    }

    // delete previous enemy
    // and create enemy on the board in random space
    public void CreateEnemy()
    {
        DeleteEnemy();
        
        int currentEnemy = 0;
        while (currentEnemy < _enemyNumber)
        {
            int x = UnityEngine.Random.Range(0, _width);
            int y = UnityEngine.Random.Range(0, _height);
            // Allow placing even if cell has objects (but check if empty for initial placement)
            if (_board[x, y].Count == 0)
            {
                GameObject enemy = enemyManager.CreateEnemy(x, y);
                _board[x, y].Add(enemy);
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
                _board[x, y].RemoveAll(obj => obj == null || obj.GetComponent<Enemy>() != null);
            }
        }
    }

    // Button click handler - explode bombs in order then move enemies
    public void OnExplodeButtonClick()
    {
        // Skip if not playing or turn is already in progress
        if (_currentState != GameState.Playing || _isTurnInProgress)
            return;
        
        // Start the turn sequence coroutine
        StartCoroutine(ExecuteTurnSequence());
    }
    
    // Execute turn sequence with proper timing
    private IEnumerator ExecuteTurnSequence()
    {
        // Set flag to prevent button spam
        _isTurnInProgress = true;
        
        // Explode all bombs in placement order (with waiting)
        yield return StartCoroutine(ExplodeAllBombsCoroutine());
        
        // Enemy turn: move all enemies in their direction
        yield return StartCoroutine(MoveAllEnemiesCoroutine());
        
        // Reset explode button text to PASS
        bombManager.ResetExplodeButtonText();
        
        // Check game state after turn
        CheckGameState();
        
        // Clear flag - turn is complete
        _isTurnInProgress = false;
    }
    
    // Explode all auxiliary bombs and push enemies (coroutine version)
    private IEnumerator ExplodeAllBombsCoroutine()
    {
        while (_auxiliaryBombs.Count > 0)
        {
            Vector2Int bombCoordinate = _auxiliaryBombs[0];
            _auxiliaryBombs.RemoveAt(0);
            int x = bombCoordinate.x;
            int y = bombCoordinate.y;
            
            // Find bomb in the cell
            GameObject bombObj = _board[x, y].Find(obj => obj != null && obj.GetComponent<AuxiliaryBomb>() != null);
            
            if (bombObj == null)
                continue;
            
            // Get bomb properties before exploding
            AuxiliaryBomb bomb = bombObj.GetComponent<AuxiliaryBomb>();
            int range = bomb.GetRange();
            int knockbackDistance = bomb.GetKnockbackDistance();
            
            // Remove bomb from board
            _board[x, y].Remove(bombObj);
            
            bomb.Explode();
            
            // Apply knockback with bomb's specific properties
            Knockback(x, y, range, knockbackDistance);
            
            // Wait for knockback animation to complete
            yield return new WaitForSeconds(_knockbackDuration);
        }
    }
    
    // Move all enemies in their assigned direction (coroutine version)
    private IEnumerator MoveAllEnemiesCoroutine()
    {
        // Collect all enemies first to avoid modification during iteration
        List<(int x, int y, GameObject obj, Enemy enemy)> enemies = new List<(int, int, GameObject, Enemy)>();
        
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                foreach (var obj in _board[x, y])
                {
                    if (obj != null)
                    {
                        Enemy enemy = obj.GetComponent<Enemy>();
                        if (enemy != null)
                        {
                            enemies.Add((x, y, obj, enemy));
                        }
                    }
                }
            }
        }
        
        // Move each enemy
        foreach (var (x, y, obj, enemy) in enemies)
        {
            Direction dir = enemy.GetMoveDirection();
            
            // Move enemy visually
            enemy.MoveInDirection();
            
            // Update board position
            ReflectMoveInBoard(x, y, obj, dir, 1);
        }
        
        // Wait for walk animation to complete
        if (enemies.Count > 0)
        {
            yield return new WaitForSeconds(_walkDuration);
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

    // Get initial bomb count by type
    public int GetInitialBombCount(BombType bombType)
    {
        switch (bombType)
        {
            case BombType.BlueBomb:
                return _initialBlueBomb;
            case BombType.GreenBomb:
                return _initialGreenBomb;
            case BombType.PinkBomb:
                return _initialPinkBomb;
            default:
                return 0;
        }
    }

    public string GetBoardSpritePath()
    {
        return _boardSpritePath;
    }



    // find enemy within range of (x,y) and push them (call Enemy.Knockback)
    // range: how far from bomb center enemies are affected (1 = adjacent, 2 = 2 tiles away)
    // knockbackDistance: how far enemies are pushed back
    private void Knockback(int x, int y, int range, int knockbackDistance)
    {
        // Check all 8 directions
        Direction[] directions = { Direction.Up, Direction.Down, Direction.Right, Direction.Left,
                                   Direction.UpRight, Direction.UpLeft, Direction.DownRight, Direction.DownLeft };
        int[,] dirOffsets = { {0, 1}, {0, -1}, {1, 0}, {-1, 0}, {1, 1}, {-1, 1}, {1, -1}, {-1, -1} };
        
        // Collect enemies to knockback (to avoid modification during iteration)
        List<(int targetX, int targetY, GameObject obj, Enemy enemy, Direction dir)> toKnockback = 
            new List<(int, int, GameObject, Enemy, Direction)>();
        
        // Apply knockback to enemies within range
        for (int r = 1; r <= range; r++)
        {
            for (int d = 0; d < directions.Length; d++)
            {
                int targetX = Mod(x + dirOffsets[d, 0] * r, _width);
                int targetY = Mod(y + dirOffsets[d, 1] * r, _height);
                
                foreach (var obj in _board[targetX, targetY])
                {
                    if (obj != null)
                    {
                        Enemy enemy = obj.GetComponent<Enemy>();
                        if (enemy != null)
                        {
                            toKnockback.Add((targetX, targetY, obj, enemy, directions[d]));
                        }
                    }
                }
            }
        }
        
        // Apply knockback
        foreach (var (targetX, targetY, obj, enemy, dir) in toKnockback)
        {
            enemy.Knockback(dir, knockbackDistance);
            ReflectMoveInBoard(targetX, targetY, obj, dir, knockbackDistance);
        }
    }
    
    // update board when object movement occurs
    private void ReflectMoveInBoard(int x, int y, GameObject obj, Direction direction, int distance)
    {
        if (obj == null)
            return;

        Vector3 target = GetWrappedTarget(direction, distance, new Vector3(x, y, 0), 0, _width, 0, _height);
        
        // Remove from old position
        _board[x, y].Remove(obj);
        
        // Add to new position
        _board[(int)target.x, (int)target.y].Add(obj);
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
    // or select bomb type from UI area
    private void MouseClickProcess()
    {
        Vector3 screenPos = Input.mousePosition;
        
        // Check if clicking on bomb selection UI area (pixel coordinates)
        // X range: 480~820 for all bombs
        if (screenPos.x >= 1440 && screenPos.x <= 1780)
        {
            // Check Y ranges for each bomb type
            if (screenPos.y >= 890 && screenPos.y <= 990)
            {
                // BlueBomb selection
                bombManager.SetCurrentBombType(BombType.BlueBomb);
                return;
            }
            else if (screenPos.y >= 760 && screenPos.y <= 860)
            {
                // GreenBomb selection
                bombManager.SetCurrentBombType(BombType.GreenBomb);
                return;
            }
            else if (screenPos.y >= 630 && screenPos.y <= 730)
            {
                // PinkBomb selection  
                bombManager.SetCurrentBombType(BombType.PinkBomb);
                return;
            }
        }
        
        // Check if a bomb type is selected
        if (!bombManager.HasBombSelected())
            return;
        
        // Handle board click for bomb placement
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

        int x = GlobalToGridX(worldPos.x);
        int y = GlobalToGridY(worldPos.y);

        if (x >= 0 && x < _width && y >= 0 && y < _height
            && _board[x, y].Count == 0)
        {
            CreateAuxiliaryBomb(x, y);
        }
    }
    
    // if (x,y) is empty, fill the cell by an auxiliary bomb
    private void CreateAuxiliaryBomb(int x, int y)
    {
        if (_board[x, y].Count > 0)
            return;

        GameObject bomb = bombManager.PlantAuxiliaryBomb(x, y);
        if (bomb != null)
        {
            _board[x, y].Add(bomb);
            _auxiliaryBombs.Add(new Vector2Int(x, y));
        }
    }
    
    // global coordinate -> board coordinate (with cell size scaling)
    private int GlobalToGridX(float x)
    {
        float cellSize = boardManager.GetCellSize();
        return Mathf.FloorToInt(x / cellSize + _width / 2f);
    }
    
    private int GlobalToGridY(float y)
    {
        float cellSize = boardManager.GetCellSize();
        return Mathf.FloorToInt(y / cellSize + _height / 2f);
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
        // Load from Resources/Json folder (works in both editor and build)
        TextAsset jsonFile = Resources.Load<TextAsset>("Json/Stage/stage" + stageId);
        if (jsonFile == null)
        {
            Debug.LogError($"Failed to load stage{stageId}.json from Resources/Json/Stage/");
            return;
        }
        
        StageDifferentData differentData = JsonUtility.FromJson<StageDifferentData>(jsonFile.text);
        _width = differentData.width;
        _height = differentData.height;
        _enemyNumber = differentData.enemyNumber;
        _initialBlueBomb = differentData.initialBlueBomb;
        _initialGreenBomb = differentData.initialGreenBomb;
        _initialPinkBomb = differentData.initialPinkBomb;
        _boardSpritePath = differentData.boardSpritePath;
    }
}
