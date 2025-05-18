import compileall
import os
from fnmatch import fnmatch
from pathlib import Path
import zipfile

def build_apworld(world_name: str, output_directory: str, ignore: list[str] = ["*__pycache__/*", "*.gitignore"]):
    file_name = os.path.join(output_directory, f"{world_name}.apworld")
    with zipfile.ZipFile(file_name, "w", zipfile.ZIP_DEFLATED, compresslevel = 9) as zf:
        paths = Path('.').rglob("*.*")
        for path in paths:
            include = True
            for i in ignore:
                if fnmatch(path, i):
                    include = False
                    break
            if include:
                relative_path = os.path.join(world_name, *path.parts)
                zf.write(path, relative_path)

world_name = "oni"
ap_directory = "C:\\ProgramData\\Archipelago"
ignore_list = ["*__pycache__/*", "*.gitignore", "*env*", "*.vs*", "*test*", "*.pyproj"]
apworld_output = "Oni Release"
world_dir = os.path.join(ap_directory, "lib", "worlds", world_name)
build_apworld(world_name, os.path.join(ap_directory, apworld_output), ignore_list)

for file in os.listdir('.\\test'):
    if file.endswith('.py'):
        os.remove(os.path.join('.\\test', file))
compileall.compile_dir(".", 0, None, True)
for file in os.listdir('.'):
    if file.endswith('.py'):
        newName = file.split(".")[0] +'.' + file.split(".")[-1]
        os.system("copy " + os.path.join(".", file) + " " + os.path.join(".\\test", newName))
        os.system("copy " + os.path.join(".", file) + " " + os.path.join(world_dir, newName))
        