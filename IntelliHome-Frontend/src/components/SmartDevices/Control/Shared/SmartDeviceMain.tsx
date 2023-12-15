import {Box, Container, CssBaseline, Typography} from "@mui/material";
import {useEffect, useMemo, useState} from "react";
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
import SmartDevice from "../../../../models/interfaces/SmartDevice.ts";
import {useQuery, useQueryClient} from "react-query";

const SmartDeviceMain = () => {
    const params = useParams();
    const queryClient = useQueryClient();
    const [isConnected, setIsConnected] = useState(false);
    const [selectedTab, setSelectedTab] = useState(0);
    const [deviceType, setDeviceType] = useState(params.type);
    const [smartDeviceId, setSmartDeviceId] = useState(params.id);
    const [isRan, setIsRan] = useState(false);
    // @ts-ignore
    const [smartDevice, setSmartDevice] = useState<SmartDevice>({});


    useQuery({
        queryKey: ['getSmartDevice'], queryFn: () => {
            axios.get(environment + `/api/${deviceType}/Get?Id=${smartDeviceId}`).then(res => {
                if (!isRan) {
                    setSmartDevice(res.data)
                    setIsConnected(res.data.isConnected)
                }
            }).catch(err => {
                console.log(err)
            });
        },
        refetchOnWindowFocus: false
    });
    const getSmartDevice = () => {
        return smartDevice;
    }
    useEffect(() => {
        queryClient.invalidateQueries('getSmartDevice');
    }, [selectedTab])
    useEffect(()=>{
        console.log("AAAAAAAASDASDAS")
    },[])

    const signalRSmartDeviceService = useMemo(() => new SignalRSmartDeviceService(), [smartDeviceId]);

    function initSmartDeviceSocketConnection() {
        const subscriptionResultCallback = (result) => {
            result = JSON.parse(result);
            console.log('Subscription result:', result);
            setSmartDevice(prevSmartDevice => ({
                ...prevSmartDevice,
                ...result
            }));
            result.isConnected !== undefined && setIsConnected(result.isConnected);
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
        })
    }

    useEffect(() => {
        const fetchData = async () => {
            try {
                if (smartDeviceId) {
                    await getSmartDevice();
                }
                await initSmartDeviceSocketConnection();
            } catch (error) {
                console.error('Error:', error);
            }
        };
        fetchData();

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
                    deviceType == "Lamp" ? <LampControl device={smartDevice}/> :
                        deviceType == "SolarPanelSystem" ? <SolarPanelsControl solarPanelSystem={smartDevice}/> :
                            deviceType == "VehicleGate" ? <GateControl device={smartDevice}/> :
                                deviceType == "BatterySystem" ? <BatteryControl batterySystem={smartDevice}/> :
                                    <></>


            : selectedTab == 2 ? <SmartDeviceReportAction inputData={[
                {action: "some actino", by: "Vukasin", date: new Date(dayjs().subtract(1, "hour").toString())},
                {action: "some actino", by: "Marko", date: new Date(dayjs().subtract(5, "hour").toString())},
                {action: "some actino", by: "Vukasin", date: new Date(dayjs().subtract(1, "day").toString())},
                {action: "some actino", by: "Vukasin", date: new Date(dayjs().subtract(3, "day").toString())},
                {action: "some actino", by: "Dusan", date: new Date(dayjs().toString())},
                {action: "some actino", by: "Vukasin", date: new Date(dayjs().toString())},
                {action: "some actino", by: "Vukasin", date: new Date(dayjs().toString())},
            ]}/> : <></>}

    </>
}

export default SmartDeviceMain;