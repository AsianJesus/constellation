from threading import Thread
from queue import Queue
import time

class ExceptionHandler(Thread):
    def __init__(self, inputs: [], outputs:[]):
        self.inputs = inputs
        self.outputs = outputs
        self.isRunning = False
        super().__init__()
    def start(self):
        self.isRunning = True
        super().start()
    def run(self)->None:
        while True:
            time.sleep(3/4)
            if not self.isRunning:
                continue
            for input in self.inputs:
                while not input.empty():
                    self.__handleException(input.get())            
    def stop(self)->None:
        self.isRunning = False
    def __output(self,msg:str)->None:
        for output in self.outputs:
            output.put(msg)
    def __handleException(exp : Exception)->None:
        recTime = time.localtime()
        line = "[{0}:{1}:{2}]{3}".format(recTime[3],recTime[4],recTime[5],exp)
        self.__output(line)
