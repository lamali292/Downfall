using Godot;

namespace Downfall.DownfallCode.Utils;


public static class PortraitCompositor
{
    /// Blits vertical slices of each texture side by side. Null if no usable images.
    public static ImageTexture? SliceHorizontally(IReadOnlyList<Texture2D?> textures)
    {
        var images = textures
            .Select(ExtractImage)
            .OfType<Image>()
            .Where(img => !img.IsEmpty())
            .ToList();

        if (images.Count == 0) return null;

        var width = images[0].GetWidth();
        var height = images[0].GetHeight();
        var result = Image.CreateEmpty(width, height, false, Image.Format.Rgba8);
        var sliceWidth = width / images.Count;

        for (var i = 0; i < images.Count; i++)
        {
            var src = images[i];

            if (src.GetFormat() != Image.Format.Rgba8) src.Convert(Image.Format.Rgba8);
            if (src.GetWidth() != width || src.GetHeight() != height) src.Resize(width, height);

            var w = i == images.Count - 1 ? width - i * sliceWidth : sliceWidth;
            result.BlitRect(src, new Rect2I(i * sliceWidth, 0, w, height), new Vector2I(i * sliceWidth, 0));
        }

        return ImageTexture.CreateFromImage(result);
    }

    private static Image? ExtractImage(Texture2D? texture)
    {
        switch (texture)
        {
            case null:
                return null;

            case AtlasTexture atlasTex when atlasTex.Atlas != null:
            {
                // GetImage() on an AtlasTexture blit_rects out of the atlas,
                // which fails if the atlas is VRAM-compressed. Do it manually.
                var atlasImage = ExtractImage(atlasTex.Atlas);
                if (atlasImage == null || atlasImage.IsEmpty()) return null;

                var r = atlasTex.Region;
                var region = new Rect2I(
                    (int)r.Position.X, (int)r.Position.Y,
                    (int)r.Size.X, (int)r.Size.Y);
                return atlasImage.GetRegion(region);
            }

            default:
            {
                var image = texture.GetImage();
                if (image == null || image.IsEmpty()) return null;
                if (image.IsCompressed()) image.Decompress();
                return image;
            }
        }
    }
}