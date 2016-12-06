import os
import platform
import subprocess

class Tool:
    _PLATFORM = platform.system()
    _ARCH = platform.machine()
    
    _MSBUILD = ''
    
    _ON_WINDOWS = _PLATFORM == 'Windows'
    
    def __init__(self):
        if self._ON_WINDOWS:
            if self._ARCH != "AMD64":
                self._MSBUILD = 'C:\\Program Files (x86)\\MSBuild\\12.0\\bin\\MSBuild.exe'
                if not os.path.isfile(self._MSBUILD):
                    self._MSBUILD = 'C:\\Program Files (x86)\\MSBuild\\14.0\\bin\\MSBuild.exe'
            else:
                self._MSBUILD = 'C:\\Program Files (x86)\\MSBuild\\12.0\\bin\\amd64\\MSBuild.exe'
                if not os.path.isfile(self._MSBUILD):
                    self._MSBUILD = 'C:\\Program Files (x86)\\MSBuild\\14.0\\bin\\amd64\\MSBuild.exe'
        
            if not os.path.isfile(self._MSBUILD):
                raise Exception('Neither MSBuild 12.0 or 14.0 could not be found, please install Visual Studios 2012+, OR: '
                                'MSBuild 12.0 Standalone: https://www.microsoft.com/en-us/download/details.aspx?id=40760\n'
                                'MSBuild 14.0 Standalone: https://www.microsoft.com/en-in/download/details.aspx?id=48159')
        else:
            self._MSBUILD = subprocess.check_output(['which', 'xbuild']).strip()
            if not self._MSBUILD:
                raise Exception("xbuild command is not present on your system, please install mono")
                
    def run(self,*arguments):
        subprocess.call([self._MSBUILD]+list(arguments))


    def is_windows(self):
        return self._ON_WINDOWS

    def is_mono(self):
        return not self._ON_WINDOWS

    def mono_ver(self):
        output = subprocess.check_output(["mono", "--version"]).decode("utf-8")
        output = output[26:]
        output = output[:output.find(' ')].split(".")
        return [int(i) for i in output]

