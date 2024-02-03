from locust import HttpUser, task, between


class User(HttpUser):
    wait_time = between(1, 3)
    host = "http://localhost:5283"

    def on_start(self):
        response = self.client.post("/api/User/login", json={"username": "crni", "password": "crni"})
        if response.status_code != 200:
            self.environment.runner.quit()
        self.client.cookies.update(response.cookies)

    @task
    def register_smart_device(self):
        smart_home_id = "603223ce-8a82-41d6-aadc-9f96dfce35d9"
        vehicle_gate_data = {
            "AllowedLicencePlates": ["SM023SA", "SM023AS"],
            "PowerPerHour": 2,
            "Name": "Gate"
        }
        response = self.client.post(
            f"/api/SPU/CreateVehicleGate/{smart_home_id}",
            headers={"Cookie": "auth=" + str(self.client.cookies.get("auth"))}, data=vehicle_gate_data
        )
        if response.status_code != 200:
            self.environment.runner.quit()
