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
            EffectProcessor.ExecuteEffect(
                new Effects.EffectData { type = Effects.EffectType.Damage, value = 5 },
                enemy,
                player
            );

            // Tick statuses for enemy turn
            player.statusManager.TickStatuses(TickTiming.StartOfTurn);
            enemy.statusManager.TickStatuses(TickTiming.StartOfTurn);

            Phase = BattlePhase.PlayerTurn;
            Debug.Log("Back to Player Turn");
        }
    }
}
