import time
import threading

s = time.localtime()

e = Exception()

try:
    z = threading.Thread()
    z.join()
except Exception as exp:
    e = exp

line = "[{0}:{1}:{2}] {3}".format(s[3],s[4],s[5],e)

print(line)
