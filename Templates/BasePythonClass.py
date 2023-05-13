import ctypes

class CLASS_NAME:
    __library = None

    def __init__(self):
        self.__library = ctypes.cdll.LoadLibrary('./LIB_NAME')

FUNCTION_BLOCK