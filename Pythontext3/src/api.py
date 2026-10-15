import os
from pathlib import Path
from tempfile import NamedTemporaryFile
from fastapi import FastAPI, File, HTTPException, UploadFile, Depends
from sqlalchemy.orm import Session

from src.config import DetectionConfig
from src.models import DetectResponse
from src.services.part_locator import PartLocator
from src.db.connection import get_db, engine, Base
from src.db.repositories.job_repository import JobRepository

# 啟動時自動建立資料表 (實務上通常用 Alembic 進行版本控制)
Base.metadata.create_all(bind=engine)

app = FastAPI(title="零件定位 API 服務 (分層與資料庫版)")
locator = PartLocator(DetectionConfig())

@app.get("/")
def health_check():
    return {
        "message": "API 伺服器運作中！"
    }
@app.post("/detect", response_model=DetectResponse)
async def detect_part(
        file: UploadFile = File(...),
        db: Session = Depends(get_db)  # 注入資料庫連線
):
    # 1. 建立資料庫 Repository 並記錄初始狀態
    job_repo = JobRepository(db)
    job = job_repo.create_job(filename=file.filename)

    output_dir = Path("output")
    output_dir.mkdir(parents=True, exist_ok=True)

    with NamedTemporaryFile(delete=False, suffix=".jpg") as temp_file:
        temp_file.write(await file.read())
        temp_input_path = Path(temp_file.name)

    output_path = output_dir / f"result_{job.id}_{file.filename}"

    try:
        # 2. 執行影像處理核心
        result = locator.run(temp_input_path, output_path)

        # 3. 處理成功，更新資料庫狀態
        job_repo.update_job_success(job_id=job.id, roi_path=str(output_path))

        return result

    except Exception as e:
        # 3. 處理失敗，記錄錯誤訊息至資料庫
        error_msg = str(e)
        job_repo.update_job_failed(job_id=job.id, error_message=error_msg)

        raise HTTPException(status_code=400, detail=error_msg)

    finally:
        if temp_input_path.exists():
            os.remove(temp_input_path)