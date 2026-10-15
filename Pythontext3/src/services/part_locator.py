from pathlib import Path

import cv2
import numpy as np

from src.config import DetectionConfig
from src.services.image_io import read_image, save_image


class PartLocator:
    def __init__(self, config: DetectionConfig = DetectionConfig()):
        self.config = config

    def preprocess(self, image: np.ndarray) -> np.ndarray:
        gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)

        blurred = cv2.GaussianBlur(gray, self.config.blur_kernel, 0)

        edges = cv2.Canny(
            blurred,
            self.config.canny_threshold1,
            self.config.canny_threshold2
        )

        return edges

    def find_candidates(
        self,
        edges: np.ndarray,
        image_shape,
    ):
        contours, _ = cv2.findContours(
            edges,
            cv2.RETR_EXTERNAL,
            cv2.CHAIN_APPROX_SIMPLE,
        )

        if not contours:
            raise ValueError("沒有找到輪廓")

        image_area = image_shape[0] * image_shape[1]

        candidates = []

        for contour in contours:

            area = cv2.contourArea(contour)

            if (
                area < self.config.min_area
                or area > image_area * self.config.max_area_ratio
            ):
                continue

            x, y, w, h = cv2.boundingRect(contour)

            if h == 0:
                continue

            ratio = w / h

            if (
                ratio < self.config.min_ratio
                or ratio > self.config.max_ratio
            ):
                continue

            candidates.append(contour)

        if not candidates:
            raise ValueError("沒有符合條件的輪廓")

        return candidates

    def select_best_contour(self, candidates):

        best_contour = None
        best_diff = float("inf")

        for contour in candidates:

            x, y, w, h = cv2.boundingRect(contour)

            if h == 0:
                continue

            ratio = w / h

            diff = abs(
                ratio - self.config.target_ratio
            )

            if diff < best_diff:
                best_diff = diff
                best_contour = contour

        if best_contour is None:
            raise ValueError("找不到最佳輪廓")

        return best_contour

    def crop_roi(
        self,
        image: np.ndarray,
        contour,
    ):

        x, y, w, h = cv2.boundingRect(contour)

        padding = self.config.padding

        x = max(0, x - padding)
        y = max(0, y - padding)

        w = min(
            image.shape[1] - x,
            w + padding * 2,
        )

        h = min(
            image.shape[0] - y,
            h + padding * 2,
        )

        roi = image[
            y:y + h,
            x:x + w,
        ]

        if roi.size == 0:
            raise ValueError("ROI 為空")

        return roi, x, y, w, h

    def run(
        self,
        input_path: Path,
        output_path: Path,
    ):

        image = read_image(input_path)

        edges = self.preprocess(image)

        candidates = self.find_candidates(
            edges,
            image.shape,
        )

        contour = self.select_best_contour(
            candidates
        )

        roi, x, y, w, h = self.crop_roi(
            image,
            contour,
        )

        save_image(
            roi,
            output_path,
        )

        return {
            "status": "success",

            "x": int(x),
            "y": int(y),

            "width": int(w),
            "height": int(h),

            "imageWidth": int(image.shape[1]),
            "imageHeight": int(image.shape[0])
        }