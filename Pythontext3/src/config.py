#config.py
#"""演算法預設門檻設定檔."""
from dataclasses import dataclass
from dataclasses import dataclass
from typing import Tuple
@dataclass
class DetectionConfig:
    # 這裡放我們原本在 __init__ 裡面的參數
    padding: int = 10
    min_area: int = 1000
    max_area_ratio: float = 0.8
    min_ratio: float = 0.8
    max_ratio: float = 2.5
    target_ratio: float = 1.5
    # 影像處理參數 (新增)
    blur_kernel: Tuple[int, int] = (5, 5)
    canny_threshold1: int = 30
    canny_threshold2: int = 100

    # 路徑設定 (新增)
    default_output_dir: str = "output"