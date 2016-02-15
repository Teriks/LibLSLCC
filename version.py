import subprocess
import os
import re
import shutil

def setVersion(dir,  version):
    file = os.path.join(dir,"Properties", "AssemblyInfo.cs")

    with open(file, 'r') as content_file:
        content = content_file.read();

    tempfile = file+".tmp";
    with open(tempfile, 'w+') as content_file:
        content = re.sub(r'\[assembly: AssemblyVersion\("[^"]*"\)\]',
                         r'[assembly: AssemblyVersion("'+version+'")]', content)

        content = re.sub(r'\[assembly: AssemblyFileVersion\("[^"]*"\)\]',
                         r'[assembly: AssemblyFileVersion("'+version+'")]', content)

        content_file.write(content)

    #if theres an exception prior, the temp file will not move over.
    #an exception above will likely cause the temp file to be blank.
    #blanking the source file would be bad.
    shutil.move(tempfile, file)


dirs = [x for x in next(os.walk('.'))[1] if os.path.isfile(os.path.join(x,"version.txt"))]

print("")

for d in dirs:
    versionFile = os.path.join(d, "version.txt")

    with open(versionFile, 'r') as content_file:
        versionFileContent = content_file.read()

    parts = versionFileContent.split('\n')

    lastTag = parts[0].strip()

    commitsSinceLastTag = int(subprocess.check_output("git rev-list "+lastTag.rstrip()+"..HEAD --count -- "+d).decode("utf-8"))

    versionTemplate = parts[1].strip()

    versionTemplate = versionTemplate.replace("{commits}", str(commitsSinceLastTag))

    print(d + " = " + versionTemplate + "\n")
    setVersion(d, versionTemplate)