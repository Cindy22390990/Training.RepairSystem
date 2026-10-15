import pytest
from pathlib import Path
from src.config import DetectionConfig
from src.services.part_locator import PartLocator


def test_part_locator_run(tmp_path):
    # 準備環境與測試用圖片
    locator = PartLocator(DetectionConfig())
    input_img = Path("input/sample.jpg")  # 確保此圖片存在
    output_img = tmp_path / "test_output.jpg"

    # 執行定位
    result = locator.run(input_img, output_img)

    # 驗證輸出結果
    assert result["status"] == "success"
    assert output_img.exists()
    assert result["width"] > 0
    assert result["height"] > 0