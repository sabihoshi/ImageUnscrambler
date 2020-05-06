using System.Drawing;

namespace ImageUnscrambler.Ignored
{
    public enum Position
    {
        Top,
        Bottom,
        Left,
        Right
    }

    public class IgnoredChunk
    {
        public IgnoredChunk(Image image, Position position)
        {
            Image = image;
            Position = position;
        }

        public Image Image { get; }

        public Position Position { get; }
    }
}