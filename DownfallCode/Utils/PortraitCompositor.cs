using Godot;

namespace Downfall.DownfallCode.Utils;


public static class PortraitCompositor
{
    /// Blits vertical slices of each texture side by side. Null if no usable images.
    public static ImageTexture? SliceHorizontally(IReadOnlyList<Texture2D?> textures)
    {
        var images = textures
            .Select(t => t?.GetImage())
            .OfType<Image>()
            .ToList();

        if (images.Count == 0) return null;

        var width = images[0].GetWidth();
        var height = images[0].GetHeight();
        var result = Image.CreateEmpty(width, height, false, Image.Format.Rgba8);
        var sliceWidth = width / images.Count;

        for (var i = 0; i < images.Count; i++)
        {
            var src = images[i];

            if (src.IsCompressed()) src.Decompress();
            if (src.GetFormat() != Image.Format.Rgba8) src.Convert(Image.Format.Rgba8);
            if (src.GetWidth() != width || src.GetHeight() != height) src.Resize(width, height);

            var w = i == images.Count - 1 ? width - i * sliceWidth : sliceWidth;
            result.BlitRect(src, new Rect2I(i * sliceWidth, 0, w, height), new Vector2I(i * sliceWidth, 0));
        }

        return ImageTexture.CreateFromImage(result);
    }
}