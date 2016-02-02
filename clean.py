#!/usr/bin/python3

import os;

import BuildScripts.MSBuild

scriptPath = os.path.dirname(os.path.realpath(__file__))

msbuild = BuildScripts.MSBuild.Tool()

solution = ''

if msbuild.is_windows():
    solution = os.path.join(scriptPath, 'LibLSLCC-WithEditor-WithInstaller.sln')
else:
    solution = os.path.join(scriptPath, 'LibLSLCC-NoEditor.sln')


msbuild.run(solution, '/t:clean', '/p:Configuration=Release','/p:Platform=Any CPU',)
msbuild.run(solution, '/t:clean', '/p:Configuration=Debug','/p:Platform=Any CPU',)

msbuild.run(solution, '/t:clean', '/p:Configuration=Release','/p:Platform=x86',)
msbuild.run(solution, '/t:clean', '/p:Configuration=Debug','/p:Platform=x86',)

msbuild.run(solution, '/t:clean', '/p:Configuration=Release','/p:Platform=x64',)
msbuild.run(solution, '/t:clean', '/p:Configuration=Debug','/p:Platform=x64',)