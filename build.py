#!/usr/bin/python3

import zipfile
import os
import datetime
import shutil
import platform
import subprocess
import BuildScripts.MSBuild
import re
from argparse import ArgumentParser
from argparse import RawTextHelpFormatter

scriptPath = os.path.dirname(os.path.realpath(__file__))


def get_liblslcc_version():
    with open(os.path.join(scriptPath, "versions", "LibLSLCC", "Version.cs"), 'r') as content_file:
        content = content_file.read()

    match = re.search(r'\[assembly: AssemblyVersion\("([^"]*)"\)\]', content)
    return match.group(1)


def get_lslcceditor_version():
    with open(os.path.join(scriptPath, "versions", "LSLCCEditor", "Version.cs"), 'r') as content_file:
        content = content_file.read()

    match = re.search(r'\[assembly: AssemblyVersion\("([^"]*)"\)\]', content)
    return match.group(1)


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

            print('Zip:\n\t' + os.path.relpath(full_path, scriptPath) + ' -> ' + arc_path)
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
    default=os.path.join(scriptPath, 'BinaryRelease'),
    dest='binary_release_dir',
    help='The folder to create and place the binary release files in.'
)

args = args_parser.parse_args()

if args.only_build_liblslcc:
    args.build_scraper = False
    args.build_lslcc_cmd = False
    args.build_demo_area = False
    args.build_installer = False
    args.build_editor = False

if args.make_binary_release:
    args.build_scraper = False
    args.build_demo_area = False

MSBuild = BuildScripts.MSBuild.Tool()

no_editor_solution = os.path.join(scriptPath, 'LibLSLCC-NoEditor.sln')
editor_solution = os.path.join(scriptPath, 'LibLSLCC-WithEditor-WithInstaller.sln')

LSLCCEditorTargetFramework = "/p:TargetFrameworkVersion=4.5"
LibLSLCCTargetFramework = "/p:TargetFrameworkVersion=v4.0"

# mono 4.x / xbuild seems to have issues with project files targeting .NET 4.0
if args.liblslcc_net_45 or MSBuild.is_mono() and MSBuild.mono_ver()[0] > 3:
    LibLSLCCTargetFramework = "/p:TargetFrameworkVersion=v4.5"

LibLSLCCBuildTargets = "/t:LibLSLCC"

if args.build_lslcc_cmd:
    LibLSLCCBuildTargets += ";lslcc_cmd"

if args.build_scraper:
    LibLSLCCBuildTargets += ";LibraryDataScrapingTool"

if args.build_demo_area:
    LibLSLCCBuildTargets += ";DemoArea"

if not args.release_only:
    MSBuild.run(no_editor_solution, LibLSLCCBuildTargets, '/p:Configuration=Debug', '/p:Platform=Any CPU',
                LibLSLCCTargetFramework)

if not args.debug_only:
    MSBuild.run(no_editor_solution, LibLSLCCBuildTargets, '/p:Configuration=Release', '/p:Platform=Any CPU',
                LibLSLCCTargetFramework)

# build the installers on windows
if MSBuild.is_windows() and args.build_installer and args.build_editor:
    if not args.release_only:
        MSBuild.run(editor_solution, '/t:LSLCCEditor', '/p:Configuration=Debug', '/p:Platform=Any CPU',
                    LSLCCEditorTargetFramework)

    MSBuild.run(editor_solution, '/t:LSLCCEditorInstaller', '/p:Configuration=Release', '/p:Platform=x86',
                '/p:Version=' + LSLCCEditor_Version, LSLCCEditorTargetFramework)
    MSBuild.run(editor_solution, '/t:LSLCCEditorInstaller', '/p:Configuration=Release', '/p:Platform=x64',
                '/p:Version=' + LSLCCEditor_Version, LSLCCEditorTargetFramework)

if MSBuild.is_windows() and not args.build_installer and args.build_editor:
    if not args.release_only:
        MSBuild.run(editor_solution, '/t:LSLCCEditor', '/p:Configuration=Debug', '/p:Platform=Any CPU',
                    LSLCCEditorTargetFramework)

    if not args.debug_only:
        MSBuild.run(editor_solution, '/t:LSLCCEditor', '/p:Configuration=Release', '/p:Platform=Any CPU',
                    LSLCCEditorTargetFramework)

if not args.make_binary_release:
    exit(0)

print('===========================================')
print('\n')

installerBasicName = 'LSLCCEditorInstaller'

installerExtension = '.msi'

binaryReleaseOutputDir = args.binary_release_dir

binariesZipPath = os.path.join(binaryReleaseOutputDir, 'LibLSLCC_Binaries_' + LibLSLCC_Version + '.zip')

LibLSLCC_Path = os.path.join(scriptPath, 'LibLSLCC')

lslcc_cmd_Path = os.path.join(scriptPath, 'lslcc_cmd')

installerPath = os.path.join(scriptPath, 'LSLCCEditorInstaller')

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

# make the timestamped binary release zip file
with zipfile.ZipFile(binariesZipPath, 'w', zipMode) as zip_file:
    if not args.debug_only:
        zip_dir_relative(os.path.join(LibLSLCC_AnyCpu_Path, "Release"), zip_file,
                         archDirTransform=remove_second_folder_down)

    if not args.release_only:
        zip_dir_relative(os.path.join(LibLSLCC_AnyCpu_Path, "Debug"), zip_file,
                         archDirTransform=remove_second_folder_down)

    zip_dir_relative(LibLSLCC_ThirdPartyLicenses_Path, zip_file, archDirTransform=remove_second_folder_down)

    zip_dir_relative(lslcc_cmd_ThirdPartyLicenses_Path, zip_file, archDirTransform=remove_second_folder_down)

    lslcc_cmd_Arch_Path = os.path.basename(lslcc_cmd_Path)

    # only the release build of lslcc_cmd gets put in the zip
    if args.build_lslcc_cmd and not args.debug_only:
        zip_dir_relative(os.path.join(lslcc_cmd_AnyCpu_Path, "Release"), zip_file,
                         archDirTransform=remove_second_folder_down)

    zip_file.write(LibLSLCC_Licence_Path, os.path.basename('LICENSE'))

# copy and time stamp the installers when on windows
if MSBuild.is_windows():
    x64_installer = os.path.relpath(
        os.path.join(installerPath, 'bin', 'x64', 'Release', installerBasicName + installerExtension), scriptPath)
    x86_installer = os.path.relpath(
        os.path.join(installerPath, 'bin', 'x86', 'Release', installerBasicName + installerExtension), scriptPath)

    x64_installerDest = os.path.relpath(
        os.path.join(binaryReleaseOutputDir, installerBasicName + '_x64_' + LSLCCEditor_Version + installerExtension),
        scriptPath)
    x86_installerDest = os.path.relpath(
        os.path.join(binaryReleaseOutputDir, installerBasicName + '_x86_' + LSLCCEditor_Version + installerExtension),
        scriptPath)

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
