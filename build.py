#!/usr/bin/python3

import zipfile
import os
import datetime
import shutil
import platform
import subprocess
import BuildScripts.MSBuild
from argparse import ArgumentParser
from argparse import RawTextHelpFormatter

scriptPath = os.path.dirname(os.path.realpath(__file__))


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
    help='Only build the LibLSLCC library and nothing else.\n\n'
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
    '--binary-release-zip', 
    action='store_true',
    dest='make_binary_release_zip',
    help='Create a timestamped binary release zip and move it, plus the\ninstallers to the specified binary release directory.\n\n'
    )

args_parser.add_argument(
    '--binary-release-zip-dir',
    metavar='PATH',
    default=os.path.join(scriptPath, 'BinaryRelease'), 
    dest='binary_release_zip_dir',
    help='The folder to create and place the binary release files in.'
    )



args = args_parser.parse_args();


if args.only_build_liblslcc:
    
    args.build_scraper = False
    args.build_lslcc_cmd = False
    args.build_demo_area = False
    args.build_installer = False
    args.build_editor = False


msbuild = BuildScripts.MSBuild.Tool();


no_editor_solution = os.path.join(scriptPath, 'LibLSLCC-NoEditor.sln');
editor_solution = os.path.join(scriptPath, 'LibLSLCC-WithEditor-WithInstaller.sln');


libLSLCCtargetFramework = "/p:TargetFrameworkVersion=v4.0"

if msbuild.is_mono() and msbuild.mono_ver()[0] > 3:
    libLSLCCtargetFramework = "/p:TargetFrameworkVersion=v4.5"



if not args.release_only:
    msbuild.run(no_editor_solution, '/t:LibLSLCC', '/p:Configuration=Debug', '/p:Platform=Any CPU',
                libLSLCCtargetFramework)

if not args.debug_only:
    msbuild.run(no_editor_solution, '/t:LibLSLCC', '/p:Configuration=Release', '/p:Platform=Any CPU',
                libLSLCCtargetFramework)


if args.build_lslcc_cmd:
    if not args.release_only and not args.make_binary_release_zip:
        msbuild.run(no_editor_solution, '/t:lslcc_cmd', '/p:Configuration=Debug', '/p:Platform=Any CPU',
                    libLSLCCtargetFramework)

    if not args.debug_only:
        msbuild.run(no_editor_solution, '/t:lslcc_cmd', '/p:Configuration=Release', '/p:Platform=Any CPU',
                    libLSLCCtargetFramework)


if args.build_scraper:
    if not args.release_only:
        msbuild.run(no_editor_solution, '/t:LibraryDataScrapingTool', '/p:Configuration=Debug', '/p:Platform=Any CPU',
                    libLSLCCtargetFramework)

    if not args.debug_only:
        msbuild.run(no_editor_solution, '/t:LibraryDataScrapingTool', '/p:Configuration=Release', '/p:Platform=Any CPU',
                    libLSLCCtargetFramework)


if args.build_demo_area:
    if not args.release_only:
        msbuild.run(no_editor_solution, '/t:DemoArea', '/p:Configuration=Debug', '/p:Platform=Any CPU',
                    libLSLCCtargetFramework)

    if not args.debug_only:
        msbuild.run(no_editor_solution, '/t:DemoArea', '/p:Configuration=Release', '/p:Platform=Any CPU',
                    libLSLCCtargetFramework)


# build the installers on windows
if msbuild.is_windows() and args.build_installer and args.build_editor:
    msbuild.run(editor_solution, '/t:LSLCCEditorInstaller', '/p:Configuration=Release', '/p:Platform=x86',
                     '/p:TargetFrameworkVersion=v4.5')
    msbuild.run(editor_solution, '/t:LSLCCEditorInstaller', '/p:Configuration=Release', '/p:Platform=x64',
                     '/p:TargetFrameworkVersion=v4.5')



if msbuild.is_windows() and not args.build_installer and args.build_editor:
    if not args.release_only:
        msbuild.run(editor_solution, '/t:LSLCCEditor', '/p:Configuration=Debug', '/p:Platform=Any CPU',
                    '/p:TargetFrameworkVersion=v4.5')

    if not args.debug_only:
        msbuild.run(editor_solution, '/t:LSLCCEditor', '/p:Configuration=Release', '/p:Platform=Any CPU',
                    '/p:TargetFrameworkVersion=v4.5')



if not args.make_binary_release_zip:
    exit(0)



curTime = datetime.datetime.now()
release_stamp = '{dt.month}-{dt.day}-{dt.year}_{dt:%I}-{dt:%M}{dt:%p}'.format(dt=curTime)


print('Release Stamp: ' + release_stamp)

print('===========================================')
print('\n')


outputDir = args.binary_release_zip_dir

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

if not os.path.isdir(outputDir):
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

    if not args.debug_only:
        zip_dir_relative(os.path.join(lib_anyCpu, "Release"), zip_file, archDirTransform=remove_second_folder_down)

    if not args.release_only:
        zip_dir_relative(os.path.join(lib_anyCpu, "Debug"), zip_file, archDirTransform=remove_second_folder_down)

    zip_dir_relative(lib_thirdparty_licenses, zip_file, archDirTransform=remove_second_folder_down)

    lslccArchDir = os.path.basename(lslccPath)

    #only the release build of lslcc_cmd gets put in the zip
    if args.build_lslcc_cmd and not args.debug_only:
        zip_dir_relative(os.path.join(lslcc_anyCpu, "Release"), zip_file, archDirTransform=remove_second_folder_down)

    zip_file.write(lib_licence, os.path.basename('LICENSE'))



# copy and time stamp the installers when on windows
if msbuild.is_windows():
    x64_installer = os.path.relpath(os.path.join(installerPath, 'bin', 'x64', 'Release', installerBasicName + installerExtension), scriptPath)
    x86_installer = os.path.relpath(os.path.join(installerPath, 'bin', 'x86', 'Release', installerBasicName + installerExtension), scriptPath)

    x64_installerDest = os.path.relpath(os.path.join(outputDir, installerBasicName + '_x64_' + release_stamp + installerExtension), scriptPath)
    x86_installerDest = os.path.relpath(os.path.join(outputDir, installerBasicName + '_x86_' + release_stamp + installerExtension), scriptPath)

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

