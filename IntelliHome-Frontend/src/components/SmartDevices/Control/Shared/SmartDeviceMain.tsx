import {Box, FormControlLabel, styled, Switch, SwitchProps, Typography} from "@mui/material";
import React, {useEffect, useState} from "react";
import AmbientSensorControl from "../PKA/AmbientSensorControl";
import AirConditionerControl from "../PKA/AirConditionerControl";
import LampControl from "../SPU/LampControl";
import SolarPanelControl from "../VEU/SolarPanelControl.tsx";
import GateControl from "../SPU/GateControl";
import SmartDeviceReportAction from "./SmartDeviceReportAction";
import dayjs from "dayjs";
import BatteryControl from "../VEU/BatteryControl";
import axios from "axios";
import {environment} from "../../../../security/Environment.tsx";
import {useParams} from "react-router-dom";
import SignalRSmartDeviceService from "../../../../services/smartDevices/SignalRSmartDeviceService.ts";
import SmartDevice from "../../../../models/interfaces/SmartDevice.ts";
import AmbientSensorReport from "../PKA/AmbientSensorReport.tsx";
import AirConditionerReport from "../PKA/AirConditionerReport.tsx";
import LampReport from "../SPU/LampReport.tsx";
import SolarPanelReport from "../VEU/SolarPanelReport.tsx";
import GateReport from "../SPU/GateReport.tsx";
import BatteryReport from "../VEU/BatteryReport.tsx";

const SmartDeviceMain = () => {
    const params = useParams();
    const [isConnected, setIsConnected] = useState(false);
    const [selectedTab, setSelectedTab] = useState(0);
    const [deviceType, setDeviceType] = useState(params.type);
    const [smartDeviceId, setSmartDeviceId] = useState(params.id);
    const [isOn, setIsOn] = useState(false);
    // @ts-ignore
    const [smartDevice, setSmartDevice] = useState<SmartDevice>({});
    const signalRSmartDeviceService = new SignalRSmartDeviceService();

    const subscriptionResultCallback = (result) => {
        console.log('Subscription result:', result);
    }

    const dataResultCallback = (result) => {
        result = JSON.parse(result);
        console.log('data result:', result);
        setSmartDevice(prevSmartDevice => ({
            ...prevSmartDevice,
            ...result
        }));
        result.isConnected !== undefined && setIsConnected(result.isConnected);
        result.isOn !== undefined && setIsOn(result.isOn);
    };

    useEffect(() => {
        if (smartDeviceId) {
            getSmartDevice();
        }

        signalRSmartDeviceService.startConnection().then(() => {
            console.log('SignalR connection established');
            console.log(smartDeviceId);
            signalRSmartDeviceService.receiveSmartDeviceSubscriptionResult(subscriptionResultCallback);
            signalRSmartDeviceService.receiveSmartDeviceData(dataResultCallback);
            signalRSmartDeviceService.subscribeToSmartDevice(smartDeviceId);
        });

        return () => {
            signalRSmartDeviceService.stopConnection().then(() => {
                console.log('SignalR connection stopped');
            });
        }
    }, [smartDeviceId]);

    function getSmartDevice() {
        axios.get(environment + `/api/${deviceType}/Get?Id=${smartDeviceId}`).then(res => {
            setSmartDevice(res.data)
            setIsConnected(res.data.isConnected)
            setIsOn(res.data.isOn)
        }).catch(err => {
            console.log(err)
        });
    }

    function toggleSmartDevice(on) {
        axios.put(environment + `/api/${deviceType}/Toggle?Id=${smartDeviceId}&TurnOn=${on}`).then(res => {
                setIsOn(on)
                smartDevice.isOn = on;
            }
        ).catch(err => {
            console.log(err)
        });
    }

    const SwitchPower = styled((props: SwitchProps) => (
        <Switch focusVisibleClassName=".Mui-focusVisible" checked={isOn} onChange={(e) => {
            setIsOn(e.target.checked);
            toggleSmartDevice(e.target.checked);
        }} size="medium" disableRipple {...props} />
    ))(({theme}) => ({
        width: 105,
        height: 52,
        padding: 0,
        '& .MuiSwitch-switchBase': {
            padding: 0,
            margin: 4,
            transitionDuration: '300ms',
            '&.Mui-checked': {
                transform: 'translateX(52px)',
                color: '#fff',
                '& + .MuiSwitch-track': {
                    backgroundColor: theme.palette.mode === 'dark' ? '#2ECA45' : '#65C466',
                    opacity: 1,
                    border: 0,
                },
                '&.Mui-disabled + .MuiSwitch-track': {
                    opacity: 0.5,
                },
            },
            '&.Mui-focusVisible .MuiSwitch-thumb': {
                color: '#33cf4d',
                border: '6px solid #fff',
            },
            '&.Mui-disabled .MuiSwitch-thumb': {
                color:
                    theme.palette.mode === 'light'
                        ? theme.palette.grey[100]
                        : theme.palette.grey[600],
            },
            '&.Mui-disabled + .MuiSwitch-track': {
                opacity: theme.palette.mode === 'light' ? 0.7 : 0.3,
            },
        },
        '& .MuiSwitch-thumb': {
            boxSizing: 'border-box',
            width: 44,
            height: 44,
        },
        '& .MuiSwitch-track': {
            borderRadius: 26,
            backgroundColor: theme.palette.mode === 'light' ? '#E9E9EA' : '#39393D',
            opacity: 1,
            transition: theme.transitions.create(['background-color'], {
                duration: 500,
            }),
        },
    }));

    return <>
        <Box display="flex" flexDirection="row" alignItems="center" width="100%">
            <Typography fontSize="40px" fontWeight="650">{smartDevice.name}</Typography>
            <Box mx={1} ml={4} sx={{marginTop: "3px"}} width="15px" height="15px" borderRadius="50px"
                 bgcolor={isConnected ? "green" : "red"}/>
            <Typography fontSize="30px" sx={{marginTop: "3px"}} color={isConnected ? "green" : "red"}
                        fontWeight="500">{isConnected ? "Online" : "Offline"}</Typography>
            <Box display="flex" flexDirection="row" alignItems="center" borderRadius="25px" ml={"auto"}>
                <Typography fontSize="30px" fontWeight="600" mr={"20px"}> POWER </Typography>
                <FormControlLabel sx={{marginRight: 0}} control={<SwitchPower/>} label=""/>
            </Box>

        </Box>
        <Box width="100%">
            <Typography fontSize="25px" color="secondary" fontWeight="600">{deviceType}</Typography>
        </Box>
        <Box width="100%" height={"4px"} my={2} bgcolor="#343F71FF"></Box>
        <Box display="flex" flexDirection="row">
            <Typography px={2} py={1} sx={{
                backgroundColor: selectedTab == 0 ? "#FBC40E" : "#D0D2E1",
                borderRadius: "12px 0px 0px 12px",
                ':hover': {backgroundColor: selectedTab == 0 ? "#FBC40E" : "#a4a5af", cursor: "pointer"}
            }} onClick={() => setSelectedTab(0)} fontSize="25px" fontWeight="500">Control</Typography>
            <Typography px={2} py={1} sx={{
                backgroundColor: selectedTab == 1 ? "#FBC40E" : "#D0D2E1",
                borderRadius: "0px 12px 12px 0px",
                ':hover': {backgroundColor: selectedTab == 1 ? "#FBC40E" : "#a4a5af", cursor: "pointer"}
            }} onClick={() => setSelectedTab(1)} fontSize="25px" fontWeight="500">Reports</Typography>
        </Box>
        {selectedTab == 0 ? deviceType == "AmbientSensor" ? <AmbientSensorControl smartDeviceId={smartDeviceId}/> :
                deviceType == "AirConditioner" ? <AirConditionerControl/> :
                    deviceType == "Lamp" ? <LampControl device={smartDevice} setSmartDeviceParent={setSmartDevice}/> :
                        deviceType == "SolarPanelSystem" ? <SolarPanelControl solarPanelSystem={smartDevice}/> :
                            deviceType == "VehicleGate" ?
                                <GateControl device={smartDevice} setSmartDeviceParent={setSmartDevice}/> :
                                deviceType == "BatterySystem" ? <BatteryControl batterySystem={smartDevice}/> :
                                    <></>


            : selectedTab == 1 ? deviceType == "AmbientSensor" ? <AmbientSensorReport ambientSensor={smartDevice}/> :
                deviceType == "AirConditioner" ? <AirConditionerReport airConditioner={smartDevice}/> :
                    deviceType == "Lamp" ? <LampReport device={smartDevice} setSmartDeviceParent={setSmartDevice}/> :
                        deviceType == "SolarPanelSystem" ? <SolarPanelReport solarPanelSystem={smartDevice}/> :
                            deviceType == "VehicleGate" ?
                                <GateReport device={smartDevice} setSmartDeviceParent={setSmartDevice}/> :
                                deviceType == "BatterySystem" ? <BatteryReport batterySystem={smartDevice}/> :
                                    <></> : <></>}

    </>
}

export default SmartDeviceMain;