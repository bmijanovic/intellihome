from Models.SmartDevice import SmartDevice


class VehicleGate(SmartDevice):
    def __init__(self, device_id, smart_home_id, device_category, device_type, is_public, allowed_licence_plates):
        super().__init__(device_id, smart_home_id, device_category, device_type)
        self.is_public = is_public
        self.allowed_licence_plates = allowed_licence_plates
