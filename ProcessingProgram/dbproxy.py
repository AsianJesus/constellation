import mysql.connector
from cuiinterface import Flag
from threading import Thread
from queue import Queue

class DBInterface:
    def __init__(self,host:str,uid:str,passw:str,schema:str):
        pass
    def open(self)->None:
        pass
    def close(self)->None:
        pass
    def execute(self,command:str,arg:tuple):
        pass
    def isOpen(self)->bool:
        pass

class DataBaseWorker(Thread):
    errorLog : Queue
    def __init__(self, host:str,uid:str,passw:str,schema:str,input:Queue,output:Queue,opFlag: Flag):
        pass
    def start(self):
        pass
    def stop(self):
        pass
    def __handler(self):
        pass
    def __insert(self,value:object):
        pass
    def get(self)->object:
        pass
