import Header from './layout/header'
import Navbar from './layout/navbar'
import Body from './layout/body'
import Footer from './layout/footer'
import { BrowserRouter } from 'react-router-dom'
import { useState, useEffect } from 'react'
import axios from 'axios'
import './index.css'

function App() {
    const [isAuthenticated, setIsAuthenticated] = useState(false);

    useEffect(() => {
        checkIfAuthenticated();
    }, [isAuthenticated]);

    return (
        <div className="relative min-h-screen bg-indigo-50">
            <BrowserRouter>
                <Header isAuthenticated={isAuthenticated} setIsAuthenticated={setIsAuthenticated} />
                <Navbar />
                <Body isAuthenticated={isAuthenticated} setIsAuthenticated={setIsAuthenticated} />
                <Footer />
            </BrowserRouter>
        </div>
    );

    async function checkIfAuthenticated() {
        const response = await axios.get('https://localhost:7127/API/Identity/IsAuthenticated', {
            withCredentials: true
        }).catch(e => console.error(e));
        if (response.data) {
            setIsAuthenticated(true);
        }
    }
}

export default App;