using UnityEngine;

namespace DB.Core
{
    public class ManaSystem : MonoBehaviour
    {
        public int maxMana = 3;
        public int currentMana = 3;

        public void ResetMana()
        {
            currentMana = maxMana;
            Debug.Log($"Mana reset: {currentMana}/{maxMana}");
        }

        public bool CanAfford(int cost) => currentMana >= cost;

        public void Spend(int cost)
        {
            currentMana = Mathf.Max(currentMana - cost, 0);
            Debug.Log($"Spent {cost} mana → {currentMana}/{maxMana}");
        }
    }
}