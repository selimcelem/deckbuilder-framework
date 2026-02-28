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
        private Canvas canvas;

        [Header("Perfect UI")]
        public RectTransform perfectZone; // assign PerfectZone RectTransform

        [Header("Tuning")]
        public float shieldSpeed = 1200f;
        public float ballSpeed = 700f;
        public float wallPadding = 20f;     
        public float missPadding = 30f; 

        private bool running;
        private Vector2 vel;
        private Action<BlockOutcome> onDone;
        private BlockOutcome pendingOutcome;
        private bool finishing;
        private Vector2 prevBallScreen;
        private bool hasBouncedOffShield;

        public void StartMinigame(Action<BlockOutcome> onDone)
        {
            if (!canvas) canvas = panel.GetComponentInParent<Canvas>();
            CancelInvoke();     // stop any pending Close() calls
            finishing = false;  // reset safety flag

            this.onDone = onDone;
            panel.SetActive(true);
            if (perfectZone)
            {
                perfectZone.gameObject.SetActive(true);
            }
            running = false;

            message.text = "Get ready to Block!";
            Invoke(nameof(Begin), 0.6f);
        }

        private void Begin()
        {
            running = true;
            message.text = "";
            hasBouncedOffShield = false;
            // spawn ball near top-ish and shoot downward at random angle
            ball.anchoredPosition = new Vector2(UnityEngine.Random.Range(-250f, 250f), 320f);
            prevBallScreen = GetBallScreenPoint();
            float x = UnityEngine.Random.Range(-0.8f, 0.8f);
            vel = new Vector2(x, -1f).normalized * ballSpeed;
        }

        private Vector2 GetBallScreenPoint()
        {
            if (!canvas) canvas = panel.GetComponentInParent<Canvas>();

            Camera cam = (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                ? canvas.worldCamera
                : null;

            // IMPORTANT: use the ball rect CENTER, not pivot
            Vector3 worldCenter = ball.TransformPoint(ball.rect.center);
            return RectTransformUtility.WorldToScreenPoint(cam, worldCenter);
        }

        private Rect GetPanelRect()
        {
            return ((RectTransform)panel.transform).rect; // local-space rect of the panel
        }

        private bool SweepScreenPointThroughZone(Vector2 from, Vector2 to, int steps)
        {
            if (!perfectZone) return false;

            Camera cam = (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                ? canvas.worldCamera
                : null;

            for (int i = 0; i <= steps; i++)
            {
                float t = i / (float)steps;
                Vector2 sp = Vector2.Lerp(from, to, t);

                if (RectTransformUtility.RectangleContainsScreenPoint(perfectZone, sp, cam))
                    return true;
            }
            return false;
        }

        private void Update()
        {
            if (!running) return;

            // shield follows mouse X (camera-aware)
            Vector2 localMouse;

            // Use the canvas camera if the canvas is not Screen Space Overlay
            Camera cam = (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                ? canvas.worldCamera
                : null;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)panel.transform,
                Input.mousePosition,
                cam,
                out localMouse
            );

            float targetX = localMouse.x;
            Vector2 shieldPos = shield.anchoredPosition;
            shieldPos.x = Mathf.MoveTowards(shieldPos.x, targetX, shieldSpeed * Time.deltaTime);
            shield.anchoredPosition = shieldPos;

            // move ball
            Vector2 prev = ball.anchoredPosition;        // old position
            Vector2 p = prev + vel * Time.deltaTime;     // new position

            // side wall bounce ONLY while going downward
            Rect pr = GetPanelRect();
            float xMin = pr.xMin + wallPadding;
            float xMax = pr.xMax - wallPadding;

            if (vel.y < 0f && (p.x < xMin || p.x > xMax))
            {
                p.x = Mathf.Clamp(p.x, xMin, xMax);
                vel.x = -vel.x;
            }

            ball.anchoredPosition = p;

            // miss (hit bottom)
            Rect pr2 = GetPanelRect();
            float missY = pr2.yMin - missPadding;

            if (p.y < missY)
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
                hasBouncedOffShield = true;
                vel = new Vector2(offset, 1f).normalized * ballSpeed;

                // after hit, walls do NOT bounce anymore; if it hits side, it disappears (we treat as normal block)
                // "Perfect" if it goes up and passes near enemy center at top
            }

            // PERFECT = ball passes through PerfectZone (screen-space robust)
            if (hasBouncedOffShield && vel.y > 0f && perfectZone != null)
            {
                Vector2 curBallScreen = GetBallScreenPoint();

                Camera camDbg = (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                    ? canvas.worldCamera
                    : null;

                bool insideNow = RectTransformUtility.RectangleContainsScreenPoint(perfectZone, curBallScreen, camDbg);

                Debug.Log($"Perfect test: prev={prevBallScreen} cur={curBallScreen} insideNow={insideNow}");

                if (SweepScreenPointThroughZone(prevBallScreen, curBallScreen, 20))
                {
                    Finish(BlockOutcome.Perfect, "Perfect Block!");
                    return;
                }
            }

            // If ball goes above the top line but NOT in perfect zone -> normal block
            if (vel.y > 0f)
            {
                float topY = ((RectTransform)panel.transform).rect.yMax;
                if (ball.anchoredPosition.y > topY)
                {
                    Finish(BlockOutcome.Block, "Block!");
                    return;
                }
            }

            // after shield hit: if it goes out the side -> block (orb disappears)
            Rect pr3 = GetPanelRect();
            float xMin2 = pr3.xMin + wallPadding;
            float xMax2 = pr3.xMax - wallPadding;

            if (vel.y > 0f && (p.x < xMin2 || p.x > xMax2))
            {
                Finish(BlockOutcome.Block, "Block!");
            }

            prevBallScreen = GetBallScreenPoint();
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

            Invoke(nameof(Close), 0.30f);
        }

        private void Close()
        {
            finishing = false;

            if (panel) panel.SetActive(false);
            if (perfectZone) perfectZone.gameObject.SetActive(false);
            if (message) message.text = "";

            onDone?.Invoke(pendingOutcome);
            onDone = null;
        }
    }
}