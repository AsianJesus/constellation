from threading import Thread
from queue import Queue
from flag import Flag
from session import Session

class Packet(object):
    def __init__(self,packID:int,recTime:int,flyTime:int,temp:float = None,press:float = None,height:float = None,humidity:float = None,speed:float = None,voltage:float = None,batch:float = None,gpsX:float = None,gpsY:float = None,gpsZ:float = None,co2:float = None,nh3:float = None,no2:float = None):
        self.flyID = None
        self.packID = packID
        self.recTime = recTime
        self.flyTime = flyTime
        self.temperature = temp
        self.pressure = press
        self.height = height
        self.humidity = humidity
        self.speed = speed
        self.voltage = voltage
        self.charge = batch
        self.gpsX = gpsX
        self.gpsY = gpsY
        self.gpsZ = gpsZ
        self.co2 = co2
        self.no2 = no2
        self.nh3 = nh3
        return super().__init__(*args, **kwargs)
    def setFlyID(self,id:int):
        self.flyID = id
    def getInfo(self) -> tuple:
        return (self.flyID,self.packID,self.recTime,self.flyTime,self.temperature,self.pressure,self.height,self.humidity,self.speed,self.voltage,self.charge,self.gpsX,self.gpsY,self.gpsZ,self.co2,self.no2,self.nh3)
    
class DataHandler(Thread):
    errorLog: Queue
    def __init__(self,ses:Session,input:Queue,output:Queue,opFlag:Flag, processAlgorithm = None):
        pass
    def start(self):
        pass
    def stop(self):
        pass
    def __defProcessAlgorithm(raw:str)->Packet:
        pass
    def run(self):
        pass
    def terminate(self):
        self.exitFlag = True
