using UnityEngine;

namespace DB.Battle
{
    public class BattleInputTester : MonoBehaviour
    {
        public BattleController battle;

        private void Update()
        {
            if (battle == null) return;

            if (Input.GetKeyDown(KeyCode.Alpha1))
                battle.PlayCardFromHand(0);

            if (Input.GetKeyDown(KeyCode.Alpha2))
                battle.PlayCardFromHand(1);

            if (Input.GetKeyDown(KeyCode.Alpha3))
                battle.PlayCardFromHand(2);

            if (Input.GetKeyDown(KeyCode.Space))
                battle.EndPlayerTurn();
        }
    }
}