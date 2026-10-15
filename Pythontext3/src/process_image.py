import json
import sys
import cv2


def process_image(
    input_path,
    output_path
):

    # 讀取圖片
    image = cv2.imread(
        input_path
    )


    # 圖片無法讀取
    if image is None:

        return {
            "success": False,
            "message": "圖片無法讀取"
        }


    # 灰階
    gray = cv2.cvtColor(
        image,
        cv2.COLOR_BGR2GRAY
    )


    # Gaussian 模糊
    blurred = cv2.GaussianBlur(
        gray,
        (5,5),
        0
    )


    # Canny 邊緣偵測
    edges = cv2.Canny(
        blurred,
        30,
        100
    )


    # 儲存預覽圖片
    result = cv2.imwrite(
        output_path,
        edges
    )


    if not result:

        return {
            "success":False,
            "message":"預覽圖儲存失敗"
        }


    return {

        "success":True,

        "previewPath":output_path

    }



def main():

    # 參數檢查
    if len(sys.argv) != 3:

        error = {

            "success":False,

            "message":"參數錯誤"

        }


        print(
            json.dumps(
                error,
                ensure_ascii=False
            ),
            file=sys.stderr
        )


        sys.exit(1)



    input_path = sys.argv[1]

    output_path = sys.argv[2]


    try:

        result = process_image(
            input_path,
            output_path
        )


        # stdout 只輸出 JSON
        print(
            json.dumps(
                result,
                ensure_ascii=False
            )
        )


        if not result["success"]:

            sys.exit(1)



    except Exception as e:


        print(
            json.dumps(
                {
                    "success":False,
                    "message":str(e)
                },
                ensure_ascii=False
            ),
            file=sys.stderr
        )


        sys.exit(1)



if __name__ == "__main__":

    main()