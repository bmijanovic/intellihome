import {useParams} from "react-router-dom";
import SmartDeviceMain from "../components/SmartDevices/Control/Shared/SmartDeviceMain";
import {Box} from "@mui/material";

const SmartDevicePage = () => {
    const { id, type } = useParams();
    return (
        <><Box
            sx={{display: "flex", flexDirection: "column", width: "100%", backgroundColor: "#DBDDEB", padding: "20px"}}><SmartDeviceMain smartDeviceId={id} deviceType={type}/>
        </Box></>
    )
}

export default SmartDevicePage;