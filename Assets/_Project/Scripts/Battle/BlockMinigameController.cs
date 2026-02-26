using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DB.Battle
{
    public enum BlockOutcome { Miss, Block, Perfect }

    public class BlockMinigameController : MonoBehaviour
    {
        [Header("UI")]
        public GameObject panel;
        public RectTransform shield;
        public RectTransform ball;
        public TMP_Text message;

        [Header("Tuning")]
        public float shieldSpeed = 1200f;
        public float ballSpeed = 700f;
        public float wallBounceX = 520f;       // adjust to your canvas size
        public float bottomYFail = -430f;      // adjust
        public float topYPerfect = 380f;       // adjust
        public float enemyCenterXWindow = 120f;

        private bool running;
        private Vector2 vel;
        private Action<BlockOutcome> onDone;
        private BlockOutcome pendingOutcome;
        private bool finishing;

        public void StartMinigame(Action<BlockOutcome> onDone)
        {
            CancelInvoke();     // stop any pending Close() calls
            finishing = false;  // reset safety flag

            this.onDone = onDone;
            panel.SetActive(true);
            running = false;

            message.text = "Get ready to Block!";
            Invoke(nameof(Begin), 0.6f);
        }

        private void Begin()
        {
            running = true;
            message.text = "";

            // spawn ball near top-ish and shoot downward at random angle
            ball.anchoredPosition = new Vector2(UnityEngine.Random.Range(-250f, 250f), 320f);

            float x = UnityEngine.Random.Range(-0.8f, 0.8f);
            vel = new Vector2(x, -1f).normalized * ballSpeed;
        }

        private void Update()
        {
            if (!running) return;

            // shield follows mouse X (simple + feels good)
            Vector2 localMouse;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)panel.transform, Input.mousePosition, null, out localMouse
            );

            float targetX = localMouse.x;
            Vector2 shieldPos = shield.anchoredPosition;
            shieldPos.x = Mathf.MoveTowards(shieldPos.x, targetX, shieldSpeed * Time.deltaTime);
            shield.anchoredPosition = shieldPos;

            // move ball
            Vector2 p = ball.anchoredPosition;
            p += vel * Time.deltaTime;

            // side wall bounce ONLY while going downward
            if (vel.y < 0f && Mathf.Abs(p.x) > wallBounceX)
            {
                p.x = Mathf.Sign(p.x) * wallBounceX;
                vel.x = -vel.x;
            }

            ball.anchoredPosition = p;

            // miss (hit bottom)
            if (p.y < bottomYFail)
            {
                Finish(BlockOutcome.Miss, "Miss!");
                return;
            }

            // collision vs shield (simple AABB)
            if (RectOverlaps(ball, shield))
            {
                // reflect angle based on hit offset
                float offset = (ball.anchoredPosition.x - shield.anchoredPosition.x) / (shield.rect.width * 0.5f);
                offset = Mathf.Clamp(offset, -1f, 1f);

                vel = new Vector2(offset, 1f).normalized * ballSpeed;

                // after hit, walls do NOT bounce anymore; if it hits side, it disappears (we treat as normal block)
                // "Perfect" if it goes up and passes near enemy center at top
            }

            // after shield hit: if it reaches top near center -> perfect
            if (vel.y > 0f && p.y > topYPerfect)
            {
                if (Mathf.Abs(p.x) <= enemyCenterXWindow)
                    Finish(BlockOutcome.Perfect, "Perfect Block!");
                else
                    Finish(BlockOutcome.Block, "Block!");
            }

            // after shield hit: if it goes out the side -> block (orb disappears)
            if (vel.y > 0f && Mathf.Abs(p.x) > wallBounceX)
            {
                Finish(BlockOutcome.Block, "Block!");
            }
        }

        private bool RectOverlaps(RectTransform a, RectTransform b)
        {
            var ar = GetWorldRect(a);
            var br = GetWorldRect(b);
            return ar.Overlaps(br);
        }

        private Rect GetWorldRect(RectTransform rt)
        {
            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);
            return new Rect(corners[0], corners[2] - corners[0]);
        }

        private void Finish(BlockOutcome outcome, string text)
        {
            if (finishing) return;
            finishing = true;

            running = false;
            pendingOutcome = outcome;

            if (message) message.text = text;

            Invoke(nameof(Close), 0.6f);
        }

        private void Close()
        {
            finishing = false;

            if (panel) panel.SetActive(false);
            if (message) message.text = "";

            onDone?.Invoke(pendingOutcome);
            onDone = null;
        }
    }
}