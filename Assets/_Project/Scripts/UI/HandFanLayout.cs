using UnityEngine;

public class HandFanLayout : MonoBehaviour
{
    public float radius = 600f;
    public float maxAngle = 50f;
    public float yOffset = 0f;

    private void LateUpdate()
    {
        Layout();
    }

    public void Layout()
    {
        int count = transform.childCount;
        if (count == 0) return;

        float half = (count - 1) * 0.5f;

        float degreesPerGap = 10f;
        float angleRange = Mathf.Clamp((count - 1) * degreesPerGap, 12f, maxAngle);
        float angleStep = (count > 1) ? angleRange / (count - 1) : 0f;

        for (int i = 0; i < count; i++)
        {
            var card = transform.GetChild(i) as RectTransform;
            if (!card) continue;

            float t = i - half;
            float angle = t * angleStep;
            float rad = angle * Mathf.Deg2Rad;

            float x = Mathf.Sin(rad) * radius;
            float y = Mathf.Cos(rad) * radius;

            Vector3 pos = new Vector3(x, y - radius + yOffset, 0f);

            card.localPosition = pos;
            card.localRotation = Quaternion.Euler(0f, 0f, -angle);
        }
    }
}