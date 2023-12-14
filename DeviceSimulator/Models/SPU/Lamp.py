from Models.SmartDevice import SmartDevice
from datetime import datetime
import math
import asyncio
import json


def generate_lumens():
    current_time = datetime.now().time()
    hours, minutes, seconds = current_time.hour, current_time.minute, current_time.second

    lumens = 0

    if 8 <= hours <= 18:
        lumens = 1000
    elif 19 <= hours <= 21 or 6 <= hours <= 7:
        lumens = 500
    elif 22 <= hours <= 23:
        lumens = 100
    elif 0 <= hours <= 5:
        lumens = 0

    return lumens


class Lamp(SmartDevice):
    def __init__(self, device_id, smart_home_id, device_category, device_type, brightness_limit, power_per_hour, is_auto, is_working=False):
        super().__init__(device_id, smart_home_id, device_category, device_type)
        self.brightness_limit = brightness_limit
        self.power_per_hour = power_per_hour
        self.is_auto = is_auto
        self.is_working = is_working

    def on_data_receive(self, client, user_data, msg):
        super().on_data_receive(client, user_data, msg)
        if msg.topic == self.receive_topic:
            data = json.loads(msg.payload.decode())
            print(data)
            if data.get("action", None) == "auto":
                self.is_auto = True
            elif data.get("action", None) == "manual":
                self.is_auto = False
            elif data.get("action", None) == "turn_lamp_on":
                self.is_working = True
            elif data.get("action", None) == "turn_lamp_off":
                self.is_working = False
            elif "set_brightness_limit" in data.get("action", None):
                self.brightness_limit = eval(data["action"].split("=")[1])

    async def send_data(self):
        i = 0
        while True:
            i += 1
            if not self.is_on:
                break
            lumens = generate_lumens()
            print(f"Is auto: {self.is_auto}, lumens: {lumens + i}, brightness limit: {self.brightness_limit}, is working: {self.is_working}")
            if self.is_auto:
                self.is_working = lumens < self.brightness_limit

            self.client.publish(self.send_topic, json.dumps({"currentBrightness": lumens + i,
                                                             "isWorking": self.is_working,
                                                             "isAuto": self.is_auto,
                                                             "consumptionPerMinute": round(self.power_per_hour / 60,
                                                                                                4)}), retain=False)
            await asyncio.sleep(10)
