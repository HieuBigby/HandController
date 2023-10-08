import math
import os
import clr

from keras.models import Sequential
from keras.layers import LSTM, Dense, Dropout
from keras.callbacks import TensorBoard
from keras.regularizers import l2
import mediapipe as mp
import cv2
import numpy as np
import random

# clr.AddReference(r"Assets/Pythons/python-3.8.0-embed/EventHandler.cs")
# import PythonEvent
# from PIL import ImageFont, ImageDraw, Image


def generate_distinct_colors(num_colors):
    distinct_colors = []
    for _ in range(num_colors): 
        color = "#{:06x}".format(random.randint(0, 0xFFFFFF))
        distinct_colors.append(color)
    return distinct_colors


def prob_viz(res, actions, input_frame, colors):
    output_frame = input_frame.copy()
    for num, prob in enumerate(res):
        cv2.rectangle(output_frame, (0, 60 + num * 40), (int(prob * 100), 90 + num * 40), colors[num], -1)
        cv2.putText(output_frame, actions[num], (0, 85 + num * 40), cv2.FONT_HERSHEY_SIMPLEX, 1, (255, 255, 255), 2,
                    cv2.LINE_AA)

    return output_frame


def mediapipe_detection(image, model):
    image = cv2.cvtColor(image, cv2.COLOR_BGR2RGB) # COLOR CONVERSION BGR 2 RGB
    image.flags.writeable = False                  # Image is no longer writeable
    results = model.process(image)                 # Make prediction
    image.flags.writeable = True                   # Image is now writeable
    image = cv2.cvtColor(image, cv2.COLOR_RGB2BGR) # COLOR COVERSION RGB 2 BGR
    return image, results


def draw_styled_landmarks(image, results):    
    mp_drawing = mp.solutions.drawing_utils # Drawing utilities      
    mp_holistic = mp.solutions.holistic # Holistic model
    
    # Draw pose connections
    mp_drawing.draw_landmarks(image, results.pose_landmarks, mp_holistic.POSE_CONNECTIONS,
                             mp_drawing.DrawingSpec(color=(80,22,10), thickness=2, circle_radius=4),
                             mp_drawing.DrawingSpec(color=(80,44,121), thickness=2, circle_radius=2)
                             )
    # Draw left hand connections
    mp_drawing.draw_landmarks(image, results.left_hand_landmarks, mp_holistic.HAND_CONNECTIONS,
                             mp_drawing.DrawingSpec(color=(121,22,76), thickness=2, circle_radius=4),
                             mp_drawing.DrawingSpec(color=(121,44,250), thickness=2, circle_radius=2)
                             )
    # Draw right hand connections
    mp_drawing.draw_landmarks(image, results.right_hand_landmarks, mp_holistic.HAND_CONNECTIONS,
                             mp_drawing.DrawingSpec(color=(245,117,66), thickness=2, circle_radius=4),
                             mp_drawing.DrawingSpec(color=(245,66,230), thickness=2, circle_radius=2)
                             )


def extract_keypoints(results):
    lh_list = []
    if results.left_hand_landmarks:
        lh_pos = results.left_hand_landmarks.landmark[0]
        lh_pos = [round(lh_pos.x, 2), round(lh_pos.y, 2), round(lh_pos.z, 2)]
        lh_list.append(lh_pos)
        lh_vectors = normalized_vectors(results.left_hand_landmarks)
        lh_list.extend(lh_vectors)

    rh_list = []
    if results.right_hand_landmarks:
        rh_pos = results.right_hand_landmarks.landmark[0]
        rh_pos = [round(rh_pos.x, 2), round(rh_pos.y, 2), round(rh_pos.z, 2)]
        rh_list.append(rh_pos)
        rh_vectors = normalized_vectors(results.right_hand_landmarks)
        rh_list.extend(rh_vectors)

    lh_array = np.array(lh_list).flatten() if lh_list else np.zeros(20 * 3 + 3)
    rh_array = np.array(rh_list).flatten() if rh_list else np.zeros(20 * 3 + 3)

    return np.concatenate([lh_array, rh_array])


def normalized_vectors(hand_landmarks):
    vectors = []
    # Define the order of points for calculations
    point_order = [
        (0, 1), (1, 2), (2, 3), (3, 4),
        (0, 5), (5, 6), (6, 7), (7, 8),
        (0, 9), (9, 10), (10, 11), (11, 12),
        (0, 13), (13, 14), (14, 15), (15, 16),
        (0, 17), (17, 18), (18, 19), (19, 20)
    ]

    for start_idx, end_idx in point_order:
        start_point = hand_landmarks.landmark[start_idx]
        end_point = hand_landmarks.landmark[end_idx]

        normalized_vector = normalize_vector(end_point, start_point)
        vectors.append(normalized_vector)

    return vectors


def normalize_vector(point1, point2):
    # Calculate the vector between the two NormalizedLandmark points
    vector = (point2.x - point1.x, point2.y - point1.y, point2.z - point1.z)

    # Calculate the length of the vector
    length = math.sqrt(vector[0] ** 2 + vector[1] ** 2 + vector[2] ** 2)

    # Normalize the vector and return it
    normalized_vector = (round(vector[0] / length, 2), round(vector[1] / length, 2), round(vector[2] / length, 2))

    return normalized_vector


def apply_replacements(input_array):
    replacements = {
        "Toi Ten": "Toi ten la",
        # Thêm các cụm từ và cụm từ thay thế khác vào đây
    }

    output_array = []
    i = 0

    while i < len(input_array):
        combined_word = input_array[i]
        found_match = False

        for j in range(i + 1, len(input_array)):
            combined_word += " " + input_array[j]

            if combined_word in replacements:
                output_array.append(replacements[combined_word])
                i = j + 1
                found_match = True
                break

        if not found_match:
            output_array.append(input_array[i])
            i += 1

    return output_array


def get_keypoints(frame):
    # Load Model
    mp_holistic = mp.solutions.holistic # Holistic model

    # Set mediapipe model
    with mp_holistic.Holistic(min_detection_confidence=0.5, min_tracking_confidence=0.5) as holistic:
        # # Make detections
        # image, results = mediapipe_detection(frame, holistic)
        results = holistic.process(frame)  
        keypoints = extract_keypoints(results)

        return keypoints
    
def get_keypoints_path(img_path):
    frame = cv2.imread(img_path)
    return frame
    # get_keypoints(frame)


def detect(sequences, model, threshold = 0.9):
    res = model.predict(np.expand_dims(sequences, axis=0))[0]

    if res[np.argmax(res)] > threshold:
        return np.argmax(res)


def video_capture():
    cap = cv2.VideoCapture(0)

    # Check if camera opened successfully
    if (cap.isOpened() == False):
        print("Error opening video stream or file")

    # Read until video is completed
    while cap.isOpened():
        # Capture frame-by-frame
        ret, frame = cap.read()
        if ret:
            # Display the resulting frame
            cv2.imshow('Frame', frame)

            # Press Q on keyboard to  exit
            if cv2.waitKey(25) & 0xFF == ord('q'):
                break
        # Break the loop
        else:
            break

    # When everything done, release the video capture object
    cap.release()
    # Closes all the frames
    cv2.destroyAllWindows()


def begin_detect(model_path):
    # Danh sách các action đã tạo
    actions = np.array(['A', 'B', 'C', 'D', 'E'])
    # eventHandler = PythonEvent.EventHandler()

    # Videos are going to be 10 frames in length
    sequence_length = 10

    model = Sequential()
    model.add(LSTM(64, return_sequences=True, activation='relu', input_shape=(sequence_length, 126))) # input_shape=(sequence_length, 258)
    model.add(Dropout(0.2))
    model.add(LSTM(128, return_sequences=True, activation='relu'))
    model.add(Dropout(0.2))
    model.add(LSTM(64, return_sequences=False, activation='relu'))
    model.add(Dense(64, activation='relu')) # , kernel_regularizer=l2(0.01)  # Add L2 regularization
    model.add(Dense(32, activation='relu')) # , kernel_regularizer=l2(0.01)  # Add L2 regularization
    model.add(Dense(actions.shape[0], activation='softmax'))
    model.compile(optimizer='Adam', loss='categorical_crossentropy', metrics=['categorical_accuracy'])
    model.load_weights(model_path)

    # Load Model
    mp_holistic = mp.solutions.holistic # Holistic model

    # 1. New detection variables
    sequences = []
    sentences = []
    predictions = []
    threshold = 0.9

    cap = cv2.VideoCapture(0)
    # Set mediapipe model
    with mp_holistic.Holistic(min_detection_confidence=0.5, min_tracking_confidence=0.5) as holistic:
        while cap.isOpened():
            # Read feed
            ret, frame = cap.read()
            print(type(frame))
            print(frame.shape)

            # Make detections
            image, results = mediapipe_detection(frame, holistic)
            # print(results)

            # # Draw landmarks
            # draw_styled_landmarks(image, results)

            # 2. Prediction logic
            keypoints = extract_keypoints(results)

            sequences.append(keypoints)
            sequences = sequences[-10:]

            if len(sequences) == sequence_length:
                res = model.predict(np.expand_dims(sequences, axis=0))[0]
                # print(res)
                print(actions[np.argmax(res)])
                predictions.append(np.argmax(res))
                predictions = predictions[-15:]
                # print('prediction length: ' + str(len(predictions)))

                # 3. Viz logic
                # Tìm ra hành động được dự đoán nhiều nhất trong 10 video đưa vào
                if res[np.argmax(res)] > threshold:
                    if len(sentences) > 0:
                        most_res = np.bincount(predictions).argmax()
                        if actions[most_res] != sentences[-1]:
                            sentences.append(actions[most_res])
                    else:
                        sentences.append(actions[np.argmax(res)])

                # Reset lại sentences
                sentences = sentences[-10:]

            # Loại bỏ hành động 'Break' ra khỏi phần hiển thị
            filtered_sentences = [s for s in sentences if s != 'None']
            filtered_sentences = apply_replacements(filtered_sentences)
            sum_lengths = sum(len(s) for s in filtered_sentences)
            # print(sum_lengths)
            if sum_lengths > 25:
                filtered_sentences = filtered_sentences[-5:]

            cv2.rectangle(image, (0, 0), (640, 40), (245, 117, 16), -1)
            cv2.putText(image, ' '.join(filtered_sentences), (3, 30),
                cv2.FONT_HERSHEY_SIMPLEX, 1, (255, 255, 255), 2, cv2.LINE_AA)
            
            str_words = str(' '.join(filtered_sentences))
            # eventHandler.PrintText(str_words)
            # img_pil = Image.fromarray(image)
            # draw = ImageDraw.Draw(img_pil)
            # draw.text((10, 5), ' '.join(filtered_sentences), font=font, fill=(0,255,0,0))
            # image = np.array(img_pil)

            # Show to screen
            cv2.imshow('OpenCV Feed', image)

            # Break gracefully
            if cv2.waitKey(1) & 0xFF == ord('q'):
                break

        cap.release()
        cv2.destroyAllWindows()


def load_model(model_path, input_shape, output_shape):
    model = Sequential()
    model.add(LSTM(64, return_sequences=True, activation='relu', input_shape=input_shape)) # input_shape=(sequence_length, 258)
    model.add(Dropout(0.2))
    model.add(LSTM(128, return_sequences=True, activation='relu'))
    model.add(Dropout(0.2))
    model.add(LSTM(64, return_sequences=False, activation='relu'))
    model.add(Dense(64, activation='relu')) # , kernel_regularizer=l2(0.01)  # Add L2 regularization
    model.add(Dense(32, activation='relu')) # , kernel_regularizer=l2(0.01)  # Add L2 regularization
    model.add(Dense(output_shape, activation='softmax'))
    model.compile(optimizer='Adam', loss='categorical_crossentropy', metrics=['categorical_accuracy'])
    model.load_weights(model_path)
    return model


def main():
    begin_detect('ActionDetect/action_test_2.h5')

if __name__ == '__main__':
    main()