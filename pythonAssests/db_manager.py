import sqlite3 as sq

def init_db():
    sq.connect("pillbox.db")

init_db