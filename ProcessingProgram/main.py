import time
import threading
from fileio import FileIO
from flag import Flag
from queue import Queue
from exceptionhandler import ExceptionHandler
from dbproxy import DataBaseWorker, DBCommand
from mysql.connector import connect
from cuiinterface import Processes, CUI
from session import Session

class QueueShower(threading.Thread):
    def __init__(self,q):
        self.q = q
        self.exitFlag = False
        super().__init__()
    def run(self):
        while True:
            if self.exitFlag:
                break
            while not self.q.empty():
                element = self.q.get()
                if element:
                    print(element)
            time.sleep(1/2)
    def terminate(self):
        self.exitFlag = True
fname = "C:\\Users\\fruit\\Desktop\\log.txt"

dbCommands = {
        'ins_id': 'INSERT INTO test(id) VALUES(%s)',
        'ins_name' : 'INSERT INTO test(name) VALUES(%s);',
        'ins_all' : 'INSERT INTO test(id,name) VALUES(%s,%s);',
        'get_all' : 'SELECT * FROM test;'
    }

host = "localhost"
uid = "root"
passw = "1123581321ElViNBV"
schema = "pourtest"

flags = {
        Processes.BasicIO: Flag(True),
        Processes.Processing:Flag(True),
        Processes.Distribution : Flag(True),
        Processes.DatabaseIO : Flag(True),
        Processes.FileIO : Flag(True)
    }
s = Session(5,"Nope")

c = CUI(s,flags)

c.start()
c.join()

#con = connect(host=host,user=uid,password=passw,database=schema)
#cur = con.cursor(True)
#cur.execute(dbCommands['ins_id'],(6,))
#con.commit()
print("Ending")
input("")
exit()


dbIn = Queue(100)
dbOut = Queue(100)
errorLog = Queue(100)
errorOut = Queue(100)

errorHandler = ExceptionHandler([errorLog],[errorOut])
errorHandler.start()

f = FileIO(fname,errorOut,Flag(True))

f.start()

DBCommand.comList = dbCommands
DataBaseWorker.errorLog = errorLog

sh = QueueShower(dbOut)
db = DataBaseWorker(host,uid,passw,schema,dbIn,dbOut,Flag(True))

sh.start()
db.start()

dbIn.put("D")

while True:
    com = input("Type command or 'q' for exit: ")
    if com == 'q':
        sh.terminate()
        db.terminate()
        errorHandler.terminate()
        f.terminate()
        exit()
    args = com.split(' ')
    com = args[0]
    args = tuple(args[1:])
    if not com in dbCommands:
        print("Wrong command")
        continue
    dbIn.put(DBCommand(com,args))