class Session(object):
    def __init__(self,id:int):
        self.id = id
        self.isrunning = False
    def start(self)->None:
        self.isrunning = True
    def end(self)->None:
        self.isrunning = False
    def setID(self,id:int)->None:
        if self.id != id:
            self.end()
            self.id = id
    def getID(self)->int:
        return self.id
