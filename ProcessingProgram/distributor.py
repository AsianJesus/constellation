from threading import Thread
from queue import Queue
from cuiinterface import Flag
from processing import Packet
from dbproxy import DBCommand
from time import sleep

class Distributor(Thread):
    errorLog : Queue
    def __init__(self,input: Queue,dbOutput:Queue,fileIOF:Queue,fileIOS:Queue,opFlag: Flag):
        self.input = input
        self.dbout = dbOutput
        self.foutF = fileIOF
        self.foutS = fileIOS
        self.opFlag = opFlag
        self.exitFlag = False
    def start(self):
        self.opFlag.setState(True)
        super().start()
    def run(self):
        while True:
            if self.exitFlag:
                break
            sleep(1/2)
            if not self.opFlag.getState():
                continue
            while not self.input.empty():
                elem = self.input.get()
                if isinstance(elem,Packet):
                    self.__distrib(elem)
    def stop(self):
        self.opFlag.setState(False)
    def __distrib(self,element:Packet):
        self.__inputDB(element)
        self.__inputFiles(element)
    def __inputDB(self,element:Packet):
        self.dbout.put(DBCommand("ins_all",element.getInfo()))
    def __inputFiles(self,element:Packet):
        self.foutF.put(element.getInfo().join(','))
        self.foutS.put(element.getInfo().join(';'))
    def terminate(self):
        self.exitFlag = True