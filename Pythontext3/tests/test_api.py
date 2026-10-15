from fastapi.testclient import TestClient
from pathlib import Path
from src.api import app

client = TestClient(app)


def test_health_check():
    response = client.get("/")
    assert response.status_code == 200
    assert response.json() == {"message": "API 伺服器運作中！"}


def test_detect_part_api():
    file_path = Path("input/sample.jpg")

    with open(file_path, "rb") as f:
        response = client.post(
            "/detect",
            files={"file": ("sample.jpg", f, "image/jpeg")}
        )

    assert response.status_code == 200
    data = response.json()
    assert data["status"] == "success"
    assert "x" in data and "y" in data