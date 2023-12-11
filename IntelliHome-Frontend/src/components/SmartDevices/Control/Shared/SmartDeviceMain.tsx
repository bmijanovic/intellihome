import {Box, Container, CssBaseline, Typography} from "@mui/material";
import {useEffect, useState} from "react";
import AmbientSensorControl from "../PKA/AmbientSensorControl";
import AirConditionerControl from "../PKA/AirConditionerControl";
import LampControl from "../SPU/LampControl";
import SolarPanelsControl from "../VEU/SolarPanelsControl";
import GateControl from "../SPU/GateControl";
import SmartDeviceReportAction from "./SmartDeviceReportAction";
import dayjs from "dayjs";
import BatteryControl from "../VEU/BatteryControl";
import axios from "axios";
import {environment} from "../../../../security/Environment.tsx";
import {useParams} from "react-router-dom";
import SignalRSmartDeviceService from "../../../../services/smartDevices/SignalRSmartDeviceService.ts";

const SmartDeviceMain = () => {
    const params = useParams();
    const [isConnected, setIsConnected] = useState(false);
    const [selectedTab, setSelectedTab] = useState(0);
    const [deviceType, setDeviceType] = useState(params.type);
    const [smartDeviceId, setSmartDeviceId] = useState(params.id);
    const [smartDevice, setSmartDevice] = useState({});



    function getSmartDevice() {
        axios.get(environment + `/api/${deviceType}/Get?Id=${smartDeviceId}`).then(res => {
            setSmartDevice(res.data)
            setIsConnected(res.data.isConnected)
        }).catch(err => {
            console.log(err)
        });
    }


    useEffect(() => {
        if(smartDeviceId)
        {
            getSmartDevice()
        }
        const signalRSmartDeviceService = new SignalRSmartDeviceService();
        const subscriptionResultCallback = (result) => {
            result = JSON.parse(result);
            setSmartDevice(prevSmartDevice => ({
                ...prevSmartDevice,
                currentBrightness: result.currentBrightness,
                isWorking: result.isWorking,
            }));

        }

        const resultCallback = (result) => {
            console.log('Subscription result:', result);
        };


        signalRSmartDeviceService.startConnection().then(() => {
            console.log('SignalR connection established');
            console.log(smartDeviceId);
            signalRSmartDeviceService.subscribeToSmartDevice(smartDeviceId);
            signalRSmartDeviceService.receiveSmartDeviceData(subscriptionResultCallback);
            signalRSmartDeviceService.receiveSmartDeviceSubscriptionResult(resultCallback);

        });

        return () => {
            signalRSmartDeviceService.stopConnection().then(() => {
                console.log('SignalR connection stopped');
            });
        }

    }, []);

    return <>
        <Box display="flex" flexDirection="row" alignItems="center">
            <Typography fontSize="40px" fontWeight="650">{smartDevice.name}</Typography>
            <Box mx={1} ml={4} sx={{marginTop: "3px"}} width="15px" height="15px" borderRadius="50px"
                 bgcolor={isConnected ? "green" : "red"}/>
            <Typography fontSize="30px" sx={{marginTop: "3px"}} color={isConnected ? "green" : "red"}
                        fontWeight="500">{isConnected ? "Online" : "Offline"}</Typography>
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
                ':hover': {backgroundColor: selectedTab == 1 ? "#FBC40E" : "#a4a5af", cursor: "pointer"}
            }} fontSize="25px" fontWeight="500" onClick={() => setSelectedTab(1)}>Management</Typography>
            <Typography px={2} py={1} sx={{
                backgroundColor: selectedTab == 2 ? "#FBC40E" : "#D0D2E1",
                borderRadius: "0px 12px 12px 0px",
                ':hover': {backgroundColor: selectedTab == 2 ? "#FBC40E" : "#a4a5af", cursor: "pointer"}
            }} onClick={() => setSelectedTab(2)} fontSize="25px" fontWeight="500">Reports</Typography>
        </Box>
        {selectedTab == 0 ? deviceType == "AmbientSensor" ? <AmbientSensorControl smartDeviceId={smartDeviceId}/> :
                deviceType == "AirConditioner" ? <AirConditionerControl/> :
                    deviceType == "Lamp" ? <LampControl device={smartDevice} setSmartDeviceParent={setSmartDevice}/> :
                        deviceType == "SolarPanel" ? <SolarPanelsControl/> :
                            deviceType=="Gate"?<GateControl/>:
                                deviceType=="Battery"?<BatteryControl/>:
                            <></>


            :selectedTab == 2?<SmartDeviceReportAction inputData={[
                {action: "some actino", by: "Vukasin", date: new Date(dayjs().subtract(1, "hour").toString())},
                {action: "some actino", by: "Marko", date: new Date(dayjs().subtract(5, "hour").toString())},
                {action: "some actino", by: "Vukasin", date: new Date(dayjs().subtract(1, "day").toString())},
                {action: "some actino", by: "Vukasin", date: new Date(dayjs().subtract(3, "day").toString())},
                {action: "some actino", by: "Dusan", date: new Date(dayjs().toString())},
                {action: "some actino", by: "Vukasin", date: new Date(dayjs().toString())},
                {action: "some actino", by: "Vukasin", date: new Date(dayjs().toString())},
            ]}/>: <></>}

    </>
}

export default SmartDeviceMain;