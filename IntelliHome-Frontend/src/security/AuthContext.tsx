import {createContext, useState, useEffect} from 'react';
import axios from "axios";
import {environment} from "./Environment.tsx";

export const AuthContext = createContext({
    isAuthenticated: false,
    role: null,
    changedPassword:null,
    isLoading: true
});

export const AuthProvider = ({ children } : any) => {
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [role, setRole] = useState(null);
    const [changedPassword, setChangedPassword] = useState(null);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        setIsLoading(true);
        axios.get(environment + `/api/User/whoAmI`)
            .then(res => {
                console.log(res)
                if (res.status === 200){
                    setIsAuthenticated(true);
                    setRole(res.data.role);
                    setChangedPassword(res.data.passwordChanged)
                }
                setIsLoading(false);
            })
            .catch(() => {
                setIsAuthenticated(false);
                setIsLoading(false);
                setChangedPassword(null);
            });
    }, []);


    return (
        <AuthContext.Provider value={{ isAuthenticated, isLoading, role,changedPassword  }}>
            {children}
        </AuthContext.Provider>
    );
};
