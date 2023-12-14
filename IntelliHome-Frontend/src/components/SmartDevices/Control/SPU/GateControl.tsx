import {
    Box, Button, Checkbox, FormControl,
    FormControlLabel,
    Grid,
    IconButton, MenuItem, Modal, Select,
    styled,
    Switch,
    SwitchProps, TextField,
    Typography
} from "@mui/material";
import React, {useEffect, useState} from "react";
import {Add, Close, KeyboardArrowDown, KeyboardArrowUp} from "@mui/icons-material";
import dayjs from "dayjs";
import axios from "axios";
import {environment} from "../../../../security/Environment.tsx";
import {v4 as uuidv4} from 'uuid';
import {useQueryClient} from "react-query";


const GateControl = ({device}) => {
    const queryClient = useQueryClient()
    const [lastPlate, setLastPlate] = useState(device.currentLicencePlate)
    const [isOpenGate, setIsOpenGate] = useState(device.isOpen)
    const [isOpenGateByUser, setIsOpenGateByUser] = useState(device.isOpenByUser || false)
    const [isPublic, setIsPublic] = useState(device.isPublic)
    const [open, setIsOpen] = useState(false)
    type TDate = TDate | null;
    const [history, setHistory] = useState([]);
    const [myPlates, setMyPlates] = useState(device.allowedLicencePlates ? device.allowedLicencePlates : []);
    const [newPlate, setNewPlate] = useState("")
    const DatePickerStyle = {
        "& .MuiPickersLayout-actionBar": {display: "none"},
        minHeight: "520px"
    }
    const styledInput = {
        "& label.Mui-focused": {
            color: "#FBC40E"
        },
        "& .MuiOutlinedInput-root": {
            "&.Mui-focused fieldset": {
                borderColor: "#FBC40E",
                borderRadius: "10px"

            },
            borderRadius: "10px"
        },
        margin: "8px auto", borderRadius: "10px"

    }
    const SwitchState = styled((props: SwitchProps) => (
        <Switch focusVisibleClassName=".Mui-focusVisible" checked={isOpenGateByUser} onChange={(e) => {
            // setIsOpenGate(e.target.checked)
            changeState(e.target.checked)
        }} size="large" disableRipple {...props} />
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
    const SwitchMode = styled((props: SwitchProps) => (
        <Switch focusVisibleClassName=".Mui-focusVisible" checked={isPublic} onChange={(e) => {
            // setIsPublic(e.target.checked)
            changeMode(e.target.checked)
        }} size="large" disableRipple {...props} />
    ))(({theme}) => ({
        width: 78,
        height: 41,
        padding: 0,
        '& .MuiSwitch-switchBase': {
            padding: 0,
            margin: 4,
            transitionDuration: '300ms',
            '&.Mui-checked': {
                transform: 'translateX(39px)',
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
            width: 33,
            height: 33,
        },
        '& .MuiSwitch-track': {
            borderRadius: 22,
            backgroundColor: theme.palette.mode === 'light' ? '#E9E9EA' : '#39393D',
            opacity: 1,
            transition: theme.transitions.create(['background-color'], {
                duration: 500,
            }),
        },
    }));

    useEffect(() => {
        console.log(device)
        axios.put(environment + `/api/VehicleGate/TurnOnSmartDevice?Id=${device.id}&TurnOn=${true}`).then(res => {
            console.log(res.data)
        }).catch(err => {
            console.log(err)
        });
        const currentDate = new Date();

        const oneHourBefore = new Date(currentDate);
        oneHourBefore.setHours(currentDate.getHours() - 24);

        const formattedCurrentDate = currentDate.toISOString();
        const formattedOneHourBefore = oneHourBefore.toISOString();

        axios.get(environment + `/api/VehicleGate/GetHistoricalData?Id=${device.id}&From=${formattedOneHourBefore}&To=${formattedCurrentDate}`).then(res => {
            const licencePlates = res.data
                .filter(item => item.isOpen === true)
                .filter(item => item.licencePlate.length > 0)
                .map(item => [item.licencePlate, item.isEntering]);
            console.log(licencePlates)
            setHistory(licencePlates)
        }).catch(err => {
            console.log(err)
        });
    }, [device.id]);



    useEffect(() => {
        console.log("promenio parent")
        setIsOpenGate(device.isOpen)
        setIsOpenGateByUser(device.isOpenByUser)
        setIsPublic(device.isPublic)
        setMyPlates(device.allowedLicencePlates)
        setLastPlate(device.licencePlate)

        if (device.licencePlate !== undefined && device.licencePlate.length > 0 && device.isOpen == true)
            history.push([device.licencePlate, device.isEntering])
        setHistory(history)
    }, [device]);


    const addNewLicencePlate = () => {
        axios.put(environment + `/api/VehicleGate/AddLicencePlate?Id=${device.id}&LicencePlate=${newPlate}`).then(res => {
            device.allowedLicencePlates = [...myPlates, newPlate]
            setMyPlates([...myPlates, newPlate])
            setIsOpen(false)
        }).catch(err => {
            console.log(err)
        });
    }

    const removeLicencePlate = (plate) => {
        // console.log(plate)
        axios.put(environment + `/api/VehicleGate/RemoveLicencePlate?Id=${device.id}&LicencePlate=${plate}`).then(res => {
            device.allowedLicencePlates = myPlates.filter(item => item !== plate)
            setMyPlates(myPlates.filter(item => item !== plate))
        }).catch(err => {
            console.log(err)
        });
    }

    const changeMode = (value) => {
        if (isOpenGateByUser)
            return
        axios.put(environment + `/api/VehicleGate/ChangeMode?Id=${device.id}&IsPublic=${value}`).then(res => {
            setIsPublic(value)
            queryClient.invalidateQueries('getSmartDevice')
        }).catch(err => {
            console.log(err)
        });
    }

    const changeState = (value) => {
        console.log(value)
        axios.put(environment + `/api/VehicleGate/OpenCloseGate?Id=${device.id}&IsOpen=${value}`).then(res => {
            setIsOpenGate(value)
            setIsOpenGateByUser(value)
            queryClient.invalidateQueries('getSmartDevice')
        }).catch(err => {
            console.log(err)
        });
    }


    return <>
        <Modal
            open={open}
            onClose={() => setIsOpen(false)}
            aria-labelledby="modal-modal-title"
            aria-describedby="modal-modal-description"
        >
            <Grid borderRadius="25px" container spacing={2} width="20%" minWidth="500px" bgcolor="white" padding={4}
                  sx={{
                      position: "absolute", top: "50%", left: "50%",
                      transform: 'translate(-50%, -50%)'
                  }}>
                <Grid item xs={12}>
                    <Typography id="modal-modal-title" variant="h6" component="h2">
                        Add New Plate
                    </Typography>
                </Grid>

                <Grid item xs={12} display="flex" margin="0 auto" justifyContent="center" alignItems="center"
                      flexDirection="column">

                    <TextField fullWidth type="text" name="myPlateModal"
                               placeholder="Plate Number" sx={styledInput} onChange={(e) => setNewPlate(e.target.value)}
                               mb={3}></TextField>

                </Grid>
                <Grid item xs={12} display="flex" alignItems="flex-end" justifyContent="end">
                    <Box display="flex">
                        <Button mx={2} onClick={() => setIsOpen(false)} sx={{
                            backgroundColor: "white",
                            border: "1px solid #FBC40E",
                            color: "black",
                            borderRadius: "7px",
                            marginRight: "20px",
                            ':hover': {backgroundColor: "white"}
                        }}>Cancel</Button>
                        <Button mx={2} onClick={addNewLicencePlate} sx={{
                            backgroundColor: "#FBC40E",
                            border: "1px solid #FBC40E",
                            color: "black",
                            borderRadius: "7px",
                            ':hover': {backgroundColor: "#EDB90D"}
                        }}>Create</Button>
                    </Box>

                </Grid>
            </Grid>
        </Modal>
        <Box mt={1} display="grid" gap="10px" gridTemplateColumns="4fr 7fr"
             gridTemplateRows="170px 170px 200px 140px">

            <Box gridColumn={1} height="350px" gridRow={1} display="flex" flexDirection="column"
                 alignItems="center" bgcolor="white" borderRadius="25px">
                <Typography fontSize="30px" mt={1} fontWeight="600"> STATE</Typography>
                <Typography fontSize="80px" color="#343F71" mt={8}
                            fontWeight="600"> {isOpenGate ? "OPENED" : "CLOSED"}</Typography>
                <FormControlLabel sx={{marginRight: 0}} control={<SwitchState sx={{ml: "10px", mt: "20px"}}/>}
                />
            </Box>
            <Box gridColumn={1} gridRow={3} display="flex" justifyContent="center" flexDirection="column"
                 alignItems="center" bgcolor="white" borderRadius="25px">
                <Typography fontSize="30px" mt={1} mb={2} fontWeight="600"> MODE</Typography>
                <Typography fontSize="40px" color="#343F71"
                            fontWeight="600"> {isPublic ? "PUBLIC" : "PRIVATE"}</Typography>
                <FormControlLabel sx={{marginRight: 0}} control={<SwitchMode sx={{ml: "10px", mt: "5px"}}/>}/>
            </Box>
            <Box gridColumn={1} gridRow={4} display="flex" justifyContent="center" flexDirection="column"
                 alignItems="center" bgcolor="white" borderRadius="25px">
                <Typography fontSize="30px" fontWeight="600"> LAST PLATE</Typography>
                <Typography fontSize="50px" fontWeight="600"> {lastPlate}</Typography>

            </Box>

            <Box gridColumn={2} gridRow={1} height="350px" display="flex"
                 flexDirection="column"
                 bgcolor="white" borderRadius="25px">
                <Box display="flex" mt={1} justifyContent={"center"}
                     flexDirection="row">
                    <Typography fontSize="30px" fontWeight="600"> HISTORY</Typography>

                </Box>
                <Box display="flex" width="100%" flexDirection="column" overflow="auto">
                    {[...history].reverse().map((item) => <Box key={uuidv4()}>
                        <Box width="98%" margin="0 auto" height={"2px"} bgcolor="rgba(0, 0, 0, 0.20)"/>
                        <Box width={"100%"} my={1.5} display="flex" alignItems="center" flexDirection={"row"}>
                            <Box display="flex" width="100%" justifyContent="space-between">
                                <Typography ml={2} fontSize="20px" fontWeight="500"> {item[0]}</Typography>
                                <Typography mr={2} fontSize="20px"
                                            fontWeight="500"> {item[1] ? "In" : "Out"}</Typography>

                            </Box>
                        </Box>
                    </Box>)}
                </Box>
            </Box>

            <Box gridColumn={2} gridRow={3} height="350px" display="flex"
                 flexDirection="column"
                 bgcolor="white" borderRadius="25px">
                <Box display="flex" mt={1} justifyContent={"center"}
                     flexDirection="row">
                    <Typography fontSize="30px" fontWeight="600"> MY PLATES</Typography>
                    <IconButton onClick={() => setIsOpen(true)}
                                sx={{
                                    height: "40px",
                                    width: "40px",
                                    marginTop: "2px",
                                    marginLeft: 2
                                }}><Add/></IconButton>
                </Box>
                <Box display="flex" width="100%" flexDirection="column" overflow="auto">
                    {myPlates ? myPlates.map((item) => <Box key={uuidv4()}>
                        <Box width="98%" margin="0 auto" height={"2px"} bgcolor="rgba(0, 0, 0, 0.20)"/>
                        <Box width={"100%"} my={1} display="flex" alignItems="center" flexDirection={"row"}>
                            <Box display="flex" width="100%" justifyContent="space-between">
                                <Typography ml={2} fontSize="20px" fontWeight="500"> {item}</Typography>
                            </Box>
                            <IconButton onClick={() => removeLicencePlate(item)}><Close/></IconButton>
                        </Box>
                    </Box>) : <></>}
                </Box>
            </Box>


        </Box></>
}

export default GateControl;