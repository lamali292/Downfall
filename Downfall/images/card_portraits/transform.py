import os
import re

def clean_and_rename_assets(directory):
    # Change to the target directory
    os.chdir(directory)
    
    # 1. Get all png files in the root
    files = [f for f in os.listdir('.') if f.endswith('.png')]
    
    for filename in files:
        name_part, ext = os.path.splitext(filename)
        
        # Scenario A: File DOES NOT have '_p' -> Delete it
        if not name_part.endswith('_p'):
            try:
                os.remove(filename)
                print(f"Deleted: {filename}")
            except OSError as e:
                print(f"Error deleting {filename}: {e}")
                
        # Scenario B: File HAS '_p' -> Transform to lower_snake_case
        else:
            # Remove the '_p' suffix first
            base_name = name_part[:-2] 
            
            # Convert CamelCase to snake_case
            # Finds 'aB' and turns it into 'a_B'
            snake_name = re.sub(r'([a-z0-9])([A-Z])', r'\1_\2', base_name).lower()
            new_filename = f"{snake_name}{ext}"
            
            try:
                os.rename(filename, new_filename)
                print(f"Renamed: {filename} -> {new_filename}")
            except OSError as e:
                print(f"Error renaming {filename}: {e}")

if __name__ == "__main__":
    # Use '.' for the current folder where the script is located
    # Or paste your full path: r'C:\Users\lamal\Desktop\Programmieren\sts2mods\Downfall'
    target_path = '.' 
    clean_and_rename_assets(target_path)