# cconv
C# Converter to another language wrapper. Currently the app only outputs Python, but others will be added in the future.

## Example Usage
Input a C# DLL and output a Python file that uses it under the hood (assuming the final output is an AOT library)

Issue the following command:

` 
.\csharp2py.exe inputlibrary .\Bibliothek.dll outputlanguage python
`

Outputs the following Python class:

```python
import ctypes

class DasGut:
    __library = None

    def __init__(self):
        self.__library = ctypes.cdll.LoadLibrary('./Bibliothek.dll')

	def SieSquare(x):
		return __library.SieSquare(x)
```
