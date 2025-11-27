using UnityEngine;

namespace Entity
{
    public class StageCommonData
    {
        public float walkDuration;
        public float knockbackDuration;
        public int knockbackDistance;
        public Color enemyColor;
        public Color auxiliaryBombColor;
        public Color tileColor1;
        public Color tileColor2;

        public StageCommonData(float walkDuration, float knockbackDuration,
            int knockbackDistance, Color enemyColor, Color auxiliaryBombColor,
            Color tileColor1, Color tileColor2)
        {
            this.walkDuration = walkDuration;
            this.knockbackDuration = knockbackDuration;
            this.knockbackDistance = knockbackDistance;
            this.enemyColor = enemyColor;
            this.auxiliaryBombColor = auxiliaryBombColor;
            this.tileColor1 = tileColor1;
            this.tileColor2 = tileColor2;
        }
    }
}