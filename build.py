#!/usr/bin/python3
import sys
import os

if sys.version_info[0] == 2:
    print('Please use python3 to run this script, preferably version 3.4+')
    exit()

script_path = os.path.dirname(os.path.realpath(__file__))
sys.path.insert(0, os.path.join(script_path, 'BuildScriptLibs'))


msbuildpy_version = '0.5.1.0'
msbuildpy_pip_install_target = 'git+https://github.com/Teriks/msbuildpy.git@'+msbuildpy_version


import zipfile
import datetime
import shutil
import platform
import subprocess
import re
import json
import importlib
import time
from argparse import ArgumentParser
from argparse import RawTextHelpFormatter


def set_version(file, version):
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

    
def update_assembly_versions():
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
            subprocess.check_output(["git", "rev-list", lastTag.rstrip() + "..HEAD", "--count"]).decode("utf-8"))

        versionTemplate = versionFileContent[dir]["version_template"]

        versionTemplate = versionTemplate.replace("{commits_since_last_tag}", str(commitsSinceLastTag))

        was = set_version(versionFile, versionTemplate)

        if versionTemplate != was:
            print(dir + " = " + versionTemplate + " was " + was)
        else:
            print(dir + " version already up to date.")


    print("")


def get_liblslcc_version():
    with open(os.path.join(script_path, "versions", "LibLSLCC", "Version.cs"), 'r') as content_file:
        content = content_file.read()

    match = re.search(r'\[assembly: AssemblyVersion\("([^"]*)"\)\]', content)
    return match.group(1)


def get_lslcceditor_version():
    with open(os.path.join(script_path, "versions", "LSLCCEditor", "Version.cs"), 'r') as content_file:
        content = content_file.read()

    match = re.search(r'\[assembly: AssemblyVersion\("([^"]*)"\)\]', content)
    return match.group(1)


def zip_dir_relative(path, zip_file, **kwargs):

    into_arch_dir = kwargs.get('archTopDir', None)
    rel_dir_transform = kwargs.get('archDirTransform', None)
    file_filter = kwargs.get('fileFilter', None)
    
    for root, dirs, files in os.walk(path):
        for file in files:
            full_path = os.path.join(root, file)
            rel_path = os.path.relpath(root, script_path)

            arc_path = os.path.join(rel_path, file)

            if rel_dir_transform is not None:
                arc_path = rel_dir_transform(arc_path)

            if into_arch_dir is not None:
                arc_path = os.path.join(into_arch_dir, rel_path, file)
                
            if file_filter is not None:
                if not file_filter(full_path):
                    continue

            print('Zip:\n\t' + os.path.relpath(full_path, script_path) + ' -> ' + arc_path)
            zip_file.write(full_path, arc_path)


def remove_second_folder_down(path):
    pathparts = path.split(os.sep)
    pathparts.pop(1)
    return os.path.join(*pathparts)


LibLSLCC_Version = get_liblslcc_version()

LSLCCEditor_Version = get_lslcceditor_version()

args_parser = ArgumentParser(formatter_class=RawTextHelpFormatter)

args_parser.add_argument(
    '--only-release',
    action='store_true',
    dest='release_only',
    help='Only build Release binaries.\n\n'
)

args_parser.add_argument(
    '--only-debug',
    action='store_true',
    dest='debug_only',
    help='Only build Debug binaries.\n\n'
)

args_parser.add_argument(
    '--only-liblslcc',
    action='store_true',
    dest='only_build_liblslcc',
    help='Only build the LibLSLCC library project and nothing else.\n\n'
)

args_parser.add_argument(
    '--liblslcc-net-v45',
    action='store_true',
    dest='liblslcc_net_45',
    help='Force the LibLSLCC library project to target .NET 4.5 instead of .NET 4.0.\n\n'
)

args_parser.add_argument(
    '--no-scraping-tool',
    action='store_false',
    dest='build_scraper',
    help='Prevent the LibraryDataScrapingTool project from being built.\n\n'
)

args_parser.add_argument(
    '--no-lslcc-cmd',
    action='store_false',
    dest='build_lslcc_cmd',
    help='Prevent the lslcc_cmd project from being built.\n\n'
)

args_parser.add_argument(
    '--no-demo-area',
    action='store_false',
    dest='build_demo_area',
    help='Prevent the DemoArea project from being built.\n\n'
)

args_parser.add_argument(
    '--no-editor',
    action='store_false',
    dest='build_editor',
    help='Prevent both the editor and installer from being built.\nOn Mono these do not build regardless.\n\n'
)

args_parser.add_argument(
    '--no-installer',
    action='store_false',
    dest='build_installer',
    help='Prevent the installer from being built if you do not have WiX.\nOn Mono it does not build regardless.\n\n'
)

args_parser.add_argument(
    '--binary-release',
    action='store_true',
    dest='make_binary_release',
    help=
    'Create a version stamped zip file of LibLSLCC and lslcc_cmd (plus installer file\'s on windows if --no-installer is not specified),\n' +
    'inside the directory specified by --binary-release-dir (Default directory is BinaryRelease).\n\n'
)

args_parser.add_argument(
    '--binary-release-dir',
    metavar='PATH',
    default=os.path.join(script_path, 'BinaryRelease'),
    dest='binary_release_dir',
    help='The folder to create and place the binary release files in.'
)

args_parser.add_argument(
    '--clean',
    action='store_true',
    dest='clean_build',
    help='Clean the build.'
)

args_parser.add_argument(
    '--update-versions',
    action='store_true',
    dest='update_versions',
    help='Update assembly versions in accordance with version.json.'
)


args = args_parser.parse_args()


if args.update_versions:
    update_assembly_versions()
    exit()



def install_deps():
    try:
        import pip
    except ImportError:
        print('Please install pip package manager for python3, see README.md for help')
        exit()
    pip.main(['install', '--upgrade', '--target', os.path.join(script_path, 'BuildScriptLibs'), msbuildpy_pip_install_target])    


def re_run():
    time.sleep(1)
    subprocess.call([sys.executable, os.path.realpath(__file__)]+sys.argv[1:])
    exit()


try:
    import msbuildpy
    if msbuildpy.__version__ != msbuildpy_version: 
        install_deps()
        re_run()
except ImportError:
    install_deps()
    re_run()


import msbuildpy.sysinspect


if msbuildpy.sysinspect.is_windows():
    MSBuild = msbuildpy.find_msbuild('msbuild >=12.*')
    if len(MSBuild) == 0:
        print('Could not find a compatible version of msbuild')
        
    # get the most recent major version
    MSBuild = MSBuild[0]
else:
    MSBuild = msbuildpy.find_msbuild('xbuild >=12.*')
    if len(MSBuild) == 0:
        print('Could not find a compatible version of xbuild')
        
    # get the most recent major version
    MSBuild = MSBuild[0]


def call_msbuild(*args):
    subprocess.call([MSBuild.path]+list(args))


if args.clean_build:
    if msbuildpy.sysinspect.is_windows():
        solution = os.path.join(script_path, 'LibLSLCC-WithEditor-WithInstaller.sln')
    else:
        solution = os.path.join(script_path, 'LibLSLCC-NoEditor.sln')


    call_msbuild(solution, '/t:clean', '/p:Configuration=Release','/p:Platform=Any CPU',)
    call_msbuild(solution, '/t:clean', '/p:Configuration=Debug','/p:Platform=Any CPU',)

    call_msbuild(solution, '/t:clean', '/p:Configuration=Release','/p:Platform=x86',)
    call_msbuild(solution, '/t:clean', '/p:Configuration=Debug','/p:Platform=x86',)

    call_msbuild(solution, '/t:clean', '/p:Configuration=Release','/p:Platform=x64',)
    call_msbuild(solution, '/t:clean', '/p:Configuration=Debug','/p:Platform=x64',)
    exit()


if args.only_build_liblslcc:
    args.build_scraper = False
    args.build_lslcc_cmd = False
    args.build_demo_area = False
    args.build_installer = False
    args.build_editor = False


if args.make_binary_release:
    args.build_scraper = False
    args.build_demo_area = False


no_editor_solution = os.path.join(script_path, 'LibLSLCC-NoEditor.sln')
editor_solution = os.path.join(script_path, 'LibLSLCC-WithEditor-WithInstaller.sln')

LSLCCEditorTargetFramework = "/p:TargetFrameworkVersion=4.5"
LibLSLCCTargetFramework = "/p:TargetFrameworkVersion=v4.0"

mono_vm = msbuildpy.sysinspect.get_mono_vm()


# mono 4.x / cannot build v4.0 assemblies
if args.liblslcc_net_45 or (not msbuildpy.sysinspect.is_windows() and mono_vm.version[0] > 3):
    LibLSLCCTargetFramework = "/p:TargetFrameworkVersion=v4.5"

LibLSLCCBuildTargets = "/t:LibLSLCC"

if args.build_lslcc_cmd:
    LibLSLCCBuildTargets += ";lslcc_cmd"


if args.build_scraper:
    LibLSLCCBuildTargets += ";LibraryDataScrapingTool"


if args.build_demo_area:
    LibLSLCCBuildTargets += ";DemoArea"


if not args.release_only:
    call_msbuild(no_editor_solution, LibLSLCCBuildTargets, '/p:Configuration=Debug', '/p:Platform=Any CPU',
                LibLSLCCTargetFramework)


if not args.debug_only:
    call_msbuild(no_editor_solution, LibLSLCCBuildTargets, '/p:Configuration=Release', '/p:Platform=Any CPU',
                LibLSLCCTargetFramework)


# build the installers on windows
if msbuildpy.sysinspect.is_windows() and args.build_installer and args.build_editor:
    if not args.release_only:
        call_msbuild(editor_solution, '/t:LSLCCEditor', '/p:Configuration=Debug', '/p:Platform=Any CPU',
                    LSLCCEditorTargetFramework)

    call_msbuild(editor_solution, '/t:LSLCCEditorInstaller', '/p:Configuration=Release', '/p:Platform=x86',
                '/p:Version=' + LSLCCEditor_Version, LSLCCEditorTargetFramework)
    call_msbuild(editor_solution, '/t:LSLCCEditorInstaller', '/p:Configuration=Release', '/p:Platform=x64',
                '/p:Version=' + LSLCCEditor_Version, LSLCCEditorTargetFramework)


if msbuildpy.sysinspect.is_windows() and not args.build_installer and args.build_editor:
    if not args.release_only:
        call_msbuild(editor_solution, '/t:LSLCCEditor', '/p:Configuration=Debug', '/p:Platform=Any CPU',
                    LSLCCEditorTargetFramework)

    if not args.debug_only:
        call_msbuild(editor_solution, '/t:LSLCCEditor', '/p:Configuration=Release', '/p:Platform=Any CPU',
                    LSLCCEditorTargetFramework)


if not args.make_binary_release:
    exit(0)

print('===========================================')
print('\n')

installerBasicName = 'LSLCCEditorInstaller'

installerExtension = '.msi'

binaryReleaseOutputDir = args.binary_release_dir

binariesZipPath = os.path.join(binaryReleaseOutputDir, 'LibLSLCC_Binaries_' + LibLSLCC_Version + '.zip')

LibLSLCC_Path = os.path.join(script_path, 'LibLSLCC')

lslcc_cmd_Path = os.path.join(script_path, 'lslcc_cmd')

installerPath = os.path.join(script_path, 'LSLCCEditorInstaller')

LibLSLCC_AnyCpu_Path = os.path.join(LibLSLCC_Path, 'bin', 'AnyCPU')

LibLSLCC_ThirdPartyLicenses_Path = os.path.join(LibLSLCC_Path, 'bin', 'ThirdPartyLicenses')

LibLSLCC_Licence_Path = os.path.join(LibLSLCC_Path, 'bin', 'LICENSE')

lslcc_cmd_AnyCpu_Path = os.path.join(lslcc_cmd_Path, 'bin', 'AnyCPU')

lslcc_cmd_ThirdPartyLicenses_Path = os.path.join(lslcc_cmd_Path, 'bin', 'ThirdPartyLicenses')

if not os.path.isdir(binaryReleaseOutputDir):
    os.mkdir(binaryReleaseOutputDir)

try:
    import zlib

    zipMode = zipfile.ZIP_DEFLATED
except:
    zipMode = zipfile.ZIP_STORED
    
    
def zip_extension_filter(path):
    return os.path.splitext(path)[1] != ".tmp"

# make the timestamped binary release zip file
with zipfile.ZipFile(binariesZipPath, 'w', zipMode) as zip_file:
    if not args.debug_only:
        zip_dir_relative(os.path.join(LibLSLCC_AnyCpu_Path, "Release"), zip_file,
                         archDirTransform=remove_second_folder_down,
                         fileFilter=zip_extension_filter)

    if not args.release_only:
        zip_dir_relative(os.path.join(LibLSLCC_AnyCpu_Path, "Debug"), zip_file,
                         archDirTransform=remove_second_folder_down,
                         fileFilter=zip_extension_filter)

    zip_dir_relative(LibLSLCC_ThirdPartyLicenses_Path, zip_file, 
                     archDirTransform=remove_second_folder_down, 
                     fileFilter=zip_extension_filter)

    zip_dir_relative(lslcc_cmd_ThirdPartyLicenses_Path, zip_file, 
                     archDirTransform=remove_second_folder_down, 
                     fileFilter=zip_extension_filter)

    lslcc_cmd_Arch_Path = os.path.basename(lslcc_cmd_Path)

    # only the release build of lslcc_cmd gets put in the zip
    if args.build_lslcc_cmd and not args.debug_only:
        zip_dir_relative(os.path.join(lslcc_cmd_AnyCpu_Path, "Release"), zip_file,
                         archDirTransform=remove_second_folder_down,
                         fileFilter=zip_extension_filter)

    zip_file.write(LibLSLCC_Licence_Path, os.path.basename('LICENSE'))

# copy and time stamp the installers when on windows
if msbuildpy.sysinspect.is_windows():
    x64_installer = os.path.relpath(
        os.path.join(installerPath, 'bin', 'x64', 'Release', installerBasicName + installerExtension), script_path)
    x86_installer = os.path.relpath(
        os.path.join(installerPath, 'bin', 'x86', 'Release', installerBasicName + installerExtension), script_path)

    x64_installerDest = os.path.relpath(
        os.path.join(binaryReleaseOutputDir, installerBasicName + '_x64_' + LSLCCEditor_Version + installerExtension),
        script_path)
    x86_installerDest = os.path.relpath(
        os.path.join(binaryReleaseOutputDir, installerBasicName + '_x86_' + LSLCCEditor_Version + installerExtension),
        script_path)

    if args.build_installer:
        print('\n')
        print('===========================================')
        print('\n')

        print('Copy x64 Installer:\n\t' + x64_installer + ' -> ' + x64_installerDest)
        shutil.copy2(x64_installer, x64_installerDest)

        print('\n')

        print('Copy x64 Installer:\n\t' + x86_installer + ' -> ' + x86_installerDest)
        shutil.copy2(x86_installer, x86_installerDest)

        print('\n')
