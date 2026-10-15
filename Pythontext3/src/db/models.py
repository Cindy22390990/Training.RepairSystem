# src/db/models.py
from datetime import datetime
from sqlalchemy import Column, Integer, String, DateTime
from src.db.connection import Base

class DetectionJob(Base):
    __tablename__ = "detection_jobs"

    id = Column(Integer, primary_key=True, index=True)
    filename = Column(String(255), nullable=False)
    status = Column(String(50), nullable=False) # e.g., 'processing', 'succeeded', 'failed'
    roi_path = Column(String(500), nullable=True)
    error_message = Column(String(1000), nullable=True)
    created_at = Column(DateTime, default=datetime.utcnow)