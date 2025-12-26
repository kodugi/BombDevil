using UnityEngine;

namespace Entity
{
    public class StageCommonData
    {
        public float walkDuration;
        public float knockbackDuration;
        public Color enemyColor;

        public StageCommonData(float walkDuration, float knockbackDuration, Color enemyColor)
        {
            this.walkDuration = walkDuration;
            this.knockbackDuration = knockbackDuration;
            this.enemyColor = enemyColor;
        }
    }
}
