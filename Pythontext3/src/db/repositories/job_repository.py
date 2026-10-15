# src/db/repositories/job_repository.py
from sqlalchemy.orm import Session
from src.db.models import DetectionJob

class JobRepository:
    def __init__(self, db: Session):
        self.db = db

    def create_job(self, filename: str) -> DetectionJob:
        job = DetectionJob(filename=filename, status="processing")
        self.db.add(job)
        self.db.commit()
        self.db.refresh(job)
        return job

    def update_job_success(self, job_id: int, roi_path: str) -> DetectionJob:
        job = self.db.query(DetectionJob).filter(DetectionJob.id == job_id).first()
        if job:
            job.status = "succeeded"
            job.roi_path = roi_path
            self.db.commit()
            self.db.refresh(job)
        return job

    def update_job_failed(self, job_id: int, error_message: str) -> DetectionJob:
        job = self.db.query(DetectionJob).filter(DetectionJob.id == job_id).first()
        if job:
            job.status = "failed"
            job.error_message = error_message
            self.db.commit()
            self.db.refresh(job)
        return job