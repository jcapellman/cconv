import ctypes

bibliothek = ctypes.cdll.LoadLibrary('./Bibliothek.dll')

result = bibliothek.siesquare(50)

print (result)