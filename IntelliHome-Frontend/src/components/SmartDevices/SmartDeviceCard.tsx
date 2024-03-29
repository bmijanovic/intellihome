import {Box, Typography} from "@mui/material";
import React from "react";
import {useNavigate} from "react-router-dom";
import {getSmartDeviceTypeValueByKey} from "../../models/enums/SmartDeviceType.ts";
import {environment} from "../../utils/Environment.ts";


const SmartDeviceCard = (props) => {

    const smartDevice = props.smartDevice;
    const colors = {"PKA":"#676E79", "SPU":"#F43F5E","VEU": "#2691D9"};
    const navigate = useNavigate();

    const smartDeviceTypes = [
        "AmbientSensor",
        "AirConditioner",
        "WashingMachine",
        "Lamp",
        "VehicleGate",
        "Sprinkler",
        "SolarPanelSystem",
        "BatterySystem",
        "VehicleCharger"
    ];


    function navigateToSmartDevice() {
        let type = getSmartDeviceTypeValueByKey(smartDevice.type);
        navigate(`/smartDevice/${type}/${smartDevice.id}`);
    }

    return (
        <Box sx={{ background: "white", width: "25vw", height: "12vh", borderRadius: "15px", mt: "10px", display: "flex", position: "relative" }} onClick={navigateToSmartDevice}>
            {/* Rounded left edge */}
            <div style={{ height: "100%", width: "10px", position: "absolute", left: 0, top: 0, backgroundColor:`${colors[smartDevice.category]}`, borderTopLeftRadius: "15px", borderBottomLeftRadius: "15px" }}></div>

            {/* Your content goes here */}
            <div style={{ margin: "auto 5px" }}>
                <img
                    src={environment + '/' + smartDevice.image}
                    alt="Smart HomePage Image"
                    style={{ width: "70px", height: "70px", minWidth:"70px", maxWidth:"70px", minHeight:"70px", maxHeight:"70px", border: "5px solid #343F71", borderRadius: "8px", marginLeft: "20px" }}
                />
            </div>
            <Typography sx={{margin:"auto 15px", fontSize:"23px", fontWeight:"600", color:"#343F71"}}>{smartDevice.name}</Typography>
            <div style={{ width: "12px", height: "12px", position: "absolute", top: 10, right: 10, backgroundColor: `${smartDevice.isConnected ? "green" : "red"}`, borderRadius: "50%" }}></div>


        </Box>


    )

}

export default SmartDeviceCard;