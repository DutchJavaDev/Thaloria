using Thaloria.Game.Interface;
using Raylib_cs;
using static Raylib_cs.Raylib;
using Thaloria.Loaders;
using System.Numerics;

namespace Thaloria.Game.Scenes
{
    public sealed class MenuScene : IScene
    {
        public SceneManagerEnum SceneReference => SceneManagerEnum.MenuScene;
        private SceneManager? _sceneManager;

        private readonly string _playText = "..Play..";
        private const int _fontSize = 45;
        private const int _spacing = 5;
        private Font _imortalFont;
        private Color _playColor = Color.Red;

        private Rectangle _playRectangle;
        private Vector2 _playTextPosition;

        public void Init(SceneManager sceneManager)
        {
            _sceneManager = sceneManager;
        }

        public Task LoadAsync()
        {
            _imortalFont = FontManager.GetFont(0);
            var screenWidth = (float)GetScreenWidth();
            var screenHeight = (float)GetScreenHeight();
            
            var fontSize = MeasureTextEx(_imortalFont, _playText, _fontSize, _spacing);
            // Place text in the center of that rectangle, have to do this instead, normal way aint working
            var buttonWidth = fontSize.X;
            var buttonHeight = fontSize.Y;

            // Place rectangle at center of screen
            var position = new Vector2(screenWidth / 2-(buttonWidth / 2),screenHeight/2-(buttonHeight / 2));
            _playRectangle = new() 
            {
                Position = position,
                Width = buttonWidth,
                Height = buttonHeight
            };

            // Place text in the center of that rectangle
            var x = _playRectangle.X;
            var y = _playRectangle.Y;
            _playTextPosition = new Vector2(x, y);
            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public void Update()
        {
            var mouse = GetMousePosition();

            if (CheckCollisionPointRec(mouse, _playRectangle))
            {
                _playColor = Color.Green;
            }
            else
            {
                _playColor = Color.Red;
            }

            if (IsMouseButtonPressed(MouseButton.Left) && CheckCollisionPointRec(mouse,_playRectangle))
            {
                _sceneManager?.SwitchToScene(SceneManagerEnum.GameScene);
            }
        }

        public void Render()
        {
            BeginDrawing();
            DrawRectangleLinesEx(_playRectangle,2,Color.LightGray);
            DrawTextEx(_imortalFont, _playText, _playTextPosition, _fontSize, _spacing, _playColor);
            EndDrawing();
        }
    }
}
