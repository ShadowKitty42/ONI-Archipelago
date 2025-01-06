import compileall
import os

for file in os.listdir('.\\test'):
    if file.endswith('.py'):
        os.remove(os.path.join('.\\test', file))
compileall.compile_dir(".", 0, None, True)
for file in os.listdir('.'):
    if file.endswith('.py'):
        newName = file.split(".")[0] +'.' + file.split(".")[-1]
        os.system("copy " + os.path.join(".", file) + " " + os.path.join(".\\test", newName))
        os.system("copy " + os.path.join(".", file) + " " + os.path.join("C:\ProgramData\Archipelago\lib\worlds\oni", newName))
        