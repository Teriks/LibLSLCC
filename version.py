#!/usr/bin/python3

import subprocess
import os
import re
import shutil
import json


def setVersion(file, version):

    with open(file, 'r') as content_file:
        content = content_file.read();

    match = re.search(r'\[assembly: AssemblyVersion\("([^"]*)"\)\]', content)

    was = match.group(1)

    if was == version:
        return was

    content = re.sub(r'\[assembly: AssemblyVersion\("[^"]*"\)\]',
                     r'[assembly: AssemblyVersion("' + version + '")]', content)

    content = re.sub(r'\[assembly: AssemblyFileVersion\("[^"]*"\)\]',
                     r'[assembly: AssemblyFileVersion("' + version + '")]', content)

    tempfile = file + ".tmp";

    with open(tempfile, 'w+') as content_file:
        content_file.write(content)

    # if theres an exception prior, the temp file will not move over.
    # an exception above will likely cause the temp file to be blank.
    # blanking the source file would be bad.
    shutil.move(tempfile, file)
    return was


print("")
print("Updating assembly versions...")
print("")

scriptDir = os.path.dirname(os.path.realpath(__file__));

versionsDir = os.path.join(scriptDir, "versions")

with open("version.json", 'r') as content_file:
    versionFileContent = json.load(content_file)

for dir in versionFileContent.keys():

    versionFile = os.path.join(versionsDir, dir, "Version.cs")

    lastTag = versionFileContent[dir]["last_tag"]

    commitsSinceLastTag = int(
        subprocess.check_output("git rev-list " + lastTag.rstrip() + "..HEAD --count -- " + dir).decode("utf-8"))

    versionTemplate = versionFileContent[dir]["version_template"]

    versionTemplate = versionTemplate.replace("{commits_since_last_tag}", str(commitsSinceLastTag))

    was = setVersion(versionFile, versionTemplate)

    if versionTemplate != was:
        print(dir + " = " + versionTemplate + " was " + was)
    else:
        print(dir + " version already up to date.")


print("")
