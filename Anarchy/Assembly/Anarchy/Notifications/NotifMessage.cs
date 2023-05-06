using System.Collections.Generic;
using UnityEngine;

namespace Anarchy.Notifications
{
    internal class NotifMessage : MonoBehaviour
    {
        private static readonly Queue<Notification> Notifications = new Queue<Notification>();
        private const float Width = 300;
        private Notification? _current;
        private float _width;
        private GUIStyle _title;
        private GUIStyle _message;
        private Texture2D _black;
        private Texture2D _white;
        public static bool NotificationIsVisible;
        private static Texture2D Texture(byte r, byte g, byte b, byte a = 255) => Texture(Color(r, g, b, a));
        private static Color Color(byte r, byte g, int b, byte a = 255) => new Color(r / 255f, g / 255f, b / 255f, a / 255f);
        public static NotifMessage message;


        private void Start()
        {
            message = this;
        }


        private void OnGUI()
        {
            if (NotificationIsVisible)
                Render();
        }

        private void Update()
        {
            if (!NotificationIsVisible)
                return;

            if (!_current.HasValue)
            {
                if (Notifications.Count <= 0)
                {
                    Disable();
                    return;
                }

                _current = Notifications.Dequeue();
                _done = false;
                _showTime = 0;
            }

            if (!_done && _width < Width)
            {
                const float changeInValue = 1.25f;
                const float acceleration = 0.75f;

                _width += (Width * changeInValue + _width * acceleration) * Time.deltaTime;
            }
            else if (!_done)
            {
                if (_showTime > _current.Value.Duration)
                {
                    _width = Width;
                    _done = true;
                }
                _showTime += Time.deltaTime;
            }
            else if (_done && _width > 0)
            {
                const float changeInValue = 3f;
                const float acceleration = -2f;

                _width -= (Width * changeInValue + _width * acceleration) * Time.deltaTime;
                if (_width < 0)
                    _current = null;
            }
        }

        private void Enable()
        {
            OnShow();
            NotificationIsVisible = true;
        }

        private void Disable()
        {
            OnHide();
            NotificationIsVisible = false;
        }

        private static Texture2D Texture(Color color)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(1, 1, color);
            texture.Apply();
            return texture;
        }

        private void OnShow()
        {
            _white = Texture(255, 255, 255, 140);
            _black = Texture(137, 137, 137, 140);

            _message = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                fontSize = 15,
                normal = { textColor = UnityEngine.Color.white },
            };
            _title = new GUIStyle(_message)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 16,
                normal = { textColor = UnityEngine.Color.white },
            };

            _width = 0f;
        }

        private bool _done;
        private float _showTime;



        private void Render()
        {
            if (!_current.HasValue) return;
            var current = _current.Value;
            Rect rect = new Rect(Screen.width - _width, 300, _width, current.Height);

            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, rect.height), _black); //whole message color
            GUI.DrawTexture(new Rect(rect.x - 2, rect.y, 2, rect.height), _white); //left vertical string

            GUI.Label(new Rect(rect.x, rect.y, Width, 30), current.Title, _title);
            GUI.Label(new Rect(rect.x, rect.y + 30, Width, rect.height - 30), current.Message, _message);
        }

        private void OnHide()
        {
            Destroy(_white);
            Destroy(_black);
        }

        public void New(string message, float duration, float height = 30F)
        {
            New(message, string.Empty, duration, height);
        }

        public void New(string title, string message, float duration, float height = 100)
        {
            Notifications.Enqueue(new Notification(title, message, duration, height));
            Enable();
        }


        private struct Notification
        {
            private readonly string _title;
            private readonly string _message;
            private readonly float _duration;
            private readonly float _height;

            public Notification(string title, string message, float duration, float height)
            {
                _title = title;
                _message = message;
                _duration = duration;
                _height = height;
            }

            public string Title => _title;
            public string Message => _message;
            public float Height => _height;
            public float Duration => _duration;
        }
    }
}