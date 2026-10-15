# src/db/connection.py
import os
from collections.abc import Generator
from dotenv import load_dotenv
from sqlalchemy import create_engine
from sqlalchemy.orm import sessionmaker, Session, declarative_base

load_dotenv()
DATABASE_URL = os.getenv("DATABASE_URL")
if not DATABASE_URL:
    raise ValueError("請確認已在 .env 檔案中設定 DATABASE_URL 環境變數")
engine = create_engine(DATABASE_URL, pool_pre_ping=True)
SessionLocal = sessionmaker(autocommit=False, autoflush=False, bind=engine)

Base = declarative_base()

def get_db() -> Generator[Session, None, None]:
    """FastAPI 的依賴注入，用於取得資料庫連線"""
    db = SessionLocal()
    try:
        yield db
    finally:
        db.close()