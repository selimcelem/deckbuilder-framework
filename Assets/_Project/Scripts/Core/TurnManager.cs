using UnityEngine;

namespace DB.Battle
{
    public enum BattlePhase { PlayerTurn, EnemyTurn, Resolving }

    public class TurnManager : MonoBehaviour
    {
        public BattlePhase Phase { get; private set; } = BattlePhase.PlayerTurn;

        public void EndPlayerTurn()
        {
            // TODO: tick statuses, apply end-of-turn effects, then switch phase
            Phase = BattlePhase.EnemyTurn;
            // TODO: enemy acts, then back to player
        }
    }
}
