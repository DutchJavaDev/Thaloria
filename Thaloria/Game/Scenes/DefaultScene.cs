using Thaloria.Game.Interface;
using Raylib_cs;
using static Raylib_cs.Raylib;
using System.Numerics;
using Thaloria.Loaders;

namespace Thaloria.Game.Scenes
{

    /// <summary>
    /// Loading scene
    /// </summary>
    public sealed class DefaultScene : IScene
    {
        public SceneManagerEnum SceneReference => SceneManagerEnum.DefaultScene;
        private SceneManager? _sceneManager;

        private const string _loadingText = "...Loading...";
        private const int _fontSize = 45;
        private const int _spacing = 5;
        private Font _imortalFont;
        private Vector2 _loadTextPosition;
        private Color _loadColor;
        private float _loadColorAlpha = 1f;
        private int _ticks = 0;
        private readonly int _updateTickRate = 6;
        private float _direction = -0.2f;

        public void Init(SceneManager sceneManager)
        {
            _sceneManager = sceneManager;
            _imortalFont = FontManager.GetFont(0);
            var fontSize = MeasureTextEx(_imortalFont, _loadingText, _fontSize, _spacing);
            var width = GetScreenWidth();
            var height = GetScreenHeight();
            var x = (width / 2) - fontSize.X / 2;
            var y = (height / 2) - fontSize.Y / 2;
            _loadTextPosition = new Vector2(x, y);
            _loadColor = Color.LightGray;
        }

        public Task Dispose()
        {
            return Task.CompletedTask;
        }

        public Task Load()
        {
            return Task.CompletedTask;
        }

        public void Update()
        {
            UpdateAlpha();
        }

        private void UpdateAlpha()
        {
            _ticks++;

            if (_ticks == _updateTickRate)
            {
                _loadColorAlpha += _direction;

                if (_loadColorAlpha < 0.3f)
                {
                    _direction = 0.2f;
                    _loadColorAlpha = 0.3f;
                }

                if (_loadColorAlpha > 1f)
                {
                    _direction = -0.2f;
                    _loadColorAlpha = 1f;
                }

                _ticks = 0;

                _loadColor = ColorAlpha(_loadColor, _loadColorAlpha);
            }

        }

        public void Render()
        {
            BeginDrawing();
            DrawTextEx(_imortalFont, _loadingText, _loadTextPosition, _fontSize, _spacing, _loadColor);
            EndDrawing();
        }
    }
}
