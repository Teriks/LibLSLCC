#!/usr/bin/env python

import zipfile
import os
import datetime
import shutil
import platform
import subprocess

scriptPath = os.path.dirname(os.path.realpath(__file__));




PLATFORM = platform.system();
ARCH = platform.machine();


MSBUILD = '';


ON_WINDOWS = PLATFORM == 'Windows';

if ON_WINDOWS:
    SOLUTION = os.path.join(scriptPath,'LibLSLCC-WithEditor-WithInstaller.sln');
    if ARCH != "AMD64":
        MSBUILD = 'C:/Program Files (x86)/MSBuild/12.0/bin/MSBuild.exe';
        if not os.path.isfile(MSBUILD):
            MSBUILD = 'C:/Program Files (x86)/MSBuild/14.0/bin/MSBuild.exe';
    else:
        MSBUILD = 'C:/Program Files (x86)/MSBuild/12.0/bin/amd64/MSBuild.exe';
        if not os.path.isfile(MSBUILD):
            MSBUILD = 'C:/Program Files (x86)/MSBuild/14.0/bin/amd64/MSBuild.exe';

    if not os.path.isfile(MSBUILD):
        print("Neither MSBuild 12.0 or 14.0 could not be found, please install Visual Studios 2012+, OR:");
        print("MSBuild 12.0 Standalone: https://www.microsoft.com/en-us/download/details.aspx?id=40760");
        print("MSBuild 14.0 Standalone: https://www.microsoft.com/en-in/download/details.aspx?id=48159");
else:
    SOLUTION = os.path.join(scriptPath,'LibLSLCC-NoEditor.sln');
    MSBUILD = subprocess.check_output(['which', 'xbuild']).strip();
    if not MSBUILD:
        print("xbuild command is not present on your system, please install mono")



old_wd = os.getcwd();


os.chdir(os.path.dirname(MSBUILD));

#build LibLSLCC in Any CPU mode since the editor installer does not depend on it
subprocess.call([MSBUILD, SOLUTION, '/t:LibLSLCC', '/p:Configuration=Release', '/p:Platform=Any CPU', '/p:TargetFrameworkVersion=v4.0']);

#build an Any CPU lslcc binary for distribution
subprocess.call([MSBUILD, SOLUTION, '/t:lslcc_cmd', '/p:Configuration=Release', '/p:Platform=Any CPU', '/p:TargetFrameworkVersion=v4.0']);


#these won't get built in the next step if we are not on windows
#when building the editor installer they are auto built because they are dependencies
#but not on mono
if not ON_WINDOWS:
    subprocess.call([MSBUILD, SOLUTION, '/t:LibLSLCC', '/p:Configuration=Release', '/p:Platform=x86', '/p:TargetFrameworkVersion=v4.0']);
    subprocess.call([MSBUILD, SOLUTION, '/t:LibLSLCC', '/p:Configuration=Release', '/p:Platform=x64', '/p:TargetFrameworkVersion=v4.0']);


#build the installers on windows
if ON_WINDOWS:
    subprocess.call([MSBUILD, SOLUTION, '/t:LSLCCEditorInstaller', '/p:Configuration=Release', '/p:Platform=x86', '/p:TargetFrameworkVersion=v4.5']);
    subprocess.call([MSBUILD, SOLUTION, '/t:LSLCCEditorInstaller', '/p:Configuration=Release', '/p:Platform=x64', '/p:TargetFrameworkVersion=v4.5']);


os.chdir(old_wd);


curTime = datetime.datetime.now();
releasestamp = '{dt.day}-{dt.month}-{dt.year}_{dt:%I}-{dt:%M}{dt:%p}'.format(dt = curTime);


print('Release Stamp: '+releasestamp)

print('===========================================');
print('\n');



outputDir = os.path.join(scriptPath,'BinaryRelease');

binariesZip = os.path.join(outputDir,'LibLSLCC_Binaries_'+releasestamp+'.zip');

libraryPath = os.path.join(scriptPath,'LibLSLCC');

lslccPath = os.path.join(scriptPath,'lslcc_cmd');

installerPath = os.path.join(scriptPath,'LSLCCEditorInstaller')

installerBasicName = 'LSLCCEditorInstaller'

installerExtension = '.msi'


lib_anyCpu = os.path.join(libraryPath,'bin','AnyCPU');
lib_x86 = os.path.join(libraryPath,'bin','x86');
lib_x64 = os.path.join(libraryPath,'bin','x64');
lib_thirdparty_licenses = os.path.join(libraryPath,'bin','ThirdPartyLicenses');
lib_licence = os.path.join(libraryPath,'bin','LICENSE');


lslcc_anyCpu = os.path.join(lslccPath,'bin','AnyCPU');



if os.path.isdir(outputDir):
    shutil.rmtree(outputDir,True);



os.mkdir(outputDir);


def zipdir(path, ziph, intoArchDir=None):
    for root, dirs, files in os.walk(path):
        for file in files:
            full_path = os.path.join(root, file);
            rel_path = os.path.commonprefix([path, full_path]);
            root_base = os.path.basename(root);
            rel_base = os.path.basename(rel_path);

            arc_path = '';
            if intoArchDir == None:
                arc_path = os.path.join(rel_base,root_base,file);
            else:
                arc_path = os.path.join(intoArchDir,rel_base,root_base,file);

            print('Zip: '+full_path+' -> '+arc_path);
            ziph.write(full_path,arc_path)



#make the timestamped binary release zip file
with zipfile.ZipFile(binariesZip, 'w') as zip:

    libArchDir = os.path.basename(libraryPath);

    zipdir(lib_anyCpu, zip, libArchDir);
    zipdir(lib_x64, zip, libArchDir);
    zipdir(lib_x86, zip, libArchDir);
    zipdir(lib_thirdparty_licenses, zip, libArchDir);

    lslccArchDir = os.path.basename(lslccPath);

    zipdir(lslcc_anyCpu, zip, lslccArchDir);

    zip.write(lib_licence,os.path.basename('LICENSE'))



#copy and time stamp the installers when on windows
if ON_WINDOWS:

    x64_installer = os.path.join(installerPath,'bin','x64','Release',installerBasicName+installerExtension);
    x86_installer = os.path.join(installerPath,'bin','x86','Release',installerBasicName+installerExtension);

    x64_installerDest = os.path.join(outputDir,installerBasicName+'_x64_'+releasestamp+installerExtension);
    x86_installerDest = os.path.join(outputDir,installerBasicName+'_x86_'+releasestamp+installerExtension);

    print('===========================================');
    print('\n');

    print('Copy x64 Installer: '+x64_installer+' -> '+x64_installerDest);
    shutil.copy2(x64_installer, x64_installerDest);

    print('===========================================');
    print('\n');

    print('Copy x64 Installer: '+x86_installer+' -> '+x86_installerDest);
    shutil.copy2(x86_installer, x86_installerDest);

    print('\n');

