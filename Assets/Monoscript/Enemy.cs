using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    
    private float _walkDuration;
    private float _knockbackDuration;

    // initializing internal attribute
    void Awake()
    {
        _walkDuration = GameManager.Instance.walkDuration;
        _knockbackDuration = GameManager.Instance.knockbackDuration;
    }
    
    // walk API
    public void Walk(Direction direction)
    {
        StartCoroutine(Move(direction, 1, _walkDuration));
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
            transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        transform.position = target;
    }

    private static Vector3 GetTarget(Direction direction, int distance, Vector3 start)
    {
        switch (direction)
        {
            case Direction.North:
                return start + new Vector3(0, distance, 0);
            
            case Direction.South:
                return start - new Vector3(0, distance, 0);
            
            case Direction.East:
                return start + new Vector3(distance, 0, 0);
            
            case Direction.West:
                return start - new Vector3(distance, 0, 0);
        }

        return new Vector3();
    }
}
