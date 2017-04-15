#!/usr/bin/python3

import os
import sys

scriptPath = os.path.dirname(os.path.realpath(__file__))
sys.path.append(os.path.join(scriptPath, 'BuildScriptLibs'))

msbuildpy_version = '0.3.1.0'

try:
    import msbuildpy
    if msbuildpy.__version__ != msbuildpy_version: raise ImportError()
except ImportError:
    import pip
    pip.main(['install', 'git+https://github.com/Teriks/msbuildpy.git@'+msbuildpy_version, '--upgrade', '--target', os.path.join(scriptPath, 'BuildScriptLibs')])
    import msbuildpy

import msbuildpy.inspect

import subprocess

solution = ''


if msbuildpy.inspect.is_windows():
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

if msbuildpy.inspect.is_windows():
    solution = os.path.join(scriptPath, 'LibLSLCC-WithEditor-WithInstaller.sln')
else:
    solution = os.path.join(scriptPath, 'LibLSLCC-NoEditor.sln')


call_msbuild(solution, '/t:clean', '/p:Configuration=Release','/p:Platform=Any CPU',)
call_msbuild(solution, '/t:clean', '/p:Configuration=Debug','/p:Platform=Any CPU',)

call_msbuild(solution, '/t:clean', '/p:Configuration=Release','/p:Platform=x86',)
call_msbuild(solution, '/t:clean', '/p:Configuration=Debug','/p:Platform=x86',)

call_msbuild(solution, '/t:clean', '/p:Configuration=Release','/p:Platform=x64',)
call_msbuild(solution, '/t:clean', '/p:Configuration=Debug','/p:Platform=x64',)
