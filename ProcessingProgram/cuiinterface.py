from threading import Thread
from flag import Flag
from session import Session

#enumerate Processes

class ProcessesController:
    def __init__(self,flags:{}):
        pass
    def stopAll(self):
        pass
    def stop(self,process):
        pass
    def startAll(self):
        pass
    def start(self,process):
        pass

class CUI(Thread):
    def __init__(self,ses:Session,commandList: {}):
        pass
    def start(self):
        pass
    def stop(self):
        pass
    def waitCommand(self)->str:
        pass
    def __handleCommand(comm:str):
        pass