#src/models.py
from pydantic import BaseModel

# 定義 API 成功時回傳的資料結構
class DetectResponse(BaseModel):
    status: str
    output: str
    x: int
    y: int
    width: int
    height: int
