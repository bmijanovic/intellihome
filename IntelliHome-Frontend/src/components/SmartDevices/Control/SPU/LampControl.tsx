import {
    Box,
    FormControlLabel,
    IconButton,
    styled,
    Switch,
    SwitchProps,
    Typography
} from "@mui/material";
import React, {useEffect, useState} from "react";
import {KeyboardArrowDown, KeyboardArrowUp} from "@mui/icons-material";
import axios from "axios";
import SmartDeviceAvailabilityPie from "../Shared/SmartDeviceAvailabilityPie.tsx";
import {environment} from "../../../../utils/Environment.ts";

const LampControl = ({device, setSmartDeviceParent}) => {
    console.log(device)
    const [brightness, setBrightness]=useState(device.currentBrightness)
    const [threshold, setThreshold]= useState(device.brightnessLimit)
    const [isAuto, setIsAuto]=useState(device.isAuto)
    const [isShining, setisShining]=useState(device.isShining)
    const SwitchPower = styled((props: SwitchProps) => (
        <Switch focusVisibleClassName=".Mui-focusVisible" checked={isShining} onChange={(e) => {
            setisShining(e.target.checked);
            turnOnDevice(e.target.checked);
        }} size="medium" disableRipple {...props} />
    ))(({theme}) => ({
        width: 210,
        height: 95,
        padding: 0,
        '& .MuiSwitch-switchBase': {
            padding: 0,
            margin: 4,
            transitionDuration: '300ms',
            '&.Mui-checked': {
                transform: 'translateX(114px)',
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
            width: 88,
            height: 88,
        },
        '& .MuiSwitch-track': {
            borderRadius: 52,
            backgroundColor: theme.palette.mode === 'light' ? '#E9E9EA' : '#39393D',
            opacity: 1,
            transition: theme.transitions.create(['background-color'], {
                duration: 500,
            }),
        },
    }));
    const SwitchAuto = styled((props: SwitchProps) => (
        <Switch focusVisibleClassName=".Mui-focusVisible" checked={isAuto} onChange={(e) => {
            setIsAuto(e.target.checked)
            setAutoMode(e.target.checked);
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

    useEffect(() => {

        device.isAuto = isAuto;
        device.brightnessLimit = threshold;
        device.currentBrightness = brightness;
        device.isShining = isShining;
        setSmartDeviceParent(device);

    }, [isShining, isAuto, threshold, brightness]);

    useEffect(() => {
        setBrightness(device.currentBrightness);
        setThreshold(device.brightnessLimit);
        setIsAuto(device.isAuto);
        setisShining(device.isShining);
    }, [device]);




    function turnOnDevice(isOn){
        console.log(isOn);
        axios.put(environment + `/api/Lamp/TurnLightOnOff?Id=${device.id}&TurnOn=${isOn}`).then(res => {
            console.log(res.data)
        }).catch(err => {
            console.log(err)
        });
    }

    function setAutoMode(isAuto) {
        console.log(isAuto);
        axios.put(environment + `/api/Lamp/ChangeMode?Id=${device.id}&IsAuto=${isAuto}`).then(res => {
            console.log(res.data)
        }).catch(err => {
            console.log(err)
        });
    }


    function setThresholdLimit(threshold) {
        setThreshold(threshold)
        axios.put(environment + `/api/Lamp/ChangeBrightnessLimit?Id=${device.id}&Brightness=${threshold}`).then(res => {
            console.log(res.data)
        }).catch(err => {
            console.log(err)
        });
    }



    return <>
        <Box mt={1} display="grid" gap="10px" gridTemplateColumns="4fr 3fr 5fr"
                  gridTemplateRows="170px 170px 170px">

            <Box gridColumn={1} height="350px" gridRow={1} display="flex" justifyContent="center" flexDirection="column"
                 alignItems="center" bgcolor="white" borderRadius="25px">
                <Typography fontSize="50px" fontWeight="600"> POWER</Typography>
                <FormControlLabel sx={{marginRight: 0}}
                                  control={<SwitchPower sx={{ml: "10px", mt: "20px"}}/>}
                 label=""/>
            </Box>
            <Box display="grid" gridColumn={2} height="350px" gridRow={1} gap="10px" gridTemplateRows="170px 170px">
                <Box gridColumn={1} height="170px" gridRow={1} display="flex" justifyContent="center" flexDirection="column"
                     alignItems="center" bgcolor="white" borderRadius="25px">
                    <Typography fontSize="30px" fontWeight="600"> AUTO</Typography>
                    <FormControlLabel sx={{marginRight: 0}} control={<SwitchAuto sx={{ml: "10px", mt: "20px"}}/>} label=""/>
                </Box>
                <Box gridColumn={1} height="170px" gridRow={2} display="flex" justifyContent="center" flexDirection="column"
                     alignItems="center" bgcolor="white" borderRadius="25px">
                    <Typography fontSize="30px" fontWeight="600"> THRESHOLD</Typography>
                    <Box display="flex" mt={2} flexDirection="row" justifyContent="center" alignItems="center">
                        <Typography fontSize="40px" fontWeight="700" display="flex" alignItems="flex-end">{threshold}<Typography mb={1} fontSize="20px" >nit</Typography></Typography>
                        <Box display="flex" flexDirection="column">
                            <IconButton
                                onClick={() => setThresholdLimit(threshold + 50)}><KeyboardArrowUp/></IconButton>
                            <IconButton
                                onClick={() => setThresholdLimit(threshold - 50)}><KeyboardArrowDown/></IconButton>
                        </Box>
                    </Box>
                </Box>
            </Box>
            <Box gridColumn={3} height="350px" gridRow={1} display="flex" justifyContent="center" flexDirection="column"
               alignItems="center" bgcolor="white" borderRadius="25px">
                <Typography fontSize="30px" fontWeight="600"> BRIGHTNESS</Typography>
                <Typography fontSize="50px" fontWeight="700" display="flex" alignItems="flex-end"> {brightness}<Typography mb={2} fontSize="20px" >nit</Typography></Typography>

            </Box>

        </Box>
    </>
}

export default LampControl;