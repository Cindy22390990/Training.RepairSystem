#src/services/image_io.py
from pathlib import Path

import cv2
import numpy as np

def read_image(input_path: Path)-> np.ndarray:
    image = cv2.imread(str(input_path))
    if image is None:
        raise ValueError(f"圖片讀取失敗: {input_path}")
    return image

def save_image(image, output_path: Path):
    output_path.parent.mkdir(parents=True, exist_ok=True)
    success = cv2.imwrite(str(output_path), image)

    if not success:
        raise OSError(f"圖片儲存失敗：{output_path}")