#!/usr/bin/env python3

import base64
import json
import logging
import os
import sys

protocol_output = sys.stdout
sys.stdout = sys.stderr
os.environ.setdefault("PADDLE_PDX_DISABLE_MODEL_SOURCE_CHECK", "True")
logging.getLogger().setLevel(logging.WARNING)

import cv2
import numpy
from paddleocr import PaddleOCR


def create_model():
    return PaddleOCR(
        text_detection_model_name="PP-OCRv6_medium_det",
        text_recognition_model_name="PP-OCRv6_medium_rec",
        use_doc_orientation_classify=False,
        use_doc_unwarping=False,
        use_textline_orientation=False,
        device="cpu",
    )


def normalize_result(result):
    value = result.json

    if callable(value):
        value = value()

    if isinstance(value, str):
        value = json.loads(value)

    return value.get("res", value)


def recognize(model, encoded_image):
    image_bytes = base64.b64decode(encoded_image)
    image = cv2.imdecode(numpy.frombuffer(image_bytes, dtype=numpy.uint8), cv2.IMREAD_COLOR)

    if image is None:
        raise ValueError("OCR request contained an invalid image")

    items = []

    for result in model.predict(image):
        data = normalize_result(result)
        texts = data.get("rec_texts", [])
        scores = data.get("rec_scores", [])
        polygons = data.get("rec_polys") or data.get("dt_polys") or []

        for text, score, polygon in zip(texts, scores, polygons):
            items.append({
                "text": str(text),
                "confidence": float(score),
                "polygon": numpy.asarray(polygon).tolist(),
            })

    return items


def write_response(response):
    protocol_output.write("VOID_OCR_RESPONSE " + json.dumps(response, separators=(",", ":")) + "\n")
    protocol_output.flush()


model = create_model()

if "--warmup" in sys.argv:
    write_response({"ready": True})
    raise SystemExit(0)

for line in sys.stdin:
    request = {}

    try:
        request = json.loads(line)
        write_response({"id": request["id"], "items": recognize(model, request["image"])})
    except Exception as exception:
        write_response({"id": request.get("id"), "error": str(exception)})
