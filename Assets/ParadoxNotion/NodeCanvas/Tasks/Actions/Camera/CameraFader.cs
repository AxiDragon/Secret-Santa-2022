﻿using System.Collections;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
    public class CameraFader : MonoBehaviour
    {
        private static CameraFader _current;
        private Texture2D _blackTexture;
        private float alpha;

        private Texture2D blackTexture
        {
            get
            {
                if (_blackTexture == null)
                {
                    _blackTexture = new Texture2D(1, 1);
                    _blackTexture.SetPixel(1, 1, Color.black);
                    _blackTexture.Apply();
                }

                return _blackTexture;
            }
        }

        public static CameraFader current
        {
            get
            {
                if (_current == null)
                    _current = FindObjectOfType<CameraFader>();
                if (_current == null)
                    _current = new GameObject("_CameraFader").AddComponent<CameraFader>();
                return _current;
            }
        }

        private void OnGUI()
        {
            if (alpha <= 0) return;

            GUI.color = new Color(1, 1, 1, alpha);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), blackTexture);
            GUI.color = Color.white;
        }

        public void FadeIn(float time)
        {
            StartCoroutine(CoroutineFadeIn(time));
        }

        public void FadeOut(float time)
        {
            StartCoroutine(CoroutineFadeOut(time));
        }

        private IEnumerator CoroutineFadeIn(float time)
        {
            alpha = 1;
            if (time <= 0) alpha = 0;
            while (alpha > 0)
            {
                yield return null;
                alpha -= 1 / time * Time.deltaTime;
            }
        }

        private IEnumerator CoroutineFadeOut(float time)
        {
            alpha = 0;
            if (time <= 0) alpha = 1;
            while (alpha < 1)
            {
                yield return null;
                alpha += 1 / time * Time.deltaTime;
            }
        }
    }
}