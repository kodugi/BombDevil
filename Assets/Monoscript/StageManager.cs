using UnityEngine;
using Entity;

public class StageManager : MonoBehaviour
{
    // setting option (common in all stages)
    public int knockbackDistance;
    public float walkDuration;
    public float knockbackDuration;
    public Color enemyColor;
    public Color auxiliaryBombColor;
    public Color tileColor1;
    public Color tileColor2;
    
    // StageRoot prefab
    public GameObject stageRootPrefab;
    // current stage
    public GameObject currStage;

    // init new stage
    public void StageInitialize(int stageId)
    {
        StageDestroy();
        currStage = Instantiate(stageRootPrefab);
        StageCommonData commonData = new StageCommonData(walkDuration, knockbackDuration, knockbackDistance,
            enemyColor, auxiliaryBombColor, tileColor1, tileColor2);
        currStage.GetComponent<StageRoot>().Install(stageId, commonData);
    }

    // destroy current stage
    public void StageDestroy()
    {
        if (currStage != null)
            Destroy(currStage);
    }
}
