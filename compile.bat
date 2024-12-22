import compileall
import os

for file in os.listdir('.\\test'):
    if file.endswith('.pyc'):
        os.remove(os.path.join('.\\test', file))
compileall.compile_dir(".", 0, None, True)
for file in os.listdir('.\\__pycache__'):
    if file.endswith('.pyc'):
        newName = file.split(".")[0] +'.' + file.split(".")[-1]
        os.system("copy " + os.path.join(".\\__pycache__", file) + " " + os.path.join(".\\test", newName))
        os.system("copy " + os.path.join(".\\__pycache__", file) + " " + os.path.join("C:\ProgramData\Archipelago\lib\worlds\oni", newName))
        