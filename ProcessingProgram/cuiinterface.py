from threading import Thread
from flag import Flag
from session import Session
from enum import Enum

class Processes(Enum):
    BasicIO = 'xbeeio'
    Processing = 'processing'
    Distribution = 'distrib'
    DatabaseIO = 'dbworker'
    FileIO = 'fileio'
    def getAll()->tuple:
        return ((Processes.BasicIO,'xbeeio'),(Processes.Processing,'processing'),(Processes.Distribution,'distrib'),(Processes.DatabaseIO,'dbworker'),(Processes.FileIO,'fileio'))
    def doesExists(element:str)->bool:
        return element in [x[1] for x in Processes.getAll()]
    def getByValue(elem: str):
        if not Processes.doesExists(elem):
            return None
        for (key,val) in Processes.getAll():
            if val == elem:
                return key
    def getByName(elem):
        if elem not in [x[0] for x in Processes.getAll()]:
            return None;
        for (key,val) in Processes.getAll():
            if key == elem:
                return val

class ProcessesController:
    def __init__(self,flags:{}):
        self.curFlags = {Processes.getByName(name):flag for name,flag in flags.items()}
        self.savedFlags = {}
    def stopAll(self):
        for process in self.curFlags:
            self.stop(process)
    def stop(self,process):
        self.curFlags[process].setState(False)
    def startAll(self):
        for process in self.curFlags:
            self.start(process)
    def start(self,process):
        self.curFlags[process].setState(True)
    def pause(self,process):
        self.savedFlags[process] = self.curFlags[process]
        self.curFlags[process].setState(False)
    def pauseAll(self):
        for process in self.curFlags:
            self.pause(process)
    def resume(self,process):
        if process in self.savedFlags:
            self.curFlags[self.savedFlags[process].getState()]
    def resumeAll(self):
        for process in self.curFlags:
            self.resume(process)

class CUI(Thread):
    #Command : comment
    comList = {
       "stop" : "Stop process. Using: stop xbeeio",
       "stop_all" : "Stop all processes. Using: stop_all",
       "start" : "Start process. Using: start xbeeio",
       "start_all" : "Start all processes: Using: start_all",
       "pause_all": "Pause all processes: Using: pause_all",
       "pause" : "Pause process. Using: pause fileio",
       "resume" : "Resume paused process. Using: resume fileio",
       "resume_all" : "Resume all paused process. Using: resume_all",
       "show_states":"Show processes states. Using: show_states",
       "session_show" : "Show current session. Using: session_show",
       "session_stop" : "Stop current session. Using: session_stop",
       "session_start" : "Start current session. Using: session_start",
       "session_change" : "Change session. Using: session_change 5",
       "help" : "Show commands"
       }
    def __init__(self,ses:Session,flags:{str:Flag}):
        self.session = ses
        self.flags = flags
        self.procControl = ProcessesController(self.flags)
        self.exitFlag = False
        super().__init__()
    def start(self):
        super().start()
    def end(self):
        self.exitFlag = True
    def run(self):
        while True:
            if self.exitFlag:
                break
            rawCom = CUI.waitCommand()
            if not rawCom:
                continue
            args = rawCom[1:]
            com = rawCom[0]
            self.__handleCommand(com,args)
    def waitCommand()->tuple():
        com = input("Input command: ")
        return [x for x in com.split(' ') if x != '']
    def __handleCommand(self,comm:str,args:tuple):
        if comm not in CUI.comList:
            CUI.__showHelp()
            return False
        if comm == "help":
            CUI.__showCommands()
            return True
        if comm == "show_states":
            self.__showStates()
            return True
        if comm in ["stop","stop_all","start","start_all","pause_all","pause","resume","resume_all"]:
            if comm in ["stop","start","pause","resume"]:
                proc = args[0]
                if not Processes.doesExists(proc):
                    CUI.__printProcesses()
                    return False
                if comm == "stop":
                    print("Stopping process - ",proc)
                    self.procControl.stop(proc)
                if comm == "start":
                    print("Starting process - ",proc)
                    self.procControl.start(proc)
                if comm == "pause":
                    print("Pausing process - ", proc)
                    self.procControl.pause(proc)
                if comm == "resume":
                    print("Resuming process - ", proc)
                    self.procControl.resume(proc)
            if comm == "start_all":
                print("Starting all processes")
                self.procControl.startAll()
            if comm == "stop_all":
                print("Stopping all processes")
                self.procControl.stopAll()
            if comm == "resume_all":
                print("Resuming all processes")
                self.procControl.resumeAll()
            if comm == "pause_all":
                print("Pausing all processes")
                self.procControl.pauseAll()
        else:
            if comm == "session_show":
                self.__showSession()
            if comm == "session_start":
                self.__startSession()
            if comm == "session_stop":
                self.__stopSession()
            if comm == "session_change":
                if len(args) == 0:
                    CUI.__showCommandInfo("session_change")
                elif len(args) == 1:
                    self.__changeSession(int(args[0]))
                else:
                    self.__changeSession(int(args[0]),args[1])
        return True
    def __showSession(self):
        print("ID\tName\n{0}\t{1}".format(self.session.getID(),self.session.getName()))
    def __stopSession(self):
        self.session.stop()
    def __startSession(self):
        self.session.start()
    def __changeSession(self,id:int,name:str = "undefined"):
        self.session.setID(id)
        self.session.setName(name)
    def __showCommands():
        for command in CUI.comList:
            CUI.__showCommandInfo(command)
    def __printProcesses():
        print("Invalid process")
        print("List of available processes:")
        for (proc,code) in Processes.getAll():
            print("{0}         \t{1}".format(proc,code))
    def __showHelp():
        print("Invalid command\nTo view all commands type 'help' .")
    def __showCommandInfo(command:str):
        print("{0}\t{1}".format(command,CUI.comList[command]))
    def __showStates(self):
        for name,state in self.flags.items():
            print("Process - {0}\tActive? - {1}".format(name,state.getState()))