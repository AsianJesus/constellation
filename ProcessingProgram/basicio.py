from serial import Serial
from xbee.thread import XBee
from threading import Thread
from queue import Queue
from cuiinterface import Flag
from enum import Enum
from time import sleep

class XBeeInterface:
    def __init__(self,port:str,baudRate:int):
        pass
    def open(self)->None:
        pass
    def close(self)->None:
        pass
    def isOpen(self)->bool:
        pass
    def send(self,msg:str)->None:
        pass
    def read(self)->str:
        pass

class BasicIOCommands(Enum):
    send = 1
    read = 0
    

class BasicIO(Thread):
    errorLog:Queue
    def __init__(self,port:str,baudRate:int,comInput: Queue,output:Queue,opFlag:Flag):
        self.XBee = XBeeInterface(port,baudRate)
        self.output = output
        self.opFlag = opFlag
        self.state = "read"
        self.comInput = comInput
        super().__init__()
    def start(self):
        self.opFlag.setState(True)
        super().start()
    def run(self):
        while True:
            sleep(1/4)
            if self.opFlag.getState():
                continue
            self.__operate()            
    def stop(self):
        self.opFlag.setState(False)
    def __operate(self)->None:
        if not self.comInput.empty():
             comm = self.comInput.get()
             self.comHandler(comm[0],comm[1])
        if self.state == BasicIOCommands.read:
             try:
                 self.output.put(self.read())
             except Exception as e:
                  BasicIO.errorLog.put(e)
    def read(self)->str:
        try:
            result = self.XBee.read()
            return result
        except Exception as e:
            BasicIO.errorLog.put(e)
        return ""
    def write(self,msg:str):
        try:
            self.XBee.send(msg)
        except Exception as e:
            BasicIO.errorLog.put(e)
    def comHandler(self,com,arg = ""):
        if com == BasicIOCommands.send:
            self.state = BasicIOCommands.send
            write(str(arg))
        else:
           if com == BasicIOCommands.read:
                self.state = BasicIO.read
           else:
                BasicIO.errorLog.put(Exception("Invalid command in BasicIO"))
    def setErrorLog(log: Queue):
        BasicIO.errorLog = log
    def setCommands(comm):
        BasicIO.commands = comm
