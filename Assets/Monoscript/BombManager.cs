using TMPro;
using UnityEngine;

public class BombManager : MonoBehaviour
{
    public GameObject auxiliaryBomb;
    public Transform auxiliaryBombSet;
    public TMP_Text leftoverAuxiliaryBombText;

    // internal variables
    private int _width;
    private int _height;
    private int _leftoverAuxiliaryBomb;
    private Color _auxiliaryBombColor;

    public void Initialize(GameObject auxiliaryBomb, GameManager gameManager, Transform auxiliaryBombSet, TMP_Text leftoverAuxiliaryBombText)
    {
        this.auxiliaryBomb = auxiliaryBomb;
        this.auxiliaryBombSet = auxiliaryBombSet;
        this.leftoverAuxiliaryBombText = leftoverAuxiliaryBombText;
        _width = gameManager.GetWidth();
        _height = gameManager.GetHeight();
        _leftoverAuxiliaryBomb = gameManager.GetInitialAuxiliaryBomb();
        _auxiliaryBombColor = gameManager.GetAuxiliaryBombColor();
        leftoverAuxiliaryBombText.text = $"leftover: {_leftoverAuxiliaryBomb}";
    }
    
    // planting auxiliary bomb API (call from GameManager)
    public GameObject PlantAuxiliaryBomb(int x, int y)
    {
        if (_leftoverAuxiliaryBomb <= 0)
            return null;
        
        GameObject bomb = Instantiate(auxiliaryBomb, CalculatePosition(x, y), Quaternion.identity, auxiliaryBombSet);
        bomb.GetComponent<SpriteRenderer>().color = _auxiliaryBombColor;

        _leftoverAuxiliaryBomb--;
        leftoverAuxiliaryBombText.text = $"leftover: {_leftoverAuxiliaryBomb}";
        
        return bomb;
    }
    
    // grid coordinate -> global coordinate
    private Vector3 CalculatePosition(int x, int y)
    {
        float xCoordination = x - (_width - 1) / 2f;
        float yCoordination = y - (_height - 1) / 2f;
        return new Vector3(xCoordination, yCoordination, 0);
    }
}
