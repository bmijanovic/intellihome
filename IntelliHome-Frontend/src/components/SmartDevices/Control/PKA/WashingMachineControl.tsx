import {
    Box, Button, Grid,
    IconButton, MenuItem, Modal, Select,
    Typography
} from "@mui/material";
import React, {useEffect, useState} from "react";
import {
    Add,
    PlayArrowRounded,
    StopRounded
} from "@mui/icons-material";
import {LocalizationProvider, StaticDateTimePicker} from "@mui/x-date-pickers";
import {AdapterDayjs} from "@mui/x-date-pickers/AdapterDayjs";
import dayjs from "dayjs";
import {v4 as uuidv4} from 'uuid';
import axios from "axios";
import {environment} from "../../../../utils/Environment.ts";


const WashingMachineControl = ({smartDevice, setSmartDeviceParent}) => {

    const [selectedMode, setSelecteedMod] = useState(smartDevice.mode)
    const [scheduledMode, setScheduledMode] = useState("mixed wash");
    const [availableModes, setAvailableModes] = useState([])
    const [isOn, setIsOn] = useState(smartDevice.isOn)
    const [open, setIsOpen] = useState(false)
    // @ts-ignore
    type TDate = TDate | null;
    const [value, setValue] = React.useState<TDate>(dayjs());
    const [scheduled, setScheduled] = useState(smartDevice.schedules);
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

    function parseDateString(dateString: string): Date | null {
        const parts = dateString.split(' ');
        const [day, month, year] = parts[0].split('/').map(part => parseInt(part, 10))
        const [hour, minute] = parts[1].split(':').map(part => parseInt(part, 10))

        const parsedDate = new Date(year, month - 1, day, hour, minute);

        return parsedDate;
    }

    const changeMode = (mode) => {
        axios.put(environment + `/api/WashingMachine/ChangeMode?Id=${smartDevice.id}&mode=${mode}`).then(res => {
        }).catch(err => {
            console.log(err)
        });
    }

    useEffect(() => {
            smartDevice.mode = selectedMode;
            setSmartDeviceParent(smartDevice);
            setSmartDeviceParent(smartDevice);
        },
        [selectedMode])

    useEffect(() => {
        setSelecteedMod(smartDevice.mode??"mixed wash")
        setIsOn(smartDevice.isOn)
        setScheduled(smartDevice.schedules)
        setAvailableModes(smartDevice.modes??[])
    }, [smartDevice.id])
    useEffect(() => {
        setSelecteedMod(smartDevice.mode??"mixed wash")
        setIsOn(smartDevice.isOn)
        setScheduled(smartDevice.schedules)
    }, [smartDevice])

    const handleAddSchedule = () => {
        axios.post(environment + '/api/WashingMachine/AddScheduledWork', {
            id: smartDevice.id,
            mode: scheduledMode.toString().toLowerCase(),
            startDate: value.subtract(1, 'hour').format('DD/MM/YYYY HH:mm'),
        })
            .then((res) => {
                if (res.status == 200) {
                    smartDevice.schedules = [...smartDevice.schedules, {
                        date: value.subtract(1, 'hour').format('DD/MM/YYYY HH:mm').toString()+" - "+value.subtract(55, 'minute').format('DD/MM/YYYY HH:mm').toString(),
                        mode: scheduledMode,
                    }]
                    setSmartDeviceParent(smartDevice)
                    setValue(dayjs() as TDate)
                    setIsOpen(false)
                }
            })
            .catch((error) => {
                console.log(error);
            });

    }

    return <>
        <Modal
            open={open}
            onClose={() => setIsOpen(false)}
            aria-labelledby="modal-modal-title"
            aria-describedby="modal-modal-description"
        >
            <Grid borderRadius="25px" container spacing={2} width="50%" bgcolor="white" padding={4} sx={{
                position: "absolute", top: "50%", left: "50%",
                transform: 'translate(-50%, -50%)'
            }}>
                <Grid item xs={12}>
                    <Typography id="modal-modal-title" variant="h6" component="h2">
                        Add Schedule For Air Conditioner
                    </Typography>
                </Grid>
                <Grid item xs={12}>
                    <LocalizationProvider dateAdapter={AdapterDayjs}>
                        <StaticDateTimePicker sx={DatePickerStyle} ampm={false} disablePast value={value}
                                              onChange={(newDate) => setValue(newDate)}/>
                    </LocalizationProvider>
                </Grid>
                <Grid item xs={12} display="flex" margin="0 auto" justifyContent="center" alignItems="center"
                      flexDirection="column">
                    <Select
                        value={scheduledMode}
                        fullWidth
                        sx={{borderRadius: "10px"}}
                        onChange={(event) => {
                            setScheduledMode(event.target.value as string)
                        }}

                    >
                        {availableModes.includes("mixed wash") && <MenuItem value={"mixed wash"}>MIXED WASH</MenuItem>}
                        {availableModes.includes("antiallergy") && <MenuItem value={"antiallergy"}>ANTIALLERGY</MenuItem>}
                        {availableModes.includes("white wash") && <MenuItem value={"white wash"}>WHITE WASH</MenuItem>}
                    </Select>

                </Grid>

                <Grid item xs={12} display="flex" alignItems="flex-end" justifyContent="end">
                    <Button onClick={() => setIsOpen(false)} type="submit" sx={{
                        backgroundColor: "transparent",
                        border: "1px solid #EDB90D",
                        color: "black",
                        paddingY: "10px",
                        borderRadius: "7px",
                    }}>Cancel</Button>
                    <Button type="submit" onClick={() => handleAddSchedule()} sx={{
                        backgroundColor: "#FBC40E",
                        color: "black",
                        paddingY: "10px",
                        borderRadius: "7px",
                        ml: "10px",
                        ':hover': {backgroundColor: "#EDB90D"}
                    }}>Create</Button>

                </Grid>

            </Grid>
        </Modal>
        <Box mt={1} display="grid" gap="10px" gridTemplateColumns="4fr 6fr"
             gridTemplateRows="170px 170px 170px 170px">
            <Box gridColumn={1} gridRow={1} display="flex" justifyContent="center"
                 flexDirection="column"
                 alignItems="center" bgcolor="white" borderRadius="25px" height="350px">
                <Typography fontSize="30px" fontWeight="600"> CURRENT STATE</Typography>
                <Box display="flex" flexDirection="row" justifyContent="center"
                     alignItems="center">
                    <Typography fontSize="110px"
                                fontWeight="700">{isOn ? "ON" : "OFF"}</Typography>

                </Box>
                <Typography fontSize="30px" fontWeight="600">{selectedMode!=undefined?selectedMode.toUpperCase( ):selectedMode}</Typography>
            </Box>

                <Box gridColumn={1} gridRow={3} height="350px" display="flex"
                     justifyContent="center"
                     flexDirection="column"
                     alignItems="center" bgcolor="white" borderRadius="25px">
                    <Typography fontSize="25px" fontWeight="500">MODE</Typography>
                    {availableModes.includes("mixed wash") &&
                        <Typography my={1.8} fontSize={selectedMode == "mixed wash" ? "40px" : "30px"}
                                    color={selectedMode == "mixed wash" ? "#343F71" : "black"}
                                    sx={{cursor: "pointer"}}
                                    onClick={() => {
                                        setSelecteedMod("mixed wash");
                                        changeMode("mixed wash")
                                    }} fontWeight="600">MIXED WASH</Typography>}
                    {availableModes.includes("antiallergy") &&
                        <Typography my={1.8} fontSize={selectedMode == "antiallergy" ? "40px" : "30px"}
                                    color={selectedMode == "antiallergy" ? "#343F71" : "black"}
                                    sx={{cursor: "pointer"}}
                                    onClick={() => {
                                        setSelecteedMod("antiallergy");
                                        changeMode("antiallergy")
                                    }} fontWeight="600">ANTIALLERGY</Typography>}
                    {availableModes.includes("white wash") &&
                        <Typography my={1.8} fontSize={selectedMode == "white wash" ? "40px" : "30px"}
                                    color={selectedMode == "white wash" ? "#343F71" : "black"}
                                    sx={{cursor: "pointer"}}
                                    onClick={() => {
                                        setSelecteedMod("white wash");
                                        changeMode("white wash")
                                    }} fontWeight="600">WHITE WASH</Typography>}
                </Box>

                <Box gridColumn={2} gridRow={1} height="710px" display="flex"
                     flexDirection="column"
                     bgcolor="white" borderRadius="25px">
                    <Box display="flex" mt={1} justifyContent={"center"}
                         flexDirection="row">
                        <Typography fontSize="30px" fontWeight="600"> SCHEDULED</Typography>
                        <IconButton onClick={() => setIsOpen(true)}
                                    sx={{
                                        height: "40px",
                                        width: "40px",
                                        marginTop: "2px",
                                        marginLeft: 2
                                    }}><Add/></IconButton>
                    </Box>
                    <Box display="flex" width="100%" flexDirection="column" overflow="auto">
                        {scheduled && scheduled.length > 0 && scheduled.sort((a, b) => parseDateString(b.date.split('-')[0]).getTime() - parseDateString(a.date.split('-')[0]).getTime()).map((item) =>
                            <Box key={uuidv4()}>
                                <Box width="98%" margin="0 auto" height={"2px"}
                                     bgcolor="rgba(0, 0, 0, 0.20)"/>
                                <Box width={"100%"} my={1} display="flex" alignItems="center"
                                     flexDirection={"row"}>
                                    <Box px={2} display="grid" width="100%"
                                         gridTemplateColumns="6fr 1fr 2fr">
                                        <Typography textAlign="left" gridColumn={1} fontSize="20px"
                                                    fontWeight="500"> {item.date}</Typography>
                                        <Typography gridColumn={3} textAlign="right" fontSize="20px"
                                                    fontWeight="500"> {item.mode!=undefined?item.mode.toUpperCase():item.mode}</Typography>
                                    </Box>
                                </Box>
                            </Box>)}
                    </Box>
                </Box>


            </Box>
        </>
        }

        export default WashingMachineControl;