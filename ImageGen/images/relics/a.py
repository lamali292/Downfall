#!/usr/bin/env python3
"""Crop every 256x256 PNG down to a 128x128, in place, WITHOUT cutting color.

Strategy: the target is the centered inner 128x128 box. If the image's content
(non-transparent pixels) spills outside that box, the crop box is grown
symmetrically by the largest overreach on any side, then scaled back down to
128x128. So no colored pixel is ever lost -- content just gets slightly smaller.

  content fits inner 128        -> plain crop to 128, no scaling
  content spills e.g. L3 U3 R3  -> crop 134x134 (128 + max(3)*2), scale to 128
  fully transparent / empty     -> plain crop to 128

DRY-RUN BY DEFAULT. Add --apply to overwrite originals.

Usage:
    python crop_inner.py [dir]           # preview: report per-file plan, write nothing
    python crop_inner.py [dir] --apply   # do it, overwriting each file in place
"""

import sys
from pathlib import Path

from PIL import Image

SRC = 256
INNER = 128
OFFSET = (SRC - INNER) // 2  # 64
INNER_BOX = (OFFSET, OFFSET, OFFSET + INNER, OFFSET + INNER)  # l,t,r,b = 64,64,192,192
ALPHA_THRESHOLD = 0

SCRIPT_DIR = Path(__file__).resolve().parent


def content_bbox(im: Image.Image):
    mask = im.getchannel("A").point(lambda a: 255 if a > ALPHA_THRESHOLD else 0)
    return mask.getbbox()  # (l,t,r,b) or None


def plan(path: Path):
    """Return (status, margin, box) describing how to crop this file. No writes."""
    with Image.open(path) as im:
        im = im.convert("RGBA")
        if im.size != (SRC, SRC):
            return ("badsize", im.size, None)
        bbox = content_bbox(im)

    if bbox is None:
        return ("fit", 0, INNER_BOX)  # empty -> plain inner crop

    l, t, r, b = bbox
    over_l = max(0, OFFSET - l)
    over_t = max(0, OFFSET - t)
    over_r = max(0, r - (OFFSET + INNER))
    over_b = max(0, b - (OFFSET + INNER))
    m = max(over_l, over_t, over_r, over_b)

    if m == 0:
        return ("fit", 0, INNER_BOX)

    m = min(m, OFFSET)  # can't grow past the image edge (max box = full 256)
    box = (OFFSET - m, OFFSET - m, OFFSET + INNER + m, OFFSET + INNER + m)
    return ("scale", m, box)


def apply_crop(path: Path, box, need_scale: bool):
    with Image.open(path) as im:
        im = im.convert("RGBA")
        cropped = im.crop(box)
        if need_scale:
            cropped = cropped.resize((INNER, INNER), Image.LANCZOS)
        cropped.save(path)


def main() -> None:
    args = sys.argv[1:]
    do_apply = "--apply" in args
    positional = [a for a in args if not a.startswith("--")]
    in_dir = Path(positional[0]) if positional else SCRIPT_DIR

    if not in_dir.is_dir():
        print(f"error: not a directory: {in_dir}")
        sys.exit(1)

    print(f"mode: {'APPLY (overwriting in place)' if do_apply else 'PREVIEW (no writes)'}")
    print(f"dir:  {in_dir}\n")

    fit = scaled = skipped = written = 0

    for png in sorted(in_dir.glob("*.png")):
        status, m, box = plan(png)

        if status == "badsize":
            print(f"  SKIP   {png.name}: not {SRC}x{SRC} (is {m[0]}x{m[1]})")
            skipped += 1
            continue

        if status == "fit":
            side = box[2] - box[0]
            print(f"  fit    {png.name}: content within inner {INNER}, plain crop {side}->{INNER}")
            fit += 1
            if do_apply:
                apply_crop(png, box, need_scale=False)
                written += 1
        else:  # scale
            side = box[2] - box[0]
            print(f"  scale  {png.name}: overreach {m}px -> crop {side}x{side}, resize to {INNER}x{INNER}")
            scaled += 1
            if do_apply:
                apply_crop(png, box, need_scale=True)
                written += 1

    print()
    print(f"summary: {fit} plain-fit, {scaled} scaled-down, {skipped} wrong size")
    if do_apply:
        print(f"written: {written} file(s) overwritten")
    else:
        print("preview only -- add --apply to overwrite the files in place")


if __name__ == "__main__":
    main()