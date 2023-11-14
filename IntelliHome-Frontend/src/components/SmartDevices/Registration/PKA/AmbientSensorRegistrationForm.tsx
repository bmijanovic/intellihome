import React, { useState } from 'react';
import SmartDeviceRegistrationForm from "../Shared/SmartDeviceRegistrationForm.tsx";
import {
    Box,
    Container,
    Typography
} from "@mui/material";
import CommonSmartDeviceFields from "../../../../models/interfaces/CommonSmartDeviceFields.ts";
import SmartDeviceService from "../../../../services/smartDevices/SmartDeviceService.ts";
import SmartDeviceType from "../../../../models/enums/SmartDeviceType.ts";
import smartDeviceCategory from "../../../../models/enums/SmartDeviceCategory.ts";
import PowerPerHourInput from "../Shared/PowerPerHourInput.tsx";
import DeviceRegistrationButtons from "../Shared/DeviceRegistrationButtons.tsx";

interface AmbientSensorAdditionalFields {
    PowerPerHour: number;
}

interface AmbientSensorRegistrationFormProps {
    smartHomeId: string;
}

const AmbientSensorRegistrationForm : React.FC<AmbientSensorRegistrationFormProps> = ({smartHomeId}) => {
    const [additionalFormData, setAdditionalFormData] = useState<AmbientSensorAdditionalFields>({
        PowerPerHour: 0
    });

    const [commonFormData, setCommonFormData] = useState<CommonSmartDeviceFields>({
        Name: "Ambient Sensor",
        Image: new Blob([])
    });

    const handlePowerValueChange = (powerValue: number) => {
        setAdditionalFormData((prevData) => ({
            ...prevData,
            PowerPerHour: powerValue
        }));
    };

    const handleCommonFormInputChange = (smartDeviceData: CommonSmartDeviceFields) => {
        setCommonFormData(smartDeviceData);
    };

    const handleAmbientSensorSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        SmartDeviceService.registerSmartDevice({...commonFormData, ...additionalFormData}, smartHomeId, smartDeviceCategory.PKA, SmartDeviceType.AmbientSensor);
    };

    return (
        <Container
            component="main"
            maxWidth="xs"
            sx={{
                display: "flex",
                flexDirection: "column",
                alignItems: "center",
                backgroundColor: "white",
                borderRadius: 3,
                justifyContent: "start"
            }}
        >
            <Box
                component="form"
                onSubmit={handleAmbientSensorSubmit}
                sx={{
                    display: "flex",
                    flexDirection: "column",
                    alignItems: "start",
                    width: 1,
                    margin: 2
                }}
            >
                <Typography variant="h4" sx={{ textAlign: "left", width: 1 }}>
                    Add Ambient Sensor
                </Typography>

                <SmartDeviceRegistrationForm
                    formData={commonFormData}
                    onFormChange={handleCommonFormInputChange}
                />

                <PowerPerHourInput
                    onValueChange={handlePowerValueChange}
                />

                <DeviceRegistrationButtons onCancel={() => {}}/>
            </Box>
        </Container>
    );
};

export default AmbientSensorRegistrationForm;