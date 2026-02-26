using UnityEngine;
using DB.Core;
using DB.Statuses;
using DB.Effects;

namespace DB.Battle
{
    public enum BattlePhase { PlayerTurn, EnemyTurn }

    public class TurnManager : MonoBehaviour
    {
        public BattlePhase Phase { get; private set; } = BattlePhase.PlayerTurn;

        public Entity player;
        public Entity enemy;
        public BattleController battleController;

        public void EndPlayerTurn()
        {
            Debug.Log("End Player Turn");
            player.statusManager.TickStatuses(TickTiming.EndOfTurn);
            enemy.statusManager.TickStatuses(TickTiming.EndOfTurn);

            Phase = BattlePhase.EnemyTurn;
            EnemyTurn();
        }

        private void EnemyTurn()
        {
            Debug.Log("Enemy Turn");
            // Simple enemy attack simulation
            // TEMP: simple enemy action
            battleController.ResolveEnemyAttack(5);

            // Tick statuses for enemy turn
            player.statusManager.TickStatuses(TickTiming.StartOfTurn);
            enemy.statusManager.TickStatuses(TickTiming.StartOfTurn);

            Phase = BattlePhase.PlayerTurn;
            Debug.Log("Back to Player Turn");
        }
    }
}
