import os
import re

# Set this to your specific mod folder directory to be 100% safe
# e.g., "res://Downfall" or "." for the current folder
TARGET_DIRECTORY = "."

# Regex pattern to match uid="uid://..." including any trailing space
UID_PATTERN = re.compile(r'\buid="uid://[^"]*"\s*')

def nuke_uids_in_file(file_path):
    # CRITICAL: Skip if the file itself is a symlink
    if os.path.islink(file_path):
        return

    try:
        with open(file_path, 'r', encoding='utf-8') as f:
            content = f.read()
        
        if 'uid="uid://' in content:
            cleaned_content = UID_PATTERN.sub('', content)
            
            with open(file_path, 'w', encoding='utf-8') as f:
                f.write(cleaned_content)
            print(f"Cleaned UIDs from local file: {file_path}")
    except Exception as e:
        print(f"Error processing {file_path}: {e}")

def main():
    print(f"Scanning '{os.path.abspath(TARGET_DIRECTORY)}' strictly for local physical files...")
    
    target_extensions = ('.tscn', '.tres')
    count = 0

    for root, dirs, files in os.walk(TARGET_DIRECTORY):
        # 1. Skip the engine internal cache folder
        if '.godot' in root.split(os.sep):
            continue
            
        # 2. Prevent os.walk from stepping into symlinked directories
        # Modifying the dirs list in-place lets us filter out symlinked folders dynamically
        dirs[:] = [d for d in dirs if not os.path.islink(os.path.join(root, d))]
            
        for file in files:
            if file.endswith(target_extensions):
                file_path = os.path.join(root, file)
                nuke_uids_in_file(file_path)
                count += 1

    print(f"Done! Processed {count} local scene/resource files. Symlinks bypassed safely.")

if __name__ == "__main__":
    main()