#!/usr/bin/python3

import zipfile
import os
import datetime
import shutil
import platform
import subprocess

scriptPath = os.path.dirname(os.path.realpath(__file__))

PLATFORM = platform.system()
ARCH = platform.machine()

MSBUILD = ''

ON_WINDOWS = PLATFORM == 'Windows'

if ON_WINDOWS:
    SOLUTION = os.path.join(scriptPath, 'LibLSLCC-WithEditor-WithInstaller.sln')
    if ARCH != "AMD64":
        MSBUILD = 'C:/Program Files (x86)/MSBuild/12.0/bin/MSBuild.exe'
        if not os.path.isfile(MSBUILD):
            MSBUILD = 'C:/Program Files (x86)/MSBuild/14.0/bin/MSBuild.exe'
    else:
        MSBUILD = 'C:/Program Files (x86)/MSBuild/12.0/bin/amd64/MSBuild.exe'
        if not os.path.isfile(MSBUILD):
            MSBUILD = 'C:/Program Files (x86)/MSBuild/14.0/bin/amd64/MSBuild.exe'

    if not os.path.isfile(MSBUILD):
        print("Neither MSBuild 12.0 or 14.0 could not be found, please install Visual Studios 2012+, OR:")
        print("MSBuild 12.0 Standalone: https://www.microsoft.com/en-us/download/details.aspx?id=40760")
        print("MSBuild 14.0 Standalone: https://www.microsoft.com/en-in/download/details.aspx?id=48159")
else:
    SOLUTION = os.path.join(scriptPath, 'LibLSLCC-NoEditor.sln')
    MSBUILD = subprocess.check_output(['which', 'xbuild']).strip()
    if not MSBUILD:
        print("xbuild command is not present on your system, please install mono")

old_wd = os.getcwd()

os.chdir(os.path.dirname(MSBUILD))



# build an Any CPU lslcc binary for distribution
subprocess.call([MSBUILD, SOLUTION, '/t:lslcc_cmd', '/p:Configuration=Release', '/p:Platform=Any CPU',
                 '/p:TargetFrameworkVersion=v4.0'])


# these won't get built in the next step if we are not on windows.
# when building the editor installer they are auto built because they are dependencies
# but not on mono
if not ON_WINDOWS:
    subprocess.call([MSBUILD, SOLUTION, '/t:LibLSLCC', '/p:Configuration=Release', '/p:Platform=Any CPU',
                 '/p:TargetFrameworkVersion=v4.0'])
    subprocess.call([MSBUILD, SOLUTION, '/t:LibLSLCC', '/p:Configuration=Debug', '/p:Platform=Any CPU',
                 '/p:TargetFrameworkVersion=v4.0'])


# build the installers on windows
if ON_WINDOWS:
    print(MSBUILD)
    subprocess.call([MSBUILD, SOLUTION, '/t:LSLCCEditorInstaller', '/p:Configuration=Release', '/p:Platform=x86',
                     '/p:TargetFrameworkVersion=v4.5'])
    subprocess.call([MSBUILD, SOLUTION, '/t:LSLCCEditorInstaller', '/p:Configuration=Release', '/p:Platform=x64',
                     '/p:TargetFrameworkVersion=v4.5'])

os.chdir(old_wd)

curTime = datetime.datetime.now()
release_stamp = '{dt.month}-{dt.day}-{dt.year}_{dt:%I}-{dt:%M}{dt:%p}'.format(dt=curTime)


print('Release Stamp: ' + release_stamp)

print('===========================================')
print('\n')

outputDir = os.path.join(scriptPath, 'BinaryRelease')

binariesZip = os.path.join(outputDir, 'LibLSLCC_Binaries_' + release_stamp + '.zip')

libraryPath = os.path.join(scriptPath, 'LibLSLCC')

lslccPath = os.path.join(scriptPath, 'lslcc_cmd')

installerPath = os.path.join(scriptPath, 'LSLCCEditorInstaller')

installerBasicName = 'LSLCCEditorInstaller'

installerExtension = '.msi'

lib_anyCpu = os.path.join(libraryPath, 'bin', 'AnyCPU')

lib_thirdparty_licenses = os.path.join(libraryPath, 'bin', 'ThirdPartyLicenses')

lib_licence = os.path.join(libraryPath, 'bin', 'LICENSE')

lslcc_anyCpu = os.path.join(lslccPath, 'bin', 'AnyCPU')

if os.path.isdir(outputDir):
    shutil.rmtree(outputDir, True)


while os.path.isdir(outputDir):
    print("Waiting for rmtree...");
    
os.mkdir(outputDir)


def zip_dir_relative(path, zip_file, **kwargs):
    for root, dirs, files in os.walk(path):
        for file in files:

            into_arch_dir = kwargs.get('archTopDir', None)
            rel_dir_transform = kwargs.get('archDirTransform', None)

            full_path = os.path.join(root, file)
            rel_path = os.path.relpath(root, scriptPath)

            arc_path = os.path.join(rel_path, file)

            if rel_dir_transform is not None:
                arc_path = rel_dir_transform(arc_path)

            if into_arch_dir is not None:
                arc_path = os.path.join(into_arch_dir, rel_path, file)

            print('Zip:\n\t' + os.path.relpath(full_path,scriptPath) + ' -> ' + arc_path)
            zip_file.write(full_path, arc_path)



def remove_second_folder_down(path):
    pathParts = path.split(os.sep)
    pathParts.pop(1)
    return os.path.join(*pathParts)


# make the timestamped binary release zip file
with zipfile.ZipFile(binariesZip, 'w') as zip_file:

    zip_dir_relative(lib_anyCpu, zip_file, archDirTransform=remove_second_folder_down)
	
    zip_dir_relative(lib_thirdparty_licenses, zip_file, archDirTransform=remove_second_folder_down)

    lslccArchDir = os.path.basename(lslccPath)

    #only the release build of lslcc_cmd gets put in the zip

    zip_dir_relative(os.path.join(lslcc_anyCpu, "Release"), zip_file, archDirTransform=remove_second_folder_down)

    zip_file.write(lib_licence, os.path.basename('LICENSE'))



# copy and time stamp the installers when on windows
if ON_WINDOWS:
    x64_installer = os.path.relpath(os.path.join(installerPath, 'bin', 'x64', 'Release', installerBasicName + installerExtension), scriptPath)
    x86_installer = os.path.relpath(os.path.join(installerPath, 'bin', 'x86', 'Release', installerBasicName + installerExtension), scriptPath)

    x64_installerDest = os.path.relpath(os.path.join(outputDir, installerBasicName + '_x64_' + release_stamp + installerExtension), scriptPath)
    x86_installerDest = os.path.relpath(os.path.join(outputDir, installerBasicName + '_x86_' + release_stamp + installerExtension), scriptPath)

    print('\n')
    print('===========================================')
    print('\n')

    print('Copy x64 Installer:\n\t' + x64_installer + ' -> ' + x64_installerDest)
    shutil.copy2(x64_installer, x64_installerDest)

    print('\n')

    print('Copy x64 Installer:\n\t' + x86_installer + ' -> ' + x86_installerDest)
    shutil.copy2(x86_installer, x86_installerDest)

    print('\n')

