using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiv.Fast2D;
using OpenTK;

namespace boids
{
    class TextChar
    {
        protected char character;
        protected Font font;
        protected Vector2 textureOffset;
        protected Sprite sprite; 
        protected Texture texture;

        public float HalfWidth { get => texture.Width * 0.5f; }
        public float HalfHeight { get => texture.Height * 0.5f; }

        public char Character { get { return character; } set { character = value; ComputeOffset(); } }

        public TextChar(Vector2 spritePosition, char character, Font font)
        {
            texture = font.Texture;
            sprite = new Sprite(30, 30);

            sprite.position = spritePosition;
            this.font = font;
            Character = character;
            sprite.pivot = Vector2.Zero;
        }

        protected void ComputeOffset()
        {
            textureOffset = font.GetOffset(character);
        }

        public void Draw()
        {
                sprite.DrawTexture(texture, (int)textureOffset.X, (int)textureOffset.Y, font.CharacterWidth, font.CharacterHeight);
        }

        public void Destroy()
        {
            sprite = null;
            texture = null;
        }
    }
}
