from PIL import Image, ImageTk
import numpy as np
from scipy.ndimage import gaussian_filter
import os
import math
import hashlib
import tkinter as tk
from tkinter import ttk, filedialog
import threading

# ============================================================
# CONFIG
# ============================================================
INPUT_DIR              = "relics"
ATLAS_SPRITES          = "relic_atlas.sprites"
IMG_SIZE               = 93
INSET                  = 4
REGION_SIZE            = 85
ATLAS_FILENAME         = "relic_atlas.png"
OUTLINE_ATLAS_FILENAME = "relic_outline_atlas.png"
CROP_BOX               = (56, 56, 200, 200)
OUTLINE_RADIUS         = 10
OUTLINE_SIGMA          = 0.5
# ============================================================

import hashlib

def deterministic_uid(name: str, length=7) -> str:
    return hashlib.md5(name.encode()).hexdigest()[:length]

def write_tres(path, atlas_res_path, x, y, size):
    stem = os.path.splitext(os.path.basename(path))[0]
    content = f'''[gd_resource type="AtlasTexture" load_steps=2 format=3 uid="uid://{deterministic_uid(stem)}"]
[ext_resource type="Texture2D" path="{atlas_res_path}" id="1"]
[resource]
atlas = ExtResource("1")
region = Rect2({x}, {y}, {size}, {size})
'''
    with open(path, "w") as f:
        f.write(content)

def clean_dir(folder, extensions):
    if not os.path.exists(folder):
        os.makedirs(folder)
        return
    for f in os.listdir(folder):
        if any(f.endswith(ext) for ext in extensions):
            os.remove(os.path.join(folder, f))

def process_image(path, crop_box=CROP_BOX, radius=OUTLINE_RADIUS, sigma=OUTLINE_SIGMA, big_size=256, atlas_size=IMG_SIZE, resample=Image.NEAREST):
    img = Image.open(path).convert("RGBA")
    img_cropped = img.crop(crop_box)
    img_upscaled = img_cropped.resize((big_size, big_size), resample)
    alpha = np.array(img_upscaled.split()[3])
    size = radius * 2 + 1
    y, x = np.ogrid[-radius:radius+1, -radius:radius+1]
    kernel = (x*x + y*y) <= radius*radius
    padded = np.pad(alpha, radius)
    dilated = np.zeros_like(alpha)
    for dy in range(size):
        for dx in range(size):
            if kernel[dy, dx]:
                dilated = np.maximum(dilated, padded[dy:dy+alpha.shape[0], dx:dx+alpha.shape[1]])
    outline_alpha = gaussian_filter(dilated.astype(np.float32), sigma=sigma)
    outline_alpha = np.clip(outline_alpha, 0, 255).astype(np.uint8)
    outline = np.zeros((alpha.shape[0], alpha.shape[1], 4), dtype=np.uint8)
    outline[..., 0] = 255
    outline[..., 1] = 255
    outline[..., 2] = 255
    outline[..., 3] = outline_alpha
    image_outline = Image.fromarray(outline, "RGBA")
    black_outline = np.zeros((alpha.shape[0], alpha.shape[1], 4), dtype=np.uint8)
    black_outline[..., 3] = (outline_alpha * 0.5).astype(np.uint8)
    black_result = Image.fromarray(black_outline, "RGBA")
    big = Image.alpha_composite(black_result, img_upscaled)
    outline_downscaled = image_outline.resize((atlas_size, atlas_size), resample)
    image_downscaled = img_cropped.resize((atlas_size, atlas_size), resample)
    return big, outline_downscaled, image_downscaled


class RelicGenApp(tk.Tk):
    def __init__(self):
        super().__init__()
        self.title("Relic Generator")
        self.geometry("1200x750")
        self.configure(bg="#1e1e1e")
        self.resizable(True, True)

        self.current_file  = None
        self.preview_job   = None
        self.resize_job    = None
        self.tk_images     = {}
        self.cached_images = {}

        self._build_ui()
        self._load_file_list()

    def _build_ui(self):
        style = ttk.Style(self)
        style.theme_use("clam")
        style.configure("TFrame",        background="#1e1e1e")
        style.configure("TLabel",        background="#1e1e1e", foreground="#cccccc", font=("Segoe UI", 10))
        style.configure("TButton",       background="#2d2d2d", foreground="#cccccc", font=("Segoe UI", 10), borderwidth=0)
        style.configure("TEntry",        fieldbackground="#2d2d2d", foreground="#cccccc", insertcolor="#cccccc")
        style.map("TButton",             background=[("active", "#3a3a3a")])
        style.configure("TScale",        background="#1e1e1e", troughcolor="#2d2d2d")
        style.configure("Green.TButton", background="#2a5c2a", foreground="#88ff88")
        style.map("Green.TButton",       background=[("active", "#3a7a3a")])

        # ── left panel ──
        left = ttk.Frame(self, width=400)
        left.pack(side=tk.LEFT, fill=tk.Y, padx=(12, 0), pady=12)
        left.pack_propagate(False)

        ttk.Label(left, text="Input folder").pack(anchor="w")
        folder_row = ttk.Frame(left)
        folder_row.pack(fill=tk.X, pady=(2, 8))
        self.folder_var = tk.StringVar(value=INPUT_DIR)
        ttk.Entry(folder_row, textvariable=self.folder_var).pack(side=tk.LEFT, fill=tk.X, expand=True)
        ttk.Button(folder_row, text="…", width=3, command=self._pick_folder).pack(side=tk.LEFT, padx=(4, 0))

        ttk.Label(left, text="Files").pack(anchor="w")
        lb_frame = ttk.Frame(left)
        lb_frame.pack(fill=tk.BOTH, expand=True, pady=(2, 8))
        scrollbar = tk.Scrollbar(lb_frame, bg="#2d2d2d")
        scrollbar.pack(side=tk.RIGHT, fill=tk.Y)
        self.file_list = tk.Listbox(lb_frame, bg="#2d2d2d", fg="#cccccc", selectbackground="#444",
                                    bd=0, highlightthickness=0, yscrollcommand=scrollbar.set,
                                    font=("Segoe UI", 10))
        self.file_list.pack(fill=tk.BOTH, expand=True)
        scrollbar.config(command=self.file_list.yview)
        self.file_list.bind("<<ListboxSelect>>", self._on_select)

        ttk.Button(left, text="↺  Refresh list", command=self._load_file_list).pack(fill=tk.X, pady=(0, 12))

        ttk.Label(left, text="Godot project root").pack(anchor="w")
        root_row = ttk.Frame(left)
        root_row.pack(fill=tk.X, pady=(2, 4))
        self.godot_root_var = tk.StringVar(value=os.path.abspath("../"))
        ttk.Entry(root_row, textvariable=self.godot_root_var).pack(side=tk.LEFT, fill=tk.X, expand=True)
        ttk.Button(root_row, text="…", width=3, command=lambda: self._pick_out(self.godot_root_var)).pack(side=tk.LEFT, padx=(4, 0))

        ttk.Label(left, text="res:// base path").pack(anchor="w")
        self.res_base_var = tk.StringVar(value="res://Downfall/images")
        ttk.Entry(left, textvariable=self.res_base_var).pack(fill=tk.X, pady=(2, 4))

        ttk.Label(left, text="Override save root (optional)").pack(anchor="w")
        save_row = ttk.Frame(left)
        save_row.pack(fill=tk.X, pady=(2, 12))
        self.save_root_var = tk.StringVar(value="")
        ttk.Entry(save_row, textvariable=self.save_root_var).pack(side=tk.LEFT, fill=tk.X, expand=True)
        ttk.Button(save_row, text="…", width=3, command=lambda: self._pick_out(self.save_root_var)).pack(side=tk.LEFT, padx=(4, 0))

        # ── sliders ──
        slider_cfg = [
            ("Crop inset",     "crop_inset", 0,   120, CROP_BOX[0]),
            ("Big size",       "big_size",   64,  512, 256),
            ("Atlas size",     "atlas_size", 16,  256, IMG_SIZE),
            ("Outline radius", "radius",     1,    30, OUTLINE_RADIUS),
            ("Outline sigma",  "sigma",      1,    50, int(OUTLINE_SIGMA * 10)),
        ]
        self.sliders       = {}
        self.slider_labels = {}
        for label, key, lo, hi, val in slider_cfg:
            row = ttk.Frame(left)
            row.pack(fill=tk.X, pady=1)
            ttk.Label(row, text=label, width=14, anchor="w").pack(side=tk.LEFT)
            lbl = ttk.Label(row, text=str(val), width=5, anchor="e")
            lbl.pack(side=tk.RIGHT)
            self.slider_labels[key] = lbl
            var = tk.IntVar(value=val)
            self.sliders[key] = var
            s = ttk.Scale(row, from_=lo, to=hi, variable=var, orient=tk.HORIZONTAL,
                          command=lambda v, k=key: self._on_slider(k))
            s.pack(side=tk.LEFT, fill=tk.X, expand=True, padx=4)

        filter_row = ttk.Frame(left)
        filter_row.pack(fill=tk.X, pady=(8, 4))
        ttk.Label(filter_row, text="Resampling", width=14, anchor="w").pack(side=tk.LEFT)
        self.filter_var = tk.StringVar(value="Nearest")
        filter_options = ["Nearest", "Bilinear", "Bicubic", "Lanczos", "Box", "Hamming"]
        filter_menu = ttk.Combobox(filter_row, textvariable=self.filter_var,
                                   values=filter_options, state="readonly", width=12)
        filter_menu.pack(side=tk.LEFT, padx=4)
        filter_menu.bind("<<ComboboxSelected>>", lambda e: self._schedule_preview())

        ttk.Button(left, text="▶  Generate ALL", style="Green.TButton",
                   command=self._generate_all).pack(fill=tk.X, pady=(16, 0))
        self.status_label = ttk.Label(left, text="", wraplength=380)
        self.status_label.pack(anchor="w", pady=(6, 0))

        # ── right panel: 2×2 scaling grid ──
        right = ttk.Frame(self)
        right.pack(side=tk.LEFT, fill=tk.BOTH, expand=True, padx=12, pady=12)
        right.rowconfigure((1, 3), weight=1)
        right.rowconfigure((0, 2), weight=0)
        right.columnconfigure((0, 1), weight=1)

        bg = "#2a2a2a"

        self.label_big     = ttk.Label(right, text="Big (256×256)")
        self.label_orig    = ttk.Label(right, text="Original")
        self.label_atlas   = ttk.Label(right, text=f"Atlas ({IMG_SIZE}×{IMG_SIZE})")
        self.label_outline = ttk.Label(right, text=f"Outline ({IMG_SIZE}×{IMG_SIZE})")

        self.label_big.grid(    row=0, column=0, sticky="sw", padx=6, pady=(0, 2))
        self.label_orig.grid(   row=0, column=1, sticky="sw", padx=6, pady=(0, 2))
        self.label_atlas.grid(  row=2, column=0, sticky="sw", padx=6, pady=(8, 2))
        self.label_outline.grid(row=2, column=1, sticky="sw", padx=6, pady=(8, 2))

        self.canvas_big     = tk.Canvas(right, bg=bg, bd=0, highlightthickness=0)
        self.canvas_orig    = tk.Canvas(right, bg=bg, bd=0, highlightthickness=0)
        self.canvas_atlas   = tk.Canvas(right, bg=bg, bd=0, highlightthickness=0)
        self.canvas_outline = tk.Canvas(right, bg=bg, bd=0, highlightthickness=0)

        self.canvas_big.grid(    row=1, column=0, sticky="nsew", padx=6, pady=2)
        self.canvas_orig.grid(   row=1, column=1, sticky="nsew", padx=6, pady=2)
        self.canvas_atlas.grid(  row=3, column=0, sticky="nsew", padx=6, pady=2)
        self.canvas_outline.grid(row=3, column=1, sticky="nsew", padx=6, pady=2)

        right.bind("<Configure>", self._on_right_resize)

    # ── path helpers ──

    def _get_filter(self):
        return {
            "Nearest":  Image.NEAREST,
            "Bilinear": Image.BILINEAR,
            "Bicubic":  Image.BICUBIC,
            "Lanczos":  Image.LANCZOS,
            "Box":      Image.BOX,
            "Hamming":  Image.HAMMING,
        }.get(self.filter_var.get(), Image.NEAREST)

    def _resolve_paths(self):
        res_base   = self.res_base_var.get().rstrip("/")
        godot_root = self.godot_root_var.get()
        save_root  = self.save_root_var.get().strip() or godot_root
        rel        = res_base.replace("res://", "").strip("/")
        out_relics  = os.path.join(save_root, rel.replace("/", os.sep), "relics")
        out_atlases = os.path.join(save_root, rel.replace("/", os.sep), "atlases")
        out_tres    = os.path.join(out_atlases, ATLAS_SPRITES)
        res_atlas   = f"{res_base}/atlases/{ATLAS_FILENAME}"
        res_outline = f"{res_base}/atlases/{OUTLINE_ATLAS_FILENAME}"
        return out_relics, out_atlases, out_tres, res_atlas, res_outline

    def _pick_out(self, var):
        d = filedialog.askdirectory(initialdir=var.get())
        if d:
            var.set(d)

    def _pick_folder(self):
        d = filedialog.askdirectory(initialdir=self.folder_var.get())
        if d:
            self.folder_var.set(d)
            self._load_file_list()

    # ── file list ──

    def _load_file_list(self):
        self.file_list.delete(0, tk.END)
        folder = self.folder_var.get()
        if not os.path.isdir(folder):
            self.status_label.config(text=f"Folder not found: {folder}")
            return
        files = [f for f in sorted(os.listdir(folder)) if f.lower().endswith(".png")]
        for f in files:
            self.file_list.insert(tk.END, f)
        self.status_label.config(text=f"{len(files)} file(s) found")

    def _on_select(self, event):
        sel = self.file_list.curselection()
        if not sel:
            return
        self.current_file = os.path.join(self.folder_var.get(), self.file_list.get(sel[0]))
        self._schedule_preview()

    # ── sliders ──

    def _on_slider(self, key):
        val = self.sliders[key].get()
        display = f"{val / 10:.1f}" if key == "sigma" else str(val)
        self.slider_labels[key].config(text=display)

        big_size   = self.sliders["big_size"].get()
        atlas_size = self.sliders["atlas_size"].get()
        self.label_big.config(    text=f"Big ({big_size}×{big_size})")
        self.label_atlas.config(  text=f"Atlas ({atlas_size}×{atlas_size})")
        self.label_outline.config(text=f"Outline ({atlas_size}×{atlas_size})")

        self._schedule_preview()

    # ── preview ──

    def _schedule_preview(self):
        if self.preview_job:
            self.after_cancel(self.preview_job)
        self.preview_job = self.after(120, self._run_preview)

    def _on_right_resize(self, event):
        if self.resize_job:
            self.after_cancel(self.resize_job)
        self.resize_job = self.after(80, self._redraw_cached)

    def _run_preview(self):
        if not self.current_file or not os.path.isfile(self.current_file):
            return
        inset      = self.sliders["crop_inset"].get()
        big_size   = self.sliders["big_size"].get()
        atlas_size = self.sliders["atlas_size"].get()
        radius     = self.sliders["radius"].get()
        sigma      = self.sliders["sigma"].get() / 10.0

        try:
            orig = Image.open(self.current_file).convert("RGBA")
            w, h = orig.size
            crop_box = (inset, inset, w - inset, h - inset)
            big, outline_ds, atlas_ds = process_image(self.current_file, crop_box, radius, sigma, big_size, atlas_size, self._get_filter())
        except Exception as e:
            self.status_label.config(text=f"Error: {e}")
            return

        self.cached_images = {
            "big":     big,
            "orig":    orig,
            "atlas":   atlas_ds,
            "outline": outline_ds,
        }
        self._redraw_cached()

    def _redraw_cached(self):
        if not self.cached_images:
            return
        canvases = {
            "big":     self.canvas_big,
            "orig":    self.canvas_orig,
            "atlas":   self.canvas_atlas,
            "outline": self.canvas_outline,
        }
        for key, canvas in canvases.items():
            img = self.cached_images.get(key)
            if img is None:
                continue
            cw = canvas.winfo_width()
            ch = canvas.winfo_height()
            if cw < 4 or ch < 4:
                continue
            size = min(cw, ch)
            img_resized = img.resize((size, size), Image.NEAREST)
            photo = ImageTk.PhotoImage(img_resized)
            canvas.delete("all")
            canvas.create_image(cw // 2, ch // 2, anchor="center", image=photo)
            self.tk_images[key] = photo

    # ── generate ──

    def _generate_all(self):
        def run():
            try:
                folder = self.folder_var.get()
                out_relics, out_atlases, out_tres, res_atlas, res_outline = self._resolve_paths()
                clean_dir(out_relics, [".png", ".import"])
                clean_dir(out_tres,   [".tres"])
                os.makedirs(out_atlases, exist_ok=True)

                inset      = self.sliders["crop_inset"].get()
                big_size   = self.sliders["big_size"].get()
                atlas_size = self.sliders["atlas_size"].get()
                radius     = self.sliders["radius"].get()
                sigma      = self.sliders["sigma"].get() / 10.0

                files = [f for f in sorted(os.listdir(folder)) if f.lower().endswith(".png")]
                entries = []
                for i, file in enumerate(files):
                    self.after(0, lambda i=i, f=file: self.status_label.config(
                        text=f"Processing {i+1}/{len(files)}: {f}"))
                    in_path  = os.path.join(folder, file)
                    stem     = os.path.splitext(file)[0]
                    img      = Image.open(in_path).convert("RGBA")
                    w, h     = img.size
                    crop_box = (inset, inset, w - inset, h - inset)
                    big, outline_ds, atlas_ds = process_image(in_path, crop_box, radius, sigma, big_size, atlas_size, self._get_filter())
                    entries.append((stem, big, outline_ds, atlas_ds))

                n    = len(entries)
                cols = math.ceil(math.sqrt(n))
                rows = math.ceil(n / cols)
                atlas_w = cols * atlas_size
                atlas_h = rows * atlas_size

                atlas         = Image.new("RGBA", (atlas_w, atlas_h), (0, 0, 0, 0))
                outline_atlas = Image.new("RGBA", (atlas_w, atlas_h), (0, 0, 0, 0))

                for i, (stem, big, outline_ds, atlas_ds) in enumerate(entries):
                    col = i % cols
                    row = i // cols
                    x   = col * atlas_size
                    y   = row * atlas_size
                    atlas.paste(atlas_ds, (x, y))
                    outline_atlas.paste(outline_ds, (x, y))
                    big.save(os.path.join(out_relics, f"{stem}.png"))
                    write_tres(os.path.join(out_tres, f"{stem}.tres"),         res_atlas,   x + INSET, y + INSET, REGION_SIZE)
                    write_tres(os.path.join(out_tres, f"{stem}_outline.tres"), res_outline, x + INSET, y + INSET, REGION_SIZE)

                atlas.save(os.path.join(out_atlases, ATLAS_FILENAME))
                outline_atlas.save(os.path.join(out_atlases, OUTLINE_ATLAS_FILENAME))

                self.after(0, lambda: self.status_label.config(
                    text=f"Done! {n} images ({cols}×{rows} grid)"))
            except Exception as e:
                self.after(0, lambda: self.status_label.config(text=f"Error: {e}"))

        threading.Thread(target=run, daemon=True).start()


if __name__ == "__main__":
    app = RelicGenApp()
    app.mainloop()