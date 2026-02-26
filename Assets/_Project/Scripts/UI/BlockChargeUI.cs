using TMPro;
using UnityEngine;
using DB.Battle;

namespace DB.UI
{
    public class BlockChargeUI : MonoBehaviour
    {
        public BlockChargeManager charges;
        public TMP_Text text;

        private void Update()
        {
            if (!charges || !text) return;
            text.text = charges.Charges > 0 ? $"BLOCK: {charges.Charges}" : "";
        }
    }
}