import React, { useState } from 'react';
import SmartDeviceRegistrationForm from "../Shared/SmartDeviceRegistrationForm.tsx";
import {Box, Container, TextField, Typography} from "@mui/material";
import CommonSmartDeviceFields from "../../../../models/interfaces/CommonSmartDeviceFields.ts";
import InputAdornment from "@mui/material/InputAdornment";
import SmartDeviceService from "../../../../services/smartDevices/SmartDeviceService.ts";
import smartDeviceCategory from "../../../../models/enums/SmartDeviceCategory.ts";
import SmartDeviceType from "../../../../models/enums/SmartDeviceType.ts";
import DeviceRegistrationButtons from "../Shared/DeviceRegistrationButtons.tsx";

interface VehicleChargerAdditionalFields {
    Power: number;
    NumberOfChargingPoints: number;
}

interface VehicleChargerRegistrationFormProps {
    smartHomeId: string;
}

const VehicleChargerRegistrationForm : React.FC<VehicleChargerRegistrationFormProps> = ({smartHomeId}) => {
    const [additionalFormData, setAdditionalFormData] = useState<VehicleChargerAdditionalFields>({
        Power: 1,
        NumberOfChargingPoints: 2
    });

    const [commonFormData, setCommonFormData] = useState<CommonSmartDeviceFields>({
        Name: "VehicleCharger",
        Image: new Blob([])
    });

    const handleCommonFormInputChange = (smartDeviceData: CommonSmartDeviceFields) => {
        setCommonFormData(smartDeviceData);
    };

    const handleAdditionalFormInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setAdditionalFormData({
            ...additionalFormData,
            [e.target.name]: e.target.value
        });
    };

    const handleVehicleChargerSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        SmartDeviceService.registerSmartDevice({...commonFormData, ...additionalFormData}, smartHomeId, smartDeviceCategory.VEU, SmartDeviceType.VehicleCharger);
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
                onSubmit={handleVehicleChargerSubmit}
                sx={{
                    display: "flex",
                    flexDirection: "column",
                    alignItems: "start",
                    width: 1,
                    margin: 2
                }}
            >
                <Typography variant="h4" sx={{ textAlign: "left", width: 1 }}>
                    Add Vehile Charger
                </Typography>

                <SmartDeviceRegistrationForm
                    formData={commonFormData}
                    onFormChange={handleCommonFormInputChange}
                />

                <Box sx={{display:"flex", width:1}}>
                    <TextField
                        variant="outlined"
                        margin="normal"
                        required
                        fullWidth
                        id="Power"
                        label="Power"
                        name="Power"
                        type="number"
                        value={additionalFormData.Power}
                        onChange={handleAdditionalFormInputChange}
                        InputProps={{
                            endAdornment: <InputAdornment position="end">KW</InputAdornment>,
                        }}
                        inputProps={{
                            min: 1,
                            max: 1000,
                        }}
                        sx={{marginRight:1}}
                    />

                    <TextField
                        variant="outlined"
                        margin="normal"
                        required
                        fullWidth
                        id="NumberOfChargingPoints"
                        label="Charging points"
                        name="NumberOfChargingPoints"
                        type="number"
                        value={additionalFormData.NumberOfChargingPoints}
                        onChange={handleAdditionalFormInputChange}
                        InputProps={{
                            endAdornment: <InputAdornment position="end">Count</InputAdornment>,
                        }}
                        inputProps={{
                            min: 1,
                            max: 4,
                        }}
                        sx={{marginLeft:1}}
                    />
                </Box>

                <DeviceRegistrationButtons onCancel={() => {}}/>
            </Box>
        </Container>
    );
};

export default VehicleChargerRegistrationForm;