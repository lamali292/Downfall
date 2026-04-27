import os
import numpy as np
from PIL import Image
from scipy.ndimage import gaussian_filter

# ============================================================
# CONFIG
# ============================================================
OUTLINE_RADIUS = 3   # Thickness of the outline
OUTLINE_SIGMA  = 0.5 # Softness/Anti-aliasing
# ============================================================

def create_outline(img_path):
    img = Image.open(img_path).convert("RGBA")
    alpha = np.array(img.split()[3])
    
    # Create circular kernel for dilation
    size = OUTLINE_RADIUS * 2 + 1
    y, x = np.ogrid[-OUTLINE_RADIUS:OUTLINE_RADIUS+1, -OUTLINE_RADIUS:OUTLINE_RADIUS+1]
    kernel = (x*x + y*y) <= OUTLINE_RADIUS*OUTLINE_RADIUS
    
    # Perform dilation
    padded = np.pad(alpha, OUTLINE_RADIUS)
    dilated = np.zeros_like(alpha)
    for dy in range(size):
        for dx in range(size):
            if kernel[dy, dx]:
                dilated = np.maximum(dilated, padded[dy:dy+alpha.shape[0], dx:dx+alpha.shape[1]])
    
    # Smooth the edges
    outline_alpha = gaussian_filter(dilated.astype(np.float32), sigma=OUTLINE_SIGMA)
    outline_alpha = np.clip(outline_alpha, 0, 255).astype(np.uint8)
    
    # Construct white outline
    outline = np.zeros((*alpha.shape, 4), dtype=np.uint8)
    outline[..., :3] = 255 
    outline[..., 3] = outline_alpha
    
    return Image.fromarray(outline, "RGBA")

def main():
    # Get the directory where THIS script is located
    current_dir = os.path.dirname(os.path.abspath(__file__))
    
    # Find all PNGs that aren't already outlines
    files = [
        f for f in os.listdir(current_dir) 
        if f.lower().endswith(".png") and not f.lower().endswith("_outline.png")
    ]
    
    if not files:
        print("No source PNGs found in this directory.")
        return

    print(f"Processing {len(files)} images in {current_dir}...")

    for file in files:
        full_path = os.path.join(current_dir, file)
        outline_name = os.path.splitext(file)[0] + "_outline.png"
        outline_path = os.path.join(current_dir, outline_name)
        
        try:
            outline_img = create_outline(full_path)
            outline_img.save(outline_path)
            print(f"✓ Generated: {outline_name}")
        except Exception as e:
            print(f"✗ Failed {file}: {e}")

    print("\nDone!")

if __name__ == "__main__":
    main()