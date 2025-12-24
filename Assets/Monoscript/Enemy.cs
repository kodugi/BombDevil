using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    // motion duration (get from GameManager)
    private float _walkDuration;
    private float _knockbackDuration;
    
    // cell size for scaling movement
    private float _cellSize;
    
    // direction attribute for this enemy (Up, Down, Left, Right only)
    private Direction _moveDirection;
    
    // boundary coordinate (get from BoardManager)
    private float _minX, _maxX, _minY, _maxY;

    // initializing internal attribute
    public void Initialize(GameManager gameManager, BoardManager boardManager, Sprite sprite)
    {
        _walkDuration = gameManager.getWalkDuration();
        _knockbackDuration = gameManager.getKnockbackDuration();
        _cellSize = boardManager.GetCellSize();
        _minX = boardManager.GetMinX();
        _maxX = boardManager.GetMaxX();
        _minY = boardManager.GetMinY();
        _maxY = boardManager.GetMaxY();
        
        // Set sprite if provided
        if (sprite != null)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = sprite;
            }
        }
    }
    
    // walk API
    public void Walk(Direction direction)
    {
        StartCoroutine(Move(direction, 1, _walkDuration));
    }
    
    // set random direction (Up, Down, Left, Right) and apply rotation
    public void SetRandomDirection()
    {
        Direction[] cardinalDirections = { Direction.Up, Direction.Down, Direction.Left, Direction.Right };
        int randomIndex = Random.Range(0, cardinalDirections.Length);
        _moveDirection = cardinalDirections[randomIndex];
        
        // Apply rotation based on direction
        ApplyDirectionRotation();
    }
    
    // Apply rotation based on move direction
    // Default sprite faces Down (0 degrees)
    private void ApplyDirectionRotation()
    {
        float rotationZ = 0f;
        
        switch (_moveDirection)
        {
            case Direction.Down:
                rotationZ = 0f;      // Default - facing down
                break;
            case Direction.Up:
                rotationZ = 180f;    // Facing up
                break;
            case Direction.Left:
                rotationZ = -90f;    // Facing left (or 270)
                break;
            case Direction.Right:
                rotationZ = 90f;     // Facing right
                break;
        }
        
        transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);
    }
    
    // get current direction
    public Direction GetMoveDirection()
    {
        return _moveDirection;
    }
    
    // move one cell in the assigned direction
    public void MoveInDirection()
    {
        Walk(_moveDirection);
    }
    
    // knockback API
    public void Knockback(Direction direction, int distance)
    {
        StartCoroutine(Move(direction, distance, _knockbackDuration));
    }
    
    // internal move API
    private IEnumerator Move(Direction direction, int distance, float duration)
    {
        Vector3 start = transform.position;
        Vector3 target = GetTarget(direction, distance, start);
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            transform.position = LerpWrap(start, target, t, _minX, _maxX, _minY, _maxY);
            yield return null;
        }

        transform.position = GetWrappedTarget(direction, distance, start, _minX, _maxX, _minY, _maxY);
    }

    // It can process boundary condition - wraps position within bounds
    private static Vector3 LerpWrap(Vector3 a, Vector3 b, float t,
        float minX = float.NegativeInfinity, float maxX = float.PositiveInfinity,
        float minY = float.NegativeInfinity, float maxY = float.PositiveInfinity,
        float minZ = float.NegativeInfinity, float maxZ = float.PositiveInfinity)
    {
        // Simple linear interpolation
        float x = a.x + (b.x - a.x) * t;
        float y = a.y + (b.y - a.y) * t;
        float z = a.z + (b.z - a.z) * t;
        
        // Wrap x within bounds
        float widthX = maxX - minX;
        if (widthX > 0)
        {
            while (x > maxX) x -= widthX;
            while (x < minX) x += widthX;
        }
        
        // Wrap y within bounds
        float widthY = maxY - minY;
        if (widthY > 0)
        {
            while (y > maxY) y -= widthY;
            while (y < minY) y += widthY;
        }
        
        // Wrap z within bounds
        float widthZ = maxZ - minZ;
        if (widthZ > 0)
        {
            while (z > maxZ) z -= widthZ;
            while (z < minZ) z += widthZ;
        }

        return new Vector3(x, y, z);
    }

    // get destination from direction, distance, start point (scaled by cellSize)
    private Vector3 GetTarget(Direction direction, int distance, Vector3 start)
    {
        float scaledDistance = distance * _cellSize;
        
        switch (direction)
        {
            case Direction.Up:
                return start + new Vector3(0, scaledDistance, 0);
            
            case Direction.Down:
                return start - new Vector3(0, scaledDistance, 0);
            
            case Direction.Right:
                return start + new Vector3(scaledDistance, 0, 0);
            
            case Direction.Left:
                return start - new Vector3(scaledDistance, 0, 0);
            
            case Direction.UpRight:
                return start + new Vector3(scaledDistance, scaledDistance, 0);
            
            case Direction.UpLeft:
                return start + new Vector3(-scaledDistance, scaledDistance, 0);
            
            case Direction.DownRight:
                return start + new Vector3(scaledDistance, -scaledDistance, 0);
            
            case Direction.DownLeft:
                return start - new Vector3(scaledDistance, scaledDistance, 0);
        }

        return new Vector3();
    }
    
    // get destination wrt boundary condition
    private Vector3 GetWrappedTarget(Direction direction, int distance, Vector3 start,
        float minX = float.NegativeInfinity, float maxX = float.PositiveInfinity,
        float minY = float.NegativeInfinity, float maxY = float.PositiveInfinity,
        float minZ = float.NegativeInfinity, float maxZ = float.PositiveInfinity)
    {

        Vector3 result = GetTarget(direction, distance, start);
        
        if (result.x < minX || result.x > maxX)
            result.x = Mod(result.x - minX, maxX - minX) + minX;
        if (result.y < minY || result.y > maxY)
            result.y = Mod(result.y - minY, maxY - minY) + minY;
        if (result.z < minZ || result.z > maxZ)
            result.z = Mod(result.z - minZ, maxZ - minZ) + minZ;

        return result;
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
}

