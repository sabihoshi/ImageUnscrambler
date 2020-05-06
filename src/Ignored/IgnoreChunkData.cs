namespace ImageUnscrambler.Ignored
{
    public readonly struct IgnoreChunkData
    {
        public IgnoreChunkData(Position position, int pixels)
        {
            Position = position;
            Pixels = pixels;
        }

        public static bool TryParse(out IgnoreChunkData data)
        {
            var position = "Enter position [Top|Bottom|Left|Right]".Prompt<Position>(true);
            var pixels = $"Enter pixels starting from {position}".Prompt<int>(int.TryParse);

            data = new IgnoreChunkData(position, pixels);

            return pixels > 0;
        }

        public Position Position { get; }

        public int Pixels { get; }
    }
}