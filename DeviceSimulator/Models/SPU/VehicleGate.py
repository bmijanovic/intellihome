import asyncio
import json
import random
import random

from Models.SmartDevice import SmartDevice


def yield_licence_plate(allowed_licence_plates):
    while True:
        if random.random() < 0.5:
            yield random.choice(allowed_licence_plates)
        else:
            yield random.choice(["ZR001AA", "ZR002AA", "ZR003AA", "ZR004AA", "ZR005AA", "ZR006AA", "ZR007AA", "ZR008AA",
                                 "SM001AA", "SM002AA", "SM003AA", "SM004AA", "SM005AA", "SM006AA", "SM007AA", "SM008AA",
                                 "BG001AA", "BG002AA", "BG003AA", "BG004AA", "BG005AA", "BG006AA", "BG007AA", "BG008AA",
                                 "NS001AA", "NS002AA", "NS003AA", "NS004AA", "NS005AA", "NS006AA", "NS007AA",
                                 "NS008AA"])


class VehicleGate(SmartDevice):
    def __init__(self, device_id, smart_home_id, device_category, device_type, is_public, allowed_licence_plates,
                 power_per_hour, is_open=False, is_opened_by_user=False, user='System'):
        super().__init__(device_id, smart_home_id, device_category, device_type)
        self.is_public = is_public
        self.is_open = is_open
        self.allowed_licence_plates = allowed_licence_plates
        self.power_per_hour = power_per_hour
        self.is_opened_by_user = is_opened_by_user
        self.user = user
        self.licence_plates_in_home = []

    def on_data_receive(self, client, user_data, msg):
        super().on_data_receive(client, user_data, msg)
        if msg.topic == self.receive_topic:
            data = json.loads(msg.payload.decode())
            if data.get("action", None) == "public":
                self.is_public = True
            elif data.get("action", None) == "private":
                self.is_public = False
            elif data.get("action", None) == "open":
                self.is_open = True
            elif data.get("action", None) == "close":
                self.is_open = False
            elif data.get("action", None) == "open_by_user":
                self.user = data.get("user", None)
                self.is_opened_by_user = True
                self.is_open = True
                self.publish_data("", False)
                print("Gate is opened by user {}".format(self.user))
            elif data.get("action", None) == "close_by_user":
                self.user = data.get("user", None)
                self.is_opened_by_user = False
                self.is_open = False
                self.publish_data("", False)
                print("Gate is closed by user {}".format(self.user))
            elif data.get("action", None) == "add_licence_plate":
                licence_plate_to_add = data.get("licence_plate", None)
                self.allowed_licence_plates.append(licence_plate_to_add)
            elif data.get("action", None) == "remove_licence_plate":
                licence_plate_for_removal = data.get("licence_plate", None)
                if licence_plate_for_removal in self.allowed_licence_plates:
                    self.allowed_licence_plates.remove(licence_plate_for_removal)

    def publish_data(self, licence_plate, is_entering):
        self.client.publish(self.send_topic, json.dumps({"licencePlate": licence_plate,
                                                         "isPublic": self.is_public,
                                                         "isOpen": self.is_open,
                                                         "isEntering": is_entering,
                                                         "isOpenByUser": self.is_opened_by_user,
                                                         "actionBy": self.user,
                                                         "consumptionPerMinute": round(self.power_per_hour / 60, 4)}),
                            retain=False)

    async def send_data(self):
        while True:
            if not self.is_on:
                break
            await asyncio.sleep(10)

            print("Allowed licence plates: {}".format(self.allowed_licence_plates))
            print("Licence plates in home: {}".format(self.licence_plates_in_home))

            licence_plate = next(yield_licence_plate(self.allowed_licence_plates))

            if self.is_opened_by_user:
                if licence_plate in self.licence_plates_in_home:
                    print(
                        "Car with licence plate {} is leaving home because gate is opened by {}".format(licence_plate,
                                                                                                        self.user))
                    self.publish_data(licence_plate, True)
                    self.licence_plates_in_home.remove(licence_plate)
                    await asyncio.sleep(2)
                    continue
                else:
                    print(
                        "Car with licence plate {} is entering home because gate is opened by {}".format(licence_plate,
                                                                                                         self.user))
                    self.publish_data(licence_plate, False)
                    self.licence_plates_in_home.append(licence_plate)
                    await asyncio.sleep(2)
                    continue

            self.user = "System"

            # car leaving home
            if licence_plate in self.licence_plates_in_home:
                print("Car with licence plate {} is leaving home".format(licence_plate))
                # gate is open
                self.is_open = True
                self.publish_data(licence_plate, False)

                await asyncio.sleep(2)

                # gate is closed
                self.is_open = False
                self.licence_plates_in_home.remove(licence_plate)
                self.publish_data(licence_plate, False)

            else:
                # car entering home
                if self.is_public:
                    print("Car with licence plate {} is entering home".format(licence_plate))
                    # gate is open
                    self.is_open = True
                    self.publish_data(licence_plate, True)
                    self.licence_plates_in_home.append(licence_plate)

                    await asyncio.sleep(2)

                    # gate is closed
                    self.is_open = False
                    self.publish_data(licence_plate, True)
                else:
                    if licence_plate in self.allowed_licence_plates:
                        print("Car with licence plate {} is allowed and entering home".format(licence_plate))
                        # gate is open
                        self.is_open = True
                        self.publish_data(licence_plate, True)
                        self.licence_plates_in_home.append(licence_plate)
                        await asyncio.sleep(2)

                        # gate is closed
                        self.is_open = False
                        self.publish_data(licence_plate, True)
                    else:
                        print("Car with licence plate {} is not allowed to enter home".format(licence_plate))

