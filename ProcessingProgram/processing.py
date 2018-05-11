from threading import Thread
from queue import Queue
from flag import Flag
from session import Session

class Packet(object):
    def __init__(self,flyID:int,packID:int,recTime:int,flyTime:int,temp:float = None,press:float = None,height:float = None,humidity:float = None,speed:float = None,voltage:float = None,batch:float = None,gpsX:float = None,gpsY:float = None,gpsZ:float = None,co2:float = None,nh3:float = None,no2:float = None):
        return super().__init__(*args, **kwargs)
    def getInfo(self) -> tuple:
        pass

class DataHandler(Thread):
    errorLog: Queue
    def __init__(self,ses:Session,input:Queue,output:Queue,opFlag:Flag, processAlgorithm = None):
        pass
    def start(self):
        pass
    def stop(self):
        pass
    def __defProcessAlgorithm(raw:str,id:int)->Packet:
        pass
    def run(self):
        pass
