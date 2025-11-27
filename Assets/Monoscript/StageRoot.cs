using UnityEngine;
using TMPro;
using Entity;

public class StageRoot : MonoBehaviour
{
    // Manager Objects
    public GameManager gameManager;
    public EnemyManager enemyManager;
    public BombManager bombManager;
    public BoardManager boardManager;
    public GameObject enemy;
    public GameObject auxiliaryBomb;
    public GameObject tile;
    public Transform enemySet;
    public Transform auxiliaryBombSet;
    public Transform board;
    public TMP_Text leftoverAuxiliaryBombText;
    

    public void Install(int stageId, StageCommonData commonData)
    {
        gameManager = GetComponentInChildren<GameManager>();
        enemyManager = GetComponentInChildren<EnemyManager>();
        bombManager = GetComponentInChildren<BombManager>();
        boardManager = GetComponentInChildren<BoardManager>();
        
        gameManager.Initialize(enemyManager, bombManager, stageId, commonData);
        enemyManager.Initialize(enemy, enemySet, gameManager);
        bombManager.Initialize(auxiliaryBomb, gameManager, auxiliaryBombSet, leftoverAuxiliaryBombText);
        boardManager.Initialize(tile, board, gameManager);
    }
}
