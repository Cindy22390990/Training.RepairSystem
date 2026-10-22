# Training.RepairSystem


## 技術

.NET 10 Web API

SQL Server

JWT

Python OpenCV


## 啟動方式


1. 修改 appsettings.json

設定 SQL Server


2. 執行 Migration


Update-Database


3. 啟動 API


dotnet run


4. Python環境


cd Pythontext3

venv\Scripts\activate


## 測試帳號


Employee

帳號1:
testUser

密碼:
123456
AQAAAAIAAYagAAAAEFC9V9MSjbuHexPjyMs+ErCrBXez9RedBjouOZVYbytFPDwrYo7vjpFlYnoqL24yGQ==

帳號2:
testUser2

密碼:
123456
AQAAAAIAAYagAAAAEM19bQ51PppXI+Jba8Rl+eT7cVZpgof2jtt4VOgAGQ9cQ3Fj7AuR0W//Qyc2Ft43xA==


Maintenance

帳號:
maintenance

密碼:
123456
AQAAAAIAAYagAAAAEK57nxZYNNPkAN13O8Ge7Z/9ff3AVzYOCoMwQNRly/r1kkrWndU4NMfSKGr0MOYxFg==

## API流程

1. Login取得JWT

2. Bearer Token

3. 建立工單

4. 查詢工單

5. 更新狀態


1. ## 權限

Employee:
- 建立自己的報修工單
- 查詢自己的工單

Maintenance:
- 查看全部工單
- 更新工單狀態


## 圖片處理

- 支援 JPG/JPEG/PNG
- 最大 5MB
- Python OpenCV:
  - GrayScale
  - Gaussian Blur
  - Canny Edge Detection
- 產生 Preview 圖
