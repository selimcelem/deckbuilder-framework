using UnityEngine;

namespace DB.Battle
{
    public class BlockChargeManager : MonoBehaviour
    {
        [SerializeField] private int charges;
        public int Charges => charges;

        public void AddCharges(int amount)
        {
            charges = Mathf.Max(charges + amount, 0);
            Debug.Log($"Block charges: {charges}");
        }

        // called once after EACH enemy turn
        public void DecayAfterEnemyTurn()
        {
            if (charges > 0) charges--;
            Debug.Log($"Block charges after decay: {charges}");
        }
    }
}