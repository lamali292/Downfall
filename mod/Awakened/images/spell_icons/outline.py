from PIL import Image, ImageFilter
import sys
import os


def add_outline(input_path: str, output_path: str, size: int):
    image = Image.open(input_path).convert("RGBA")

    # Extract alpha channel
    alpha = image.getchannel("A")

    # Expand the alpha mask by the requested pixel size
    expanded = alpha.filter(
        ImageFilter.MaxFilter(size * 2 + 1)
    )

    # Create white, semi-transparent outline
    outline = Image.new(
        "RGBA",
        image.size,
        (0, 0, 0, 128)
    )

    # Only keep the expanded area
    outline.putalpha(expanded.point(lambda a: min(a, 128)))

    # Put the original image on top
    result = Image.alpha_composite(outline, image)

    result.save(output_path)


if __name__ == "__main__":
    if len(sys.argv) != 4:
        print("Usage: python outline.py input.png output.png size")
        sys.exit(1)

    input_path = sys.argv[1]
    output_path = sys.argv[2]
    size = int(sys.argv[3])

    add_outline(input_path, output_path, size)

    print(f"Saved: {output_path}")